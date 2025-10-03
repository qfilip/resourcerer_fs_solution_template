module Resourcerer.Logic.DependencyInjection

open System.Reflection
open Resourcerer.Utilities.Reflection
open Microsoft.Extensions.DependencyInjection
open Resourcerer.Logic.Abstractions
open Resourcerer.Logic.Repositories

let private addHandlers (services: IServiceCollection) (assembly: Assembly) =
    let asyncHandlerType = typeof<IAsyncHandler<_,_>>
    let asyncHandlers = scanGeneric asyncHandlerType assembly
    asyncHandlers
    |> Array.iter (fun (_, impl) -> services.AddScoped(impl) |> ignore)

    let asyncVoidHandlerType = typeof<IAsyncVoidHandler<_>>
    let asyncVoidHandlers = scanGeneric asyncVoidHandlerType assembly
    asyncVoidHandlers
    |> Array.iter (fun (_, impl) -> services.AddScoped(impl) |> ignore)

let private addRepositories (services: IServiceCollection) (assembly: Assembly) =
    let repositoryType = typeof<IRepository>
    let repositories = scanRepositories repositoryType assembly
    
    repositories |> Array.iter (fun (abs, impl) -> services.AddScoped(abs, impl) |> ignore)

let register (services: IServiceCollection) =
    let assembly = Assembly.GetExecutingAssembly()
    
    addHandlers services assembly
    addRepositories services assembly
    