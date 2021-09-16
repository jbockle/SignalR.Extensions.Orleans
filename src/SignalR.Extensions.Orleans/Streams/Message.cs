using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SignalR.Extensions.Orleans.Internals;
using SignalR.Extensions.Orleans.Streams.Recipients;

namespace SignalR.Extensions.Orleans.Streams
{
    [Serializable]
    public class Message
    {
        public string HubTypeName { get; set; } = string.Empty;

        public string SenderNodeId { get; set; } = string.Empty;

        public string Method { get; set; } = string.Empty;

        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.All)]
        public object[] Args { get; set; } = Array.Empty<object>();

        [JsonProperty(TypeNameHandling = TypeNameHandling.All)]
        public Recipient Recipient { get; set; } = default!;

        public Task Visit(IHubLifetimeManager manager)
        {
            return Recipient.Visit(manager, this);
        }
    }
}
