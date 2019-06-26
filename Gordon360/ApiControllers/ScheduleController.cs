using System.Web.Http;
using Gordon360.Models;
using Gordon360.Services;
using Gordon360.Repositories;
using Gordon360.Models.ViewModels;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using System;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Exceptions.CustomExceptions;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/schedule")]
    [CustomExceptionFilter]
    [Authorize]
    public class ScheduleController : ApiController
    {
        //declare services we are going to use.
        private IProfileService _profileService;
        private IAccountService _accountService;
        private IRoleCheckingService _roleCheckingService;

        private IScheduleService _scheduleService;


        public ScheduleController()
        {
            var _unitOfWork = new UnitOfWork();
            _scheduleService = new ScheduleService(_unitOfWork);
            _profileService = new ProfileService(_unitOfWork);
            _accountService = new AccountService(_unitOfWork);
            _roleCheckingService = new RoleCheckingService(_unitOfWork);
        }

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        /// <summary>
        ///  Gets all schedule objects for a user
        /// </summary>
        /// <returns>A IEnumerable of schedule objects</returns>
        [HttpGet]
        [Route("get")]
        public IHttpActionResult Get()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            var id = _accountService.GetAccountByUsername(username).GordonID;
            //var idInt = Int32.Parse(id);

            var role = _roleCheckingService.getCollegeRole(username);

            if (role=="student"){
                var result = _scheduleService.GetScheduleStudent(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }

            else if (role=="facstaff"){
                var result = _scheduleService.GetScheduleFaculty(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            return NotFound();
           
        }

        /// <summary>
        ///  Gets all schedule objects for a user
        /// </summary>
        /// <returns>A IEnumerable of schedule objects</returns>
        [HttpGet]
        [Route("{username}")]
        public IHttpActionResult Get(string username)
        {
            //probably needs privacy stuff like ProfilesController and service
            var id = _accountService.GetAccountByUsername(username).GordonID;
            //var idInt = Int32.Parse(id);

            var role = _roleCheckingService.getCollegeRole(username);

            if (role == "student")
            {
                var result = _scheduleService.GetScheduleStudent(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }

            else if (role == "facstaff")
            {
                var result = _scheduleService.GetScheduleFaculty(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            return NotFound();
        }


        
        }
    }
