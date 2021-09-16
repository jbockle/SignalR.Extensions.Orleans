using System;
using Orleans;
using Xunit;

namespace SignalR.Extensions.Orleans.Tests.Fixtures
{
    [CollectionDefinition(Name)]
    public class ChatApiCollection : ICollectionFixture<ChatApiFixture>
    {
        public const string Name = nameof(ChatApiCollection);
    }

    [Collection(ChatApiCollection.Name)]
    public abstract class UsingChatApiFixtureCollection
    {
        protected UsingChatApiFixtureCollection(ChatApiFixture fixture)
        {
            Fixture = fixture;
        }

        public ChatApiFixture Fixture { get; }
    }

    public class ChatApiFixture : IDisposable
    {
        private readonly SiloClusterFixture _silo;

        public ChatApiFixture()
        {
            _silo = new SiloClusterFixture();
            NodeA = new ChatApiNodeFixture(_silo);
            NodeB = new ChatApiNodeFixture(_silo);
        }

        public ChatApiNodeFixture NodeA { get; }

        public ChatApiNodeFixture NodeB { get; }

        public IClusterClient ClusterClient => _silo.Cluster.Client;

        public IGrainFactory GrainFactory => _silo.Cluster.GrainFactory;

        public void Dispose()
        {
            NodeA?.Dispose();
            NodeB?.Dispose();
            _silo?.Dispose();
        }
    }
}