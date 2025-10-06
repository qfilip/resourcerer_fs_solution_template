module Resourcerer.Api.Services.Functions

open Microsoft.AspNetCore.Http
open Resourcerer.Logic.Abstractions
open Resourcerer.Models.Abstractions
open Resourcerer.Logic.Types
open Resourcerer.Messaging.MailboxProcessors.Abstractions

let mapHttpResponse (xr: Result<'a, AppError>) (okMapper: ('a -> IResult) option): IResult =
    match xr with
    | Ok x ->
        match okMapper with
        | Some map -> map x
        | None -> Results.Ok x
    | Error appError ->
        match appError with
        | AppError.Validation e -> Results.BadRequest e
        | AppError.NotFound e -> Results.NotFound e
        | AppError.Rejected e -> Results.Conflict e

let pipe<'dto, 'req, 'res, 'handler when 'handler :> IAsyncHandler<'req, 'res>>
    (request: IRequest<'dto, 'req>)
    (handler: 'handler)
    (okMapper: ('res -> IResult) option) = async {
        match request.Validate() with
        | Error es -> return Results.BadRequest es
        | Ok validated ->
            let! result = handler.Handle validated
            return mapHttpResponse result okMapper    
    }

let pipeEmpty<'res, 'handler when 'handler :> IAsyncHandler<unit, 'res>>
    (handler: 'handler)
    (okMapper: ('res -> IResult) option) = async {
        let! result = handler.Handle ()
        return mapHttpResponse result okMapper    
    }

let pipeMessage<'dto, 'req, 'res, 'processor when 'processor :> IAsyncReplyProcessor<'req, IResult>>
    (request: IRequest<'dto, 'req>)
    (processor: 'processor) = async {
        match request.Validate() with
        | Error es -> return Results.BadRequest es
        | Ok validated ->
            return! processor.Post validated    
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