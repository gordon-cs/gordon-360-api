using Gordon360.Authorization;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services.RecIM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gordon360.Controllers.RecIM
{
    [Route("api/recim/[controller]")]
    [AllowAnonymous]
    public class TeamsController : GordonControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        ///<summary>
        ///Get a Team object by ID number
        ///</summary>
        /// <param name="teamID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{teamID}")]
        public ActionResult<TeamViewModel> GetTeamByID(int teamID)
        {
            var team = _teamService.GetTeamByID(teamID);

            if (team == null)
            {
                return NotFound();
            }
            return Ok(team);
        }

        /// <summary>
        /// Create a new team with the requesting user set to team captain
        /// </summary>
        /// <param name="newTeam"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{teamID}")]
        public async Task<ActionResult> CreateTeam(TeamUploadViewModel newTeam)
        {
            var username = AuthUtils.GetUsername(User);
            var teamID = await _teamService.PostTeamAsync(newTeam, username);

            return Ok(teamID);
        }

        /// <summary>
        /// Add a participant to a team
        /// </summary>
        /// <param name="participantUsername"></param>
        /// <param name="teamID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{teamID}/users")]
        public async Task<ActionResult> AddUserToTeam(string participantUsername, int teamID)
        {
            var username = AuthUtils.GetUsername(User);

            // only team captain is allowed to add user to team
            if (_teamService.IsTeamCaptain(teamID, username))
            {
                await _teamService.AddUserToTeamAsync(teamID, participantUsername);
                return Ok();
            }

            return Unauthorized();
        }

        /// <summary>
        /// Update the participant role in a team
        /// </summary>
        /// <param name="teamID"></param>
        /// <param name="participantID"></param>
        /// <param name="participantRoleID"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{teamID}/users/{userID}")]
        public async Task<ActionResult> UpdateParticipantRole(int teamID, int participantID, int participantRoleID)
        {
            var username = AuthUtils.GetUsername(User);

            // only team captain is allowed to add user to team
            if (_teamService.IsTeamCaptain(teamID, username))
            {
                await _teamService.UpdateParticipantRoleAsync(teamID, participantID, participantRoleID);
                return Ok();
            }

            return Unauthorized();
        }

        /// <summary>
        /// Update a team info
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{teamID}")]
        public async Task<ActionResult> UpdateTeamInfo(TeamPatchViewModel team)
        {
            var username = AuthUtils.GetUsername(User);

            // only team captain is allowed to add user to team
            if (_teamService.IsTeamCaptain(team.ID, username))
            {
                await _teamService.UpdateTeamAsync(team);
                return Ok();
            }

            return Unauthorized();
        }
    }
}
