namespace Resourcerer.Messaging.MailboxProcessors.Types

type VoidProcessorMessage<'a> = 'a * ('a -> Async<unit>)
type ReplyProcessorMessage<'a, 'b> = 'a * ('a -> Async<'b>) * AsyncReplyChannel<'b>

type AsyncVoidProcessor<'a>() =
    let agent = MailboxProcessor<VoidProcessorMessage<'a>>.Start(fun mailbox ->
        let rec loop () = async {
            let! message, asyncHandler = mailbox.Receive()
            do! asyncHandler message
            
            do! loop ()
        }

        loop ()
    )
    
    member _.Post x asyncHandler = agent.Post (x, asyncHandler)

type AsyncReplyProcessor<'a, 'b>() =
    let agent = MailboxProcessor<ReplyProcessorMessage<'a, 'b>>.Start(fun mailbox ->
        let rec loop () = async {
            let! message, asyncHandler, rc = mailbox.Receive()
            let! reply = asyncHandler message
            rc.Reply(reply)
            
            do! loop ()
        }

        loop ()
    )
    
    member _.Post x asyncHandler = agent.PostAndAsyncReply(fun rc -> x, asyncHandler, rc)

