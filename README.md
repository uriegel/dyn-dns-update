# dyn-dns-update
DynDns client based on .NET Core for Linux and Windows
Similar to ddclient

## Installation
```
Settings are saved in ```~/.config/dyndns-updater.conf``` (Linux)
```

## Executing the tool
```
dyn-dns-update
``` 

follow Instructions for giving necessary parameters

Settings are saved in ```~/.config/dyndns-updater.conf``` (Linux)

## Executing tool every day (Linux)

```
crontab -e
```

Append

```
0 3 * * * /home/pi/.dotnet/tools/dyn-dns-update > /home/pi/logs/dyn-dns-update.log 2>&1
```

This executes dns update every day at 3 AM. Last log is saved in ```/home/pi/logs/dyn-dns-update.log```
