module Client

open Elmish
open Elmish.React

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fable.PowerPack.Fetch

open Thoth.Json

open Fulma
open Fable.Core
open Fable.Import

type IServerOptions =
    abstract language : string with get, set
    [<Emit("$0[\"x-optimize\"]{{=$1}}")>]
    abstract ``x-optimize`` : string with get, set
    [<Emit("$0[\"x-target\"]{{=$1}}")>]
    abstract ``x-target`` : string with get, set

type ICodeMirrorInstance =
    abstract getCodeMirror : unit -> obj
    abstract sendServerOptions : IServerOptions -> unit

type Span =
    abstract start : int with get
    abstract length : int with get

type Diagnostic =
    abstract id : string with get
    abstract message : string with get
    abstract severity : string with get
    abstract span : Span with get
    abstract tags : obj[] with get
(*

type AstItemRaw =
    /// value, trivia, node, token
    [<Emit("$0[\"type\"]{{=$1}}")>]
    abstract asttype : string with get
    
    abstract children : AstItemRaw[] with get

    // value
    abstract value : string with get
    abstract range : string with get
    
    // trivia
    abstract kind : string with get

    // token
    abstract property : string with get

    // node
*)
type ICodeMirrorResult =
    abstract diagnostics : Diagnostic[] with get
    abstract x : obj[] with get

type ICodeMirrorCallbacks =
    abstract slowUpdateWait : (unit -> unit) with get, set
    abstract slowUpdateResult : (obj -> unit) with get, set
    abstract connectionChange : (obj -> unit) with get, set
    abstract textChange : (obj -> unit) with get, set
    abstract serverError : (obj -> unit) with get, set

type IMirrorSharpOptions =
    abstract serviceUrl : string with get, set
    abstract on : ICodeMirrorCallbacks with get, set
    //abstract value : string with get, set

JsInterop.importAll "mirrorsharp/mirrorsharp.less"
let mirrorSharp (x: Browser.HTMLElement) (y:IMirrorSharpOptions) : ICodeMirrorInstance = JsInterop.importAll "mirrorsharp"


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
