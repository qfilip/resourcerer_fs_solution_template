namespace Resourcerer.Api.HttpMiddleware

open System.Net
open System.Text.Json
open Microsoft.AspNetCore.Http

type HttpErrorMiddleware(next: RequestDelegate) =
    member _.InvokeAsync(context: HttpContext) = task {
        try
            return! next.Invoke(context)
        with
        | ex ->
            let errorDetails = JsonSerializer.Serialize({| Message = ex.Message |})
            context.Response.ContentType <- "application/json"
            context.Response.StatusCode <- (int)HttpStatusCode.InternalServerError

            return! context.Response.WriteAsync(errorDetails)
    }