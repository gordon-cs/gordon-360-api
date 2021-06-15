using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Gordon360;
using Gordon360.Utils;
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
        public async Task refreshMessages(List<string> userIds, SendTextViewModel message, string userId)
         {
            DirectMessageService _DirectMessageService = new DirectMessageService();
            var connectionIds =_DirectMessageService.GetUserConnectionIds(userIds);
            
            foreach (var connections in connectionIds)
            {
                foreach (var userConnections in connections)
                {
                    await Clients.Client(userConnections.connection_id).SendAsync(message, userId);
                }
            }
         }

        public string savedUserId;
        public string savedConnectionId;

        public async Task saveFunction(string id)
        {
            DirectMessageService _DirectMessageService = new DirectMessageService();
            _DirectMessageService.StoreUserConnectionIds(id, Context.ConnectionId);
        }

        public  async Task saveConnection(string id)
        {
            savedUserId = id;
            savedConnectionId = Context.ConnectionId;
            await saveFunction(id);

            Clients.All.BroadcastMessage("Saved Connection!");

        }

        public override Task OnConnected()
        {
            Clients.All.BroadcastMessage("connectedToServer");
            return base.OnConnected();
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