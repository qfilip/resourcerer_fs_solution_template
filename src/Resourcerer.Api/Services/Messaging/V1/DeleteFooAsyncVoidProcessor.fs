namespace Resourcerer.Api.Services.Messaging.V1

open Resourcerer.Logic.V1.Foos
open Resourcerer.Messaging.MailboxProcessors.Types
open Microsoft.Extensions.DependencyInjection
open Resourcerer.Messaging.MailboxProcessors.Abstractions
open Resourcerer.Logic.Abstractions
open System

type DeleteRowAsyncVoidProcessor(scopeFactory: IServiceScopeFactory) =
    let processor = AsyncVoidProcessor()
    
    interface IAsyncVoidProcessor<Guid> with
        member _.Post (x: Guid) =
            let scope = scopeFactory.CreateScope()
            let handler = 
                scope.ServiceProvider.GetRequiredService<V1RemoveHandler>()
                :> IAsyncVoidHandler<Guid>
            
            processor.Post x handler.Handle
    
    