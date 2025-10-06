namespace Resourcerer.Api.Endpoints.V1.Foos

open System
open Microsoft.AspNetCore.Http
open Resourcerer.Api.Endpoints.Functions
open Resourcerer.Api.Services.Messaging.V1
open Resourcerer.Models.Primitives

type DeleteFooEndpoint() =
    let handler =
        Func<Guid, DeleteRowAsyncVoidProcessor, Async<IResult>>(
            fun 
                ([<FromRoute>] id: Guid)
                ([<FromService>] processor: DeleteRowAsyncVoidProcessor) ->
                let req: ValidatedRequest<Guid> = { Data = id }
                pipeVoidMessage req processor $"Deletion request sent for {id}" (Some (fun x -> Results.Accepted x))
        )

    interface IEndpoint with
        member _.Major with get () = 1
        member _.Minor with get () = 0
        member _.Path with get () = "foos/{id}"
        member _.HttpMethod with get () = HttpMethod.Delete
        member _.Handler with get () = handler :> Delegate