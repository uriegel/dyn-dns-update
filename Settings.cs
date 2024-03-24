// module Settings

// open FSharpTools
// open FSharpTools.Functional
// open System.IO
// open System.Text.Json
// open System
// open System.Runtime.InteropServices

// type Value = {
//     Domains: string array    
//     Provider: string
//     Account: string 
//     Passwd: string
// }

// let getSettings = 

//     let options = JsonSerializerOptions (PropertyNameCaseInsensitive = true)

//     let readPasswd () =
//         let rec readKey charList =
//             match (Console.ReadKey true).KeyChar with
//             | '\r' | '\n' -> 
//                 Console.WriteLine ()
//                 charList
//             | '\b' | '\u007f' -> 
//                 match charList with
//                 | head :: tail -> 
//                     Console.SetCursorPosition (Console.CursorLeft - 1, Console.CursorTop)
//                     Console.Write " "
//                     Console.SetCursorPosition (Console.CursorLeft - 1, Console.CursorTop)
//                     readKey <| tail 
//                 | [] -> readKey []        
//             | chr -> 
//                 Console.Write '*'
//                 readKey <| chr :: charList
//         let secstr = new Security.SecureString ()
//         readKey []
//         |> List.rev
//         |> List.iter secstr.AppendChar
//         secstr

//     let readSecureString (secstr: Security.SecureString) =
//         let mutable valuePtr = IntPtr.Zero
//         try 
//             valuePtr <- Marshal.SecureStringToGlobalAllocUnicode secstr
//             Marshal.PtrToStringUni valuePtr
//         finally 
//             Marshal.ZeroFreeGlobalAllocUnicode valuePtr

//     let getPasswd () =
//         let getPasswdFromConsole = readPasswd >> readSecureString

//         let getPasswdFromConsole again = 
//             printfn "Enter your dyndns password%s" <| if again then " again" else ""
//             getPasswdFromConsole ()

//         let rec getPasswd () =
//             match getPasswdFromConsole false, getPasswdFromConsole true with
//             | a, b when a = b -> a
//             | _, _ -> 
//                 printfn "Passwords did not match, please try again:"
//                 getPasswd ()
//         getPasswd ()                

//     let getSettings () =
//         let settingsFile = 
//             Path.Combine (Environment.GetFolderPath Environment.SpecialFolder.ApplicationData, "dyndns-updater.conf")
//         printfn "Settings file: %s" settingsFile    

//         match File.Exists settingsFile with
//         | true -> 
//             use file = File.OpenRead settingsFile
//             JsonSerializer.Deserialize<Value> (file, options)
//         | false -> 
//             printfn "Enter domain names, comma separated:"
//             let text = Console.ReadLine ()
//             let domains = text |> String.splitChar ',' |> Array.map String.trim
//             printfn "Enter your dyndns provider (e.g. 'dyndns.strato.com'):"
//             let provider = Console.ReadLine ()
//             printfn "Enter your dyndns account name:"
//             let account = Console.ReadLine ()
//             let passwd = getPasswd ()
//             let settings = { 
//                 Domains = domains
//                 Provider = provider
//                 Account = account
//                 Passwd = passwd
//             }
//             use file = File.Create settingsFile
//             JsonSerializer.Serialize (file, settings, options)
//             settings
//     memoizeSingle getSettings