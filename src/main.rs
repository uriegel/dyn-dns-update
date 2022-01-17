use std::{fs, net::IpAddr, str::FromStr};

use dns_lookup::lookup_host;
use serde::Deserialize;

#[derive(Debug)]
#[derive(Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct Settings {
    pub domains: Vec<String>,
    pub provider: String,
    pub account: String,
    pub passwd: String,
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
    let log_settings = Settings {
        passwd: "***".to_string(),
        ..settings
    };
    println!("Settings: {log_settings:#?}");

    let public_ip = get_public_ip().expect("Could not get public ip");
    println!("body = {public_ip:#?}", );


    let hostname = "familie.uriegel.de";

    let ips: Vec<IpAddr> = lookup_host(hostname).unwrap();
    println!("dns lookup {hostname}: {ips:?}");    

    let hostname = "uriegel.de";

    let ips: Vec<IpAddr> = lookup_host(hostname).unwrap();
    println!("dns lookup {hostname}: {ips:?}");    
}

fn get_public_ip() -> Result<IpAddr, reqwest::Error> {
    let text = reqwest::blocking::get("http://checkip.dyndns.org")?.text()?;
    let pos = text.find("Address:").expect("No Address found") + 8;
    let ipstr = &text[pos..];
    let pos = ipstr.find("</body").expect("No Address found");
    let ipstr = ipstr[.. pos].trim();
    Ok(IpAddr::from_str(ipstr).unwrap())
}