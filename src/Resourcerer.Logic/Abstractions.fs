namespace Resourcerer.Logic.Abstractions

open Resourcerer.Logic.Types

type IAsyncHandler<'a, 'b> =
    abstract member Handle: request: 'a -> Async<Result<'b, AppError>>

type IAsyncVoidHandler<'a> =
    abstract member Handle: request: 'a -> Async<unit>