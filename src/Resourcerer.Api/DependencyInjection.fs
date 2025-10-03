module Resourcerer.Api.DependencyInjection

open System.Reflection
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection

let register (builder: WebApplicationBuilder) =
    builder.Services.AddCors(fun b -> b.AddDefaultPolicy(
        fun o -> 
            o.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin() |> ignore) |> ignore) |> ignore

    let authEnabled = Resourcerer.Identity.DependencyInjection.register builder
    Resourcerer.Messaging.DependencyInjection.register builder.Services builder.Configuration (Assembly.GetExecutingAssembly())
    Resourcerer.Logic.DependencyInjection.register builder.Services
    Resourcerer.DataAccess.DependencyInjection.Register (builder.Services, builder.Environment)

    authEnabled

