using System;
using System.Net.Http;
using System.Threading.Tasks;
using CsTools.Async;
using CsTools.Extensions;
using CsTools.HttpRequest;

using static CsTools.Core;
using static CsTools.HttpRequest.Core;

static class PublicIP
{
    public static Task<string> Get()
        => RepeatOnException(
            () => RequestIP()
                    .Select(ExtractIP),
            4, 
            TimeSpan.FromSeconds(5));

    static string ExtractIP(string ipStr)
        => ipStr.StringBetween("Address: ", "<");

    static Task<string> RequestIP()
        => Request.GetStringAsync(DefaultSettings with
        {
            Method = HttpMethod.Get,
            BaseUrl = "http://checkip.dyndns.org"
        });
}
