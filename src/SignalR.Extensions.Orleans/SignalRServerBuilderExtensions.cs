using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SignalR.Extensions.Orleans.Internals;
using SignalR.Extensions.Orleans.Streams;
using Package = SignalR.Extensions.Orleans;

// ReSharper disable CheckNamespace

namespace Microsoft.AspNetCore.SignalR
{
    public static class SignalRServerBuilderExtensions
    {
        public static ISignalRServerBuilder AddOrleans(this ISignalRServerBuilder builder)
        {
            var services = builder.Services;

            services.TryAddSingleton<ISignalRNode, DefaultSignalRNode>();

            services
                .AddSingleton(typeof(HubLifetimeManager<>), typeof(Package.OrleansHubLifetimeManager<>))
                .AddSingleton(typeof(InnerHubLifetimeManager<>))
                .AddSingleton<IInnerHubLifetimeManagerFactory, InnerHubLifetimeManagerFactory>()

                .AddSingleton<IMessageObserver, MessageObserver>()

                .AddSingleton<IHubManagerBackgroundService, HubManagerBackgroundService>()
                .AddHostedService(sp => sp.GetRequiredService<IHubManagerBackgroundService>())

                .AddSingleton<IMessageFactory, MessageFactory>()
                ;

            return builder;
        }
    }
}
