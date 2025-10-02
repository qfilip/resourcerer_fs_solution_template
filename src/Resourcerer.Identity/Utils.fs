namespace Resourcerer.Identity.Utils

open System
open Resourcerer.Identity.Enums
open System.Security.Claims

module Permissions =
    let allPermissions = Enum.GetValues<ePermission>() |> List.ofSeq
    let allResources = Enum.GetValues<eResource>() |> List.ofSeq

    let validate (permissionMap: Map<string, string[]>) =
        permissionMap |> Map.fold (fun acc key value ->
            let (parsed, _) = Enum.TryParse<eResource>(key)
            match parsed with
            | false -> $"Resource {key} doesnt exist"::acc
            | true ->
                let errs =
                    value
                    |> Array.fold (fun acc permission ->
                        let (parsed, _) = Enum.TryParse<ePermission>(key)
                        match parsed with
                        | false -> $"Permission {permission} doesnt exist"::acc
                        | true -> acc
                    ) []
                acc @ errs
        ) []
        |> fun errors -> if errors |> List.length > 0 then Error errors else Ok ()

    let getCompressedMap (keyCompresser: eResource -> 'a) =
        allResources
        |> List.fold (fun acc resource ->
            let permissionLevel =
                allPermissions
                |> List.fold (fun permissionLevel permission -> permissionLevel ||| (int)permission) (0)
            (keyCompresser resource, permissionLevel)::acc
        ) []
        |> Map.ofList

    let getClaimsFromPermissionMap (map: Map<string, int>) =
        map |> Map.fold (fun acc k v -> Claim(k, v.ToString())::acc) []

