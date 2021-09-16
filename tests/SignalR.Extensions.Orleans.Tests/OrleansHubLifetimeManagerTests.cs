using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApi.Hubs;
using Microsoft.AspNetCore.SignalR.Client;
using SignalR.Extensions.Orleans.Tests.Fixtures;
using Xunit;

namespace SignalR.Extensions.Orleans.Tests
{
    public class OrleansHubLifetimeManagerTests : UsingChatApiFixtureCollection, IDisposable
    {
        private readonly List<HubConnectionDetail> _connectionDetails = new List<HubConnectionDetail>();

        public OrleansHubLifetimeManagerTests(ChatApiFixture fixture)
            : base(fixture)
        {
            SetupHubs().GetAwaiter().GetResult();
        }

        protected Task SetupHubs()
        {
            _connectionDetails.AddRange(Fixture.NodeA.CreateHubConnections(
                "Bob",
                "Harry",
                "Sally"));

            _connectionDetails.AddRange(Fixture.NodeB.CreateHubConnections(
                "Amy",
                "Tim",
                "John"));

            return Task.WhenAll(_connectionDetails.Select(d => d.HubConnection.StartAsync()));
        }

        public void Dispose()
        {
            _connectionDetails.ForEach(d => d.Dispose());
        }


        [Fact]
        public async Task SendsMessageToAllConnections()
        {
            var connectionReceivedMessage = _connectionDetails.ToDictionary(k => k.ConnectionId, _ => string.Empty);

            _connectionDetails.ForEach(d =>
            {
                d.HubConnection.On<string>(nameof(IChatHub.ReceiveMessage), message =>
                {
                    connectionReceivedMessage[d.ConnectionId] = message;
                    d.Signaler.Signal();
                });
            });

            await _connectionDetails.First().HubConnection.SendAsync(nameof(ChatHub.BroadcastMessage), "hello everyone!");
            await Task.WhenAll(_connectionDetails.Select(d => d.Signaler.WaitForSignal()));

            Assert.All(connectionReceivedMessage.Values,
                message => Assert.Equal("hello everyone!", message));
        }

        [Fact]
        public async Task SendsMessageToOtherConnections()
        {
            var connectionReceivedMessage = _connectionDetails.ToDictionary(k => k.ConnectionId, _ => string.Empty);

            _connectionDetails.ForEach(d =>
            {
                d.HubConnection.On<string>(nameof(IChatHub.ReceiveMessage), message =>
                {
                    connectionReceivedMessage[d.ConnectionId] = message;
                    d.Signaler.Signal();
                });
            });

            var sender = _connectionDetails.First();
            await sender.HubConnection.SendAsync(nameof(ChatHub.SendMessageToOthers), "hello others!");
            await Task.WhenAll(_connectionDetails.Skip(1).Select(d => d.Signaler.WaitForSignal()));

            await Assert.ThrowsAsync<TimeoutException>(() => sender.Signaler.WaitForSignal());
            Assert.Equal(string.Empty, connectionReceivedMessage.First().Value);
            Assert.All(connectionReceivedMessage.Values.Skip(1),
                message => Assert.Equal("hello others!", message));
        }

        [Fact]
        public async Task SendsMessageToSpecificUser()
        {
            var connectionReceivedMessage = _connectionDetails.ToDictionary(k => k.ConnectionId, _ => string.Empty);

            _connectionDetails.ForEach(d =>
            {
                d.HubConnection.On<string>(nameof(IChatHub.ReceiveMessage), message =>
                {
                    connectionReceivedMessage[d.ConnectionId] = message;
                    d.Signaler.Signal();
                });
            });

            var sender = _connectionDetails.First();
            var expectedReceiver = _connectionDetails.Skip(1).First();
            var otherConnections = _connectionDetails.DropIndex(1).ToList();

            await sender.HubConnection.SendAsync(nameof(ChatHub.Whisper), expectedReceiver.UserId, $"hello {expectedReceiver.UserId}!");

            await Task.WhenAll(otherConnections.Select(d =>
                Assert.ThrowsAsync<TimeoutException>(() => d.Signaler.WaitForSignal())));
            Assert.Equal($"hello {expectedReceiver.UserId}!", connectionReceivedMessage[expectedReceiver.ConnectionId]);
            Assert.All(otherConnections, d => Assert.Equal(string.Empty, connectionReceivedMessage[d.ConnectionId]));
        }

