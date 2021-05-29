﻿module ReadFile

open Dapr.Client
open Microsoft.Extensions.Logging
open Saturn
open Giraffe
open FSharp.Control.Tasks
open Microsoft.AspNetCore.Http
open Shared


type CloudEvent<'a> =
    { Id: string
      SpecVersion: string
      Source: string
      Type: string
      DataContentType: string
      PubSubName: string
      TraceId: string
      Topic: string
      DataSchema: string option
      Subject: string option
      Time: string option
      Data: 'a
      DataBase64: string option }

//
type DocStoreProvider =
    | YaCloud

[<CLIMutable>]
type DocStore = {
    Url: string
    Provider: DocStoreProvider
}

//
type DocRead = { DocKey: string; DocContent: string; }
[<CLIMutable>]
type DocStored = { DocKey: string; DocStore: DocStore; }

//
type DocEntity = {
    DocId: string
    Store: DocStore option
}

let createDocEntry docId =
    {
        DocId = docId
        Store = None
    }
    

let docRead =
    fun (dapr: DaprClient) (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let logger = ctx.GetLogger()
            let! data = bindCloudEventDataAsync<DocRead> ctx
            let docEntry = createDocEntry data.DocKey
            let! res = tryCreateStateAsync dapr "statestore" data.DocKey docEntry
            match res with
            | true -> logger.LogDebug("{statestore} updated with new {document}", "statestore", docEntry)
            | false -> logger.LogDebug("{statestore} failed to update, {document} already exists", "statestore", docEntry)
            return! json res next ctx
            
        }

let docStored =
    fun (dapr: DaprClient) (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let logger = ctx.GetLogger()            
            let! data = bindCloudEventDataAsync<DocStored> ctx
            let! res = 
                tryUpdateOrCreateStateAsync 
                    dapr 
                    "statestore" 
                    data.DocKey 
                    (fun id -> createDocEntry id) 
                    (fun doc -> { doc with Store = Some data.DocStore })

            match res.IsSuccess with
            | true -> logger.LogDebug("{statestore} document with {documentId} is updated with {result}", "statestore", data.DocKey, res)
            | false -> logger.LogWarning("{statestore} document with {documentId} fail to update with {result}", "statestore", data.DocKey, res)
                        
            return! json res next ctx
            
        }

let routes dapr =
    router {
        get
            "/dapr/subscribe"
            (json (
                [ {| pubsubname = "pubsub"
                     topic = "doc-read"
                     route = "doc-read" |}
                  {| 
                    pubsubname = "pubsub"
                    topic = "doc-stored"
                    route = "doc-stored" |}                                          
                ]
            ))

        post "/doc-read" (docRead dapr)
        post "/doc-stored" (docStored dapr)
    }

let app = daprApp 5002 routes

[<EntryPoint>]
let main _ =
    run app
    0 // return an integer exit code

