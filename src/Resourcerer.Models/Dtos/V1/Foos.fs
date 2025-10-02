namespace Resourcerer.Models.Dtos.V1

open System
open Resourcerer.Models.Abstractions
open Resourcerer.Models.Domain.Foos

type Data = {
    Text: string
}

type FooDto = {
    Id: Guid
    Data: Data
}
with interface IEntityDto<Data> with
       member this.Id = this.Id
       member this.Data = this.Data

type CreateRequest = {
    Text: string
}
with interface IEntityRequest<Foo> with
        member this.Validate () = tryCreate this.Text

type UpdateRequest = {
    Id: Guid
    Text: string
}
with interface IEntityRequest<Foo> with
        member this.Validate () = tryCreate this.Text