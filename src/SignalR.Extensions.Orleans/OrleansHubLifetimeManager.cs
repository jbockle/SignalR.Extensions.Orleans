using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Orleans;
using SignalR.Extensions.Orleans.Grains;
using SignalR.Extensions.Orleans.Internals;
using SignalR.Extensions.Orleans.Streams;
using SignalR.Extensions.Orleans.Streams.Recipients;

namespace SignalR.Extensions.Orleans
{
    public class OrleansHubLifetimeManager<THub> : HubLifetimeManager<THub>
        where THub : Hub
    {
        private readonly IClusterClient _clusterClient;
        private readonly IMessageFactory _messageFactory;
        private readonly IHubLifetimeManager _inner;

        public OrleansHubLifetimeManager(
            IClusterClient clusterClient,
            IMessageFactory messageFactory,
            IInnerHubLifetimeManagerFactory managerFactory)
        {
            _clusterClient = clusterClient;
            _messageFactory = messageFactory;
            _inner = managerFactory.Create(GetHubTypeName());
        }

        protected ISignalRGrain Grain => _clusterClient.GetGrain<ISignalRGrain>(0);

        public override Task OnConnectedAsync(HubConnectionContext connection)
        {
            return _inner.OnConnectedAsync(connection);
        }

        public override Task OnDisconnectedAsync(HubConnectionContext connection)
        {
            return _inner.OnDisconnectedAsync(connection);
        }

        public override Task AddToGroupAsync(
            string connectionId,
            string groupName,
            CancellationToken cancellationToken = default)
        {
            return _inner.AddToGroupAsync(connectionId, groupName, cancellationToken);
        }

        public override Task RemoveFromGroupAsync(string connectionId, string groupName,
            CancellationToken cancellationToken = default)
        {
            return _inner.RemoveFromGroupAsync(connectionId, groupName, cancellationToken);
        }

        public override async Task SendAllAsync(
            string methodName,
            object[] args,
            CancellationToken cancellationToken = default)
        {
            await _inner.SendAllAsync(methodName, args, cancellationToken);
            await Grain.SendToNodes(_messageFactory.Create<THub>(
                methodName, args, new AllRecipients()));
        }

        public override async Task SendAllExceptAsync(
            string methodName,
            object[] args,
            IReadOnlyList<string> excludedConnectionIds,
            CancellationToken cancellationToken = default)
        {
            await _inner.SendAllExceptAsync(methodName, args, excludedConnectionIds, cancellationToken);
            await Grain.SendToNodes(_messageFactory.Create<THub>(
                methodName, args, new AllRecipients().WithoutConnectionIds(excludedConnectionIds)));
        }

        public override async Task SendConnectionAsync(
            string connectionId,
            string methodName,
            object[] args,
            CancellationToken cancellationToken = default)
        {
            await _inner.SendConnectionAsync(connectionId, methodName, args, cancellationToken);
            await Grain.SendToNodes(_messageFactory.Create<THub>(
                methodName, args, new ConnectionRecipients().WithConnectionId(connectionId)));
        }

        public override async Task SendConnectionsAsync(
            IReadOnlyList<string> connectionIds,
            string methodName,
            object[] args,
            CancellationToken cancellationToken = default)
        {
            await _inner.SendConnectionsAsync(connectionIds, methodName, args, cancellationToken);
            await Grain.SendToNodes(_messageFactory.Create<THub>(
                methodName, args, new ConnectionRecipients().WithConnectionIds(connectionIds)));
        }

        public override async Task SendGroupAsync(
            string groupName,
            string methodName,
            object[] args,
            CancellationToken cancellationToken = default)
        {
            await _inner.SendGroupAsync(groupName, methodName, args, cancellationToken);
            await Grain.SendToNodes(_messageFactory.Create<THub>(
                methodName, args, new GroupRecipients().WithGroup(groupName)));
        }

        public override async Task SendGroupsAsync(
            IReadOnlyList<string> groupNames,
            string methodName,
            object[] args,
            CancellationToken cancellationToken = default)
        {
            await _inner.SendGroupsAsync(groupNames, methodName, args, cancellationToken);
            await Grain.SendToNodes(_messageFactory.Create<THub>(
                methodName, args, new GroupRecipients().WithGroups(groupNames)));
        }

        public override async Task SendGroupExceptAsync(
            string groupName,
            string methodName,
            object[] args,
            IReadOnlyList<string> excludedConnectionIds,
            CancellationToken cancellationToken = default)
        {
            await _inner.SendGroupExceptAsync(groupName, methodName, args, excludedConnectionIds, cancellationToken);
            await Grain.SendToNodes(_messageFactory.Create<THub>(
                methodName, args, new GroupRecipients().WithGroup(groupName).WithoutConnectionIds(excludedConnectionIds)));
        }

        public override async Task SendUserAsync(
            string userId,
            string methodName,
            object[] args,
            CancellationToken cancellationToken = default)
        {
            await _inner.SendUserAsync(userId, methodName, args, cancellationToken);
            await Grain.SendToNodes(_messageFactory.Create<THub>(
                methodName, args, new UserRecipients().WithUserId(userId)));
        }

        public override async Task SendUsersAsync(
            IReadOnlyList<string> userIds,
            string methodName,
            object[] args,
            CancellationToken cancellationToken = default)
        {
            await _inner.SendUsersAsync(userIds, methodName, args, cancellationToken);
            await Grain.SendToNodes(_messageFactory.Create<THub>(
                methodName, args, new UserRecipients().WithUserIds(userIds)));
        }

        private static string GetHubTypeName()
        {
            return typeof(THub).AssemblyQualifiedName ?? throw new NullReferenceException(
                $"The hub '{typeof(THub)}' does not have an assembly qualified name");
        }
    }
}