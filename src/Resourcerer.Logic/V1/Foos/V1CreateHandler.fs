namespace Resourcerer.Logic.V1.Foos

open Resourcerer.Logic.Abstractions
open Resourcerer.Models.Primitives
open Resourcerer.Models.Domain
open Resourcerer.DataAccess.Entities
open Resourcerer.Logic.Types

type IV1CreateRepo =
    inherit IRepository
    abstract member Add: row: FooRow -> FooRow
    abstract member Commit: unit -> Async<int>

type V1CreateHandler(repo: IV1CreateRepo) =
    interface IAsyncHandler<Foo, DbRow<Foo>> with
        member _.Handle (req) = async {
            let row =
                req
                |> Foo.mapRow
                |> repo.Add

            try
                let! _ = repo.Commit()
                return Ok ({ Id = row.Id; Data = req })
            with
            | ex -> return Error (InternalError ex)
        }

type V1CreateRepo(rr: IRowRepository) =
    interface IV1CreateRepo with
        member _.Add (row: FooRow): FooRow = rr.Add row
        member _.Commit (): Async<int> = rr.Commit ()