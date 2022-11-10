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
        [Route("")]
        public async Task<ActionResult> CreateTeam(TeamUploadViewModel newTeam)
        {
            var username = AuthUtils.GetUsername(User);
            var team = await _teamService.PostTeamAsync(newTeam, username);

            return Ok(team);
        }

        /// <summary>
        /// Add a participant to a team
        /// </summary>
        /// <param name="participantUsername"></param>
        /// <param name="teamID"></param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("users")]
        public async Task<ActionResult> AddUserToTeam(string participantUsername, int teamID, int roleType = 3)
        {
            var participantTeam = await _teamService.AddUserToTeamAsync(teamID, participantUsername, roleType);
            return Ok(participantTeam);
        }

        /// <summary>
        /// Update a team info
        /// </summary>
        /// <param name="team"></param>
        /// <param name="teamID"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{teamID}")]
        public async Task<ActionResult> UpdateTeamInfo(int teamID, TeamPatchViewModel team)
        {
            var Team = await _teamService.UpdateTeamAsync(teamID, team);
            return Ok(Team);
        }
    }
}
