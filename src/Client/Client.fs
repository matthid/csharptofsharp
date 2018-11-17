module Client

open MirrorSharp
open Elmish
open Elmish.React

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch

open Thoth.Json

open Fulma
open Fable.Core
open Fable.Import


type Model = { LoadedEditor : bool }

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type Msg =
| LoadEditor

// defines the initial state and initial command (= side-effect) of the application
let init () : Model * Cmd<Msg> =
    let initialModel = { LoadedEditor = false }
    initialModel, Cmd.none

let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> =
    match msg with
    | LoadEditor ->
        let nextModel = { currentModel with LoadedEditor = true }
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
            Browser.console.warn ("textChange", getText) 
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
        nextModel, Cmd.none


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
                      DefaultValue "using System;
public class C {
    public void M() {
        Console.WriteLine(\"Test\");
    }
}" ] [ ]
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
