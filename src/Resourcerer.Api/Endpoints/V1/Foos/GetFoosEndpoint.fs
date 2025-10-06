namespace Resourcerer.Api.Endpoints.V1.Foos

open System
open Microsoft.AspNetCore.Http
open Resourcerer.Logic.V1.Foos
open Resourcerer.Models.Dtos.V1
open Resourcerer.Api.Endpoints.Types
open Resourcerer.Api.Services.Functions

type GetFoosEndpoint() =
    let handler =
        Func<V1QueryHandler, Async<IResult>>(
            fun 
                ([<FromService>] handler: V1QueryHandler) ->
                pipeEmpty handler (Some (fun xs -> Results.Ok (xs |> List.map FooDto.FromRow)))
        )

    interface IEndpoint with
        member _.Major with get () = 1
        member _.Minor with get () = 0
        member _.Path with get () = "foos/"
        member _.HttpMethod with get () = HttpMethod.Get
        member _.Handler with get () = handler :> Delegate