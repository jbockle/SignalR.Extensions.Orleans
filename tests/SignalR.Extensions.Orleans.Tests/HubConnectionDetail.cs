using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalR.Extensions.Orleans.Tests
{
    public class HubConnectionDetail : IEquatable<HubConnectionDetail>, IDisposable
    {
        public HubConnectionDetail(string nodeId, string userId)
        {
            NodeId = nodeId;
            UserId = userId;
        }

        public string UserId { get; }

        public string NodeId { get; }

        public HubConnection HubConnection { get; set; }

        public Signaler Signaler { get; set; } = Signaler.Create();

        public string ConnectionId => HubConnection.ConnectionId;

        public void Dispose()
        {
            Signaler?.Dispose();
            HubConnection.DisposeAsync().GetAwaiter().GetResult();
        }

        public bool Equals(HubConnectionDetail other)
        {
            if (other is null)
            {
                return false;
            }

            if (!ReferenceEquals(this, other))
            {
                return UserId == other.UserId && NodeId == other.NodeId;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((HubConnectionDetail)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserId, NodeId);
        }
    }
}
