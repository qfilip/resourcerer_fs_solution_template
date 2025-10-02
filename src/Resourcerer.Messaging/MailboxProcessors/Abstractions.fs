namespace Resourcerer.Messaging.MailboxProcessors.Abstractions

type IAsyncVoidProcessor<'a> =
    abstract member Post: 'a -> unit

type IAsyncReplyProcessor<'a, 'b> =
    abstract member Post: 'a -> Async<'b>