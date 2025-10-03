namespace Resourcerer.Logic.V1.Foos

open System
open Microsoft.EntityFrameworkCore
open Resourcerer.Logic.Abstractions
open Resourcerer.DataAccess.Contexts
open Resourcerer.Utilities.Common
open Resourcerer.DataAccess.Enums
open Resourcerer.DataAccess.Entities

type IV1DeleteRepo =
    inherit IRepository
    abstract member Find: id: Guid -> Async<FooRow option>
    abstract member Delete: FooRow -> Async<int>

type V1DeleteHandler(repo: IV1DeleteRepo) =
    interface IAsyncVoidHandler<Guid> with
        member _.Handle (req) = async {
            let! data = repo.Find (req)
            match data with
            | None -> return ()
            | Some row ->
                let! _ = repo.Delete row
                return ()
        }

type V1DeleteRepo(db: AppDbContext) =
    interface IV1DeleteRepo with
        member _.Find (id: Guid): Async<FooRow option> = async {
            let! data = db.Foos.FirstOrDefaultAsync(fun x -> x.Id = id) |> Async.AwaitTask
            return OptionExt.ofNullable data
        }
        member _.Delete (row): Async<int> = async {
            row.EntityStatus <- eEntityStatus.Deleted
            return! db.SaveChangesAsync() |> Async.AwaitTask
        }