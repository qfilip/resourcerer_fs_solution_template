namespace Resourcerer.Models.Dtos.V1

open System
open Resourcerer.Models.Abstractions
open Resourcerer.Models.Domain.Foos
open Resourcerer.DataAccess.Entities

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
with interface IRequest<Foo> with
        member this.Validate () = tryCreate this.Text

type UpdateFooRequest = {
    Id: Guid
    Text: string
}
with interface IRequest<Foo> with
        member this.Validate () = tryCreate this.Text