use std::{fs, net::IpAddr, str::FromStr, fmt};

use dns_lookup::lookup_host;
use serde::Deserialize;

#[derive(Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Settings {
    pub domains: Vec<String>,
    pub provider: String,
    pub account: String,
    pub passwd: String,
}

impl fmt::Debug for Settings {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        f.debug_struct("Settings")
         .field("domains", &self.domains)
         .field("provider", &self.provider)
         .field("account", &self.account)
         .field("passwd", &"***")
         .finish()
    }
}

fn main() {
    println!("Starting dyn-dns-update...");    

    let settings = 
        fs::read_to_string(dirs::config_dir()
                .expect("Could not find config dir")
                .join("dyn-dns-update")
                .join("dyn-dns-update.conf")
            ).expect("Could not read settings");
    let settings: Settings = serde_json::from_str(&settings).expect("Could not extract settings");
    println!("Settings: {settings:#?}");

    let mut counter = 0;
    let public_ip = loop {
        if let Some(ip) = get_public_ip().expect("Could not get public ip") {
            break ip;
        }
        counter += 1;
        if counter > 5 {
            panic!("Could not get public ip");
        }
    };

    println!("public ip: {public_ip:#?}");

    let is_not_up_to_date = |resolved_domain: &ResolvedDomain| {
        let res = resolved_domain.ips.contains(&public_ip);
        if res == true { println!("{} already up to date", resolved_domain.name); }
        !res
    };

    let domains_to_refresh: Vec<String> = settings.domains
        .iter()
        .map(resolve_domain)
        .filter(is_not_up_to_date)
        .map(|n|{n.name})
        .collect();

    for domain in domains_to_refresh {
        update_domain(&domain, &public_ip, &settings).expect("Could not update domain {domain}");
    }
}

fn get_public_ip() -> Result<Option<IpAddr>, reqwest::Error> {
    let response = reqwest::blocking::get("http://checkip.dyndns.org")?;
    if response.status().is_success() {
        let text = response.text()?;
        let pos = text.find("Address:").expect("No Address found") + 8;
        let ipstr = &text[pos..];
        let pos = ipstr.find("</body").expect("No Address found");
        let ipstr = ipstr[.. pos].trim();
        Ok(Some(IpAddr::from_str(ipstr).unwrap()))
    } else {
        println!("Request failed: {}", response.status());
        Ok(None)
    }
}

struct ResolvedDomain {
    ips: Vec<IpAddr>,
    name: String
}

fn resolve_domain(domain: &String) -> ResolvedDomain {
    let ips = lookup_host(domain).unwrap();
    println!("dns lookup {domain}: {ips:?}");    
    ResolvedDomain {
        ips,
        name: domain.to_string()
    }
}

fn update_domain(domain: &str, ip: &IpAddr, settings: &Settings) -> Result<(), reqwest::Error> {
    match ip {
        IpAddr::V4(ip4) => {
            let ip = ip4.to_string();
            println!("Updating domain {domain}, ip {ip}");
            let client = reqwest::blocking::Client::new();
            let url = format!("https://{}/nic/update?hostname={domain}&myip={ip}", settings.provider);
            let resp = client.get(url)
                .basic_auth(&settings.account, Some(&settings.passwd))
                .header("User-Agent", "URiegel dyn-dns-update")
                .send()?;
            let text = resp.text();
            println!("Result: {text:?}");
                
            // let pos = text.find("Address:").expect("No Address found") + 8;
            // let ipstr = &text[pos..];
            // let pos = ipstr.find("</body").expect("No Address found");
            // let ipstr = ipstr[.. pos].trim();
            // Ok(IpAddr::from_str(ipstr).unwrap())
        },
        _ => panic!("Could not update domain {domain}, ip is ipv6: {ip}")
    }
    Ok(())
}