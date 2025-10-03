namespace Resourcerer.Logic.V1.Foos

open System
open Resourcerer.Logic.Abstractions
open Resourcerer.DataAccess.Entities

type IV1RemoveRepo =
    inherit IRepository
    abstract member FindById: id: Guid -> Async<FooRow option>
    abstract member Remove: FooRow -> Async<int>

type V1RemoveHandler(repo: IV1RemoveRepo) =
    interface IAsyncVoidHandler<Guid> with
        member _.Handle (req) = async {
            let! data = repo.FindById (req)
            match data with
            | None -> return ()
            | Some row ->
                let! _ = repo.Remove row
                return ()
        }

type V1RemoveRepo(rr: IRowRepository) =
    interface IV1RemoveRepo with
        member _.FindById (id: Guid): Async<FooRow option> = rr.FindById id
        member _.Remove (row): Async<int> =
            rr.Remove row
            rr.Commit ()