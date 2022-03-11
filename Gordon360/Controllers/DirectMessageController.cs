//using System;
//using System.Security.Claims;
//using System.Net.Http;
//using Gordon360.Exceptions;
//using Gordon360.Repositories;
//using System.Collections.Generic;
//using Gordon360.Services;
//using Gordon360.Models.ViewModels;
//using Gordon360.Models;
//using System.Net.Http.Headers;
//using System.Text;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using Gordon360.Database.CCT;

//namespace Gordon360.Controllers
//{
//    [Route("api/[controller]")]
//    public class DirectMessageController : GordonControllerBase
//    {
//        private IProfileService _profileService;
//        private IAccountService _accountService;
//        private IRoleCheckingService _roleCheckingService;

//        private IDirectMessageService _DirectMessageService;

//        public DirectMessageController(CCTContext context)
//        {
//            _DirectMessageService = new DirectMessageService(context);
//            _profileService = new ProfileService(context);
//            _accountService = new AccountService(context);
//            _roleCheckingService = new RoleCheckingService(context);
//        }

//        public DirectMessageController(IDirectMessageService DirectMessageService)
//        {
//            _DirectMessageService = DirectMessageService;
//        }

//        /// <summary>
//        ///  returns messages from a specified group
//        /// </summary>
//        /// <returns>MessageViewModel</returns>
//        [HttpPut]
//        [Route("messages")]
//        public ActionResult<IEnumerable<MessageViewModel>> GetMessages([FromBody] string roomId)
//        {
//            // DateTime currentTime = DateTime.Now;
//            var result = _DirectMessageService.GetMessages(roomId);

//            if (result == null)
//            {
//                return NotFound();
//            }

//            return Ok(result);
//        }

//        /// <summary>
//        ///  returns single message from a specified group and message id
//        /// </summary>
//        /// <returns>MessageViewModel</returns>
//        [HttpPut]
//        [Route("singleMessage")]
//        public ActionResult<MessageViewModel> GetSingleMessage([FromBody] MessageSearchViewModel model)
//        {
//            var result = _DirectMessageService.GetSingleMessage(model.messageID, model.roomID);

//            if (result == null)
//            {
//                return NotFound();
//            }

//            return Ok(result);
//        }

//        /// <summary>
//        ///  returns room ids associated with this user
//        /// </summary>
//        /// <returns>MessageViewModel</returns>
//        [HttpGet]
//        [Route("roomsIds")]
//        public ActionResult<IEnumerable<GroupViewModel>> GetRooms()
//        {
//            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

//            // DateTime currentTime = DateTime.Now;
//            var result = _DirectMessageService.GetRooms(authenticatedUserIdString);

//            if (result == null)
//            {
//                return NotFound();
//            }

//            return Ok(result);
//        }


//        /// <summary>
//        ///  returns a room object Identified by room id
//        /// </summary>
//        /// <returns>MessageViewModel</returns>
//        [HttpGet]
//        [Route("rooms")]
//        public ActionResult<List<Object>> GetRoomObject()
//        {
//            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

//            var result = _DirectMessageService.GetRoomById(authenticatedUserIdString);

//            if (result == null)
//            {
//                return NotFound();
//            }

//            return Ok(result);
//        }

//        /// <summary>
//        ///  Gets a single room object specified by a room id
//        /// </summary>
//        /// <returns>true if successful </returns>
//        [HttpPost]
//        [Route("singleRoom")]
//        public ActionResult<Object> GetSingleRoom([FromBody] int roomId)
//        {

//            var result = _DirectMessageService.GetSingleRoom(roomId);

//            if (result == null)
//            {
//                return NotFound();
//            }


//            return Ok(result);

//        }


//        /// <summary>
//        ///  creates a message room in the backend
//        /// </summary>
//        /// <returns>true if successful </returns>
//        [HttpPost]
//        [Route("")]
//        public ActionResult<CreateGroupViewModel> CreateGroup([FromBody] CreateGroupViewModel chatInfo)
//        {
//            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

