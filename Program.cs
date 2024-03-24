using static System.Console;

WriteLine("Starting Dyn DNS Auto Updater...");

WriteLine(await PublicIP.Get());

WriteLine(Settings.GetPasswd());
//WriteLine(Settings.Get());
       
//     let performUpdate = 
//         let rec performUpdate res ip ipList = async {
//             match ipList, res with
//             | [] , _ -> return res 
//             | _ , Error _ -> return res 
//             | head::tail, _ -> 
//                 let! res = Update.update (getSettings ()) head ip
//                 return! performUpdate res ip tail          
//         }
        
//         performUpdate (Ok ()) 

//     let runAll ip = 
//         (getSettings ())
//             .Domains  
//             |> Array.toList
//             |> performUpdate ip

//     match! PublicIP.get () >>= runAll with
//     | Error err -> 
//         printfn "Dyn DNS Auto Updater finished: %O" err
//     | _ -> 
//         printfn "Dyn DNS Auto Updater finished"
// } 
// |> Async.RunSynchronously

