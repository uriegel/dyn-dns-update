module Utils

open FSharpTools
open System.Threading.Tasks
open System

// TODO FSharpTools
let subStringStrToStr startStr endStr str = 
    let startIndex = str |> String.indexOf startStr
    let endIndex   = str |> String.indexOfStart endStr (startIndex |> Option.defaultValue 0)
    match startIndex, endIndex with
    | Some s, Some e -> str |> String.substring2 (s + (startStr |> String.length)) (e - s - (startStr |> String.length))
    | Some s, None         -> str |> String.substring (s + (startStr |> String.length))
    | None, Some e         -> str |> String.substring2 0 e
    | None, None                -> str

// TODO FSharpTools
let toAsync a = async {
    return a
}

let toResult (task: Task<'a>) = 
    let continueFrom ((ok: Result<'a, exn> -> Unit), _, _) = 
        let continueWith (task: Task<'a>) =
            let result = 
                if task.IsCompletedSuccessfully then
                    Ok task.Result
                elif task.IsFaulted && task.Exception.InnerException <> null then
                    Error task.Exception.InnerException
                elif task.IsFaulted then
                    Error task.Exception
                elif task.IsCanceled then
                    Error <| TaskCanceledException ()
                else
                    Error <| Exception ()
            ok result
        task.ContinueWith continueWith 
        |> ignore

    Async.FromContinuations continueFrom
