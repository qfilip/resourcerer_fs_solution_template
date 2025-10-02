module Resourcerer.Models.Domain.Foos

open Resourcerer.Models.Primitives

type Foo = {
    Text: Min2String
}

let tryCreate (text: string) =
    Min2String.tryFrom text
    |> Result.map (fun x -> { Text = x })

