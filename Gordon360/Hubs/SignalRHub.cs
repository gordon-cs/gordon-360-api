using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Gordon360;
using Gordon360.Services;
using Gordon360.ApiControllers;
using System.Web.Http;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using System.Security.Claims;

namespace Gordon360.Hubs
{
    [RoutePrefix("api/signalr")]
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {
        //public DirectMessageService _DirectMessageService = new DirectMessageService();

        public async Task refreshMessages(List<string> connectionIds, IEnumerable<MessageViewModel> message)
         {
            foreach(string connections in connectionIds)
            {
                await Groups.Add(connections, "list");
            }
            await Clients.Group("list").SendAsync("Received message refresh, Message: ", message);
         }

        public string savedUserId;
        public string savedConnectionId;

        public  void saveConnection(string id)
        {
            DirectMessageService _DirectMessageService = new DirectMessageService();
            savedUserId = id;
            savedConnectionId = Context.ConnectionId;
            _DirectMessageService.StoreUserConnectionIds(id, Context.ConnectionId);
             Clients.All.BroadcastMessage("It's working boss");

        }

        public override Task OnDisconnected(bool stopCalled)
        {
            DirectMessageService _DirectMessageService = new DirectMessageService();

            _DirectMessageService.DeleteUserConnectionIds(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            DirectMessageService _DirectMessageService = new DirectMessageService();

            _DirectMessageService.DeleteUserConnectionIds(savedConnectionId);
            _DirectMessageService.StoreUserConnectionIds(savedUserId, Context.ConnectionId);
            return base.OnReconnected();
        }
    }
 

}