namespace Resourcerer.Models.Abstractions

type IDto = interface end

type IRequest<'a, 'b> =
    inherit IDto
    abstract member Validate: unit -> Result<'b, string list>