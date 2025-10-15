namespace Resourcerer.Api.Services.Messaging.V1

open Resourcerer.Messaging.MailboxProcessors.Types
open Microsoft.Extensions.DependencyInjection
open Resourcerer.Messaging.MailboxProcessors.Abstractions
open Resourcerer.Models.Messages
open Resourcerer.Logic.Abstractions
open Resourcerer.Models.Primitives
open Resourcerer.Models.Domain
open Resourcerer.Logic.V1.Foos
open Microsoft.AspNetCore.Http
open Resourcerer.Api.Services.Functions
open Resourcerer.Models.Dtos.V1

type UpdateRowAsyncReplyProcessor(scopeFactory: IServiceScopeFactory) =
    let processor = AsyncReplyProcessor()
    interface IAsyncReplyProcessor<UpdateRowMessage, IResult> with
        member _.Post (message: UpdateRowMessage) = async {
            let scope = scopeFactory.CreateScope()
            match message with
            | FooUpdate row ->
                let handler = 
                    scope.ServiceProvider.GetRequiredService<V1UpdateHandler>()
                    :> IAsyncHandler<DbRow<Foo>, DbRow<Foo>>
                let! result = processor.Post row handler.Handle
                return mapHttpResponse result (Some (fun x -> Results.Ok (FooDto.FromRow x)))
        }
    
    