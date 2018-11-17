namespace CSharpAst

type TriviaType =
    | WhitespaceTrivia
    | EndOfLineTrivia
    | UnknownTrivia of string
    
type NodeType =
    | MethodDeclaration
    | CompilationUnit
    | ClassDeclaration
    | NamespaceDeclaration
    | UnknownNode of string

type TokenType =
    | IdentifierToken
    | OpenBraceToken
    | PublicKeyword
    | UnknownToken of string

type AstItem =
    | AstValue of range:string * value:string
    | AstTrivia of range:string * kind:TriviaType * value:string
    | AstNode of range:string * children:AstItem[] * kind:NodeType
    | AstToken of range:string * children:AstItem[] * kind:TokenType * property:string
    | UnknownAstItemType of obj
