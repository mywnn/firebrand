// irc command serializing. See http://tools.ietf.org/html/rfc2812
namespace irclib.IrcCommand

open System

type OutboundIrcCommand =
    | Pong of String
    | Nick of String
    | User of String * String
    | Join of String
    | OutMessage of String * String
    
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module OutboundIrcCommand =
    // convert an OutboundIrcCommand into a string that can be understood by an IRC server
    let serialize cmd = function
        | Pong server -> "PONG " + server
        | Nick nick -> "NICK " + nick
        | User (username, realname) -> sprintf "USER %s 8 * :%s" username realname
        | Join channel -> "JOIN " + channel
        | OutMessage (target, msg) -> "PRIVMSG " + target + " :" + msg