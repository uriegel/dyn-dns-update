# DynDnsUpdater
DynDns client based on .NET Core for Linux and Windows
Similar to ddclient

## Installation (test)
```
dotnet pack -c Release
sudo dotnet tool install --tool-path /opt/dotnet --add-source ./nupkg DyndnsUpdater
(sudo ln -s /usr/local/share/dotnet-tools/DyndnsUpdater /usr/local/bin/DyndnsUpdater)
```

## Installation
```
dotnet tool install --global
```

## Executing the tool
```
dyndnsUpdate
``` 

follow Instructions for giving necessary parameters

Settings are saved in ```~/.config/dyndns-updater.conf``` (Linux)

## Executing tool every day (Linux)

```
crontab -e
```

Append

```
PATH=$PATH:/usr/share/dotnet-sdk
DOTNET_ROOT=/usr/share/dotnet-sdk

0 3 * * * /home/pi/.dotnet/tools/dyndnsUpate > /home/pi/logs/dyndns.log 2>&1
```

This executes dns update every day at 3 AM. Last log is saved in ```/home/pi/logs/dyndns.log```