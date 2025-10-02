module Resourcerer.Identity.DependencyInjection

#nowarn "20"
open System
open System.Text
open Microsoft.AspNetCore.Builder
open Microsoft.IdentityModel.Tokens
open Microsoft.Extensions.DependencyInjection
open Resourcerer.Utilities.Configuration
open Resourcerer.Identity.Abstractions
open Resourcerer.Identity.Models
open Resourcerer.Identity.Services
open Microsoft.AspNetCore.Authentication.JwtBearer

let register (builder: WebApplicationBuilder) =
    let section = loadSection builder.Configuration "Auth"
    
    let authEnabled = load<bool> section "Enabled"
    match authEnabled with
    | false -> ()
    | true ->
        builder.Services.AddScoped<AppJwtBearerEventService>(fun sp ->
            let identityService = sp.GetRequiredService<IAppIdentityService<AppIdentity>>()
            AppJwtBearerEventService(identityService)
        )
        
        let secretKey = load<string> section "SecretKey"
        let issuer = load<string> section "Issuer"
        let audience = load<string> section "Audience"
        let tets = loadValidated<int> section "TokenExpirationTimeSeconds" (fun x -> x > 60)
        let skey = SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))

        builder.Services.AddScoped<JwtTokenService>(fun _ -> JwtTokenService(skey, issuer, audience, tets))

        let jwtScheme = JwtBearerDefaults.AuthenticationScheme
        builder.Services.AddAuthentication(jwtScheme)
            .AddJwtBearer(jwtScheme, fun o ->
                let tvp = TokenValidationParameters()
                tvp.ValidIssuer <- issuer
                tvp.ValidAudience <- audience
                tvp.IssuerSigningKey <- skey

                tvp.ValidateIssuer <- true
                tvp.ValidateAudience <- true
                tvp.ValidateLifetime <- true
                tvp.ValidateIssuerSigningKey <- true
                
                o.TokenValidationParameters <- tvp
                o.EventsType <- typeof<AppJwtBearerEventService>
            )

        builder.Services.AddAuthorization(fun conf ->
            conf.AddPolicy("jwt_policy", fun b ->
                b.RequireAuthenticatedUser().AddAuthenticationSchemes(jwtScheme) |> ignore) |> ignore
        ) |> ignore

    builder.Services.AddScoped<IAppIdentityService<AppIdentity>, AppIdentityService>(fun _ ->
        let systemIdentity: AppIdentity = { Id = Guid.Empty; Name = "system"; Email = "sys@notmail.org" }
        AppIdentityService(authEnabled, systemIdentity)
    )

    authEnabled