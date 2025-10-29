module Resourcerer.Utilities.Common

let inline private notNull value = not (obj.ReferenceEquals(value, null))

module ResultExt =
    let mapResultArray (xs: Result<'a, 'b> array): Result<'a list, 'b list> =
        let folder (acc: Result<'a list, 'b list>) (current: Result<'a, 'b>) =
            match acc, current with
            | Ok okList, Ok value -> Ok (value :: okList)
            | Ok _, Error err -> Error [err]
            | Error errList, Ok _ -> Error errList
            | Error errList, Error err -> Error (err :: errList)
    
        let initial = Ok []

        xs |> Array.fold folder initial

    let verifyArray (data: Result<'a, string list> array) =
        let dataResult = data |> mapResultArray
        match dataResult with
        | Ok x -> Ok x
        | Error xs -> Error (xs |> List.collect id)

    let mapAsyncResultTo target asyncOperation = async {
        let! accepted = asyncOperation
        match accepted with
        | Ok _ -> return Ok target
        | Error errors -> return Error errors
    }

module OptionExt =
    let ofNullable x =
        match notNull x with
        | true -> Some x
        | false -> None

    let mapOrDefault x mapper =
        match mapper with
        | Some f -> f x
        | None -> x