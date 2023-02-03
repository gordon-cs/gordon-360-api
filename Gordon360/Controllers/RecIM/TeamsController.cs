using Gordon360.Authorization;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services.RecIM;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
        ///Get all team objects stored in rec-im
        ///</summary>
        /// <param name="active"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public ActionResult<TeamExtendedViewModel> GetTeams([FromQuery] bool active)
        {
            var team = _teamService.GetTeams(active);
            return Ok(team);
        }

        ///<summary>
        ///Get a Team object by ID number
        ///</summary>
        /// <param name="teamID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{teamID}")]
        public ActionResult<TeamExtendedViewModel> GetTeamByID(int teamID)
        {
            var team = _teamService.GetTeamByID(teamID);

            if (team == null)
            {
                return NotFound();
            }
            return Ok(team);
        }

        /// <summary>
        /// Returns all team lookup types
        /// </summary>
        /// <param name="type">specified team type</param>
        /// <returns></returns>
        [HttpGet]
        [Route("lookup")]
        public ActionResult<IEnumerable<LookupViewModel>> GetTeamTypes(string type)
        {
            var res = _teamService.GetTeamLookup(type);
            if (res is not null)
            {
                return Ok(res);
            }
            return BadRequest();
        }

        /// <summary>
        /// Create a new team with the requesting user set to team captain
        /// </summary>
        /// <param name="username">creator's username</param>
        /// <param name="newTeam"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<TeamViewModel>> CreateTeam([FromQuery] string username, TeamUploadViewModel newTeam)
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
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_TEAM)]
        public async Task<ActionResult<ParticipantTeamViewModel>> AddParticipantToTeam(int teamID, ParticipantTeamUploadViewModel participant)
        {
            participant.RoleTypeID = participant.RoleTypeID ?? 3;
            var participantTeam = await _teamService.AddUserToTeamAsync(teamID, participant);
            return CreatedAtAction("AddParticipantToTeam", participantTeam);
        }

        /// <summary>
        /// Updates Participant role in a team
        /// </summary>
        /// <param name="teamID"></param>
        /// <param name="participant">Default Role Value value 3 (Member)</param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{teamID}/participants")]
        public async Task<ActionResult<ParticipantTeamViewModel>> UpdateTeamParticipant(int teamID, ParticipantTeamUploadViewModel participant)
        {
            participant.RoleTypeID = participant.RoleTypeID ?? 3;
            var participantTeam = await _teamService.UpdateParticipantRoleAsync(teamID, participant);
            return CreatedAtAction("UpdateTeamParticipant", participantTeam);
        }

        /// <summary>
        /// Removes specified user from a team
        /// </summary>
        /// <param name="teamID"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{teamID}/participants")]
        public async Task<ActionResult> DeleteTeamParticipant(int teamID, string username)
        {
            await _teamService.DeleteTeamParticipantAsync(teamID, username);
            return NoContent();
        }

        /// <summary>
        /// Update a team info
        /// </summary>
        /// <param name="team"></param>
        /// <param name="teamID"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{teamID}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_TEAM)]
        public async Task<ActionResult<TeamViewModel>> UpdateTeamInfo(int teamID, TeamPatchViewModel team)
        {
            var updatedTeam = await _teamService.UpdateTeamAsync(teamID, team);
            return CreatedAtAction("UpdateTeamInfo", updatedTeam);
        }

        /// <summary>
        /// Get all team invites of the user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("invites")]
        public ActionResult<IEnumerable<TeamInviteViewModel>> GetTeamInvites()
        {
            var username = AuthUtils.GetUsername(User);
            return Ok(_teamService.GetTeamInvites(username));
        }
    }
}
