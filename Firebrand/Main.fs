open Firebrand.IrcBot
open irclib
open Bot
open System

[<EntryPoint>]
let main args = 
    let setup =
        onServer({ Server = "irc.freenode.net"; Port = 6667 })
        >>= runBot { 
            Nick = "firebrand"; 
            Username = "firebrand"; 
            Realname = "firebrand" 
        } 
        >>= inChannel "#firebrand-test"

    setup |> start

    printfn "Press any key to exit."
    Console.ReadLine() |> ignore
    0