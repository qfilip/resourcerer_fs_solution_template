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

let scanRepositories (interfaceType: Type) (assembly: Assembly) =
    let implementations =
        assembly.GetTypes()
        |> Array.filter (fun t ->
            t.IsClass &&
            not t.IsAbstract
        )

    implementations
    |> Array.map (fun impl ->
        let abstractions =
            impl.GetInterfaces()
            |> Array.filter (fun i -> i.IsGenericType)
        let abstraction = abstractions |> Array.exactlyOne
        (abstraction, impl)
    )

    //Type interfaceType = typeof(IMyInterface);
    //    var classesWithInterfaceConstructor = Assembly.GetExecutingAssembly()
    //        .GetTypes()
    //        .Where(t => t.IsClass && !t.IsAbstract)
    //        .Where(t => t.GetConstructors()
    //            .Any(c => c.GetParameters().Any(p => p.ParameterType == interfaceType)))
    //        .ToList();

    //    foreach (var cls in classesWithInterfaceConstructor)
    //    {
    //        Console.WriteLine($"Class: {cls.Name} takes {interfaceType.Name} in its constructor.");
    //    }

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

