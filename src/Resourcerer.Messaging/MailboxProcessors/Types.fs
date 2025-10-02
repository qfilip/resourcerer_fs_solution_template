namespace Resourcerer.Messaging.MailboxProcessors.Types

open Resourcerer.Messaging.MailboxProcessors.Abstractions

type ReplyProcessorMessage<'a, 'b> = 'a * AsyncReplyChannel<'b>

type AsyncVoidProcessor<'a>(asyncHandler: 'a -> Async<unit>) =
    let agent = MailboxProcessor<'a>.Start(fun mailbox ->
        let rec loop () = async {
            let! message = mailbox.Receive()
            do! asyncHandler message
            
            do! loop ()
        }

        loop ()
    )
    interface IAsyncVoidProcessor<'a> with
        member _.Post(x) = agent.Post x

type AsyncReplyProcessor<'a, 'b>(asyncHandler: 'a -> Async<'b>) =
    let agent = MailboxProcessor<ReplyProcessorMessage<'a, 'b>>.Start(fun mailbox ->
        let rec loop () = async {
            let! message, rc = mailbox.Receive()
            let! reply = asyncHandler message
            rc.Reply(reply)
            
            do! loop ()
        }

        loop ()
    )
    interface IAsyncReplyProcessor<'a, 'b> with
        member _.Post(x) = agent.PostAndAsyncReply(fun rc -> x, rc)

