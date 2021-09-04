open System.Net
open System.IO
open System
open System.Runtime.InteropServices
open Settings
open System

let settingsFile = Path.Combine (Environment.GetFolderPath Environment.SpecialFolder.ApplicationData, "dyndns-updater.conf")
// let settingsFile = "/etc/dyndns-updater.conf"

printfn "Settings file: %s" settingsFile

let readPasswd () =
    let rec readKey charList =
        match (Console.ReadKey true).KeyChar with
        | '\r' | '\n' -> 
            Console.WriteLine ()
            charList
        | '\b' | '\u007f' -> 
            match charList with
            | head :: tail -> 
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                Console.Write " "
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                readKey <| tail 
            | [] -> readKey []        
        | chr -> 
            Console.Write '*'
            readKey <| chr :: charList
    let secstr = new Security.SecureString ()
    readKey []
    |> List.rev
    |> List.iter secstr.AppendChar
    secstr

let readSecureString (secstr: Security.SecureString) =
    let mutable valuePtr = IntPtr.Zero
    try 
        valuePtr <- Marshal.SecureStringToGlobalAllocUnicode secstr
        Marshal.PtrToStringUni valuePtr
    finally 
        Marshal.ZeroFreeGlobalAllocUnicode valuePtr
 
let getPasswd () =
    let getPasswdFromConsole = readPasswd >> readSecureString

    let getPasswdFromConsole again = 
        printfn "Enter your dyndns password%s" <| if again then " again" else ""
        getPasswdFromConsole ()

    let rec getPasswd () =
        match getPasswdFromConsole false, getPasswdFromConsole true with
        | a, b when a = b -> a
        | _, _ -> 
            printfn "Passwords did not match, please try again:"
            getPasswd ()
    getPasswd ()                

let getSettings () =
    match File.Exists settingsFile with
    | true-> 
        use file = File.OpenRead settingsFile
        Json.deserializeStream<Settings.Value> file
    | false -> 
        printfn "Enter domain names, comma separated:"
        let text = Console.ReadLine ()
        let domains = text |> String.splitChar ',' |> Array.map (fun n -> n |> String.trim)
        printfn "Enter your dyndns provider (e.g. 'dyndns.strato.com'):"
        let provider = Console.ReadLine ()
        printfn "Enter your dyndns account name:"
        let account = Console.ReadLine ()
        let passwd = getPasswd ()
        let settings = { 
                domains = domains
                provider = provider
                account = account
                passwd = passwd
            }
        use file = File.Create settingsFile
        settings |> Json.serializeStream file
        settings

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

        let settings = getSettings ()

        let ip = getPublicIP ()
        printfn "Public IP Address: %s" ip

        let perform (host: string) = 
            let hostEntry = Dns.GetHostEntry host
            match hostEntry.AddressList |> Array.exists (fun n -> n.ToString () = ip) with
            | false -> 
                printfn "IP Address changed, update to provider needed"
                let url = sprintf "https://%s/nic/update?hostname=%s&myip=%s" settings.provider host ip
                printfn "Updating %s" url                             
                let request = WebRequest.Create url :?> HttpWebRequest
                let networkCredential = NetworkCredential (settings.account, settings.passwd)
                let credentialCache = CredentialCache ()
                credentialCache.Add (Uri (url), "Basic", networkCredential)
                let encoded = System.Convert.ToBase64String (System.Text.Encoding.UTF8.GetBytes (settings.account + ":" + settings.passwd))
                request.Headers.Add ("Authorization", sprintf "Basic %s" encoded)
                request.UserAgent <- "DynDNS Updater"
                let response = request.GetResponse ()
                use responseStream = response.GetResponseStream ()
                use sw = new StreamReader (responseStream)
                printfn "response: %s" <| sw.ReadToEnd ()
            | true -> 
                printfn "IP Address not changed, no action needed"

        settings.domains |> Array.iter perform
    with
    | e -> printfn "Exception: %O" e
finally
    printfn "Dyn DNS Auto Updater finished"

