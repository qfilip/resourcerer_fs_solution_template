namespace Resourcerer.Logic.Repositories

open System
open Resourcerer.DataAccess.Contexts
open Resourcerer.Logic.Abstractions
open Microsoft.EntityFrameworkCore
open Resourcerer.Utilities.Common
open Resourcerer.DataAccess.Enums

type RowRepository(db: AppDbContext) =
    interface IRowRepository with
        member _.Add (row: 'a): 'a = 
            row.Id <- Guid.NewGuid()
            db.Set<'a>().Add row |> ignore
            row
        
        member _.FindById (id: Guid): Async<'a option> = async {
            let dbSet = db.Set<'a>()
            let! result = dbSet.FirstOrDefaultAsync(fun x -> x.Id = id) |> Async.AwaitTask
            return OptionExt.ofNullable result
        }

        member _.Remove (row: 'a): unit = row.EntityStatus <- eEntityStatus.Deleted
        
        member _.Commit (): Async<int> = async {
            return! db.SaveChangesAsync() |> Async.AwaitTask
        }