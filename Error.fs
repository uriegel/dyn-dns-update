module Error

type Error = 
| PublicIP of FSharpHttpRequest.Error

let fromRequestError re =
    PublicIP re