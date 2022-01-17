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

    let public_ip = get_public_ip().expect("Could not get public ip");
    println!("body = {public_ip:#?}", );

    let is_up_to_date = |resolved_domain: &ResolvedDomain| {
        let res = resolved_domain.ips.contains(&public_ip);
        if res == true { println!("{} already up to date", resolved_domain.name); }
        res
    };

    let domains_to_refresh: Vec<ResolvedDomain> = settings.domains
        .iter()
        .map(resolve_domain)
        .filter(is_up_to_date)
        .collect();
}

fn get_public_ip() -> Result<IpAddr, reqwest::Error> {
    let text = reqwest::blocking::get("http://checkip.dyndns.org")?.text()?;
    let pos = text.find("Address:").expect("No Address found") + 8;
    let ipstr = &text[pos..];
    let pos = ipstr.find("</body").expect("No Address found");
    let ipstr = ipstr[.. pos].trim();
    Ok(IpAddr::from_str(ipstr).unwrap())
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

