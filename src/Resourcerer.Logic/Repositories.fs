namespace Resourcerer.Logic.Repositories

open System
open System.Linq
open Resourcerer.DataAccess.Contexts
open Resourcerer.Logic.Abstractions
open Microsoft.EntityFrameworkCore

type RowRepository(db: AppDbContext) =
    interface IRowRepository with
        member _.Add (row: 'a): unit = 
            row.Id <- Guid.NewGuid()
            db.Set<'a>().Add row |> ignore

        member _.Query (selector: 'a -> bool): Async<'a array> = async {
            let dbSet = db.Set<'a>()
            return! dbSet.Where(selector).ToArrayAsync() |> Async.AwaitTask
        }
        
        member _.Find (selector: 'a -> bool): Async<'a option> = async {
            let dbSet = db.Set<'a>()
            let! result = dbSet.FirstOrDefaultAsync(selector) |> Async.AwaitTask
            match result with
            | null -> return None
            | x -> return Some x
        }
        
        member _.Commit (): Async<int> = async {
            return! db.SaveChangesAsync() |> Async.AwaitTask
        }