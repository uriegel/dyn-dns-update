module PublicIP

open FSharpTools
open AsyncResult

let get () = 
    let getResult = HttpRequest.getString [||]
    let parseIp ipString = ipString |> Utils.subStringStrToStr "Address: " "<"

    //"http://checkip.dyndns.org"
    "http://uriegel.de:99"
        |> getResult
        |>> (parseIp >> Utils.toAsync)

    

