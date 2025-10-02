namespace Resourcerer.Models.Abstractions

open System

type IDto = interface end

type IEntityDto<'a> =
    inherit IDto
    abstract member Id: Guid
    abstract member Data: 'a

type IEntityRequest<'a> =
    inherit IDto
    abstract member Validate: unit -> Result<'a, string list>