        [Fact]
        public async Task SendsMessageToSpecificUserss()
        {
            var connectionReceivedMessages = _connectionDetails.ToDictionary(k => k.ConnectionId, _ => string.Empty);

            _connectionDetails.ForEach(d =>
            {
                d.HubConnection.On<string>(nameof(IChatHub.ReceiveMessage), receivedMessage =>
                {
                    connectionReceivedMessages[d.ConnectionId] = receivedMessage;
                    d.Signaler.Signal();
                });
            });

            var sender = _connectionDetails.First();
            var recipientConnections = _connectionDetails.Skip(2).ToList();

            var message = $"hello {string.Join(',', recipientConnections.Select(d => d.UserId))}!";
            await sender.HubConnection.SendAsync(nameof(ChatHub.WhisperMany), recipientConnections.Select(d => d.UserId), message);

            await Task.WhenAll(_connectionDetails.Take(2).Select(d =>
                Assert.ThrowsAsync<TimeoutException>(() => d.Signaler.WaitForSignal())));
            await Task.WhenAll(recipientConnections.Select(d => d.Signaler.WaitForSignal()));
            Assert.All(recipientConnections, d => Assert.Equal(message, connectionReceivedMessages[d.ConnectionId]));
        }

        [Fact]
        public async Task SendsMessageToSpecificConnection()
        {
            var connectionReceivedFile = _connectionDetails.ToDictionary(k => k.ConnectionId, _ => null as File);

            _connectionDetails.ForEach(d =>
            {
                d.HubConnection.On<File>(nameof(IChatHub.ReceiveFile), receivedFile =>
                {
                    connectionReceivedFile[d.ConnectionId] = receivedFile;
                    d.Signaler.Signal();
                });
            });

            var sender = _connectionDetails.First();
            var recipientConnection = _connectionDetails.Last();
            var otherConnections = _connectionDetails.Take(_connectionDetails.Count - 1);

            var file = new File
            {
                FileName = "foo.txt",
                Contents = System.Text.Encoding.UTF8.GetBytes("bar,baz"),
            };

            await sender.HubConnection.SendAsync(nameof(ChatHub.SendFile), file, recipientConnection.ConnectionId);

            await Task.WhenAll(otherConnections.Select(d =>
                Assert.ThrowsAsync<TimeoutException>(() => d.Signaler.WaitForSignal())));

            await recipientConnection.Signaler.WaitForSignal();
            var receivedFile = connectionReceivedFile[recipientConnection.ConnectionId];
            Assert.Equal("foo.txt", receivedFile.FileName);
            Assert.Equal(file.Contents.Length, receivedFile.Contents.Length);
        }

        [Fact]
        public async Task SendsMessageToSpecificConnections()
        {
            var connectionReceivedFiles = _connectionDetails.ToDictionary(k => k.ConnectionId, _ => null as File);

            _connectionDetails.ForEach(d =>
            {
                d.HubConnection.On<File>(nameof(IChatHub.ReceiveFile), receivedFile =>
                {
                    connectionReceivedFiles[d.ConnectionId] = receivedFile;
                    d.Signaler.Signal();
                });
            });

            var sender = _connectionDetails.First();
            var recipientConnections = _connectionDetails.Skip(2).ToList();

            var file = new File
            {
                FileName = "foo.txt",
                Contents = System.Text.Encoding.UTF8.GetBytes("bar,baz"),
            };

            await sender.HubConnection.SendAsync(nameof(ChatHub.SendFileToConnections), file, recipientConnections.Select(d => d.ConnectionId));

            await Task.WhenAll(_connectionDetails.Take(2).Select(d =>
                Assert.ThrowsAsync<TimeoutException>(() => d.Signaler.WaitForSignal())));
            Assert.All(recipientConnections, d =>
            {
                Assert.Equal("foo.txt", connectionReceivedFiles[d.ConnectionId].FileName);
                Assert.Equal(file.Contents.Length, connectionReceivedFiles[d.ConnectionId].Contents.Length);
            });
        }

        [Fact]
        public async Task ReceivesMessagesInGroup()
        {
            var connectionReceivedMessages = _connectionDetails.ToDictionary(k => k.ConnectionId, _ => string.Empty);

            _connectionDetails.ForEach(d =>
            {
                d.HubConnection.On<string>(nameof(IChatHub.ReceiveMessage), receivedMessage =>
                {
                    connectionReceivedMessages[d.ConnectionId] = receivedMessage;
                    d.Signaler.Signal();
                });
            });

            var projectXRoomConnections = _connectionDetails.GroupBy(d => d.NodeId).Select(n => n.First()).ToList();
            var otherConnections = _connectionDetails.GroupBy(d => d.NodeId).SelectMany(n => n.Skip(1));

            await Task.WhenAll(projectXRoomConnections.Select(d =>
                d.HubConnection.SendAsync(nameof(ChatHub.JoinChatRoom), "project-x")));
            await projectXRoomConnections.First().HubConnection.SendAsync(
                nameof(ChatHub.BroadcastMessageToRoom), "project-x", "this is a priority!");
            await Task.WhenAll(projectXRoomConnections.Select(d => d.Signaler.WaitForSignal()));

            await Task.WhenAll(otherConnections.Select(d =>
                Assert.ThrowsAsync<TimeoutException>(() => d.Signaler.WaitForSignal())));
            Assert.All(projectXRoomConnections, d => Assert.Equal("this is a priority!", connectionReceivedMessages[d.ConnectionId]));

            await Task.WhenAll(projectXRoomConnections.Select(d =>
                d.HubConnection.SendAsync(nameof(ChatHub.LeaveChatRoom), "project-x")));
        }

