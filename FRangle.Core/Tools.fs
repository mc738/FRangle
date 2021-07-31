namespace FRangle.Core

open System.IO
open FRangle.Core.Domain
open FRangle.Core.Domain.Errors
open FRangle.Core.Pipelines

[<RequireQualifiedAccess>]
module Tools =

    /// Transform an object of type 'TIn to 'TOut via a handler function
    let transform<'TIn, 'TOut> (handler: 'TIn -> 'TOut) (value: 'TIn) : Result<'TOut, FRangleError> =
        switch handler value

    let create<'T> (handler: unit -> 'T) (run: unit) : Result<'T, FRangleError> = switch handler run

    let check<'T> (handler: 'T -> Result<unit, string>) (value: 'T) =
        match handler value with
        | Ok _ -> Ok value
        | Error m -> Error(Errors.FailedCheck { Message = m })

    let conditional<'TIn, 'TOut>
        (condition: 'TIn -> bool)
        (trueHandler: 'TIn -> 'TOut)
        (falseHandler: 'TIn -> 'TOut)
        (value: 'TIn)
        : Result<'TOut, FRangleError> =
        match condition value with
        | true -> switch trueHandler value
        | false -> switch falseHandler value

    /// Discards the count value and returns
    let discard value : Result<unit, FRangleError> = switch (fun _ -> ()) value


    let mkdir (paths: string list) : Result<unit, FRangleError> =
        switch
            (fun paths ->
                paths
                |> List.map (fun p -> Directory.CreateDirectory p)// TODO add proper error handling.
                |> ignore)
            paths