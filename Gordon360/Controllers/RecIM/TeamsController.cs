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
        /// <param name="username">creator's username</param>
        /// <param name="newTeam"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreateTeam([FromQuery] string username, TeamUploadViewModel newTeam)
        {
            var team = await _teamService.PostTeamAsync(newTeam, username);
            // future error handling
            // (cannot implement at the moment as we only have 4 developer accs)
            if (team is null)
            {
                return BadRequest($"{username} already is a part of a team in this activity");
            }
            return CreatedAtAction("CreateTeam",team);
        }

        /// <summary>
        /// Add a participant to a team
        /// </summary>
        /// <param name="username"></param>
        /// <param name="teamID"></param>
        /// <param name="roleType">Default value 3 (Member)</param>
        /// <returns></returns>
        [HttpPost]
        [Route("participants")]
        public async Task<ActionResult> AddParticipantToTeam(string username, int teamID, int roleType = 3)
        {
            var participantTeam = await _teamService.AddUserToTeamAsync(teamID, username, roleType);
            return CreatedAtAction("AddParticipantToTeam",participantTeam);
        }

        /// <summary>
        /// Updates Participant role in a team
        /// </summary>
        /// <param name="username"></param>
        /// <param name="teamID"></param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("participants")]
        public async Task<ActionResult> UpdateTeamParticipant(string username, int teamID, int roleType = 3)
        {
            var participantTeam = await _teamService.UpdateParticipantRoleAsync(teamID, username, roleType);
            return CreatedAtAction("UpdateTeamParticipant",participantTeam);
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
            var updatedTeam = await _teamService.UpdateTeamAsync(teamID, team);
            return CreatedAtAction("UpdateTeamInfo",updatedTeam);
        }
    }
}
