namespace FRangle.Core

open FRangle.Core.Domain
open FRangle.Core.Domain.Errors

module Pipelines =

    let bind switchFunction twoTrackInput =
        match twoTrackInput with
        | Ok r -> switchFunction r
        | Error e -> Error e

    let switch f x = f x |> Ok

    /// Combine the results of 2 switch functions in to a tuple.
    /// Useful when you want to keep the result of a previous stage though shouldn't be abused.
    /// Both switch functions will receive the same input (`x`), normally this will be a unit.
    let combine2 switch1 switch2 x =
        match switch1 x with
        | Ok r ->
            match switch2 x with
            | Ok r2 -> Ok(r, r2)
            | Error e -> Error e
        | Error e -> Error e




    /// Bind.
    let (>>=) twoTrackInput switchFunction = bind switchFunction twoTrackInput

    /// Pipe.
    let (>=>) switch1 switch2 x =
        match switch1 x with
        | Ok r -> switch2 r
        | Error e -> Error e

    /// Recover.
    let (>?>) switch1 switch2 x =
        match switch1 x with
        | Ok r -> Ok r
        | Error e -> switch2 x

    /// Pass through.
    let (>->) (switch1: 'a -> Result<'b, 'c>) (passThru: Result<'b, 'c> -> unit) (x: 'a) =
        let r = switch1 x
        passThru r
        r

    /// Infix version of combine.
    /// Combine the results of 2 switch functions that accept the same parameter (normally a unit).
    let (>+>) switch1 switch2 x = combine2 switch1 switch2 x

    let passThru<'a, 'b> (f: Result<'a, 'b> -> unit) (v: Result<'a, 'b>) =
        f v
        v

    let switchAsync (f: 'a -> Async<'b>) x =
        let r = f x |> Async.RunSynchronously
        r

    let map oneTrackFunction twoTrackInput =
        match twoTrackInput with
        | Ok r -> Ok(oneTrackFunction r)
        | Error e -> Error e

    let tee f x =
        f x |> ignore
        x

    let tryCatch f x =
        try
            f x |> Ok
        with ex -> Error ex.Message

    let doubleMap successFunc errorFunc twoTrackInput =
        match twoTrackInput with
        | Ok r -> Ok(successFunc r)
        | Error e -> Error(errorFunc e)

    let plus addSuccessFunc addFailureFunc switch1 switch2 x =
        match (switch1 x), (switch2 x) with
        | Ok r1, Ok r2 -> Ok(addSuccessFunc r1 r2)
        | Error e, Ok _ -> Error e
        | Ok _, Error e -> Error e
        | Error e1, Error e2 -> Error(addFailureFunc e1 e2)

    let start _ = Ok()



    /// Run an async function and get a result.
    let run f a =
        let r = f a |> Async.RunSynchronously
        r

    /// Transform an object of type 'TIn to 'TOut via a handler function
    let transform<'TIn, 'TOut> (handler: 'TIn -> 'TOut) (value: 'TIn) : Result<'TOut, Errors.FRangleError> =
        switch handler value

    let create<'T, 'a> (handler: 'a -> 'T) (run: 'a) : Result<'T, Errors.FRangleError> = switch handler run

    let startWith<'T> (value: 'T) (run: unit) : Result<'T, Errors.FRangleError> = switch (fun _ -> value) run

    let check<'T> (handler: 'T -> Result<unit, string>) (value: 'T) =
        match handler value with
        | Ok _ -> Ok value
        | Error m -> Error(Errors.FailedCheck { Message = m })

    let conditional<'TIn, 'TOut>
        (condition: 'TIn -> bool)
        (trueHandler: 'TIn -> 'TOut)
        (falseHandler: 'TIn -> 'TOut)
        (value: 'TIn)
        : Result<'TOut, Errors.FRangleError> =
        match condition value with
        | true -> switch trueHandler value
        | false -> switch falseHandler value

    /// Discards the count value and returns
    let discard value : Result<unit, Errors.FRangleError> = switch (fun _ -> ()) value

    /// Merge a 2 value tuple into a function. This will pass essential call `next t1 t2`.
    /// Useful for breaking down tuples created with combine2
    let merge2<'T1, 'T2, 'TOut> (next: 'T1 -> 'T2 -> 'TOut) (t1: 'T1, t2: 'T2) : Result<'TOut, FRangleError> =
        switch (next t1) t2

    /// A collection of pipelines.
    type PipelineCollection<'TIn, 'TOut> =
        { Pipelines: Map<string, 'TIn -> Result<'TOut, Errors.FRangleError>> }
        static member Create(pipelines: (string * ('TIn -> Result<'TOut, Errors.FRangleError>)) list) =
            { Pipelines = pipelines |> Map.ofList }

        member collection.Run(name: string, value: 'TIn) =
            match collection.Pipelines.TryFind name with
            | Some pipeline -> pipeline value
            | None -> Error(Errors.FRangleError.PipelineNotFound name)