//            var result = _DirectMessageService.CreateGroup(chatInfo.name, chatInfo.group, chatInfo.image, chatInfo.usernames, chatInfo.message, authenticatedUserIdString);

//            if (result == null)
//            {
//                return NotFound();
//            }


//            return Ok(result);

//        }

//        /// <summary>
//        ///  stores sent message on the back end.
//        /// </summary>
//        /// <returns>true if successful </returns>
//        [HttpPut]
//        [Route("text")]
//        public ActionResult<bool> SendMessage([FromBody] SendTextViewModel textInfo)
//            {

//                var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

//                var result = _DirectMessageService.SendMessage(textInfo, authenticatedUserIdString);

//                var data = new NotificationDataViewModel();
//                data.messageID = textInfo.id;
//                data.roomID = textInfo.room_id;

//                var connectionIDs = _DirectMessageService.GetUserConnectionIds(textInfo.users_ids);

//                if (result == false || connectionIDs == null)
//                {
//                    return NotFound();
//                }

//                foreach (IEnumerable<ConnectionIdViewModel> connections in connectionIDs)
//                {
//                    foreach (ConnectionIdViewModel model in connections)
//                    {
//                        var pushNotification = new PushNotificationViewModel();
//                        pushNotification.body = textInfo.groupText;
//                        pushNotification.to = model.connection_id;
//                        pushNotification.data = data;
//                        pushNotification.title = textInfo.groupName;

//                        var myJsonObject = Newtonsoft.Json.JsonConvert.SerializeObject(pushNotification);

//                        HttpClient httpClient = new HttpClient();

//                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


//                        var content = new StringContent(myJsonObject.ToString(), Encoding.UTF8, "application/json");
//                        var post = httpClient.PostAsync("https://exp.host/--/api/v2/push/send", content).Result;
//                    }
//                 }

//                return Ok(result);

//            }

//            /// <summary>
//            ///  stores rooms associated with a user id
//            /// </summary>
//            /// <returns>true if successful</returns>
//            [HttpPost]
//            [Route("userRooms")]
//            public ActionResult<bool> StoreUserRooms([FromBody] String roomId)
//            {

//                var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

//                var result = _DirectMessageService.StoreUserRooms(authenticatedUserIdString, roomId);

//                if (result == false)
//                {
//                    return NotFound();
//                }


//                return Ok(result);

//            }



//            /// <summary>
//            ///  stores connection associated with a user id
//            /// </summary>
//            /// <returns>true if successful</returns>
//            [HttpPost]
//            [Route("userConnectionIds")]
//            public ActionResult<bool> StoreUserConnectionIds([FromBody] String connectionId)
//            {
//                var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;

//                var result = _DirectMessageService.StoreUserConnectionIds(authenticatedUserIdString, connectionId);

//                if (result == false)
//                {
//                    return NotFound();
//                }


//                return Ok(result);

//            }

//            /// <summary>
//            ///  Gets connection ids associated with a user id
//            /// </summary>
//            /// <returns>true if successful</returns>
//            [HttpPut]
//            [Route("userConnectionIds")]
//            public ActionResult<bool> GetConnectionIds([FromBody] List<string> userIds)
//            {
//                var result = _DirectMessageService.GetUserConnectionIds(userIds);

//                if (result == null)
//                {
//                    return NotFound();
//                }

//                return Ok(result);
//            }

//            /// <summary>
//            ///  Deletes connection ids associated with a user id
//            /// </summary>
//            /// <returns>true if successful</returns>
//            [HttpPut]
//            [Route("deleteUserConnectionIds")]
//            public ActionResult<bool> DeleteConnectionIds([FromBody] String connectionId)
//            {
//                var result = _DirectMessageService.DeleteUserConnectionIds(connectionId);
//                return Ok(result);
//            }
//    }
//}