        [Fact]
        public async Task OthersReceivesMessagesInGroup()
        {
            var connectionReceivedMessages = _connectionDetails.ToDictionary(k => k.ConnectionId, _ => string.Empty);

            _connectionDetails.ForEach(d =>
            {
                d.HubConnection.On<string>(nameof(IChatHub.ReceiveMessage), receivedMessage =>
                {
                    connectionReceivedMessages[d.ConnectionId] = receivedMessage;
                    d.Signaler.Signal();
                });
            });

            var projectXRoomConnections = _connectionDetails.GroupBy(d => d.NodeId).Select(n => n.First()).ToList();
            var otherConnections = _connectionDetails.GroupBy(d => d.NodeId).SelectMany(n => n.Skip(1));

            await Task.WhenAll(projectXRoomConnections.Select(d =>
                d.HubConnection.SendAsync(nameof(ChatHub.JoinChatRoom), "project-x")));
            await projectXRoomConnections.First().HubConnection.SendAsync(
                nameof(ChatHub.BroadcastMessageToOthersInRoom), "project-x", "this is a priority!");
            await Task.WhenAll(projectXRoomConnections.Skip(1).Select(d => d.Signaler.WaitForSignal()));

            await Assert.ThrowsAsync<TimeoutException>(() =>
                projectXRoomConnections.First().Signaler.WaitForSignal());
            await Task.WhenAll(otherConnections.Select(d =>
                Assert.ThrowsAsync<TimeoutException>(() => d.Signaler.WaitForSignal())));
            Assert.All(projectXRoomConnections.Skip(1), d => Assert.Equal("this is a priority!", connectionReceivedMessages[d.ConnectionId]));
        }

        [Fact]
        public async Task ReceivesMessagesInGroups()
        {
            var connectionReceivedMessages = _connectionDetails.ToDictionary(k => k.ConnectionId, _ => string.Empty);

            _connectionDetails.ForEach(d =>
            {
                d.HubConnection.On<string>(nameof(IChatHub.ReceiveMessage), receivedMessage =>
                {
                    connectionReceivedMessages[d.ConnectionId] = receivedMessage;
                    d.Signaler.Signal();
                });
            });

            var projectXRoomConnections = _connectionDetails.GroupBy(d => d.NodeId).Select(n => n.First()).ToList();
            var projectYRoomConnections = _connectionDetails.GroupBy(d => d.NodeId).Select(n => n.Skip(1).First()).ToList();

            var otherConnections = _connectionDetails.GroupBy(d => d.NodeId).Select(n => n.Last());

            await Task.WhenAll(projectXRoomConnections.Select(d =>
                d.HubConnection.SendAsync(nameof(ChatHub.JoinChatRoom), "project-x")));
            await Task.WhenAll(projectYRoomConnections.Select(d =>
                d.HubConnection.SendAsync(nameof(ChatHub.JoinChatRoom), "project-y")));

            await projectXRoomConnections.First().HubConnection.SendAsync(
                nameof(ChatHub.BroadcastMessageToRooms), new List<string> { "project-x", "project-y" }, "why isn't this done yet?!");
            await Task.WhenAll(projectXRoomConnections.Select(d => d.Signaler.WaitForSignal()));
            await Task.WhenAll(projectYRoomConnections.Select(d => d.Signaler.WaitForSignal()));


            await Task.WhenAll(otherConnections.Select(d =>
                Assert.ThrowsAsync<TimeoutException>(() => d.Signaler.WaitForSignal())));
            Assert.All(projectXRoomConnections, d => Assert.Equal("why isn't this done yet?!", connectionReceivedMessages[d.ConnectionId]));
            Assert.All(projectYRoomConnections, d => Assert.Equal("why isn't this done yet?!", connectionReceivedMessages[d.ConnectionId]));
        }
    }
}
