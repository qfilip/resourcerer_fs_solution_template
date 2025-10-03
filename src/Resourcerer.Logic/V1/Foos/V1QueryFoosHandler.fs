namespace Resourcerer.Logic.V1.Foos

open Resourcerer.Logic.Abstractions
open Resourcerer.Models.Dtos.V1
open Resourcerer.DataAccess.Entities
open Resourcerer.DataAccess.Contexts
open Microsoft.EntityFrameworkCore

type IV1QueryRepo =
    inherit IRepository
    abstract member Query: unit -> Async<FooRow array>
    
type V1QueryHandler(repo: IV1QueryRepo) =
    interface IAsyncHandler<unit, FooDto array> with
        member _.Handle (_) = async {
            let! rows = repo.Query ()
            let result = rows |> Array.map FooDto.FromRow
            
            return Ok (result)
        }

type V1QueryRepo(db: AppDbContext) =
    interface IV1QueryRepo with
        member _.Query () = async {
            return! db.Foos.ToArrayAsync() |> Async.AwaitTask
        }