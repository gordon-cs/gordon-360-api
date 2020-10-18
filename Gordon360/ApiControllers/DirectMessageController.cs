using System;
using System.Security.Claims;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.Models;
using Gordon360.Exceptions.CustomExceptions;
using System.Linq;

namespace Gordon360.ApiControllers
{
        [RoutePrefix("api/dm")]
        [CustomExceptionFilter]
        [Authorize]
        public class DirectMessageController : ApiController
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
            ///  returns hello world example
            /// </summary>
            /// <returns>string and date</returns>
            [HttpGet]
            [Route("")]
            public IHttpActionResult Get()
            {
                DateTime currentTime = DateTime.Now;
                var result = "hello world I'm coming from the back end at: " + currentTime;

                return Ok(result);
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
            ///  returns rooms associated with this user
            /// </summary>
            /// <returns>MessageViewModel</returns>
            [HttpGet]
            [Route("rooms")]
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
            ///  creates a message room in the backend
            /// </summary>
            /// <returns>true if successful </returns>
            [HttpPost]
            [Route("")]
            public IHttpActionResult CreateGroup([FromBody] String name, bool group, DateTime lastUpdated)
            {

                Random rnd = new Random();
                int raw_id = rnd.Next(10000000, 99999999);
                var id = raw_id.ToString();

                var result = _DirectMessageService.CreateGroup(id ,name, group, lastUpdated);

                if (result == false)
                {
                    return NotFound();
                }


                return Created("Status of created group ", result);

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


                return Created("Status of Saved User Rooms ", result);

            }

            /// <summary>
            ///  stores sent message on the back end.
            /// </summary>
            /// <returns>true if successful </returns>
            [HttpPost]
            [Route("sendMessage")]
            public IHttpActionResult SendMessage([FromBody] String room_id, String text, bool system, bool sent, bool received, bool pending)
            {

                var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
                var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

                var userId = _accountService.GetAccountByUsername(username).GordonID;

                Random rnd = new Random();
                int raw_id = rnd.Next(10000000, 99999999);

                var id = raw_id.ToString();
                
                var result = _DirectMessageService.SendMessage(id, room_id, text, userId, system, sent, received, pending);

                if (result == false)
                {
                    return NotFound();
                }


                return Created("message was sent: ", result);

            }




    }

}