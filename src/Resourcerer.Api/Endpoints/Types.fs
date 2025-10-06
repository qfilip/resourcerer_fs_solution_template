namespace Resourcerer.Api.Endpoints.Types

open System

type HttpMethod =
| Get
| Put
| Patch
| Post
| Delete

type IEndpoint =
    abstract member Major: int with get
    abstract member Minor: int with get
    abstract member Path: string with get
    abstract member HttpMethod: HttpMethod with get
    abstract member Handler: Delegate with get