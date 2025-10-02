module Resourcerer.Logic.DependencyInjection

open System.Reflection
open Resourcerer.Utilities.Reflection
open Microsoft.Extensions.DependencyInjection
open Resourcerer.Logic.Abstractions

let register (services: IServiceCollection) =
    let assembly = Assembly.GetExecutingAssembly()
    
    let asyncHandlerType = typeof<IAsyncHandler<_,_>>
    let asyncHandlers = scanGeneric asyncHandlerType assembly
    asyncHandlers
    |> Array.iter (fun (_, impl) -> services.AddScoped(impl) |> ignore)

    let asyncVoidHandlerType = typeof<IAsyncVoidHandler<_>>
    let asyncVoidHandlers = scanGeneric asyncVoidHandlerType assembly
    asyncVoidHandlers
    |> Array.iter (fun (_, impl) -> services.AddScoped(impl) |> ignore)
    