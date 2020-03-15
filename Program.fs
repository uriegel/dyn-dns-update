open System.Net
open System.IO

let getPublicIP () =
    let url = "http://checkip.dyndns.org"
    let request = WebRequest.Create url
    let response = request.GetResponse ()
    let sr = new StreamReader (response.GetResponseStream ())
    let responseText = sr.ReadToEnd () |> String.trim

    let pos1 = 
        match responseText |> String.indexOf "Address: " with 
        | Some pos -> pos + 9  
        | None -> failwith "Could not determine public IP address"
    let ipStr = responseText |> String.substring pos1
    let pos2 = 
        match ipStr |> String.indexOf "<" with 
        | Some pos -> pos  
        | None -> failwith "Could not determine public IP address"
    ipStr |> String.substring2 0 pos2

try 
    try 
        printfn "Starting Dyn DNS Auto Updater..."

        let host = "uriegel.de"

        let ip = getPublicIP ()
        printfn "Public IP Address: %s" ip
        let hostEntry = Dns.GetHostEntry host
        match hostEntry.AddressList |> Array.exists (fun n -> n.ToString () = ip) with
        | false -> 
            // https://<account>:<pw>@<dyndns.strato.com>/nic/update?hostname=<hostname>&myip=<ip> 
            printfn "IP Address changed, update to provider needed"
            ()
        | true -> printfn "IP Address not changed, no action needed"
    with
    | e -> printfn "Exception: %O" e
finally
    printfn "Dyn DNS Auto Updater finished"

