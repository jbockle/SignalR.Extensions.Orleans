# SignalR.Extensions.Orleans

<p align="center">
<img src="https://raw.githubusercontent.com/jbockle/SignalR.Extensions.Orleans/main/assets/logo.png" style="width:250px">
</p>

A SignalR backend for projects using Microsoft Orleans.

> ***Wait, isn't there existing SignalR+Orleans backend packages available?!***
> 
> Yes! I was struggling to get them working properly with a co-hosted project so I decided to build my own.
> In addition, several of them have switched to target only .net 5, and I needed .net core 3.1 for now (__this package targets both__).
> 
> If you are running into issues with them as well, give this a try!  There may be some oversight as 
> this is my first crack at creating a SignalR backend, but please feel free to open an issue if you run into anything.


## Installation

Reference the package `SignalR.Extensions.Orleans` on your standalone cluster, client, and/or co-hosted project.

| [![Nuget](https://img.shields.io/nuget/v/SignalR.Extensions.Orleans?style=for-the-badge)](https://www.nuget.org/packages/SignalR.Extensions.Orleans)| `Install-Package SignalR.Extensions.Orleans` or `dotnet add package SignalR.Extensions.Orleans`|
|--|--|


### Silo

```csharp
// applies to both co-hosted and standalone clusters
siloBuilder
  .UseLocalHostClustering()

  // Add this!
  .UseSignalR()

  // Add a "PubSubStore" persistence provider if you aren't using Orleans streams yet,
  // otherwise you probably already have one added
  .AddMemoryGrainStorage("PubSubStore") // MemoryGrainStorage is not recommended for production use
  ;
```

### Client

If you are not co-hosting, add `UseSignalR()` on the ClientBuilder

```csharp
var client = new ClientBuilder()

   // Add this!
  .UseSignalR();

  .Build();
```