open System.IO
open System.Threading.Tasks

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open FSharp.Control.Tasks.V2
open Giraffe
open Saturn
open Shared

let publicPath = Path.GetFullPath "../Client/public"
let port = 8085us


let webApp = router {
    post "/api/cs2fs" (fun next ctx ->
        task {
            let! request = ctx.BindModelAsync<ProcessCSharp>()
            let resultFsharp = cs2fs.Convert.convertText request.CSharpText
            let result = { FSharpText = resultFsharp }
            return! Successful.OK result next ctx
        })
}

let configureSerialization (services:IServiceCollection) =
    services.AddSingleton<Giraffe.Serialization.Json.IJsonSerializer>(Thoth.Json.Giraffe.ThothSerializer())

let app = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    use_router webApp
    memory_cache
    use_static publicPath
    service_config configureSerialization
    use_gzip
}

run app
