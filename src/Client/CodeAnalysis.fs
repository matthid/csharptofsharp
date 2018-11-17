namespace Microsoft.CodeAnalysis

type SyntaxNode(children) =
    member x.Children : SyntaxNode[] = children
