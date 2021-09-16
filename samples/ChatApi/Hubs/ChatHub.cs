using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ChatApi.Hubs
{
    public interface IChatHub
    {
        Task ReceiveMessage(string message);

        Task UserConnected(string userId);

        Task UserDisconnected(string userId);

        Task UserJoinedChatRoom(string userId);

        Task UserLeftChatRoom(string userId);

        Task ReceiveFile(File file);
    }

    public class ChatHub : Hub<IChatHub>
    {
        public override Task OnConnectedAsync()
        {
            return Clients.Others.UserConnected(Context.UserIdentifier);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return Clients.Others.UserDisconnected(Context.UserIdentifier);
        }

        public Task BroadcastMessage(string message)
        {
            return Clients.All.ReceiveMessage(message);
        }

        public Task BroadcastMessageToRoom(string toRoom, string message)
        {
            return Clients.Group(toRoom).ReceiveMessage(message);
        }

        public Task BroadcastMessageToOthersInRoom(string toRoom, string message)
        {
            return Clients.OthersInGroup(toRoom).ReceiveMessage(message);
        }

        public Task BroadcastMessageToRooms(List<string> toRooms, string message)
        {
            return Clients.Groups(toRooms).ReceiveMessage(message);
        }

        public Task Whisper(string toUser, string message)
        {
            return Clients.User(toUser).ReceiveMessage(message);
        }

        public Task WhisperMany(List<string> toUsers, string message)
        {
            return Clients.Users(toUsers).ReceiveMessage(message);
        }

        public Task SendMessageToOthers(string message)
        {
            return Clients.Others.ReceiveMessage(message);
        }

        public async Task JoinChatRoom(string room)
        {
            await Groups.AddToGroupAsync(this.Context.ConnectionId, room);
            await Clients.Groups(room).UserJoinedChatRoom(Context.UserIdentifier);
        }

        public async Task LeaveChatRoom(string room)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
            await Clients.Groups(room).UserLeftChatRoom(Context.UserIdentifier);
        }

        public Task SendFile(File file, string connectionId)
        {
            return Clients.Client(connectionId).ReceiveFile(file);
        }

        public Task SendFileToConnections(File file, List<string> connectionIds)
        {
            return Clients.Clients(connectionIds).ReceiveFile(file);
        }
    }

    public class File
    {
        public string FileName { get; set; }

        public byte[] Contents { get; set; }
    }
}
