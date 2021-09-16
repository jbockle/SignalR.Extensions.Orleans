using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SignalR.Extensions.Orleans.Internals;

namespace SignalR.Extensions.Orleans.Streams.Recipients
{
    [Serializable]
    public class AllRecipients : Recipient
    {
        public List<string> ExcludedConnectionIds { get; } = new List<string>();

        public override Task Visit(IHubLifetimeManager manager, Message message)
        {
            return manager.SendAllExceptAsync(message.Method, message.Args, ExcludedConnectionIds);
        }

        public AllRecipients WithoutConnectionIds(IEnumerable<string> excludedConnectionIds)
        {
            ExcludedConnectionIds.AddRange(excludedConnectionIds);

            return this;
        }
    }
}
