namespace Firebrand.IrcBot

open System
open irclib.IrcCommand
open irclib
open Microsoft.FSharp.Core.Operators.Unchecked
open Common.Extensions

type Bot = { Nick : String; Username : String; Realname : String }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Bot =
    let private response cmd =
        match cmd with
        | Ping server -> Some(Pong server)
        | ChannelMessage (channel, msg) -> Some(OutMessage(channel, "You said " + msg))
        | UserMessage (user, msg) -> Some(OutMessage(user, "You said " + msg))
        | _ -> None
    let runBot name: _ IrcState = 
        IrcConn.send(Nick(name.Nick)) >>= 
        IrcConn.send(User(name.Username, name.Realname))
    let inChannel channel = channel |> Join |> IrcConn.send
    let onServer config = IrcConn.set config
    let start(setup: _ IrcState) =
        irc {
            do! setup
            let! lines = IrcConn.readStream()
            let lines = lines |> Seq.toList
            do! lines |> Seq.choose response |> Seq.map IrcConn.send |> Seq.reduce(mergeIrcTasks)
        }
        <| defaultof<IrcConn>
        |> fst