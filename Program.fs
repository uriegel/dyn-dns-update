open Settings
open FSharpTools
open AsyncResult

// TODO retry after failure (3x)

async {
    printfn "Starting Dyn DNS Auto Updater..."
        
    let performUpdate = 
        let rec performUpdate res ip ipList = async {
            match ipList, res with
            | [] , _ -> return res 
            | _ , Error _ -> return res 
            | head::tail, _ -> 
                let! res = Update.update (getSettings ()) head ip
                return! performUpdate res ip tail          
        }
        
        performUpdate (Ok ()) 

    let runAll ip = 
        (getSettings ())
            .Domains  
            |> Array.toList
            |> performUpdate ip

    match! PublicIP.get () >>= runAll with
    | Error err -> 
        printfn "Dyn DNS Auto Updater finished: %O" err
    | _ -> 
        printfn "Dyn DNS Auto Updater finished"
} 
|> Async.RunSynchronously

