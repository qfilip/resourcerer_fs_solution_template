namespace Resourcerer.Logic.Types

type AppError =
| Validation of string list
| NotFound of string
| Rejected of string
| DataCorruption of string list
| InternalError of exn
with
    static member toDataCorrupted (xr: Result<'a, string list>) =
        match xr with
        | Ok x -> Ok x
        | Error es -> Error (DataCorruption es)