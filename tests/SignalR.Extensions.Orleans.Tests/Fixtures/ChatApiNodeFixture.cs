using System;
using System.Net.Http;
using System.Threading;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using SignalR.Extensions.Orleans.Internals;

namespace SignalR.Extensions.Orleans.Tests.Fixtures
{
    public class ChatApiNodeFixture : IDisposable
    {
        private readonly ChatApiFactory _apiFactory;

        public ChatApiNodeFixture(SiloClusterFixture clusterFixture)
        {
            _apiFactory = new ChatApiFactory(clusterFixture.Cluster);

            NodeId = _apiFactory.Services.GetRequiredService<ISignalRNode>().Id;

            Client = _apiFactory.CreateClient();
        }

        public string NodeId { get; }

        public HttpClient Client { get; }

        public HubConnection CreateHubConnection(string forUser)
        {
            return new HubConnectionBuilder()
                .WithUrl(new UriBuilder(Client.BaseAddress) { Path = "chat", Scheme = "ws" }.Uri, options =>
                {
                    options.HttpMessageHandlerFactory = _ => _apiFactory.Server.CreateHandler();
                    options.Headers["x-user"] = forUser;
                })
                .Build();
        }

        public void Dispose()
        {
            Client?.Dispose();
            _apiFactory?.Services.GetService<IHubManagerBackgroundService>()?.StopAsync(CancellationToken.None);
            _apiFactory?.Dispose();
        }
    }
}