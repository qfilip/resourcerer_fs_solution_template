module Resourcerer.Messaging.DependencyInjection

open System.Reflection
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open Resourcerer.Utilities.Configuration
open Resourcerer.Utilities.Reflection
open Resourcerer.Messaging.MailboxProcessors.Abstractions

let register (services: IServiceCollection) (conf: IConfiguration) (assembly: Assembly) =
    let section = loadSection conf "Messaging"
    let useMailboxProcessors = load<bool> section "UseMailboxProcessors"
    if not useMailboxProcessors then () else

    let asyncVoidProcessors = scanGeneric typeof<IAsyncVoidProcessor<_>> assembly
    let asyncReplyProcessors = scanGeneric typeof<IAsyncReplyProcessor<_,_>> assembly

    asyncVoidProcessors
    |> Array.iter (fun (_, impl) -> services.AddSingleton(impl) |> ignore)

    asyncReplyProcessors
    |> Array.iter (fun (_, impl) -> services.AddSingleton(impl) |> ignore)