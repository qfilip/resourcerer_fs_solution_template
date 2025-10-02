module Resourcerer.Logic.DependencyInjection

open System.Reflection
open Resourcerer.Utilities.Reflection
open Microsoft.Extensions.DependencyInjection
open Resourcerer.Logic.Abstractions

let register (services: IServiceCollection) =
    let interfaceType = typeof<IAsyncHandler<_,_>>
    let assembly = Assembly.GetExecutingAssembly()

    let handlers = scanGeneric interfaceType assembly
    handlers
    |> Array.iter (fun (_, impl) ->
        services.AddScoped(impl) |> ignore
    )
    