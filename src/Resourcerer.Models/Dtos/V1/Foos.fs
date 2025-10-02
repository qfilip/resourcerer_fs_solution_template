namespace Resourcerer.Models.Dtos.V1

open System
open Resourcerer.Models.Abstractions
open Resourcerer.Models.Domain.Foos
open Resourcerer.DataAccess.Entities
open Resourcerer.Models.Primitives

type FooDto = {
    Id: Guid
    Text: string
}
with
    static member FromRow (x: FooRow) =
        { Id = x.Id; Text = x.Text }

type CreateFooRequest = {
    Text: string
}
with interface IRequest<Foo, Foo> with
        member this.Validate () = tryCreate this.Text

type UpdateFooRequest = {
    Id: Guid
    Text: string
}
with interface IRequest<Foo, Row<Foo>> with
        member this.Validate () = 
            tryCreate this.Text
            |> Result.map(fun data -> { Id = this.Id; Data = data })