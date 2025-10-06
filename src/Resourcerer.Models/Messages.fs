namespace Resourcerer.Models.Messages

open Resourcerer.Models.Primitives
open Resourcerer.Models.Domain.Foos

type UpdateRowMessage =
| FooUpdate of DbRow<Foo>