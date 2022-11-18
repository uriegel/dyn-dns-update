module DnsCheck

open System.Net

let check (domain: string) ip = 
    let equalsIp (ipAddress: IPAddress) =
        ipAddress.ToString () = ip
    
    (Dns.GetHostEntry domain).AddressList 
    |> Array.exists (equalsIp)
