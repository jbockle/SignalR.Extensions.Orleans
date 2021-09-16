using System;
using System.Text;

namespace SignalR.Extensions.Orleans.Internals
{
    public interface ISignalRNode
    {
        string Id { get; }
    }

    internal class DefaultSignalRNode : ISignalRNode
    {
        public string Id { get; } = Guid.NewGuid().ToString();

        public override string ToString()
        {
            return new StringBuilder(base.ToString())
                .AppendFormat("({0})", Id)
                .ToString();
        }
    }
}
