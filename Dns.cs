static class Dns
{
    public static bool Check(string domain, string ip)
        => System.Net.Dns.GetHostEntry(domain).AddressList.Any(n => n.ToString() == ip);
}

