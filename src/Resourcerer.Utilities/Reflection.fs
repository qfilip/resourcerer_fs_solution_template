module Resourcerer.Utilities.Reflection

open System
open System.Reflection

let scan (interfaceType: Type) (assembly: Assembly) =
    if interfaceType.IsGenericType then
        raise (System.InvalidOperationException($"Interface {interfaceType.Name} cannot be generic"))
    
    let implementations =
        assembly.GetTypes()
        |> Array.filter (fun t ->
            t.GetInterface(interfaceType.Name) <> null &&
            not t.IsAbstract &&
            not t.IsInterface
        )

    implementations

let scanGeneric (interfaceType: Type) (assembly: Assembly) =
    if not interfaceType.IsGenericType then
        raise (System.InvalidOperationException($"Interface {interfaceType.Name} must be generic"))
    let implementations =
        assembly.GetTypes()
        |> Array.filter (fun t ->
            t.GetInterface(interfaceType.Name) <> null &&
            not t.IsAbstract &&
            not t.IsInterface
        )

    implementations
    |> Array.map (fun impl ->
        let abstractions =
            impl.GetInterfaces()
            |> Array.filter (fun i -> i.IsGenericType)
        let abstraction = abstractions |> Array.exactlyOne
        (abstraction, impl)
    )

