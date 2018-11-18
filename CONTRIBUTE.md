# Contribute to the project

## Build and run

### Requirements

* Install [Fake](https://fake.build/fake-gettingstarted.html)
* Install the [.NET SDK](https://www.microsoft.com/net/download) or [here](https://www.microsoft.com/net/download/visual-studio-sdks)
* [Visual Studio Code](https://code.visualstudio.com/download) with
  * Ionide Extension
  * C# Extension (for the debugger)

### Build project

`fake build`

### Hot reloading & Running

`fake build -t run`

Now you can edit files and see your changes applied "live" in the browser and the server (server is restarted).

## cs2fs

We use [cs2fs](https://github.com/jindraivanek/cs2fs) as subtree in the `/extern/cs2fs` directory.
If you make changes there:

* Fork the project for example (for my github that would be `git@github.com:matthid/cs2fs.git` as I'd like to use ssh)
* `git remote add cs2fs git@github.com:matthid/cs2fs.git`
* `git subtree push --prefix=extern/cs2fs cs2fs mybranch`
* Open the pull request on GitHub

