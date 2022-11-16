open System.Net
open System.IO
open Settings


let asyncMain () = async {
    printfn "Starting Dyn DNS Auto Updater..."
    let test = getSettings ()
    let! ip = PublicIP.get ()




    match ip with
    | Ok ip -> printfn "Public IP Address: %s" ip
    | Error err -> printfn "Error: %O" err

    printfn "Dyn DNS Auto Updater finished"
} 

[<EntryPoint>]
let main argv = 
    try 
        asyncMain () 
        |> Async.RunSynchronously
    with
    | e -> printfn "Exception: %O" e

    0

// try 
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
// finally
//     printfn "Dyn DNS Auto Updater finished"

