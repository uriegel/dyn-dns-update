// module PublicIP

// open FSharpTools
// open FSharpHttpRequest
// open AsyncResult
// open System

// let get () = 
//     let get () = 
//         let extractIp = 
//             String.subStringBetweenStrs "Address: " "<"
//             >> (Option.defaultValue "")
//             >> Async.toAsync

//         Request.getString { Request.defaultSettings with Url = "http://checkip.dyndns.org" }
//         |> mapError Error.fromIpResult
//         |>> extractIp
    
//     get
//     |> repeatOnError (TimeSpan.FromSeconds 5) 4 