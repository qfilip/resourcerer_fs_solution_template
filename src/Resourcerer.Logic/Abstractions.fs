namespace Resourcerer.Logic.Abstractions

open System
open Resourcerer.DataAccess.Abstractions
open Resourcerer.Logic.Types
open System.Linq.Expressions

type IAsyncHandler<'a, 'b> =
    abstract member Handle: request: 'a -> Async<Result<'b, AppError>>

type IAsyncVoidHandler<'a> =
    abstract member Handle: request: 'a -> Async<unit>

type IRepository = interface end

type IRowRepository =
    inherit IRepository
    abstract member Add<'a when 'a :> IId<Guid> and 'a : not struct> : row: 'a -> 'a
    abstract member FindById<'a when 'a :> IId<Guid>and 'a : not struct> : id: Guid -> Async<'a option>
    abstract member Remove<'a when 'a :> IId<Guid>and 'a : not struct and 'a :> ISoftDeletable> : row: 'a -> unit
    abstract member Commit: unit -> Async<int>