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

        [HttpGet]
        [Route("lookup")]
        public ActionResult<IEnumerable<LookupViewModel>> GetTeamTypes(string type)
        {
            if ( type == "status" )
            {

            }
            if ( type == "role")
            {

            }
            throw new NotImplementedException();
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
        /// <param name="teamID"></param>
        /// <param name="participant">Default Role Value value 3 (Member)</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{teamID}/participants")]
        public async Task<ActionResult> AddParticipantToTeam(int teamID, ParticipantTeamUploadViewModel participant)
        {
            var username = AuthUtils.GetUsername(User);
            var isTeamCaptain = _teamService.IsTeamCaptain(username, teamID);
            if (isTeamCaptain)
            {
                participant.RoleTypeID = participant.RoleTypeID ?? 3;
                var participantTeam = await _teamService.AddUserToTeamAsync(teamID, participant);
                return CreatedAtAction("AddParticipantToTeam", participantTeam);
            }
            return Forbid();
        }

        /// <summary>
        /// Updates Participant role in a team
        /// </summary>
        /// <param name="teamID"></param>
        /// <param name="participant">Default Role Value value 3 (Member)</param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{teamID}/participants")]
        public async Task<ActionResult> UpdateTeamParticipant(int teamID, ParticipantTeamUploadViewModel participant)
        {
            var username = AuthUtils.GetUsername(User);
            var isTeamCaptain = _teamService.IsTeamCaptain(username, teamID);
            if (isTeamCaptain)
            {
                participant.RoleTypeID = participant.RoleTypeID ?? 3;
                var participantTeam = await _teamService.UpdateParticipantRoleAsync(teamID, participant);
                return CreatedAtAction("UpdateTeamParticipant", participantTeam);
            }
            return Forbid();
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
            var username = AuthUtils.GetUsername(User);
            var isTeamCaptain = _teamService.IsTeamCaptain(username, teamID);
            if (isTeamCaptain)
            {
                var updatedTeam = await _teamService.UpdateTeamAsync(teamID, team);
                return CreatedAtAction("UpdateTeamInfo", updatedTeam);
            }
            return Forbid();
;        }
    }
}
