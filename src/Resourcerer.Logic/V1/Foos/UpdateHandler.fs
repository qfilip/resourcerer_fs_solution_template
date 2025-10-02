namespace Resourcerer.Logic.V1.Foos

open Microsoft.EntityFrameworkCore
open Resourcerer.Logic.Abstractions
open Resourcerer.DataAccess.Contexts
open Resourcerer.Models.Domain.Foos
open Resourcerer.Models.Dtos.V1
open Resourcerer.Models.Primitives
open Resourcerer.Utilities.Common
open Resourcerer.Logic.Types

type UpdateHandler(db: AppDbContext) =
    interface IAsyncHandler<Row<Foo>, FooDto> with
        member _.Handle (req) = async {
            let! rowData =
                db.Foos.FirstOrDefaultAsync(fun x -> x.Id = req.Id)
                |> Async.AwaitTask

            match OptionExt.ofNullable rowData with
            | None -> return Error (AppError.NotFound $"{req.Id}")
            | Some row ->
                row.Text <- req.Data.Text |> Min2String.unmap
                let! _ = db.SaveChangesAsync() |> Async.AwaitTask

                return Ok (FooDto.FromRow row)
        }