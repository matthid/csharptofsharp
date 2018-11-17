namespace Microsoft.CodeAnalysis.CSharp.Syntax

open Microsoft.CodeAnalysis

type ExternAliasDirectiveSyntax () =
    let t = ()

type CompilationUnitSyntax(children, externs : ExternAliasDirectiveSyntax list, usings, attributeLists, members, eolToken) =
    inherit SyntaxNode(children)
    member x.Externs = externs
    member x.Usings = usings
    member x.AttributeLists = attributeLists
    member x.Members = members
    member x.EndOfFileToken = eolToken

// TODO: Lots of others :(