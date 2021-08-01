namespace FRangle.Core

open System
open System.IO
open System.Text.Json
open FRangle.Core.Domain.Errors
open FRangle.Core.Pipelines

[<RequireQualifiedAccess>]
module Json =

    /// Deserialize a stream to type 'T.    
    let deserializeStreamAsync<'T> (json: Stream) =
        async {
            try
                let! result =
                    JsonSerializer.DeserializeAsync<'T>(json).AsTask()
                    |> Async.AwaitTask

                return Ok result
            with
            | :? ArgumentNullException as e ->
                return
                    Error(
                        FRangleError.JsonParsingError
                            { Message = "`json` argument is null"
                              Exception = e.Message }
                    )
            | :? JsonException as e ->
                return
                    Error(
                        FRangleError.JsonParsingError
                            { Message =
                                  "Error: The JSON is invalid, TValue is not compatible with the JSON or there is remaining data in the stream"
                              Exception = e.Message }
                    )
            | :? NotSupportedException as e ->
                return
                    Error(
                        FRangleError.JsonParsingError
                            { Message =
                                  "There is no compatible JsonConverter for returnType or its serializable members."
                              Exception = e.Message }
                    )
            | e -> return Error (FRangleError.UnhandledException e.Message)
        }
    
    let parseStream<'T> json = run deserializeStreamAsync<'T> json

    let parse<'T> (json: string) =       
            try
                Ok (JsonSerializer.Deserialize<'T>(json))
            with
            | :? ArgumentNullException as e ->
                    Error(
                        FRangleError.JsonParsingError
                            { Message = "`json` argument is null"
                              Exception = e.Message }
                    )
            | :? JsonException as e ->
                    Error(
                        FRangleError.JsonParsingError
                            { Message =
                                  "Error: The JSON is invalid, TValue is not compatible with the JSON or there is remaining data in the stream"
                              Exception = e.Message }
                    )
            | :? NotSupportedException as e ->
                    Error(
                        FRangleError.JsonParsingError
                            { Message =
                                  "There is no compatible JsonConverter for returnType or its serializable members."
                              Exception = e.Message }
                    )
            | e -> Error (FRangleError.UnhandledException e.Message)