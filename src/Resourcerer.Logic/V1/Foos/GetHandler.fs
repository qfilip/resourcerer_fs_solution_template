namespace Resourcerer.Logic.V1.Foos

open Resourcerer.Logic.Abstractions
open Resourcerer.DataAccess.Contexts
open Microsoft.EntityFrameworkCore
open Resourcerer.Models.Dtos.V1

type GetHandler(db: AppDbContext) =
    interface IAsyncHandler<unit, FooDto array> with
        member _.Handle (req) = async {
            let! rows = db.Foos.ToArrayAsync() |> Async.AwaitTask
            let result = rows |> Array.map FooDto.FromRow
            
            return Ok (result)
        }