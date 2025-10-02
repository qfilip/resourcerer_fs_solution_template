module Resourcerer.Api.Endpoints.Functions

open System
open System.Reflection
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Resourcerer.Logic.Abstractions
open Resourcerer.Models.Abstractions
open Resourcerer.Logic.Types
open Resourcerer.Utilities.Reflection

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

let pipe (request: IRequest<'a>) (handler: IAsyncHandler<'a, 'b>) (okMapper: ('b -> IResult) option) = async {
    match request.Validate() with
    | Error es -> return Results.BadRequest es
    | Ok req ->
        let! result = handler.Handle req
        match result with
        | Ok res ->
            match okMapper with
            | Some mapper -> return mapper res
            | None -> return Results.Ok res
        | Error appError ->
            match appError with
            | AppError.Validation e -> return Results.BadRequest e
            | AppError.NotFound e -> return Results.NotFound e
            | AppError.Rejected e -> return Results.Conflict e     
}