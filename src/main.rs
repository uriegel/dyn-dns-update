use std::fs;

fn main() {
    println!("Hello, world!");
    let foo = fs::read_to_string("Cargo.toml").expect("Could not read settings");
    let test = foo;
}
