# csharptofsharp

Helper tool for newcomers in order to work with existing documentation (mostly C#) and trying to learn F#. Additionally, this tool can help advanced and expert F#'ers with porting existing C# snippets from StackOverflow for example.

## Goals

* Easy to use (Website in the Browser)
* Copy any C# code snippet (which specifying dependencies)
* Get a reasonable result and links to helpful documentation if we cannot convert some features

## Current implementation

* Use the [sharplab](https://sharplab.io/#v2:EYLgZgpghgLgrgJwgZwLQBEJinANjASQDsYIFsBjCAgWwAdcIaITYBLAeyIBoYQpkMbgBMQAagAAAQAMAAikBGANwBYAFBSAzAoBMcgMJyA3hrnmFOqQBY5AWQAUAShNmL7pQE4HAIgAqKDA+Tmrq7u52AJ4AgnRsvqSCwaHuAL4aqUA===) editor in the frontend.
* Use https://github.com/jindraivanek/cs2fs as first transformation.
* TODO: Transform Sharplab into objects usable in cs2fs (currently everything is red).

## Side facts / Open issues

* How can we forward 'range' information in order to correlate C# and F# positions (like sharplab does when marking AST-Elements)?
* Handling of cases we don't support (ie currently I changed it to write `//UNKNOWN` into the source code).
* Adding hints/comments to the generated code
* Add a second pass to make the F# code more "F#"-Like (maybe selectable via UI)
* Improve cs2fs (ie add missing cases and tests)
* UI improvements...
* Start working on deployment/domain?
* Think about other features, like marking the C# code when hovering over parts of the F# code (to see what code lead to what F# code)
* Cleanup and make the repository public

## Team

See also our [contribution guideline](CONTRIBUTE.md).

@matthid, Twitter: @matthi__d, Slack: @matthid
@JohBa, Twitter: @JoBaeurle, Slack: @johba
@jindraivanek, Twitter: ?, Slack: @jindraivanek (maker of cs2fs)
@johlrich, Twitter: ?, Slack: @johlrich
@mvsmal, Twitter: ?, Slack: ?
