using System;
using System.Threading.Tasks;
using SignalR.Extensions.Orleans.Internals;

namespace SignalR.Extensions.Orleans.Streams.Recipients
{
    [Serializable]
    public abstract class Recipient
    {
        public abstract Task Visit(IHubLifetimeManager manager, Message message);
    }
}