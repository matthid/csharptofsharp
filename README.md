# csharptofsharp

Helper tool for newcomers in order to work with existing documentation (mostly C#) and trying to learn F#. Additionally, this tool can help advanced and expert F#'ers with porting existing C# snippets from StackOverflow for example.

## Goals

* Easy to use (Website in the Browser)
* Copy any C# code snippet (which specifying dependencies)
* Get a reasonable result and links to helpful documentation if we cannot convert some features

## Current proposed implementation

* Use the [sharplab](https://sharplab.io/#v2:EYLgZgpghgLgrgJwgZwLQBEJinANjASQDsYIFsBjCAgWwAdcIaITYBLAeyIBoYQpkMbgBMQAagAAAQAMAAikBGANwBYAFBSAzAoBMcgMJyA3hrnmFOqQBY5AWQAUAShNmL7pQE4HAIgAqKDA+Tmrq7u52AJ4AgnRsvqSCwaHuAL4aqUA===) editor in the frontend.
* Use https://github.com/jindraivanek/cs2fs as first transformation.
* TODO: Transform Sharplab into objects usable in cs2fs (currently everything is red).

## Side facts / Open issues

* I'd like to use the C# AST in order to not re-implement the C# compiler, however how to get that into the browser?
  -> we get some json representation but cs2fs it not working with that, is there an easy way to fix that?
  -> Maybe we can generate the stuff, similar to https://github.com/kekyo/Microsoft.CodeAnalysis.ActivePatterns
  -> Can we generate the json serialisation as well?
* I'd like to convert C# -> F# AST (which cs2fs is doing) in order to further refine F# AST later
  -> Can we somehow use FCS-AST (instead of cs2fs custom AST) in the browser to the "real" AST?
  -> cs2fs AST is fine, as long we have some intermediate representation
* How to print F# AST to string
  -> Can/Should we extend FCS?

## Team

@matthid, Twitter: @matthi__d, Slack: @matthid
@JohBa, Twitter: @JoBaeurle, Slack: @johba
@jindraivanek, Twitter: ?, Slack: @jindraivanek (maker of cs2fs)
@johlrich, Twitter: ?, Slack: @johlrich
@mvsmal, Twitter: ?, Slack: ?
