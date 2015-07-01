module Common.Extensions

open System.Text.RegularExpressions

//active pattern for regex matching 
let (|Match|_|) (pattern:Regex) input =
    let m = pattern.Match(input) in
    if m.Success then Some (List.tail [ for g in m.Groups -> g.Value ]) else None

type StateMonad() =
    member this.Delay f = fun s -> f() s
    member this.Bind(m, f) = fun s -> m s ||> f
    member this.Return x = fun s -> x, s
    member this.ReturnFrom m = m
    member this.Zero() = fun s -> (), s