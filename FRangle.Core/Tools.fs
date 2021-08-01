namespace FRangle.Core

open System
open System.IO
open System.Text.RegularExpressions
open FRangle.Core.Domain
open FRangle.Core.Domain.Errors
open FRangle.Core.Pipelines

/// A collection of shell like tools.
[<RequireQualifiedAccess>]
module Tools =

    /// Make a directory.
    let mkdir (paths: string list) : Result<unit, FRangleError> =
        switch
            (fun paths ->
                paths
                |> List.map (fun p -> Directory.CreateDirectory p)// TODO add proper error handling.
                |> ignore)
            paths
            
    /// A grep(-like) operation to filter a string list on a regex pattern.
    let grep (pattern: string) (lines: string list) : Result<string list, FRangleError> =
        lines |> List.filter (fun l -> Regex.IsMatch(l, pattern)) |> Ok
    
    /// Concat a list of strings into one string.        
    let concat (separator: string) (lines: string list) : Result<string, FRangleError> =
         Ok(String.Join(separator, lines))
    
    /// Split one string into a list.
    let split (separator: string) (value: string) : Result<string list, FRangleError> =
        Ok(value.Split(separator) |> List.ofArray)