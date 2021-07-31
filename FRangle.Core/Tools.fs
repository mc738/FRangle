namespace FRangle.Core

open System.IO
open FRangle.Core.Domain
open FRangle.Core.Domain.Errors
open FRangle.Core.Pipelines

[<RequireQualifiedAccess>]
module Tools =

    let mkdir (paths: string list) : Result<unit, FRangleError> =
        switch
            (fun paths ->
                paths
                |> List.map (fun p -> Directory.CreateDirectory p)// TODO add proper error handling.
                |> ignore)
            paths
            
    