namespace Resourcerer.Models.Messages

open System
open Resourcerer.Models.Primitives
open Resourcerer.Models.Domain.Foos

type UpdateRowMessage =
| FooUpdate of DbRow<Foo>

type DeleteRowMessage =
| Foo of Guid