namespace FRangle.Core

open System
open System.Net.Http
open FRangle.Core.Domain.Errors
open FRangle.Core.Pipelines

[<RequireQualifiedAccess>]
module Http =

    let getAsync (url: string) (client: HttpClient) _ =
        async {
            try
                let! result = client.GetAsync url |> Async.AwaitTask
                return Ok result
            with
            | :? InvalidOperationException as e ->
                return
                    Error(
                        FRangleError.HttpError
                            { Message = "The requestUri must be an absolute URI or BaseAddress must be set."
                              Exception = e.Message }
                    )
            | :? HttpRequestException as e ->
                return
                    Error(
                        FRangleError.HttpError
                            { Message =
                                  "The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout."
                              Exception = e.Message }
                    )
            | e -> return Error(FRangleError.UnhandledException e.Message)
        }

    let getContentStreamAsync (response: HttpResponseMessage) =
        async {
            match response.IsSuccessStatusCode with
            | true ->
                let! json =
                    response.Content.ReadAsStreamAsync()
                    |> Async.AwaitTask

                return Ok json
            | false ->
                let! message =
                    response.Content.ReadAsStringAsync()
                    |> Async.AwaitTask

                return
                    Error(
                        FRangleError.HttpResponseError
                            { Code = int response.StatusCode
                              Message = message }
                    )
        }

    let getContentStringAsync (response: HttpResponseMessage) =
        async {
            match response.IsSuccessStatusCode with
            | true ->
                let! result =
                    response.Content.ReadAsStringAsync()
                    |> Async.AwaitTask

                return Ok result
            | false ->
                let! message =
                    response.Content.ReadAsStringAsync()
                    |> Async.AwaitTask

                return
                    Error(
                        FRangleError.HttpResponseError
                            { Code = int response.StatusCode
                              Message = message }
                    )
        }
    
    let getStream url client =
        run (getAsync url client) >=> run getContentStreamAsync
        
    let getString url client =
        run (getAsync url client) >=> run getContentStringAsync