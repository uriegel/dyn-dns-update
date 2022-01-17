use std::{fs, default};

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
}
