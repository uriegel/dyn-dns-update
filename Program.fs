open System.Net
open System.IO
open System
open System.Runtime.InteropServices

//let settings = "/etc/dyndns-updater.conf"
let settings = "d:/dyndns-updater.conf"

// TODO: FSharpTools
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

// TODO: FSharpTools
let readSecureString (secstr: Security.SecureString) =
    let mutable valuePtr = IntPtr.Zero
    try 
        valuePtr <- Marshal.SecureStringToGlobalAllocUnicode secstr
        Marshal.PtrToStringUni valuePtr
    finally 
        Marshal.ZeroFreeGlobalAllocUnicode valuePtr
 
let getPasswd = readPasswd >> readSecureString

let getSettings () =
    match File.Exists settings with
    | true-> ()
    | false -> 
        printfn "Enter domain names, comma separated:"
        let text = Console.ReadLine ()
        let domains = text |> String.splitChar ',' |> Array.map (fun n -> n |> String.trim)
        printfn "Enter your dyndns provider (e.g. 'dyndns.strato.com'):"
        let provider = Console.ReadLine ()
        printfn "Enter your dyndns account name:"
        let account = Console.ReadLine ()
        printfn "Enter your dyndns password:"
        let passwd = getPasswd ()
        ()

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

