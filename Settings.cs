using CsTools;
using CsTools.Extensions;

using static System.Console;

record Settings(
    string[] Domains,
    string Provider,
    string Account,
    string Passwd)
{
    public static Settings Get()
    {
        var settingsFile = Environment
                            .GetFolderPath(Environment.SpecialFolder.ApplicationData)
                            .WhiteSpaceToNull()
                            .Pipe(s => s ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).AppendPath(".config"))
                            .EnsureDirectoryExists()
                            .AppendPath("dyndns-updater.conf")
                            .SideEffect(s => WriteLine($"Settings file: {s}"));

        return null;
    }


    public
    static Settings Read()
        => new(Unit.Value
                    .SideEffect(_ => WriteLine("Enter domain names, comma separated:"))
                    .Pipe(_ => (ReadLine() ?? "")
                        .Split(',')
                        .Select(s => s.Trim()))
                        .ToArray(),
                Unit.Value
                    .SideEffect(_ => WriteLine("Enter your dyndns provider (e.g. 'dyndns.strato.com'):"))
                    .Pipe(_ => ReadLine() ?? ""),
                Unit.Value
                    .SideEffect(_ => WriteLine("Enter your dyndns account name:"))
                    .Pipe(_ => ReadLine() ?? ""),
                GetPasswd());

    static string GetPasswd() 
    {
        static string GetPasswdFromConsole(bool again)
            => Unit.Value.SideEffect(_ => WriteLine($"Enter your dyndns password{(again ? " again" : "")}"))
                .Pipe(_ => Password.ReadPassword().ReadSecureString());

        static string GetPasswd()
            => (GetPasswdFromConsole(false), GetPasswdFromConsole(true)) switch
            {
                (var a, var b) when a == b => a,
                _ => Unit.Value.SideEffect(_ => WriteLine("Passwords did not match, please try again:"))
                    .Pipe(_ => GetPasswd())
            };
        
        return GetPasswd();
    }

}

//     let options = JsonSerializerOptions (PropertyNameCaseInsensitive = true)

//     let getSettings () =
//         let settingsFile = 
//             Path.Combine (Environment.GetFolderPath Environment.SpecialFolder.ApplicationData, "dyndns-updater.conf")
//         printfn "Settings file: %s" settingsFile    

//         match File.Exists settingsFile with
//         | true -> 
//             use file = File.OpenRead settingsFile
//             JsonSerializer.Deserialize<Value> (file, options)
//         | false -> 
//             printfn "Enter domain names, comma separated:"
//             use file = File.Create settingsFile
//             JsonSerializer.Serialize (file, settings, options)
//             settings
//     memoizeSingle getSettings