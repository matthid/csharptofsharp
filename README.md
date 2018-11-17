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


