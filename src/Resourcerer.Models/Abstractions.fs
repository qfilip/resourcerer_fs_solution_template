namespace Resourcerer.Models.Abstractions

type IDto = interface end

type IRequest<'a> =
    inherit IDto
    abstract member Validate: unit -> Result<'a, string list>