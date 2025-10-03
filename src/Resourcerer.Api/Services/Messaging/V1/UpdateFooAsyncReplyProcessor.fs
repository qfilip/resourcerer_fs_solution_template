namespace Resourcerer.Api.Services.Messaging.V1

open Resourcerer.Logic.V1.Foos
open Resourcerer.Messaging.MailboxProcessors.Types
open Microsoft.Extensions.DependencyInjection
open Resourcerer.Messaging.MailboxProcessors.Abstractions
open Resourcerer.Models.Domain.Foos
open Resourcerer.Models.Dtos.V1
open Resourcerer.Logic.Abstractions
open Resourcerer.Logic.Types
open Resourcerer.Models.Primitives

type UpdateFooAsyncReplyProcessor(scopeFactory: IServiceScopeFactory) =
    let processor = AsyncReplyProcessor()
    
    interface IAsyncReplyProcessor<Row<Foo>, Result<FooDto, AppError>> with
        member _.Post (x: Row<Foo>) = async {
            let scope = scopeFactory.CreateScope()
            let handler = 
                scope.ServiceProvider.GetRequiredService<V1UpdateHandler>()
                :> IAsyncHandler<Row<Foo>, FooDto>
            
            return! processor.Post x handler.Handle
        }
    
    