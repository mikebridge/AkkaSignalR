## SignalR and Akka.NET

This is an example of how to use Akka.NET, SignalR and ASP.Net Core (on the 461 framework) together.
It accompanies [this blog post](https://mikebridge.github.io/articles/signalr-akka/).

### Usage:

You should be able to run this just from the command line if you have Node and Dotnet Core installed,
but otherwise you'll need VS2015 or greater:

- Install [Visual Studio 2015 Update 3](https://www.visualstudio.com/en-us/news/releasenotes/vs2015-update3-vs)
and/or [DotNet Core 1.1](https://www.microsoft.com/net/download/core#/current).

- Install [Node](https://nodejs.org/en/).  I'm using 6.9.5.

- Optionally install the [Node extension for Visual Studio](https://www.visualstudio.com/vs/node-js/)

```cmd
> cd src\Web
> npm install
> npm run start
```

Then in another console:

```cmd
> cd src\EchoAPI
> dotnet restore
> dotnet run
```

In a browser, navigate to [http://localhost:3000/](http://localhost:3000/).  It connects
on startup, so reloading the page will retry the connection to SignalR.
