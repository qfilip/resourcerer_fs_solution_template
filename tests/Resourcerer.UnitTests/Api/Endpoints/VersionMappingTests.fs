module Resourcerer.UnitTests.VersionMappingTests

open System
open Xunit
open FsUnit.Xunit
open Resourcerer.Api.Endpoints.Types

module Tests =
    type End = int * int * string * HttpMethod * Func<Tuple<int, int>>
    type Def = int * int * string * HttpMethod
    
    let dele action = Func<Tuple<int, int>>(action) :> Delegate
    let func (deleg: Delegate) = deleg :?> Func<Tuple<int, int>>
    
    let toEndpoint x =
        let (major, minor, path, method, (maj, min)) = x
        (major, minor, path, method, dele (fun () -> (maj, min)))
    
    let toDef x: Def =
        let (major, minor, path, method, _) = x
        (major, minor, path, method)

    let toDefWith major minor (x: Def): Def =
        let (_, _, path, method) = x
        (major, minor, path, method)

    let toEnd x =
        let (major, minor, path, method, deleg) = x
        (major, minor, path, method, func deleg)

    let finder (a: Def) (b: End) =
        let (maj, min, p, m) = a
        let (major, minor, path, method, action) = b
        maj = major && min = minor && path = p && method = m

    let getAction a =
        let (_, _, _, _, action) = a
        action

    let findInvoke a list =
        list
        |> List.find (fun x -> finder a x)
        |> fun x -> (getAction x).Invoke()
    
    [<Fact>]
    let ``basic map works`` () =
        let endpoints =
            [
                (1, 0, "foos/", HttpMethod.Get, (1, 0))
                (1, 0, "foos/", HttpMethod.Put, (1, 0))
            ]
            |> List.map toEndpoint

        endpoints |> List.length |> should equal 2
        endpoints
        |> List.map (fun (_, _, _, _, d) -> (func d).Invoke())
        |> List.forall (fun x -> x = (1, 0))

    [<Fact>]
    let ``maps minors of others`` () =
        let source = [
            (1, 0, "foos/", HttpMethod.Get, (1, 0))
            (1, 1, "foos/", HttpMethod.Get, (1, 1))
            (1, 0, "bars/", HttpMethod.Get, (1, 0))
            (1, 0, "bars/", HttpMethod.Put, (1, 0))
        ]

        let endpoints = source |> List.map toEndpoint

        // 4 original + 2 (get, put "bars/" with 1.1)
        endpoints |> List.length |> should equal 6
        
        let ends = endpoints |> List.map toEnd
        let defs = source |> List.map toDef
        
        // originals exist
        ends |> findInvoke defs[0] |> should equal (1, 0)
        ends |> findInvoke defs[1] |> should equal (1, 1)
        ends |> findInvoke defs[2] |> should equal (1, 0)
        ends |> findInvoke defs[3] |> should equal (1, 0)

        // bars/ endpoints have v1.1 with old actions
        ends |> findInvoke (defs[2] |> toDefWith 1 1) |> should equal (1, 0)
        ends |> findInvoke (defs[3] |> toDefWith 1 1) |> should equal (1, 0)

    [<Fact>]
    let ``does not map minors continously for the same resource`` () =
        let source = [
            (1, 0, "foos/", HttpMethod.Get, (1, 0))
            (1, 2, "foos/", HttpMethod.Get, (1, 2))
        ]

        let endpoints = source |> List.map toEndpoint

        // 2 originals, no v1.1
        endpoints |> List.length |> should equal 2
        
        let ends = endpoints |> List.map toEnd
        let defs = source |> List.map toDef
        
        // originals exist
        ends |> findInvoke defs[0] |> should equal (1, 0)
        ends |> findInvoke defs[1] |> should equal (1, 2)

    [<Fact>]
    let ``does not map minors continously for other resources`` () =
        let source = [
            (1, 2, "foos/", HttpMethod.Get, (1, 2))
            (1, 0, "bars/", HttpMethod.Get, (1, 0))
        ]

        let endpoints = source |> List.map toEndpoint

        // 2 originals + 2 maps [bars/ v1.2, foos/ v1.0]
        endpoints |> List.length |> should equal 4
        
        let ends = endpoints |> List.map toEnd
        let defs = source |> List.map toDef
        
        // originals exist
        ends |> findInvoke defs[0] |> should equal (1, 2)
        ends |> findInvoke defs[1] |> should equal (1, 0)

        // mapped uses original action
        ends |> findInvoke (defs[0] |> toDefWith 1 0) |> should equal (1, 2)
        ends |> findInvoke (defs[1] |> toDefWith 1 2) |> should equal (1, 0)

    [<Fact>]
    let ``maps majors`` () =
        let source = [
            (1, 0, "foos/", HttpMethod.Get, (1, 0))
            (2, 0, "foos/", HttpMethod.Get, (2, 0))
            (1, 0, "bars/", HttpMethod.Get, (1, 0))
        ]

        let endpoints = source |> List.map toEndpoint

        // 3 originals + 1 maps [bars/ v2.0]
        endpoints |> List.length |> should equal 4
        
        let ends = endpoints |> List.map toEnd
        let defs = source |> List.map toDef
        
        // originals exist
        ends |> findInvoke defs[0] |> should equal (1, 0)
        ends |> findInvoke defs[1] |> should equal (2, 0)
        ends |> findInvoke defs[2] |> should equal (1, 0)

        // mapped uses original action
        ends |> findInvoke (defs[2] |> toDefWith 2 0) |> should equal (1, 0)

    [<Fact>]
    let ``maps majors and minors`` () =
        let source = [
            (1, 0, "foos/", HttpMethod.Get, (1, 0))
            (2, 0, "foos/", HttpMethod.Get, (2, 0))
            (2, 1, "foos/", HttpMethod.Get, (2, 1))
            (1, 0, "bars/", HttpMethod.Get, (1, 0))
        ]

        let endpoints = source |> List.map toEndpoint

        // 4 originals + 2 maps [bars/ v2.0, v2.1]
        endpoints |> List.length |> should equal 6
        
        let ends = endpoints |> List.map toEnd
        let defs = source |> List.map toDef
        
        // originals exist
        ends |> findInvoke defs[0] |> should equal (1, 0)
        ends |> findInvoke defs[1] |> should equal (2, 0)
        ends |> findInvoke defs[2] |> should equal (2, 1)
        ends |> findInvoke defs[3] |> should equal (1, 0)

        // mapped
        ends |> findInvoke (defs[3] |> toDefWith 2 0) |> should equal (1, 0)
        ends |> findInvoke (defs[3] |> toDefWith 2 1) |> should equal (1, 0)

    [<Fact>]
    let ``doesn't map majors to lower major`` () =
        let source = [
            (1, 0, "foos/", HttpMethod.Get, (1, 0))
            (2, 0, "bars/", HttpMethod.Get, (2, 0))
        ]

        let endpoints = source |> List.map toEndpoint

        // bars/ was added at v2.0, v1.0 shouldn't exist
        let ends = endpoints |> List.map toEnd
        let defs = source |> List.map toDef

        ends |> findInvoke (defs[1] |> toDefWith 1 0) |> should throw typeof<exn>

    [<Fact>]
    let ``throws exception for duplicates`` () =
        let source = [
            (1, 0, "foos/", HttpMethod.Get, (1, 0))
            (1, 0, "foos/", HttpMethod.Get, (1, 1))
        ]

        let action () = source |> List.map toEndpoint
        
        action () |> should throw typeof<exn>