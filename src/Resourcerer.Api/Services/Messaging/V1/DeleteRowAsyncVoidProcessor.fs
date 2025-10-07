namespace Resourcerer.Api.Services.Messaging.V1

open System
open Resourcerer.Logic.V1.Foos
open Resourcerer.Messaging.MailboxProcessors.Types
open Microsoft.Extensions.DependencyInjection
open Resourcerer.Messaging.MailboxProcessors.Abstractions
open Resourcerer.Logic.Abstractions
open Resourcerer.Models.Messages

type DeleteRowAsyncVoidProcessor(scopeFactory: IServiceScopeFactory) =
    let processor = AsyncVoidProcessor()
    
    interface IAsyncVoidProcessor<DeleteRowMessage> with
        member _.Post (message: DeleteRowMessage) =
            let (data, handler) =
                match message with
                | Foo id ->
                    let scope = scopeFactory.CreateScope()
                    let handler = 
                        scope.ServiceProvider.GetRequiredService<V1RemoveHandler>()
                        :> IAsyncVoidHandler<Guid>
                    (id, handler.Handle)

            processor.Post data handler
    
    