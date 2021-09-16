using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SignalR.Extensions.Orleans.Streams;

namespace SignalR.Extensions.Orleans.Internals
{
    internal class InnerHubLifetimeManager<THub> : DefaultHubLifetimeManager<THub>, IHubLifetimeManager
        where THub : Hub
    {
        public InnerHubLifetimeManager(ILogger<DefaultHubLifetimeManager<THub>> logger)
            : base(logger)
        {
        }

        public Task Accept(Message message)
        {
            return message.Visit(this);
        }
    }
}
