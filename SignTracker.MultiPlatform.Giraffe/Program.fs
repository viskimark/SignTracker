module SignTracker.MultiPlatform.Giraffe.App

open System
open System.IO
open System.Text.Json
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open SignTracker.MultiPlatform
open Microsoft.AspNetCore.Http

// ---------------------------------
// Models
// ---------------------------------

type Message =
    {
        Text : string
    }

// ---------------------------------
// Views
// ---------------------------------

module Views =
    open Giraffe.ViewEngine

    let layout (content: XmlNode list) =
        html [] [
            head [] [
                title []  [ encodedText "SignTracker.MultiPlatform.Giraffe" ]
                link [ _rel  "stylesheet"
                       _type "text/css"
                       _href "/main.css" ]
            ]
            body [] content
        ]

    let partial () =
        h1 [] [ encodedText "SignTracker.MultiPlatform.Giraffe" ]

    let index (model : Message) =
        [
            partial()
            p [] [ encodedText model.Text ]
        ] |> layout

// ---------------------------------
// Web app
// ---------------------------------

let getSigns userId =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let result = JsonSerializer.Serialize <| Database.getSigns userId
            ctx.Response.ContentType <- "application/json"
            return! ctx.WriteStringAsync result
        }

let submitSign userId =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        using (new System.IO.StreamReader(ctx.Request.Body)) (fun sr ->
            task {
                let! json = sr.ReadToEndAsync()
                let sign = System.Text.Json.JsonSerializer.Deserialize<Database.Sign>(json)
                Database.addSign sign userId
                return! Successful.OK sign next ctx
            }
        )

let editSign i =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        using (new System.IO.StreamReader(ctx.Request.Body)) (fun sr ->
            task {
                let! json = sr.ReadToEndAsync()
                printfn "%s" json
                let sign = System.Text.Json.JsonSerializer.Deserialize<Database.Sign>(json)
                Database.editSign sign
                return! Successful.OK sign next ctx
            }
        )

let webApp =
    choose [
        GET >=>
            choose [
                route "/" >=> text "hello"
                route "/ping" >=> text "pong"
                routef "/signs/%i" getSigns
            ]
        POST >=>
            choose [
                routef "/editSign/%i" editSign
                routef "/submitSign/%i" submitSign
            ]
        setStatusCode 404 >=> text "Not Found"
    ]

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

let configureCors (builder : CorsPolicyBuilder) =
    builder
        .WithOrigins(
            "http://localhost:5000",
            "https://localhost:5001")
       .AllowAnyMethod()
       .AllowAnyHeader()
       |> ignore

let configureApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()
    (match env.IsDevelopment() with
    | true  ->
        app.UseDeveloperExceptionPage()
    | false ->
        app .UseGiraffeErrorHandler(errorHandler)
            .UseHttpsRedirection())
        .UseCors(configureCors)
        .UseStaticFiles()
        .UseGiraffe(webApp)

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore

let configureLogging (builder : ILoggingBuilder) =
    builder.AddConsole()
           .AddDebug() |> ignore

[<EntryPoint>]
let main args =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "WebRoot")
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                webHostBuilder
                    .UseContentRoot(contentRoot)
                    .UseWebRoot(webRoot)
                    .UseUrls("http://0.0.0.0:5010")
                    .Configure(Action<IApplicationBuilder> configureApp)
                    .ConfigureServices(configureServices)
                    .ConfigureLogging(configureLogging)
                    |> ignore)
        .Build()
        .Run()
    0