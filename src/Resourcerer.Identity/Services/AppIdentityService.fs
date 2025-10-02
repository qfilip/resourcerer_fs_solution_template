namespace Resourcerer.Identity.Services

open System
open System.Collections.Generic
open System.Security.Claims
open Resourcerer.Identity.Models
open Resourcerer.Identity.Constants
open Resourcerer.Identity.Abstractions

type AppIdentityService(authEnabled: bool, systemIdentity: AppIdentity) =
    let mutable userIdentity: AppIdentity option = None

    let getClaim (claims: IEnumerable<Claim>) (ctype: string) (parser: string -> (bool * 'a)) (optional: bool) =
        let claim = claims |> Seq.tryFind (fun x -> x.Type = ctype)
        match claim, optional with
        | None, false -> raise (System.InvalidOperationException($"Claim {ctype} not found"))
        | None, true -> None
        | Some clm, _ ->
            let (parsed, value) = parser clm.Value
            if parsed then Some value else None
    
    interface IAppIdentityService<AppIdentity> with
        member _.Identity with get () = Option.defaultValue systemIdentity userIdentity
        member _.Set (identity: AppIdentity) = userIdentity <- Some identity
        member _.Set (claims: IEnumerable<Claim>) =
            match authEnabled with
            | false -> ()
            | true ->
                let id = getClaim claims Claims.Id Guid.TryParse false |> Option.defaultValue (Guid.Empty)
                let email = getClaim claims Claims.Email (fun x -> (true, x)) false |> Option.defaultValue ""
                let name = getClaim claims Claims.Name (fun x -> (true, x)) false |> Option.defaultValue ""
                userIdentity <- Some { Id = id; Name = name; Email = email }
                
            

