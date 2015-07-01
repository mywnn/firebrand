// for sending/receiving irc commands
namespace irclib

open System
open System.IO
open System.Net.Sockets
open irclib.IrcCommand
open Common.Extensions

[<AutoOpen>]
module IrcUtils =
    let irc = new StateMonad()
    let (>>=) m1 m2 =
        irc {
            do! m1
            do! m2
        }
    let (<<=) m1 m2 =
        irc {
            do! m2
            do! m1
        }
    let mergeIrcTasks x y =
        irc { 
            do! x
            do! y
        }

type IrcConfig = { Server: string; Port: int }
type IrcConn = IrcConfig * (StreamWriter * StreamReader)
type IrcState<'x> = IrcConn -> 'x * IrcConn

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module IrcConn =
    let private getWriter()  = fun c -> c |> snd |> fst, c
    let private getReader()  = fun c -> c |> snd |> snd, c
    let set conn: unit IrcState =
        let setting = fun c -> 
            let tcp = new TcpClient();
            tcp.Connect(conn.Server, conn.Port)
            let reader = new StreamReader(tcp.GetStream())
            let writer = new StreamWriter(tcp.GetStream())
            (), (conn, (writer, reader))
        setting
    let send cmd: _ IrcState =
        irc {
            let! wr = getWriter()
            wr.WriteLine(OutboundIrcCommand.serialize(cmd))
        }
    let readStream(): _ IrcState =
        irc {
            let! rd = getReader()
            return Seq.unfold(fun(rd: StreamReader) -> 
                if not <| rd.EndOfStream then 
                    Some(rd.ReadLine() |> InboundIrcCommand.parse, rd) 
                else 
                    None
            ) rd
        }