namespace Resourcerer.Logic.V1.Foos

open System
open Resourcerer.Logic.Abstractions
open Resourcerer.Models.Domain.Foos
open Resourcerer.Models.Primitives
open Resourcerer.Logic.Types
open Resourcerer.DataAccess.Entities

type IV1UpdateRepo =
    inherit IRepository
    abstract member FindById: id: Guid -> Async<FooRow option>
    abstract member Commit: unit -> Async<int>

type V1UpdateHandler(repo: IV1UpdateRepo) =
    interface IAsyncHandler<DbRow<Foo>, DbRow<Foo>> with
        member _.Handle (req) = async {
            let! data = repo.FindById req.Id

            match data with
            | None -> return Error (AppError.NotFound $"{req.Id}")
            | Some row ->
                row.Text <- req.Data.Text |> Min2String.unmap
                let! _ = repo.Commit ()
                return Ok (req)
        }

type V1UpdateRepo(rr: IRowRepository) =
    interface IV1UpdateRepo with
        member _.FindById (id: Guid): Async<FooRow option> = rr.FindById id
        member _.Commit (): Async<int> = rr.Commit ()