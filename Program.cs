using CsTools.Async;
using CsTools.Functional;
using static System.Console;

WriteLine("Starting Dyn DNS Auto Updater...");

var ip = await PublicIP.Get();
WriteLine($"Outer IP address: {ip}");

var tasks =
    Settings
        .Get()
        .Domains
        .Select(d => Update.Run(Settings.Get(), ip, d));
foreach (var task in tasks)
{
    try
    {
        await task;
    }
    catch(Exception e)
    {
        WriteLine($"Error: {e}");
    }
}

WriteLine("Dyn DNS Auto Updater finished");

       
