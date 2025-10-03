namespace Resourcerer.Logic.V1.Foos

open Resourcerer.Logic.Abstractions
open Resourcerer.Models.Dtos.V1
open Resourcerer.DataAccess.Entities

type IGetHandlerRepo =
    abstract member Query: unit -> Async<FooRow array>

type GetHandlerRepo(rr: IRowRepository) =
    interface IGetHandlerRepo with
        member _.Query () = rr.Query(fun _ -> true)
    
type GetHandler(repo: IGetHandlerRepo) =
    interface IAsyncHandler<unit, FooDto array> with
        member _.Handle (req) = async {
            let! rows = repo.Query ()
            let result = rows |> Array.map FooDto.FromRow
            
            return Ok (result)
        }