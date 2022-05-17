using Gordon360.Database.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly CCTContext _context;

        private readonly IAccountService _accountService;
        private readonly IScheduleService _scheduleService;

        public ScheduleController(CCTContext context)
        {
            _context = context;
            _scheduleService = new ScheduleService(context);
            _accountService = new AccountService(context);
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
            var groups = AuthUtils.GetAuthenticatedUserGroups(User);

            if (groups.Contains(AuthGroup.Student.Name))
            {
                var result = _scheduleService.GetScheduleStudentAsync(authenticatedUserUsername);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }

            else if (groups.Contains(AuthGroup.FacStaff.Name))
            {
                var result = _scheduleService.GetScheduleFacultyAsync(authenticatedUserUsername);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        ///  Gets all schedule objects for a user
        /// </summary>
        /// <returns>A IEnumerable of schedule objects</returns>
        [HttpGet]
        [Route("{username}")]
        public async Task<ActionResult<JArray>> GetAsync(string username)
        {
            //probably needs privacy stuff like ProfilesController and service
            var viewerGroups = AuthUtils.GetAuthenticatedUserGroups(User);

            var groups = AuthUtils.GetGroups(username);
            var id = _accountService.GetAccountByUsername(username).GordonID;
            var scheduleControl = _context.Schedule_Control.Find(id);

            IEnumerable<ScheduleViewModel>? scheduleResult = null;
            if (groups.Contains(AuthGroup.Student.Name))
            {
                int schedulePrivacy = scheduleControl?.IsSchedulePrivate ?? 1;

                if (viewerGroups.Contains(AuthGroup.Police.Name) || viewerGroups.Contains(AuthGroup.SiteAdmin.Name))
                {
                    scheduleResult = await _scheduleService.GetScheduleStudentAsync(id);
                }
                else if (viewerGroups.Contains(AuthGroup.Student.Name) && schedulePrivacy == 0)
                {

                    scheduleResult = await _scheduleService.GetScheduleStudentAsync(id);
                }
                else if (viewerGroups.Contains(AuthGroup.Advisors.Name))
                {
                    scheduleResult = await _scheduleService.GetScheduleStudentAsync(id);
                }
            }
            else if (groups.Contains(AuthGroup.FacStaff.Name))
            {
                scheduleResult = await _scheduleService.GetScheduleFacultyAsync(id);
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
        public async Task<ActionResult<bool>> GetCanReadStudentSchedules()
        {
            var groups = AuthUtils.GetAuthenticatedUserGroups(User);
            return groups.Contains(AuthGroup.Advisors.Name);
        }
    }
}
