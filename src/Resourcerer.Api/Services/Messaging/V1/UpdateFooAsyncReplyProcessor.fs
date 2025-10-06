namespace Resourcerer.Api.Services.Messaging.V1

open Resourcerer.Messaging.MailboxProcessors.Types
open Microsoft.Extensions.DependencyInjection
open Resourcerer.Messaging.MailboxProcessors.Abstractions
open Resourcerer.Models.Messages
open Resourcerer.Logic.Abstractions
open Resourcerer.Models.Primitives
open Resourcerer.Models.Domain.Foos
open Resourcerer.Logic.V1.Foos
open Resourcerer.Logic.Types

type UpdateFooAsyncReplyProcessor(scopeFactory: IServiceScopeFactory) =
    let processor = AsyncReplyProcessor()
    let mapResult = function Ok _ -> Ok () | Error es -> Error es
    
    interface IAsyncReplyProcessor<UpdateRowMessage, Result<unit, AppError>> with
        member _.Post (message: UpdateRowMessage) = async {
            let scope = scopeFactory.CreateScope()
            match message with
            | FooUpdate row ->
                let handler = 
                    scope.ServiceProvider.GetRequiredService<V1UpdateHandler>()
                    :> IAsyncHandler<DbRow<Foo>, DbRow<Foo>>
                    
                let! result = processor.Post row handler.Handle
                return result |> mapResult
        }
    
    