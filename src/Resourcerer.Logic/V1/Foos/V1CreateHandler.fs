namespace Resourcerer.Logic.V1.Foos

open Resourcerer.Logic.Abstractions
open Resourcerer.Models.Domain.Foos
open Resourcerer.Models.Dtos.V1
open Resourcerer.DataAccess.Entities

type IV1CreateRepo =
    inherit IRepository
    abstract member Add: row: FooRow -> FooRow
    abstract member Commit: unit -> Async<int>

type V1CreateHandler(repo: IV1CreateRepo) =
    interface IAsyncHandler<Foo, FooDto> with
        member _.Handle (req) = async {
            let row =
                req
                |> mapRow
                |> repo.Add
            let! _ = repo.Commit()

            return Ok (FooDto.FromRow row)
        }

type V1CreateRepo(rr: IRowRepository) =
    interface IV1CreateRepo with
        member _.Add (row: FooRow): FooRow = rr.Add row
        member _.Commit (): Async<int> = rr.Commit ()