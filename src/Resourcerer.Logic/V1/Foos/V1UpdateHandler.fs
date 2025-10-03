namespace Resourcerer.Logic.V1.Foos

open System
open Resourcerer.Logic.Abstractions
open Resourcerer.Models.Domain.Foos
open Resourcerer.Models.Dtos.V1
open Resourcerer.Models.Primitives
open Resourcerer.Logic.Types
open Resourcerer.DataAccess.Entities
open Resourcerer.DataAccess.Contexts
open Microsoft.EntityFrameworkCore
open Resourcerer.Utilities.Common

type IV1UpdateRepo =
    inherit IRepository
    abstract member Find: id: Guid -> Async<FooRow option>
    abstract member Commit: unit -> Async<int>

type V1UpdateHandler(repo: IV1UpdateRepo) =
    interface IAsyncHandler<Row<Foo>, FooDto> with
        member _.Handle (req) = async {
            let! data = repo.Find req.Id

            match data with
            | None -> return Error (AppError.NotFound $"{req.Id}")
            | Some row ->
                row.Text <- req.Data.Text |> Min2String.unmap
                let! _ = repo.Commit ()
                return Ok (FooDto.FromRow row)
        }

type V1UpdateRepo(db: AppDbContext) =
    interface IV1UpdateRepo with
        member _.Find (id: Guid): Async<FooRow option> = async {
            let! data = db.Foos.FirstOrDefaultAsync(fun x -> x.Id = id) |> Async.AwaitTask
            return OptionExt.ofNullable data
        }
        member _.Commit (): Async<int> = async { return! db.SaveChangesAsync() |> Async.AwaitTask }