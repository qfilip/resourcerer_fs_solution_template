namespace Resourcerer.Logic.V1.Foos

open Resourcerer.Logic.Abstractions
open Resourcerer.DataAccess.Entities
open Resourcerer.DataAccess.Contexts
open Microsoft.EntityFrameworkCore
open Resourcerer.Models.Primitives
open Resourcerer.Models.Domain
open Resourcerer.Utilities.Common.ResultExt
open Resourcerer.Logic.Types

type IV1QueryRepo =
    inherit IRepository
    abstract member Query: unit -> Async<FooRow array>
    
type V1QueryHandler(repo: IV1QueryRepo) =
    interface IAsyncHandler<unit, DbRow<Foo> list> with
        member _.Handle (_) = async {
            try
                let! rows = repo.Query ()
                let results =
                    rows
                    |> Array.map (fun x -> Foo.mapDbRow x)
                    |> List.ofArray
                    |> verifyList
                    |> AppError.toDataCorrupted
            
                return results
            with
            | ex -> return Error (InternalError ex)
        }

type V1QueryRepo(db: AppDbContext) =
    interface IV1QueryRepo with
        member _.Query () = async {
            return! db.Foos.ToArrayAsync() |> Async.AwaitTask
        }