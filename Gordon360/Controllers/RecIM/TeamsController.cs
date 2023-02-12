using Gordon360.Authorization;
using Gordon360.Exceptions;
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
using System.ComponentModel;
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
            var activity = _teamService.GetTeamByID(newTeam.ActivityID);
            if (activity is null)
                return UnprocessableEntity($"This activity does not exist");
            if (_teamService.IsActivityFull(newTeam.ActivityID))
                return UnprocessableEntity($"The activity has reached the maximum team capacity");
            if (_teamService.HasTeamNameTaken(newTeam.ActivityID, newTeam.Name))
                return UnprocessableEntity($"Team name {newTeam.Name} has already been taken by another team in this activity");
           //redudant check for API as countermeasure against postman navigation around UI check
            if (_teamService.HasUserJoined(newTeam.ActivityID, username))
                return UnprocessableEntity($"Participant {username} already is a part of a team in this activity");
         

            var team = await _teamService.PostTeamAsync(newTeam, username);
            // future error handling
            // (cannot implement at the moment as we only have 4 developer accs)
            if (team is null)
            {
                return BadRequest($"Participant {username} already is a part of a team in this activity");
            }
            return CreatedAtAction("CreateTeam", team);
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
            var inviterUsername = AuthUtils.GetUsername(User);
            var participantTeam = await _teamService.AddParticipantToTeamAsync(teamID, participant, inviterUsername);
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
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_TEAM)]
        public async Task<ActionResult<ParticipantTeamViewModel>> UpdateParticipantTeam(int teamID, ParticipantTeamUploadViewModel participant)
        {
            var activityID = _teamService.GetTeamByID(teamID).Activity.ID;
            if (!_teamService.HasUserJoined(activityID, participant.Username))
            {
                participant.RoleTypeID = participant.RoleTypeID ?? 3;
                var participantTeam = await _teamService.AddParticipantToTeamAsync(teamID, participant);
                return CreatedAtAction("AddParticipantToTeam", participantTeam);
            }
            else
                return UnprocessableEntity($"Participant {participant.Username} already is a part of a team in this activity");
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
            var user_name = AuthUtils.GetUsername(User);
            var participantTeam = _teamService.GetParticipantTeam(teamID, username);
            if (participantTeam is null)
                return NotFound("The user is not part of the team.");
            if (user_name != participantTeam.ParticipantUsername)
                return Forbid($"You are not permitted to reject invitations for another participant.");

            await _teamService.DeleteParticipantTeamAsync(teamID, username);
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
            if (team.Name is not null)
            {
                var activityID = _teamService.GetTeamByID(teamID).Activity.ID;
                if (_teamService.HasTeamNameTaken(activityID, team.Name))
                    return UnprocessableEntity($"Team name {team.Name} has already been taken by another team in this activity");

            }

            var updatedTeam = await _teamService.UpdateTeamAsync(teamID, team);
            return CreatedAtAction("UpdateTeamInfo", updatedTeam);
;       }

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

        /// <summary>
        /// Accept one specified team invite and true delete others from the same activity if there's any
        /// </summary>
        /// <param name="teamID"></param>
        /// <param name="acceptedInvite"></param>
        /// <returns>The accepted TeamInviteViewModel</returns>
        [HttpPatch]
        [Route("{teamID}/invite")]
        public async Task<ActionResult<TeamInviteViewModel>> AcceptTeamInvite(int teamID, [FromBody] ParticipantTeamUploadViewModel acceptedInvite)
        {
            var username = AuthUtils.GetUsername(User);
            var invite = _teamService.GetParticipantTeam(teamID, username);
            if (invite is null)
                return NotFound("You were not invited by this team.");
            if (username != invite.ParticipantUsername)
                return Forbid($"You are not permitted to accept invitations for another participant.");

            // set the role type ID of the accepted team invite to 3 => member
            acceptedInvite.RoleTypeID = 3;
            var joinedParticipantTeam = await _teamService.UpdateParticipantRoleAsync(invite.TeamID, acceptedInvite);

            // true delete other team invites from the same activity
            IEnumerable<TeamInviteViewModel> teamInvites = _teamService.GetTeamInvites(username);
            int activityID = _teamService.GetTeamByID(invite.TeamID).Activity.ID;
            foreach(TeamInviteViewModel teamInvite in teamInvites)
            {
                if (teamInvite.ActivityID == activityID && teamInvite.TeamID != invite.TeamID)
                {
                    await _teamService.DeleteParticipantTeamAsync(teamInvite.TeamID, username);
                }
            }

            return CreatedAtAction("AcceptTeamInvite", joinedParticipantTeam);
        }
    }
}
