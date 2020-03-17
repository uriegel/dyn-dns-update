# DynDnsUpdater
DynDns client based on .NET Core for Linux and Windows
Similar to ddclient

## Installation
```dotnet pack -c Release```
```sudo dotnet tool install --tool-path /opt/dotnet --add-source ./nupkg DyndnsUpdater```
```sudo ln -s /usr/local/share/dotnet-tools/TOOLCOMMAND /usr/local/bin/TOOLCOMMAND```