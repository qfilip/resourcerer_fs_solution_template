namespace Resourcerer.Api
#nowarn "20"
open System.Reflection

open Resourcerer.Api.HttpMiddleware

open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =
        let builder = WebApplication.CreateBuilder(args)

        builder.Services.AddCors(fun b -> b.AddDefaultPolicy(
            fun o -> 
                o.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin() |> ignore) |> ignore) |> ignore

        let authEnabled = Resourcerer.Identity.DependencyInjection.register builder
        Resourcerer.DataAccess.DependencyInjection.Register builder
        Resourcerer.Messaging.DependencyInjection.register builder.Services builder.Configuration (Assembly.GetExecutingAssembly())

        let app = builder.Build()

        app.UseHttpsRedirection()

        if authEnabled then
            app.UseAuthentication()
            app.UseAuthorization() |> ignore
        else ()
        
        app.UseMiddleware<HttpErrorMiddleware>()

        app.Run()

        exitCode
