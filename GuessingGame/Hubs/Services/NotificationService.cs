using System.Threading.Tasks;
using GuessingGame.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;

namespace GuessingGame.Hubs.Services{
    public interface INotificationService{
         Task SendNotification(int gameid);
    }
    public class NotificationService:INotificationService{
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationService(IHubContext<NotificationHub> hubContext){
            this._hubContext = hubContext;
        }
        public  async Task SendNotification(int gameid){
            string groupName = "Game"+gameid.ToString(); 
            await _hubContext.Clients.Groups(groupName).SendAsync("ReceiveNotification");
        }
    }
}