# dyn-dns-update
DynDns client based on rust for Linux.
Similar to ddclient

## prerequisites

### On Debian and Ubuntu
sudo apt-get install pkg-config libssl-dev
### On Arch Linux
sudo pacman -S openssl
### On Fedora
sudo dnf install openssl-devel

## Installation
```
Settings are stored in ```~/.config/dyn-dns-update/dyn-dns-update.conf``` 
```

## Executing the tool
```
dyn-dns-update
``` 
## Executing tool every day

```
crontab -e
```

Append

```
0 3 * * * /home/pi/server/dyn-dns-update > /home/pi/logs/dyn-dns-update.log 2>&1
```

This executes dns update every day at 3 AM. Last log is saved in ```/home/pi/logs/dyn-dns-update.log```
