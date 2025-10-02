module Resourcerer.Api.Endpoints.Functions

open System
open Microsoft.AspNetCore.Http
open Resourcerer.Logic.Abstractions
open Resourcerer.Models.Abstractions
open Resourcerer.Logic.Types
open Resourcerer.Messaging.MailboxProcessors.Abstractions

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

let pipe<'dto, 'req, 'res, 'handler when 'handler :> IAsyncHandler<'req, 'res>>
    (request: IRequest<'dto, 'req>)
    (handler: 'handler)
    (okMapper: ('res -> IResult) option) = async {
        match request.Validate() with
        | Error es -> return Results.BadRequest es
        | Ok validated ->
            let! result = handler.Handle validated
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

let pipeEmpty<'res, 'handler when 'handler :> IAsyncHandler<unit, 'res>>
    (handler: 'handler)
    (okMapper: ('res -> IResult) option) = async {
        let! result = handler.Handle ()
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

let pipeMessage<'dto, 'req, 'res, 'processor when 'processor :> IAsyncReplyProcessor<'req, Result<'res, AppError>>>
    (request: IRequest<'dto, 'req>)
    (processor: 'processor)
    (okMapper: ('res -> IResult) option) = async {
        match request.Validate() with
        | Error es -> return Results.BadRequest es
        | Ok validated ->
            let! result = processor.Post validated
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

let pipeVoidMessage<'dto, 'req, 'res, 'processor when 'processor :> IAsyncVoidProcessor<'req>>
    (request: IRequest<'dto, 'req>)
    (processor: 'processor)
    (response: 'res)
    (okMapper: ('res -> IResult) option) = async {
        match request.Validate() with
        | Error es -> return Results.BadRequest es
        | Ok validated ->
            do processor.Post validated
            match okMapper with
            | Some mapper -> return mapper response
            | None -> return Results.Ok response
    }