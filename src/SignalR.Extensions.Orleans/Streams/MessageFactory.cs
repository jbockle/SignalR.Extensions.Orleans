using System;
using SignalR.Extensions.Orleans.Internals;
using SignalR.Extensions.Orleans.Streams.Recipients;

namespace SignalR.Extensions.Orleans.Streams
{
    public interface IMessageFactory
    {
        Message Create<THub>(string method, object[] args, Recipient recipient);
    }

    internal class MessageFactory : IMessageFactory
    {
        private readonly ISignalRNode _node;

        public MessageFactory(ISignalRNode node)
        {
            _node = node;
        }

        public Message Create<THub>(string method, object[] args, Recipient recipient)
        {
            return new Message
            {
                HubTypeName = GetHubTypeName<THub>(),
                Method = method,
                Args = args,
                Recipient = recipient,
                SenderNodeId = _node.Id,
            };
        }

        private static string GetHubTypeName<THub>()
        {
            return typeof(THub).AssemblyQualifiedName
                   ?? throw new NullReferenceException($"The hub '{typeof(THub)}' does not have an assembly qualified name");
        }
    }
}
