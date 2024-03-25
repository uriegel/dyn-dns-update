using CsTools.Extensions;
using CsTools.Functional;
using CsTools.HttpRequest;

using static CsTools.HttpRequest.Core;

static class Update 
{
    public static AsyncResult<string, RequestError> Run(Settings settings, string ip, string domain)   
        // await Dns.Check(domain, ip)
        // ? Unit.Value.SideEffect(_ => WriteLine($"{domain}: IP Address not changed, no action needed")).ToAsync()
        // : Unit.Value.ToAsync();

        => Request
            .Run(DefaultSettings with
                {
                    Method = HttpMethod.Get,
                    BaseUrl = $"https://{settings.Provider}",
                    Headers = [
                        new Header("User-Agent", "DynDNS Updater"),
                        BasicAuthentication.From("uriegel.de", "juliachiara1")], 
                    Url = $"/nic/update?hostname={domain}&myip={ip}"
                })
            .BindAwait(m => m.ReadAsStringAwait());
}
// let updateOnce settings host ip = 


//     let mapResult (resStr: string) = 
//         printfn "response: %s" resStr 
//         () 
//         |> Async.toAsync

//     let printUrl url = printfn "Updating %s" url

//     match DnsCheck.check host ip with
//     | false ->
//         Request.getString { 
//             Request.defaultSettings with 
//                 Url = (getUrl () ) |> sideEffect printUrl
//                 Headers = Some [| 
//                     { Key = "User-Agent"; Value = "DynDNS Updater" }
//                     BasicAuthentication.addHeader settings.Account settings.Passwd
//                 |]  
//             }
//             |> mapError Error.fromUpdateResult
//             |>> mapResult

//     | true ->
//         printfn "%s: IP Address not changed, no action needed" host
//         printfn ""
//         Ok () 
//         |> Async.toAsync

// let update settings host ip = 
//     let funToRun () = updateOnce settings host ip 
    
//     funToRun
//     |> repeatOnError (TimeSpan.FromSeconds 5) 4 