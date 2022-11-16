module HttpRequest

open System.Net.Http
open FSharpTools
open FSharpTools.Functional
open AsyncResult

let getHttpClient  =  
    let getHttpClient () = new HttpClient()
    memoizeSingle getHttpClient

type KeyValue = {
    Key: string
    Value: string
}

let getRequest (uri: string) (headers: KeyValue array) =
    let request = new HttpRequestMessage (HttpMethod.Get, uri)
    
    let addHeader keyValue = 
        request.Headers.TryAddWithoutValidation(keyValue.Key, [|keyValue.Value|])
        |> ignore

    headers
    |> Array.iter addHeader

    getHttpClient().SendAsync request
    |> Utils.toResult


let getString headers uri = 
    let getString (responseMessage: HttpResponseMessage) = async {
        return! responseMessage.Content.ReadAsStringAsync () |> Async.AwaitTask
    }

    getRequest uri headers 
    |>> getString
