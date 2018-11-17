module MirrorSharp

open Fable.Core
open Fable.Import
open CSharpAst

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

let toTriviaType (s:string) =
    match s with
    | "WhitespaceTrivia" -> WhitespaceTrivia
    | "EndOfLineTrivia" -> EndOfLineTrivia
    | _ -> UnknownTrivia s

let toNodeType (s:string) =
    match s with
    | "MethodDeclaration" -> MethodDeclaration
    | "CompilationUnit" -> CompilationUnit
    | "ClassDeclaration" -> ClassDeclaration
    | "NamespaceDeclaration" -> NamespaceDeclaration
    | _ -> UnknownNode s

let toTokenType (s:string) =
    match s with
    | "IdentifierToken" -> IdentifierToken
    | "OpenBraceToken" -> OpenBraceToken
    | "PublicKeyword" -> PublicKeyword
    | _ -> UnknownToken s

[<AutoOpen>]
module AstItemRawExt =
    type AstItemRaw with
        member x.AsAstItem =
            match x.asttype with
            | "value" ->
                AstItem.AstValue (x.range, x.value)
            | "trivia" ->
                AstItem.AstTrivia(x.range, toTriviaType x.kind, x.value)
            | "node" ->
                AstItem.AstNode(x.range, x.children |> Array.map (fun i -> i.AsAstItem), toNodeType x.kind)
            | "token" ->
                AstItem.AstToken(x.range, x.children |> Array.map (fun i -> i.AsAstItem), toTokenType x.kind, x.property)
            | _ -> UnknownAstItemType x            