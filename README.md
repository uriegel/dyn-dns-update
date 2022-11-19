# dyn-dns-update
DynDns client based on .NET Core for Linux and Windows
Similar to ddclient

## Installation
```
dotnet tool install DynDnsUpdater --global
```

## Executing the tool
```
DynDnsUpdater
``` 

follow Instructions for giving necessary parameters

Settings are saved in ```~/.config/dyndns-updater.conf``` (Linux)

## Executing tool every day (Linux)

```
crontab -e
```

Append

```
PATH=$PATH:/home/pi/.dotnet/tools
DOTNET_ROOT=/home/pi/.dotnet
0 3 * * * DynDnsUpdater > /home/pi/logs/dyndns.log 2>&1
```

This executes dns update every day at 3 AM (universal time). Last log is saved in ```/home/pi/logs/dyndns.log```
