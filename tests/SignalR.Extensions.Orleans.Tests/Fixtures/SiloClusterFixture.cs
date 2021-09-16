using System;
using Microsoft.Extensions.Configuration;
using Orleans;
using Orleans.Hosting;
using Orleans.TestingHost;

namespace SignalR.Extensions.Orleans.Tests.Fixtures
{
    public class SiloClusterFixture : IDisposable
    {
        public TestCluster Cluster { get; }

        public SiloClusterFixture()
        {
            Cluster = new TestClusterBuilder()
                .AddSiloBuilderConfigurator<TestClusterConfigurator>()
                .AddClientBuilderConfigurator<TestClusterConfigurator>()
                .Build();

            Cluster.Deploy();
        }

        public void Dispose()
        {
            Cluster?.StopAllSilos();
        }

        public class TestClusterConfigurator : ISiloConfigurator, IClientBuilderConfigurator
        {
            public void Configure(ISiloBuilder siloBuilder)
            {
                siloBuilder
                    .UseSignalR()
                    .AddMemoryGrainStorage("PubSubStore");

            }

            public void Configure(IConfiguration configuration, IClientBuilder clientBuilder)
            {
                clientBuilder.UseSignalR();
            }
        }
    }
}