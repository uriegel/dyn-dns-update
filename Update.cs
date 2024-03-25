using CsTools.Extensions;
using CsTools.Functional;
using CsTools.HttpRequest;

using static System.Console;
using static CsTools.Core;
using static CsTools.HttpRequest.Core;
using static CsTools.Functional.AsyncResultExtensions;

static class Update 
{
    public static AsyncResult<string, RequestError> Run(Settings settings, string ip, string domain)
        => RepeatOnError(() => RunOnce(settings, ip, domain), 4, TimeSpan.FromSeconds(5)); 
    
    static AsyncResult<string, RequestError> RunOnce(Settings settings, string ip, string domain)   
        => Dns.Check(domain, ip)
         ? Ok<string, RequestError>("")
            .SideEffect(_ => WriteLine($"{domain}: IP Address not changed, no action needed"))
            .ToAsync()
            .ToAsyncResult()
         : UpdateRequest(settings, ip, domain);

    static AsyncResult<string, RequestError> UpdateRequest(Settings settings, string ip, string domain)   
        => Request
            .Run(DefaultSettings with
                {
                    Method = HttpMethod.Get,
                    BaseUrl = $"https://{settings.Provider}",
                    Headers = [
                        new Header("User-Agent", "DynDNS Updater"),
                        BasicAuthentication.From("uriegel.de", "juliachiara1")], 
                    Url = $"/nic/update?hostname={domain.SideEffect(d => WriteLine($"Updating {d}"))}&myip={ip}"
                })
            .BindAwait(m => m.ReadAsStringAwait())
            .SideEffectWhenOk(s => WriteLine($"Response: {s}"));
}
