module Resourcerer.Models.Primitives

type Min2String = Min2String of string
with
    static member tryFrom (x: string) =
        match x with
        | x when (isNull x) -> Error(["Cannot be null"; "Must contain minimum 2 characters"])
        | x when (x.Length < 2) -> Error(["Must contain minimum 2 characters"])
        | x -> Ok (Min2String x)
    
    static member unmap (Min2String x) = x

