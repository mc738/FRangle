namespace FRangle.ToolKit.Data

open System
open System.IO
open System.Text.Json.Serialization
open System.Threading
open Amazon.S3
open Amazon.S3.Model
open FRangle.Core
open FRangle.Core.Domain.Errors
open FRangle.Core.Pipelines

module S3 =

    type S3Config =
        { [<JsonPropertyName("accessKey")>]
          AccessKey: string
          [<JsonPropertyName("secretKey")>]
          SecretKey: string
          [<JsonPropertyName("regionalEndpoint")>]
          RegionalEndpoint: string
          [<JsonPropertyName("serviceUrl")>]
          ServiceUrl: string }

    type S3BucketOverview = { Name: string; CreatedOn: DateTime }

    type S3Bucket =
        { Name: string
          CreatedOn: DateTime
          Objects: Map<string, S3Object> }

    and S3Object =
        { Key: string
          Size: int64
          Owner: string
          LastModified: DateTime
          ETag: string }
        
    and S3Context =
        {
            Client: AmazonS3Client
        }

    let loadConfig path =
        startWith path
        >=> Files.read
        >=> Json.parse<S3Config>

    let getBucketOverviewsAsync context _ =
        async {
            try
                let! r = context.Client.ListBucketsAsync() |> Async.AwaitTask

                return
                    Ok(r.Buckets
                    |> List.ofSeq
                    |> List.map
                        (fun b ->
                            { Name = b.BucketName
                              CreatedOn = b.CreationDate }))
            with ex -> return Error(FRangleError.UnhandledException ex.Message)            
        }

    let getBucketOverviews context = run (getBucketOverviewsAsync context) 
    
    let getBucketObjectsAsync context (bucketName: string) _ =
        async {
            try
                let! r =
                    context.Client.ListObjectsAsync(bucketName)
                    |> Async.AwaitTask

                return
                    Ok(
                        r.S3Objects
                        |> List.ofSeq
                        |> List.map
                            (fun o ->
                                { Key = o.Key
                                  Size = o.Size
                                  Owner = o.Owner.Id
                                  LastModified = o.LastModified
                                  ETag = o.ETag })
                    )
            with ex -> return Error(FRangleError.UnhandledException ex.Message)
        }

    let getBucketObjects context (bucketName: string) = run (getBucketObjectsAsync context bucketName) 
    
    let downloadObjectAsync context (bucketName: string) (key: string) filePath append _ =
        async {
            try
                let request = GetObjectRequest()

                request.BucketName <- bucketName
                request.Key <- key

                let! data = context.Client.GetObjectAsync(request) |> Async.AwaitTask

                data.WriteResponseStreamToFileAsync(filePath, append, CancellationToken.None)
                |> Async.AwaitTask
                |> ignore

                return Ok()
            with ex -> return Error(FRangleError.UnhandledException ex.Message)
        }

    let downloadObject (bucketName: string) context (key: string) filePath append =
        run (downloadObjectAsync context bucketName key filePath append) 
    
    let writeObjectToStreamAsync context (bucketName: string) (key: string) (stream: Stream) _ =
        async {
            try
                let request = GetObjectRequest()

                request.BucketName <- bucketName
                request.Key <- key

                let! data = context.Client.GetObjectAsync(request) |> Async.AwaitTask

                data.ResponseStream.CopyToAsync stream
                |> Async.AwaitTask
                |> ignore

                return Ok()
            with ex -> return Error(FRangleError.UnhandledException ex.Message)
        }

    let writeObjectToStream context (bucketName: string) (key: string) (stream: Stream) _ =
        run (writeObjectToStreamAsync context bucketName key stream)
    
    let getObjectStreamAsync context (bucketName: string) (key: string) _ =
        async {
            try
                let request = GetObjectRequest()

                request.BucketName <- bucketName
                request.Key <- key

                let! data = context.Client.GetObjectAsync(request) |> Async.AwaitTask

                return Ok(data.ResponseStream)
            with
            // TODO add more specific error handling (?)
            ex -> return Error(FRangleError.UnhandledException ex.Message)
        }

    let getObjectStream context bucketName key = run (getObjectStreamAsync context bucketName key)
    
    let uploadFileAsync context (bucketName: string) (key: string) (filePath: string) _ =
        async {
            try
                let request = PutObjectRequest()

                request.Key <- key
                request.BucketName <- bucketName
                request.FilePath <- filePath

                let! r = context.Client.PutObjectAsync(request) |> Async.AwaitTask
                return Ok(int r.HttpStatusCode)
            with ex -> return Error(FRangleError.UnhandledException ex.Message)
        }

    let uploadFile context bucketName key filePath  = run (uploadFileAsync context bucketName key filePath)   
    
    let saveStreamAsync context (bucketName: string) (key: string) (stream: Stream) _ =
        async {
            try
                let request = PutObjectRequest()

                request.Key <- key
                request.BucketName <- bucketName
                request.InputStream <- stream

                let! r = context.Client.PutObjectAsync(request) |> Async.AwaitTask
                return Ok(r.HttpStatusCode)
            with ex -> return Error(FRangleError.UnhandledException ex.Message)
        }
        
    let saveStream context bucketName key stream = run (saveStreamAsync context bucketName key stream)