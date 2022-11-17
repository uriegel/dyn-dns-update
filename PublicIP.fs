module PublicIP

open FSharpTools
open FSharpHttpRequest
open AsyncResult

let get () = 
    let extractIp = 
        String.subStringBetweenStrs "Address: " "<"
        >> (Option.defaultValue "")
        >> Async.toAsync

    Request.getString { Request.defaultSettings with Url = "http://checkip.dyndns.org" }
    |>> extractIp


    // TODO sometimes 502 bad gateway!
    

