module Update 

open System
open System.Text

open Settings
open FSharpHttpRequest
open FSharpTools
open FSharpTools.Functional
open AsyncResult
open FSharpTools.Async

// TODO to FSharpHttpRequest
let basicAuthentication name passwd = 
    {
        Key = "Authorization"
        Value = "Basic " + Convert.ToBase64String (Encoding.UTF8.GetBytes (name + ":" + passwd))
    }

let update settings host ip = 

    let getUrl () = sprintf "https://%s/nic/update?hostname=%s&myip=%s" settings.Provider host ip

    let mapResult (resStr: string) = 
        printfn "response: %s" resStr 
        () 
        |> Async.toAsync

    let printUrl url = printfn "Updating %s" url

    match DnsCheck.check host ip with
    | false ->
        Request.getString { 
            Request.defaultSettings with 
                Url = (getUrl () ) |> sideEffect printUrl
                Headers = Some [| 
                    { Key = "User-Agent"; Value = "DynDNS Updater" }
                    basicAuthentication settings.Account settings.Passwd
                |]  
            }
            |> AsyncResult.mapError Error.fromUpdateResult
            |>> mapResult

    | true ->
        printfn "%s: IP Address not changed, no action needed\n" host
        Ok () 
        |> toAsync