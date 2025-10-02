module Resourcerer.Messaging.DependencyInjection

open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open Resourcerer.Utilities.Configuration
open System.Reflection
open System
open Resourcerer.Messaging.MailboxProcessors.Abstractions

let private scan (itype: Type) (assembly: Assembly) =
    let processors =
        assembly.GetTypes()
        |> Array.filter (fun t ->
            t.GetInterface(itype.Name) <> null &&
            not t.IsAbstract &&
            not t.IsInterface
        )
    
    processors
    |> Array.map (fun p ->
        let abstractions =
            p.GetInterfaces()
            |> Array.filter (fun i -> i.IsGenericType)
        let abstraction = abstractions |> Array.exactlyOne
        (abstraction, p)
    )

let register (services: IServiceCollection) (conf: IConfiguration) (assembly: Assembly) =
    let section = loadSection conf "Messaging"
    let useMailboxProcessors = load<bool> section "UseMailboxProcessors"
    if not useMailboxProcessors then () else

    let asyncVoidProcessors = scan typeof<IAsyncVoidProcessor<_>> assembly
    let asyncReplyProcessors = scan typeof<IAsyncReplyProcessor<_,_>> assembly

    asyncVoidProcessors
    |> Array.iter (fun (abs, impl) -> services.AddSingleton(abs, impl) |> ignore)

    asyncReplyProcessors
    |> Array.iter (fun (abs, impl) -> services.AddSingleton(abs, impl) |> ignore)