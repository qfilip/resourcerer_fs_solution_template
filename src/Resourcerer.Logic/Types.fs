namespace Resourcerer.Logic.Types

type AppError =
| Validation of string list
| NotFound of string
| Rejected of string