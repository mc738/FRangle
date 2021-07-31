namespace FRangle.ToolKit.Dev

open System
open FRangle.Core.Domain.Errors
open Microsoft.TeamFoundation.WorkItemTracking.WebApi
open Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
open Microsoft.VisualStudio.Services.Common
open FRangle.Core.Pipelines

[<RequireQualifiedAccess>]
module AzureDevOps =

    type Relationship =
        { Title: string
          Relationship: string
          Type: string }

    type WorkItemDetails =
        { Title: string
          State: string
          Relationships: Relationship list }

    let getOpenTasksAsync accessToken (url: string) (project: string) _ =
        async {
            try
                // TODO add proper error handling.
                let credentials =
                    VssBasicCredential(String.Empty, accessToken)

                let query = Wiql()

                query.Query <-
                    $"""SELECT [Id]
                FROM WorkItems
                WHERE [Work Item Type] = 'Task'
                AND [System.TeamProject] = '{project}'
                AND [System.State] <> 'Done'
                ORDER BY [State] Asc, [Changed Date] Desc
                """

                use client =
                    new WorkItemTrackingHttpClient(Uri(url), VssCredentials.op_Implicit credentials)

                let! results = client.QueryByWiqlAsync(query) |> Async.AwaitTask

                let ids =
                    results.WorkItems
                    |> List.ofSeq
                    |> List.map (fun wi -> wi.Id)

                let! items =
                    client.GetWorkItemsAsync(ids, [||], results.AsOf, WorkItemExpand.All)
                    |> Async.AwaitTask

                return Ok(items |> List.ofSeq)
            with ex -> return Error(FRangleError.BespokeError ex.Message)
        }

    let toDetails (items: WorkItem list) =
        Ok(
            items
            |> List.map
                (fun wi ->
                    { Title = wi.Fields.["System.Title"].ToString()
                      State = wi.Fields.["System.State"].ToString()
                      Relationships =
                          if wi.Relations <> null then
                              wi.Relations
                              |> List.ofSeq
                              |> List.map
                                  (fun wir ->
                                      { Title = wir.Title
                                        Relationship = wir.Rel
                                        Type = wir.Attributes.["name"].ToString() })
                          else
                              [] })
        )


    let getOpenWorkItems accessToken url project =
        run (getOpenTasksAsync url project accessToken)
        >=> toDetails
