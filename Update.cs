using CsTools.Extensions;
using CsTools.Functional;
using CsTools.HttpRequest;

using static System.Console;
using static CsTools.Core;
using static CsTools.HttpRequest.Core;

static class Update 
{
    public static Task<string> Run(Settings settings, string ip, string domain)
        => RepeatOnError(async () =>
        {
            if (Dns.Check(domain, ip))
            {
                WriteLine($"{domain}: IP Address not changed, no action needed");
                return "";
            }
            else
                return await UpdateRequest(settings, ip, domain);
        }, new RepeatOnErrorOptions() {RepeatCount = 4, WaitTime = TimeSpan.FromSeconds(5) });
            
    static async Task<string> UpdateRequest(Settings settings, string ip, string domain)
    {
        var response = await Request.RunAsync(DefaultSettings with
        {
            Method = HttpMethod.Get,
            BaseUrl = $"https://{settings.Provider}",
            Headers = [
                        new Header("User-Agent", "DynDNS Updater"),
                        BasicAuthentication.From(settings.Account, settings.Passwd)],
            Url = $"/nic/update?hostname={domain.SideEffect(d => WriteLine($"Updating {d}"))}&myip={ip}"
        });
        var result = await response.Content.ReadAsStringAsync();
        WriteLine($"Response: {result}");
        return result;
    }
}
