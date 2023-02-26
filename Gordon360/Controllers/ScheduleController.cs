﻿using Gordon360.Authorization;
using Gordon360.Enums;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
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
            var authenticatedUserUsername = AuthUtils.GetUsername(User);
            var groups = AuthUtils.GetGroups(User);

            if (groups.Contains(AuthGroup.Student))
            {
                var result = _scheduleService.GetScheduleStudentAsync(authenticatedUserUsername);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }

            else if (groups.Contains(AuthGroup.FacStaff))
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
            var viewerGroups = AuthUtils.GetGroups(User);

            var groups = AuthUtils.GetGroups(username);
            var id = _accountService.GetAccountByUsername(username).GordonID;
            var scheduleControl = _context.Schedule_Control.Find(id);

            IEnumerable<ScheduleViewModel>? scheduleResult = null;
            if (groups.Contains(AuthGroup.Student))
            {
                int schedulePrivacy = scheduleControl?.IsSchedulePrivate ?? 1;

                if (viewerGroups.Contains(AuthGroup.Police) || viewerGroups.Contains(AuthGroup.SiteAdmin))
                {
                    scheduleResult = await _scheduleService.GetScheduleStudentAsync(username);
                }
                else if (viewerGroups.Contains(AuthGroup.Student) && schedulePrivacy == 0)
                {

                    scheduleResult = await _scheduleService.GetScheduleStudentAsync(username);
                }
                else if (viewerGroups.Contains(AuthGroup.Advisors))
                {
                    scheduleResult = await _scheduleService.GetScheduleStudentAsync(username);
                }
            }
            else if (groups.Contains(AuthGroup.FacStaff))
            {
                scheduleResult = await _scheduleService.GetScheduleFacultyAsync(username);
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
            var groups = AuthUtils.GetGroups(User);
            return groups.Contains(AuthGroup.Advisors);
        }
    }
}
