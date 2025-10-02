namespace Resourcerer.Api.Endpoints.V1.Foos

open System
open Microsoft.AspNetCore.Http
open Resourcerer.Models.Dtos.V1
open Resourcerer.Logic.V1.Foos
 open Resourcerer.Api.Endpoints.Functions

type Create() =
    let handler =
        Func<CreateFooRequest, CreateHandler, Async<IResult>>(
            fun 
                ([<FromBody>] request: CreateFooRequest)
                ([<FromService>] handler: CreateHandler) -> async {
                return Results.Ok()
            }
        )

    interface IEndpoint with
        member _.Major with get () = 1
        member _.Minor with get () = 0
        member _.Path with get () = "foos/"
        member _.HttpMethod with get () = HttpMethod.Put
        member _.Handler with get () = handler :> Delegate