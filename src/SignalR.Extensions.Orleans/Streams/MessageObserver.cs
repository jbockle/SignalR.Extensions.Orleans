using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.Streams;
using SignalR.Extensions.Orleans.Internals;

namespace SignalR.Extensions.Orleans.Streams
{
    internal interface IMessageObserver : IAsyncObserver<Message>
    {
    }

    internal class MessageObserver : IMessageObserver
    {
        private readonly ILogger<MessageObserver> _logger;
        private readonly IInnerHubLifetimeManagerFactory _managerFactory;
        private readonly ISignalRNode _node;

        public MessageObserver(
            ILogger<MessageObserver> logger,
            IInnerHubLifetimeManagerFactory managerFactory,
            ISignalRNode node)
        {
            _logger = logger;
            _managerFactory = managerFactory;
            _node = node;
        }

        public Task OnNextAsync(Message message, StreamSequenceToken? token = null)
        {
            using var scope = _logger.BeginScope("{SignalRNode}", _node);

            if (_node.Id.Equals(message.SenderNodeId))
            {
                _logger.LogDebug("Ignoring message from same node", message.SenderNodeId);
                return Task.CompletedTask;
            }

            _logger.LogDebug("Message received from node {NodeId}", message.SenderNodeId);

            return _managerFactory.Create(message).Accept(message);
        }

        public Task OnCompletedAsync()
        {
            _logger.LogDebug("Completed");
            return Task.CompletedTask;
        }

        public Task OnErrorAsync(Exception ex)
        {
            _logger.LogError(ex, "Exception encountered");
            return Task.CompletedTask;
        }
    }
}