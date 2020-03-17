# DynDnsUpdater
DynDns client based on .NET Core for Linux and Windows
Similar to ddclient

## Installation (test)
```dotnet pack -c Release```
```sudo dotnet tool install --tool-path /opt/dotnet --add-source ./nupkg DyndnsUpdater```
```sudo ln -s /usr/local/share/dotnet-tools/DyndnsUpdater /usr/local/bin/DyndnsUpdater```

## Installation
```sudo dotnet tool install --tool-path /opt/dotnet```
```sudo ln -s /usr/local/share/dotnet-tools/DyndnsUpdater /usr/local/bin/DyndnsUpdater```

## Executing the tool
```sudo dyndnsUpdate``` 

follow Instructions

Settings are saved in ```/etc/dyndns-updater.conf``` (Linux)
