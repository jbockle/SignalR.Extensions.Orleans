using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SignalR.Extensions.Orleans.Internals;

namespace SignalR.Extensions.Orleans.Streams.Recipients
{
    [Serializable]
    public class GroupRecipients : Recipient
    {
        public List<string> Groups { get; } = new List<string>();

        public List<string> ExcludedConnectionIds { get; } = new List<string>();

        public override Task Visit(IHubLifetimeManager manager, Message message)
        {
            return Task.WhenAll(Groups
                .Select(group => manager.SendGroupExceptAsync(group, message.Method, message.Args, ExcludedConnectionIds)));
        }

        public GroupRecipients WithGroup(string group)
        {
            Groups.Add(group);

            return this;
        }

        public GroupRecipients WithGroups(IEnumerable<string> groups)
        {
            Groups.AddRange(groups);

            return this;
        }

        public GroupRecipients WithoutConnectionIds(IEnumerable<string> excludedConnectionIds)
        {
            ExcludedConnectionIds.AddRange(excludedConnectionIds);

            return this;
        }
    }
}