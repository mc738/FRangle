namespace FRangle.Core

open System
open FRangle.Core
open Domain.Errors
open Pipelines

[<RequireQualifiedAccess>]
module Strings =
    
    let createStr (value: string) =  create (fun _ -> value)

    /// Concat a list of strings into one string.        
    let concat (separator: string) (lines: string list) : Result<string, FRangleError> =
         Ok(String.Join(separator, lines))
    
    /// Split one string into a list.
    let split (separator: string) (value: string) : Result<string list, FRangleError> =
        Ok(value.Split(separator) |> List.ofArray)

[<RequireQualifiedAccess>]
module Ints =
    
    let create (value: int) = create (fun _ -> value)
    
[<RequireQualifiedAccess>]
module Guids =
    
    let create run = create (fun _ -> Guid.NewGuid()) run
    
    let parse (value: string) =
        let parse (input: string) =
            match Guid.TryParse input with
            | true, v -> Ok v
            | false, _ -> Error (FRangleError.TypeParseError { Type = typeof<Guid>; Message = $"Could not parse from string `{value}`." })
    
        parse value
        //switch 