using System.Collections.Generic;
using ChatApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans.TestingHost;

namespace SignalR.Extensions.Orleans.Tests.Fixtures
{
    public class ChatApiFactory : WebApplicationFactory<ChatApiStartup>
    {
        private readonly TestCluster _cluster;

        public ChatApiFactory(TestCluster cluster)
        {
            _cluster = cluster;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureLogging(logging =>
            {
                logging.AddDebug();
            });

            builder.ConfigureAppConfiguration(configuration =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Logging:LogLevel:Default"] = "Debug",
                });
            });

            builder.ConfigureServices((ctx, services) =>
            {
                services.AddSingleton(_cluster.Client);
                services.AddSingleton(_cluster.GrainFactory);
            });
        }
    }
}
