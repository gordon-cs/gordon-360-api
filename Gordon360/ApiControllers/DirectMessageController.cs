using System;
using System.Security.Claims;
using System.Net;
using System.Net.Http;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using System.Collections.Generic;
using Gordon360.Services;
using Gordon360.Models.ViewModels;
using Gordon360.Models;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Services.ComplexQueries;
using System.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Gordon360.ApiControllers
{
    [RoutePrefix("api/dm")]
    [CustomExceptionFilter]
    [Authorize]
    public class DirectMessageController : ControllerBase
    {
        private IProfileService _profileService;
        private IAccountService _accountService;
        private IRoleCheckingService _roleCheckingService;

        private IDirectMessageService _DirectMessageService;

        public DirectMessageController()
        {
            var _unitOfWork = new UnitOfWork();
            _DirectMessageService = new DirectMessageService();
            _profileService = new ProfileService(_unitOfWork);
            _accountService = new AccountService(_unitOfWork);
            _roleCheckingService = new RoleCheckingService(_unitOfWork);
        }

        public DirectMessageController(IDirectMessageService DirectMessageService)
        {
            _DirectMessageService = DirectMessageService;
        }
        
        /// <summary>
        ///  returns messages from a specified group
        /// </summary>
        /// <returns>MessageViewModel</returns>
        [HttpPut]
        [Route("messages")]
        public IHttpActionResult GetMessages([FromBody] string roomId)
        {

            DateTime currentTime = DateTime.Now;
            var result = _DirectMessageService.GetMessages(roomId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        ///  returns single message from a specified group and message id
        /// </summary>
        /// <returns>MessageViewModel</returns>
        [HttpPut]
        [Route("singleMessage")]
        public IHttpActionResult GetSingleMessage([FromBody] MessageSearchViewModel model)
        {

            var result = _DirectMessageService.GetSingleMessage(model.messageID, model.roomID);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        ///  returns room ids associated with this user
        /// </summary>
        /// <returns>MessageViewModel</returns>
        [HttpGet]
        [Route("roomsIds")]
        public IHttpActionResult GetRooms()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            var userId = _accountService.GetAccountByUsername(username).GordonID;

            DateTime currentTime = DateTime.Now;
            var result = _DirectMessageService.GetRooms(userId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        
        /// <summary>
        ///  returns a room object Identified by room id
        /// </summary>
        /// <returns>MessageViewModel</returns>
        [HttpGet]
        [Route("rooms")]
        public IHttpActionResult GetRoomObject()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var userId = _accountService.GetAccountByUsername(username).GordonID;
            
            var result = _DirectMessageService.GetRoomById(userId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        ///  Gets a single room object specified by a room id
        /// </summary>
        /// <returns>true if successful </returns>
        [HttpPost]
        [Route("singleRoom")]
        public IHttpActionResult GetSingleRoom([FromBody] int roomId)
        {

            var result = _DirectMessageService.GetSingleRoom(roomId);

            if (result == null)
            {
                return NotFound();
            }


            return Ok(result);

        }


        /// <summary>
        ///  creates a message room in the backend
        /// </summary>
        /// <returns>true if successful </returns>
        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateGroup([FromBody] CreateGroupViewModel chatInfo)
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var userId = _accountService.GetAccountByUsername(username).GordonID;

            var result = _DirectMessageService.CreateGroup(chatInfo.name, chatInfo.group, chatInfo.image, chatInfo.usernames, chatInfo.message, userId);

            if (result == null)
            {
                return NotFound();
            }


            return Ok(result);

        }

        /// <summary>
        ///  stores sent message on the back end.
        /// </summary>
        /// <returns>true if successful </returns>
        [HttpPut]
        [Route("text")]
        public IHttpActionResult SendMessage([FromBody] SendTextViewModel textInfo)
            {

                var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
                var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

                var user_id = _accountService.GetAccountByUsername(username).GordonID;

                var result = _DirectMessageService.SendMessage(textInfo, user_id.ToString());

                var data = new NotificationDataViewModel();
                data.messageID = textInfo.id;
                data.roomID = textInfo.room_id;
                
                var connectionIDs = _DirectMessageService.GetUserConnectionIds(textInfo.users_ids);

                if (result == false || connectionIDs == null)
                {
                    return NotFound();
                }

                foreach (IEnumerable<ConnectionIdViewModel> connections in connectionIDs)
                {
                    foreach (ConnectionIdViewModel model in connections)
                    {
                        var pushNotification = new PushNotificationViewModel();
                        pushNotification.body = textInfo.groupText;
                        pushNotification.to = model.connection_id;
                        pushNotification.data = data;
                        pushNotification.title = textInfo.groupName;

                        var myJsonObject = Newtonsoft.Json.JsonConvert.SerializeObject(pushNotification);

                        HttpClient httpClient = new HttpClient();

                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                        var content = new StringContent(myJsonObject.ToString(), Encoding.UTF8, "application/json");
                        var post = httpClient.PostAsync("https://exp.host/--/api/v2/push/send", content).Result;
                    }
                 }
            
                return Ok(result);

            }

            /// <summary>
            ///  stores rooms associated with a user id
            /// </summary>
            /// <returns>true if successful</returns>
            [HttpPost]
            [Route("userRooms")]
            public IHttpActionResult StoreUserRooms([FromBody] String roomId)
            {

                var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
                var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

                var userId = _accountService.GetAccountByUsername(username).GordonID;

                var result = _DirectMessageService.StoreUserRooms(userId, roomId);

                if (result == false)
                {
                    return NotFound();
                }


                return Ok(result);

            }

            
        
            /// <summary>
            ///  stores connection associated with a user id
            /// </summary>
            /// <returns>true if successful</returns>
            [HttpPost]
            [Route("userConnectionIds")]
            public IHttpActionResult StoreUserConnectionIds([FromBody] String connectionId)
            {
                var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
                var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

                var userId = _accountService.GetAccountByUsername(username).GordonID;

                var result = _DirectMessageService.StoreUserConnectionIds(userId, connectionId);

                if (result == false)
                {
                    return NotFound();
                }


                return Ok(result);

            }

            /// <summary>
            ///  Gets connection ids associated with a user id
            /// </summary>
            /// <returns>true if successful</returns>
            [HttpPut]
            [Route("userConnectionIds")]
            public IHttpActionResult GetConnectionIds([FromBody] List<string> userIds)
            {


                var result = _DirectMessageService.GetUserConnectionIds(userIds);

                if (result == null)
                {
                    return NotFound();
                }


                return Ok(result);

            }

            /// <summary>
            ///  Deletes connection ids associated with a user id
            /// </summary>
            /// <returns>true if successful</returns>
            [HttpPut]
            [Route("deleteUserConnectionIds")]
            public IHttpActionResult DeleteConnectionIds([FromBody] String connectionId)
            {


                var result = _DirectMessageService.DeleteUserConnectionIds(connectionId);

                if (result == null)
                {
                    return NotFound();
                }


                return Ok(result);

            }


    }

}