module Resourcerer.Models.Domain.Foos

open Resourcerer.Models.Primitives
open Resourcerer.DataAccess.Entities

type Foo = {
    Text: Min2String
}
with
    static member tryCreate (text: string) =
        Min2String.tryFrom text
        |> Result.map (fun x -> { Text = x })

    static member mapRow (x: Foo) =
        let row = FooRow()
        row.Text <- x.Text |> Min2String.unmap
        row

    static member mapDbRow (x: FooRow) =
        Foo.tryCreate x.Text
        |> Result.map (fun data -> { Id = x.Id; Data = data })