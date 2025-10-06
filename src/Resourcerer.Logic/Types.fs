namespace Resourcerer.Logic.Types

type AppError =
| Validation of string list
| NotFound of string
| Rejected of string
with
    member this.listErrors () =
        match this with
        | Validation es -> es
        | NotFound e -> [e]
        | Rejected e -> [e]