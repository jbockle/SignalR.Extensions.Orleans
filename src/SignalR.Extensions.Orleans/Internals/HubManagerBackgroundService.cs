using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using SignalR.Extensions.Orleans.Streams;

namespace SignalR.Extensions.Orleans.Internals
{
    public interface IHubManagerBackgroundService : IHostedService
    {
        void SubscribeToStream();
    }

    internal class HubManagerBackgroundService : IHubManagerBackgroundService
    {
        private readonly IClusterClient _clusterClient;
        private readonly ISignalRNode _node;
        private readonly IMessageObserver _observer;
        private readonly ILogger<HubManagerBackgroundService> _logger;

        private IAsyncStream<Message>? _stream;

        public HubManagerBackgroundService(
            IClusterClient clusterClient,
            ISignalRNode node,
            IMessageObserver observer,
            ILogger<HubManagerBackgroundService> logger)
        {
            _clusterClient = clusterClient;
            _node = node;
            _observer = observer;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting {NodeProviderId}", _node.Id);

            SubscribeToStream();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Stopping {NodeProviderId}", _node);

            return Task.CompletedTask;
        }

        public void SubscribeToStream()
        {
            _logger.LogDebug("Subscribing to stream {NodeProviderId}", _node);

            _stream = _clusterClient
                .GetStreamProvider(SignalROrleansSdk.Constants.STREAM_PROVIDER)
                .GetStream<Message>(Guid.Empty, SignalROrleansSdk.Constants.SIGNALR_GRAIN_STREAM);

            _stream.SubscribeAsync(_observer);
        }
    }
}
