module Resourcerer.Models.Domain.Foos

open Resourcerer.Models.Primitives
open Resourcerer.DataAccess.Entities
open Resourcerer.Utilities.Common

type Foo = {
    Text: Min2String
}

let tryCreate (text: string) =
    Min2String.tryFrom text
    |> Result.map (fun x -> { Text = x })

let mapRow (modifier) (x: Foo) =
    let row = FooRow()
    row.Text <- x.Text |> Min2String.unmap
    modifier |> OptionExt.mapOrDefault row

