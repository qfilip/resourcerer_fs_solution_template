namespace Resourcerer.Api
#nowarn "20"

open Resourcerer.Api.Endpoints
open Resourcerer.Api.HttpMiddleware
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =
        let builder = WebApplication.CreateBuilder(args)

        let authEnabled = DependencyInjection.register builder

        let app = builder.Build()

        app.UseHttpsRedirection()

        VersionMapping.findAllEndpointVersions ()
        |> VersionMapping.mapEndpoints app

        if authEnabled then
            app.UseAuthentication()
            app.UseAuthorization() |> ignore
        else ()
        
        app.UseMiddleware<HttpErrorMiddleware>()

        app.Run()

        exitCode
