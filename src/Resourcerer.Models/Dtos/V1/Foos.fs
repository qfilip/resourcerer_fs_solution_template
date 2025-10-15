namespace Resourcerer.Models.Dtos.V1

open System
open Resourcerer.Models.Abstractions
open Resourcerer.Models.Domain
open Resourcerer.Models.Primitives
open Resourcerer.Models.Messages

type FooDto = {
    Id: Guid
    Text: string
} with
    static member FromRow (x: DbRow<Foo>) =
        { Id = x.Id; Text = x.Data.Text |> Min2String.unmap }

type CreateFooRequest = {
    Text: string
} with interface IRequest<Foo, Foo> with
        member this.Validate () = Foo.tryCreate this.Text

type UpdateFooRequest = {
    Id: Guid
    Text: string
} with interface IRequest<Foo, UpdateRowMessage> with
        member this.Validate () =
            Foo.tryCreate this.Text
            |> Result.map(fun data -> FooUpdate { Id = this.Id; Data = data })

type DeleteFooRequest = {
    Id: Guid
} with interface IRequest<Foo, DeleteRowMessage> with
        member this.Validate () = Ok (DeleteRowMessage.Foo this.Id)