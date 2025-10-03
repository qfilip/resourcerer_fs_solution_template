namespace Resourcerer.Logic.Abstractions

open System
open Resourcerer.DataAccess.Abstractions
open Resourcerer.Logic.Types

type IAsyncHandler<'a, 'b> =
    abstract member Handle: request: 'a -> Async<Result<'b, AppError>>

type IAsyncVoidHandler<'a> =
    abstract member Handle: request: 'a -> Async<unit>

type IRepository = interface end

type IRowRepository =
    abstract member Add<'a when 'a :> IId<Guid> and 'a : not struct> : row: 'a -> unit
    abstract member Query<'a when 'a :> IId<Guid>and 'a : not struct> : selector: ('a -> bool) -> Async<'a array>
    abstract member Find<'a when 'a :> IId<Guid>and 'a : not struct> : selector: ('a -> bool) -> Async<'a option>
    abstract member Commit: unit -> Async<int>