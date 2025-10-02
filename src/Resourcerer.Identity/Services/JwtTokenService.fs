namespace Resourcerer.Identity.Services

open System
open System.Security.Claims;
open System.IdentityModel.Tokens.Jwt;
open Microsoft.IdentityModel.Tokens;
open Resourcerer.Identity.Constants;
open Resourcerer.Identity.Models;
open Resourcerer.Identity.Utils;

type JwtTokenService(skey: SymmetricSecurityKey, issuer: string, audience: string, tokenExpirationTimeSeconds: int) =
    let writeToken (claims: Claim list) =
        let credentials = SigningCredentials(skey, SecurityAlgorithms.HmacSha256)
        let now = DateTime.UtcNow
        let token = JwtSecurityToken(
            issuer,
            audience,
            claims,
            now,
            now.AddSeconds(tokenExpirationTimeSeconds),
            credentials)
        
        let handler = JwtSecurityTokenHandler()
        handler.WriteToken(token)

    member _.CreateToken(identity: AppIdentity, permissionMap: Map<string, int>) =
        let identityClaims = [
            Claim(Claims.Id, identity.Id.ToString())
            Claim(Claims.Name, identity.Name)
            Claim(Claims.Email, identity.Email)
        ]

        let permissionClaims = Permissions.getClaimsFromPermissionMap permissionMap

        writeToken (identityClaims @ permissionClaims)