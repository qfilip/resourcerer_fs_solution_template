namespace Resourcerer.Identity.Services

open Microsoft.AspNetCore.Authentication.JwtBearer
open Resourcerer.Identity.Abstractions
open Resourcerer.Identity.Models

type AppJwtBearerEventService(identityService: IAppIdentityService<AppIdentity>) =
    inherit JwtBearerEvents()

    override _.TokenValidated (context: TokenValidatedContext): System.Threading.Tasks.Task = task {
        if context.Principal = null
        then failwith "User principal not found"
        else identityService.Set(context.Principal.Claims)

        return ()
    }

