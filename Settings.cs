using System.Text.Json;
using CsTools;
using CsTools.Extensions;
using CsTools.Functional;

using static System.Console;
using static CsTools.Functional.Memoization;

record Settings(
    string[] Domains,
    string Provider,
    string Account,
    string Passwd)
{
    public static Settings Get()
        => File.Exists(GetFile())
            ? Using.Use(GetFile().OpenFile(),
                file => JsonSerializer.Deserialize<Settings>(file, options)!) // TODO ! Validation
            : Read();

    static Func<string> GetFile { get; }
        = Memoize(() 
            =>  Environment
                    .GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    .WhiteSpaceToNull()
                    .Pipe(s => s ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).AppendPath(".config"))
                    .EnsureDirectoryExists()
                    .AppendPath("dyndns-updater.conf")
                    .SideEffect(s => WriteLine($"Settings file: {s}")));

    static Settings Read()
        => new Settings(Unit.Value
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
                GetPasswd())
                    .SideEffect(Save);

    static void Save(Settings settings)
        => Using.Use(
            File.Create(GetFile()),
            file => JsonSerializer.Serialize(file, settings, options));

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

    static readonly JsonSerializerOptions options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, 
    };
}



