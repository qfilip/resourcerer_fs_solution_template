module Resourcerer.Api.Endpoints.VersionMapping

open System
open System.Reflection
open Microsoft.AspNetCore.Builder
open Resourcerer.Utilities.Reflection
open Resourcerer.Api.Endpoints.Types

let private printPretty (path: string) (method: HttpMethod) =
    let space = 7 - method.ToString().Length
    System.Console.Write(method);
    for i in [1..space] do
        System.Console.Write(" ");

    System.Console.Write($"/{path}{Environment.NewLine}");

let mapEndpoints (app: WebApplication) (xs: (int * int * string * HttpMethod * Delegate) list) =
        printfn "Endpoints:"
        xs |> List.iter (fun (major, minor, path, method, action) ->
            let fullpath = $"v{major}.{minor}/{path}"
            printPretty fullpath method

            match method with
            | Get -> app.MapGet(fullpath, action) |> ignore
            | Put -> app.MapPut(fullpath, action) |> ignore
            | Patch -> app.MapPatch(fullpath, action) |> ignore
            | Post -> app.MapPost(fullpath, action) |> ignore
            | Delete -> app.MapDelete(fullpath, action) |> ignore
        )

let findAllEndpointVersions () =
    let interfaceType = typeof<IEndpoint>
    let assembly = Assembly.GetExecutingAssembly()
    let implementors = scan interfaceType assembly
    let endpoints =
        implementors
        |> Array.map (fun e -> 
            let i = Activator.CreateInstance(e) :?> IEndpoint
            (i.Major, i.Minor, i.Path, i.HttpMethod, i.Handler)
        )
        |> List.ofArray

    let resourceSelector (x: string) = x.Split('/')[0]

    endpoints
        |> List.iter (fun (major, minor, path, method, _) ->
            let count = 
                endpoints
                |> List.filter (fun (maj, min, p, m, _) -> major = maj && minor = min && path = p && method = m)
                |> List.length

            if count = 1 then ()
            else
                failwith $"More than one endpoint found with path {path} and method {method}"
        )
        |> ignore
        
    let resources =
        endpoints
        |> List.map (fun (_, _, path, _, _) -> resourceSelector path)
        |> List.distinct

    let majors =
        endpoints
        |> List.map (fun (v, _, _, _, _) -> v)
        |> List.distinct

    let mutable mapped = []
        
    majors |> List.iter(fun major ->
        resources |> List.iter (fun resource ->
            let minors =
                endpoints
                |> List.fold (fun acc (maj, min, _, _, _) -> if maj = major then min::acc else acc) []
                |> List.distinct

            let resourceEndpoints =
                endpoints
                |> List.filter (fun (_, _, path, _, _) -> path.StartsWith(resource))
                |> List.groupBy (fun (_, _, path, method, _) -> (path, method))

            resourceEndpoints |> List.iter (fun re ->
                let endpointVersions = snd re
                let minimumMajor =
                    endpointVersions
                    |> List.map (fun (maj, _, _, _, _) -> maj)
                    |> List.min

                if minimumMajor > major then printfn "Minimun major exceeded" else
                minors |> List.iter (fun minor ->
                    let versionOption =
                        endpointVersions
                        |> List.tryFind (fun (maj, min, _, _, _) -> major = maj && minor = min)

                    match versionOption with
                    | Some v -> mapped <- v::mapped
                    | None ->
                        let (_, _, path, method, action) =
                            endpointVersions
                            |> List.sortBy (fun (maj, min, _, _, _) -> maj, min)
                            |> List.last
                        mapped <- (major, minor, path, method, action)::mapped
                )
            )
        )
    )

    mapped