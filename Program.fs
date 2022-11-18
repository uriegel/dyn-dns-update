open Settings
// TODO retry after failure (3x)

async {
    printfn "Starting Dyn DNS Auto Updater..."
    //let settings = getSettings ()
    let! ip = PublicIP.get ()
    // TODO mapError Error PublicIP of HttpRequestError


    // TODO ROP

    match ip with
    | Ok ip -> printfn "Public IP Address: %s" ip
    | Error err -> printfn "Error: %O" err


    let checkDomain ip domain = 
        DnsCheck.check domain ip
        |> ignore

    printfn "Checking ip"
    (getSettings ()).domains  
    |> Array.iter (checkDomain "12.34.5.6")

    // TODO iterUntil settings.domains |> Array.iter perform
    // TODO arrayToList -> head ->perform recursive tail

    printfn "Dyn DNS Auto Updater finished"
} 
|> Async.RunSynchronously



//     try 

//         let perform (host: string) = 
//             let hostEntry = Dns.GetHostEntry host
//             match hostEntry.AddressList |> Array.exists (fun n -> n.ToString () = ip) with
//             | false -> 
//                 printfn "IP Address changed, update to provider needed"
//                 let url = sprintf "https://%s/nic/update?hostname=%s&myip=%s" settings.provider host ip
//                 printfn "Updating %s" url                             
//                 let request = WebRequest.Create url :?> HttpWebRequest
//                 let networkCredential = NetworkCredential (settings.account, settings.passwd)
//                 let credentialCache = CredentialCache ()
//                 credentialCache.Add (Uri (url), "Basic", networkCredential)
//                 let encoded = System.Convert.ToBase64String (System.Text.Encoding.UTF8.GetBytes (settings.account + ":" + settings.passwd))
//                 request.Headers.Add ("Authorization", sprintf "Basic %s" encoded)
//                 request.UserAgent <- "DynDNS Updater"
//                 let response = request.GetResponse ()
//                 use responseStream = response.GetResponseStream ()
//                 use sw = new StreamReader (responseStream)
//                 printfn "response: %s" <| sw.ReadToEnd ()
//             | true -> 
//                 printfn "IP Address not changed, no action needed"

//         settings.domains |> Array.iter perform
//     with
//     | e -> printfn "Exception: %O" e

