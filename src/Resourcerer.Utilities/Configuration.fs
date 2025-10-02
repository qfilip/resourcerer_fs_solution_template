module Resourcerer.Utilities.Configuration

open Microsoft.Extensions.Configuration
open Common

let load<'a> (section: IConfigurationSection) (path: string) =
    let value = section.GetValue<'a>(path)
    let hasValue = OptionExt.ofNullable value
    match hasValue with
    | None -> raise (System.InvalidOperationException($"Secret {path} not found"))
    | _ -> value

let loadValidated<'a> (section: IConfigurationSection) (path: string) (validator: 'a -> bool) =
    let value = load<'a> section path
    match validator value with
    | false -> raise (System.InvalidOperationException($"Validation for {section}-{path} failed"))
    | true -> value

let loadSection (conf: IConfiguration) (path: string) =
    let section = conf.GetSection(path)
    let hasValue = OptionExt.ofNullable section
    match hasValue with
    | None -> raise (System.InvalidOperationException($"Section {path} not found"))
    | _ -> section