using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;

namespace GuessingGame.Hubs
{
    public class NotificationHub : Hub
    {
        public Task SendNotification(string user, string message)
        {
            return Clients.All.SendAsync("ReceiveNotification", user, message);
        }
        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
        }
    }
}