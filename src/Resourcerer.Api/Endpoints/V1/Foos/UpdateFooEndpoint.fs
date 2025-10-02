namespace Resourcerer.Api.Endpoints.V1.Foos

open System
open Microsoft.AspNetCore.Http
open Resourcerer.Models.Dtos.V1
open Resourcerer.Api.Endpoints.Functions
open Resourcerer.Api.Services.Messaging.V1

type UpdateFooEndpoint() =
    let handler =
        Func<UpdateFooRequest, UpdateFooAsyncReplyProcessor, Async<IResult>>(
            fun 
                ([<FromBody>] request: UpdateFooRequest)
                ([<FromService>] processor: UpdateFooAsyncReplyProcessor) ->
                pipeMessage request processor None
        )

    interface IEndpoint with
        member _.Major with get () = 1
        member _.Minor with get () = 0
        member _.Path with get () = "foos/"
        member _.HttpMethod with get () = HttpMethod.Patch
        member _.Handler with get () = handler :> Delegate