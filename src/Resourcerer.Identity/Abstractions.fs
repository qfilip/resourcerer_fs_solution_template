namespace Resourcerer.Identity.Abstractions

open System.Security.Claims
open System.Collections.Generic

type IAppIdentityService<'T> =
    abstract member Set: identity: 'T -> unit
    abstract member Set: claims: IEnumerable<Claim> -> unit
    abstract member Identity: 'T with get