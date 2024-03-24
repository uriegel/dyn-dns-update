// module Update 

// open System

// open Settings
// open FSharpHttpRequest
// open FSharpTools
// open AsyncResult
// open FSharpTools.Functional

// let updateOnce settings host ip = 

//     let getUrl () = sprintf "https://%s/nic/update?hostname=%s&myip=%s" settings.Provider host ip
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