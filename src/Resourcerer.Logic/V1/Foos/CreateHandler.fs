namespace Resourcerer.Logic.V1.Foos

open System
open Resourcerer.Logic.Abstractions
open Resourcerer.DataAccess.Contexts
open Resourcerer.Models.Domain.Foos
open Resourcerer.Models.Dtos.V1

type CreateHandler(db: AppDbContext) =
    interface IAsyncHandler<Foo, FooDto> with
        member _.Handle (req) = async {
            let entry =
                req
                |> mapRow (Some (fun x ->
                    x.Id <- Guid.NewGuid()
                    x
                ))
                |> db.Foos.Add
            let! _ = db.SaveChangesAsync() |> Async.AwaitTask

            return Ok (FooDto.FromRow entry.Entity)
        }