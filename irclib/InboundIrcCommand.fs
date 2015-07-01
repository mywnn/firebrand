// irc command parsing. See http://tools.ietf.org/html/rfc2812
namespace irclib.IrcCommand

open System
open System.Text.RegularExpressions
open Common.Extensions

type InboundIrcCommand =
    | Ping of String
    | Notice of String
    | ChannelMessage of String * String
    | UserMessage of String * String
    | Unrecognized of String

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module InboundIrcCommand =
    // holds precompiled regexs for parsing  
    let PING = Regex("PING (.*)")
    let NOTICE = Regex(".* NOTICE (.*)")
    let CHANNELMSG = Regex(".* PRIVMSG (#.*) :(.*)")
    let USERMSG = Regex(":(.*)!.* PRIVMSG .* :(.*)")

    // convert an incoming line from the irc server into an InboundIrcCommand
    let parse (msg:String) = 
        match msg with
        | Match PING [server]-> Ping(server)
        | Match NOTICE [notice] -> Notice(notice)
        | Match CHANNELMSG [channel; msg] -> ChannelMessage(channel, msg)
        | Match USERMSG [user; msg] -> UserMessage(user, msg)
        | _ -> Unrecognized(msg)