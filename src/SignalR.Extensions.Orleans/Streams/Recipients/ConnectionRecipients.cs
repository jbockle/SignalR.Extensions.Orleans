using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SignalR.Extensions.Orleans.Internals;

namespace SignalR.Extensions.Orleans.Streams.Recipients
{
    [Serializable]
    public class ConnectionRecipients : Recipient
    {
        public List<string> ConnectionIds { get; } = new List<string>();

        public override Task Visit(IHubLifetimeManager manager, Message message)
        {
            return manager.SendConnectionsAsync(ConnectionIds, message.Method, message.Args);
        }

        public ConnectionRecipients WithConnectionId(string connectionId)
        {
            ConnectionIds.Add(connectionId);

            return this;
        }

        public ConnectionRecipients WithConnectionIds(IEnumerable<string> connectionIds)
        {
            ConnectionIds.AddRange(connectionIds);

            return this;
        }
    }
}