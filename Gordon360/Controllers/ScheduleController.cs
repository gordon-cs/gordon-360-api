using Gordon360.Database.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly CCTContext _context;

        private readonly IAccountService _accountService;
        private readonly IRoleCheckingService _roleCheckingService;
        private readonly IScheduleService _scheduleService;

        public ScheduleController(CCTContext context)
        {
            _context = context;
            _scheduleService = new ScheduleService(context);
            _accountService = new AccountService(context);
            _roleCheckingService = new RoleCheckingService(context);
        }

        /// <summary>
        ///  Gets all schedule objects for a user
        /// </summary>
        /// <returns>A IEnumerable of schedule objects</returns>
        [HttpGet]
        [Route("")]
        public ActionResult<ScheduleViewModel> Get()
        {
            var authenticatedUserUsername = AuthUtils.GetAuthenticatedUserUsername(User);
            var role = _roleCheckingService.GetCollegeRole(authenticatedUserUsername);

            if (role == "student")
            {
                var result = _scheduleService.GetScheduleStudentAsync(authenticatedUserUsername);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }

            else if (role == "facstaff")
            {
                var result = _scheduleService.GetScheduleFacultyAsync(authenticatedUserUsername);
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
        public ActionResult<JArray> Get(string username)
        {
            //probably needs privacy stuff like ProfilesController and service
            var authenticatedUserUsername = AuthUtils.GetAuthenticatedUserUsername(User);
            var viewerRole = _roleCheckingService.GetCollegeRole(authenticatedUserUsername);

            var role = _roleCheckingService.GetCollegeRole(username);
            var id = _accountService.GetAccountByUsername(username).GordonID;
            object scheduleResult = null;
            var scheduleControl = _context.Schedule_Control.Find(id);
            int schedulePrivacy = 1;

            // Getting student schedule
            if (role == "student")
            {
                try
                {
                    schedulePrivacy = scheduleControl.IsSchedulePrivate;
                }
                catch
                {
                    // schedulePrivacy = 1;
                }
                // Viewer permissions
                switch (viewerRole)
                {
                    case Position.SUPERADMIN:
                        scheduleResult = _scheduleService.GetScheduleStudentAsync(id);
                        break;
                    case Position.POLICE:
                        scheduleResult = _scheduleService.GetScheduleStudentAsync(id);
                        break;
                    case Position.STUDENT:
                        if (schedulePrivacy == 0)
                        {
                            scheduleResult = _scheduleService.GetScheduleStudentAsync(id);
                        }
                        break;
                    case Position.FACSTAFF:
                        if (_scheduleService.CanReadStudentSchedules(viewerName))
                        {
                            scheduleResult = _scheduleService.GetScheduleStudentAsync(id);
                        }
                        break;
                }
            }
            // Getting faculty / staff schedule
            else if (role == "facstaff")
            {
                scheduleResult = _scheduleService.GetScheduleFacultyAsync(id);
            }
            else
            {
                return NotFound();
            }

            JArray result = JArray.FromObject(scheduleResult);

            foreach (JObject elem in result)
            {
                elem.Property("ID_NUM").Remove();
            }

            return Ok(result);
        }

        /// <summary>
        /// Get whether the currently logged-in user can read student schedules
        /// </summary>
        /// <returns>Whether they can read student schedules</returns>
        [HttpGet]
        [Route("canreadstudent")]
        public IHttpActionResult GetCanReadStudentSchedules()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            return Ok(_scheduleService.CanReadStudentSchedules(username));
        }
    }
}
