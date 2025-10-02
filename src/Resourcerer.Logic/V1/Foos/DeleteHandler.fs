namespace Resourcerer.Logic.V1.Foos

open System
open Microsoft.EntityFrameworkCore
open Resourcerer.Logic.Abstractions
open Resourcerer.DataAccess.Contexts
open Resourcerer.Utilities.Common
open Resourcerer.DataAccess.Enums

type DeleteHandler(db: AppDbContext) =
    interface IAsyncVoidHandler<Guid> with
        member _.Handle (req) = async {
            let! rowData =
                db.Foos.FirstOrDefaultAsync(fun x -> x.Id = req)
                |> Async.AwaitTask

            match OptionExt.ofNullable rowData with
            | None -> return ()
            | Some row ->
                row.EntityStatus <- eEntityStatus.Deleted
                let! _ = db.SaveChangesAsync() |> Async.AwaitTask
                return ()
        }