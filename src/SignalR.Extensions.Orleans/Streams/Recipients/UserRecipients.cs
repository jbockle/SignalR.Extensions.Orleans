using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SignalR.Extensions.Orleans.Internals;

namespace SignalR.Extensions.Orleans.Streams.Recipients
{
    [Serializable]
    public class UserRecipients : Recipient
    {
        public List<string> UserIds { get; } = new List<string>();

        public override Task Visit(IHubLifetimeManager manager, Message message)
        {
            return manager.SendUsersAsync(UserIds, message.Method, message.Args);
        }

        public UserRecipients WithUserId(string userId)
        {
            UserIds.Add(userId);

            return this;
        }

        public UserRecipients WithUserIds(IEnumerable<string> userIds)
        {
            UserIds.AddRange(userIds);

            return this;
        }
    }
}