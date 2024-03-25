using CsTools.Async;
using CsTools.Extensions;
using CsTools.Functional;
using static System.Console;

WriteLine("Starting Dyn DNS Auto Updater...");

var ip = await PublicIP.Get();
WriteLine($"Outer IP address: {ip}");

var results = await
    Settings
        .Get()
        .Domains
        .Select(d => Update.Run(Settings.Get(), ip, d))
        .ToAsyncResults()
        .Collect();

WriteLine("Dyn DNS Auto Updater finished");
results.ForEach(n => n.Match(s => { }, e => WriteLine($"Error: {e}")));
       
