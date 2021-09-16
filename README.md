# SignalR.Extensions.Orleans

A SignalR backend for projects using Microsoft Orleans.

## Isn't there already SignalR+Orleans backends available?

Yes, I was struggling to get them working properly with a co-hosted project so I decided to build my own.

If you are running into issues with them as well, give this a try!  There may be some oversight as this is my first crack at creating a SignalR backend, but please feel to open an issue if you run into anything.


## Installation

Reference the package `SignalR.Extensions.Orleans` to your standalone cluster, client, and/or co-hosted projects.

```
tbd
```

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