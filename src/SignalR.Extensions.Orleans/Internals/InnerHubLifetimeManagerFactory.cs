using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using SignalR.Extensions.Orleans.Streams;

namespace SignalR.Extensions.Orleans.Internals
{
    public interface IInnerHubLifetimeManagerFactory
    {
        IHubLifetimeManager Create(string hubTypeName);

        IHubLifetimeManager Create(Message message);
    }

    internal class InnerHubLifetimeManagerFactory : IInnerHubLifetimeManagerFactory
    {
        private readonly Type _genericManagerType = typeof(InnerHubLifetimeManager<>);

        private readonly ConcurrentDictionary<string, IHubLifetimeManager> _managers =
            new ConcurrentDictionary<string, IHubLifetimeManager>();

        private readonly IServiceProvider _serviceProvider;

        public InnerHubLifetimeManagerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IHubLifetimeManager Create(string hubTypeName)
        {
            return _managers.GetOrAdd(hubTypeName, GetInstanceFromServiceProvider);
        }

        public IHubLifetimeManager Create(Message message)
        {
            return Create(message.HubTypeName);
        }

        private IHubLifetimeManager GetInstanceFromServiceProvider(string hubTypeName)
        {
            var hubType = Type.GetType(hubTypeName) ??
                throw new InvalidOperationException($"Hub type was unable to be located: {hubTypeName}");

            var hubLifetimeManagerType = _genericManagerType.MakeGenericType(hubType);

            return (IHubLifetimeManager)_serviceProvider.GetRequiredService(hubLifetimeManagerType);
        }
    }
}
