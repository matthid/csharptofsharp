module Client

open MirrorSharp
open Elmish
open Elmish.React

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack
open Fable.PowerPack.Fetch

open Thoth.Json

open Fulma
open Fable.Core
open Fable.Import
open Shared

type Model = 
    { LoadedEditor : bool
      LoadingFSharpResult : string option
      CurrentCSharp : string
      LastProcessingResult : ProcessingResult }

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type Msg =
| LoadEditor
| StartLoadFSharpResult of string
| LoadFSharpResultFinished of Result<ProcessingResult, exn>

let defaultCSharpText = "using System;
public class C {
    public void M() {
        Console.WriteLine(\"Test\");
    }
}"

// defines the initial state and initial command (= side-effect) of the application
let init () : Model * Cmd<Msg> =
    let initialModel = 
        { LoadedEditor = false
          LoadingFSharpResult = None
          CurrentCSharp = ""
          LastProcessingResult =  { FSharpText = "" } }
    initialModel, Cmd.none

exception RequestFailed of Response
let private errorString (response: Response) =
    string response.Status + " " + response.StatusText + " for URL " + response.Url
let fetchNextResult (p : ProcessCSharp) : JS.Promise<ProcessingResult> =
    Fetch.postRecord<ProcessCSharp> "/api/cs2fs" p []
    |> Promise.bind (fun response ->
        if not response.Ok
        then raise <| RequestFailed response
        else
            response.text()
            |> Promise.map (Decode.fromString (Decode.Auto.generateDecoder()))
            |> Promise.map (function 
                | Ok r -> r
                | Error s -> raise <| exn (s))
            )

let fetchNextCmd (csharpText) =
    Cmd.ofPromise
        (fetchNextResult)
        { CSharpText = csharpText }
        (Ok >> LoadFSharpResultFinished)
        (Error >> LoadFSharpResultFinished)
let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> =
    match msg with
    | LoadFSharpResultFinished r ->
        let newLoading, cmd1 =
            match currentModel.LoadingFSharpResult with
            | None -> None, Cmd.none
            | Some loadedText ->
                if loadedText = currentModel.CurrentCSharp then
                    None, Cmd.none
                else
                    Some currentModel.CurrentCSharp, fetchNextCmd currentModel.CurrentCSharp
            
        match r with
        | Ok r ->
            { currentModel with LastProcessingResult = r; LoadingFSharpResult = newLoading  }, cmd1
        | Error e ->
            Browser.console.error("Some error occured", e)
            { currentModel with LoadingFSharpResult = newLoading  }, cmd1

    | StartLoadFSharpResult csharpText ->
        match currentModel.LoadingFSharpResult with
        | Some t ->
            { currentModel with CurrentCSharp = csharpText }, Cmd.none
        | None ->
            let cmd = fetchNextCmd csharpText
            { currentModel with CurrentCSharp = csharpText; LoadingFSharpResult = Some csharpText }, cmd        
    | LoadEditor ->
        let nextModel = { currentModel with LoadedEditor = true }
        let cmd = Cmd.ofSub (fun dispatch ->
            let elem = Browser.document.getElementById "MirrorSharpEditor"
            let opts = JsInterop.createEmpty<IMirrorSharpOptions>
            opts.serviceUrl <- "wss://sharplab.io/mirrorsharp"
            let callbacks = JsInterop.createEmpty<ICodeMirrorCallbacks>
            callbacks.slowUpdateWait <- fun () ->
                Browser.console.warn ("slowUpdateWait")
            callbacks.slowUpdateResult <- fun re ->
                Browser.console.warn ("slowUpdateResult", re)
            callbacks.connectionChange <- fun con ->
                Browser.console.warn ("connectionChange", con)
            callbacks.textChange <- fun getText ->
                let t = getText()
                dispatch (StartLoadFSharpResult t)
                Browser.console.warn ("textChange", t) 
            callbacks.serverError <- fun message ->
                Browser.console.warn ("serverError", message)
            opts.on <- callbacks        
            //opts.value <- "namespace Test {}"
            //Browser.console.warn("Showing mirrorsharp", mirrorSharp)
            let editor = mirrorSharp elem opts
            let serverOptions = JsInterop.createEmpty<IServerOptions>
            serverOptions.language <- "C#"

            serverOptions.``x-optimize`` <- "debug"
            serverOptions.``x-target`` <- "AST"
            editor.sendServerOptions(serverOptions)
        )
        nextModel, cmd


let button txt onClick =
    Button.button
        [ Button.IsFullWidth
          Button.Color IsPrimary
          Button.OnClick onClick ]
        [ str txt ]

let view (model : Model) (dispatch : Msg -> unit) =
    div []
        [ Navbar.navbar [ Navbar.Color IsPrimary ]
            [ Navbar.Item.div [ ]
                [ Heading.h2 [ ]
                    [ str "SAFE Template" ] ] ]

          Container.container []
              [ 

                textarea
                    [ Id "MirrorSharpEditor"
                      Ref (fun element ->
                          // Ref is trigger with null once for stateless element so we need to wait for the second trigger
                          if not (isNull element) then
                              // The div has been mounted check if this is the first time
                              if not model.LoadedEditor then
                                  // This is the first time, we can trigger a draw
                                  dispatch LoadEditor 
                          )
                      DefaultValue defaultCSharpText ] [ ]

                textarea [ Value model.LastProcessingResult.FSharpText ] []                 
               ]

          Footer.footer [ ]
                [ Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
                    [  ] ] ]

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
#if DEBUG
|> Program.withConsoleTrace
//|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
#if DEBUG
//|> Program.withDebugger
#endif
|> Program.run
