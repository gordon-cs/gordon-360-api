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
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
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
        private readonly IActivityService _activityService;
        private readonly IParticipantService _participantService;

        public TeamsController(ITeamService teamService, IActivityService activityService, IParticipantService participantService)
        {
            _teamService = teamService;
            _activityService = activityService;
            _participantService = participantService;
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
            var activity = _activityService.GetActivityByID(newTeam.ActivityID);
            if (activity is null)
                return UnprocessableEntity($"This activity does not exist");
            if (_activityService.ActivityTeamCapacityReached(newTeam.ActivityID))
                return UnprocessableEntity($"The activity has reached the maximum team capacity");
            if (_teamService.HasTeamNameTaken(newTeam.ActivityID, newTeam.Name))
                return UnprocessableEntity($"Team name {newTeam.Name} has already been taken by another team in this activity");
           //redudant check for API as countermeasure against postman navigation around UI check, admins can make any number of teams
            if (_teamService.HasUserJoined(newTeam.ActivityID, username) && !_participantService.IsAdmin(username))
                return UnprocessableEntity($"Participant {username} already is a part of a team in this activity");
            if(_activityService.ActivityRegistrationClosed(newTeam.ActivityID) && !_participantService.IsAdmin(username))
                return UnprocessableEntity("Activity Registration has closed.");
            if (_activityService.ActivityTeamCapacityReached(newTeam.ActivityID))
                return UnprocessableEntity("Activity capacity has been reached. Try again later.");

            try
            {
                var team = await _teamService.PostTeamAsync(newTeam, username);
                // future error handling
                // (cannot implement at the moment as we only have 4 developer accs)
                if (team is null)
                {
                    return BadRequest($"Participant {username} already is a part of a team in this activity");
                }
                return CreatedAtAction("CreateTeam", team);
            }
            catch (Exception)
            {
                throw;
            }
            
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
            var activityID = _teamService.GetTeamActivityID(teamID);
            if (!_teamService.HasUserJoined(activityID, participant.Username) || _participantService.IsAdmin(inviterUsername))
            {
                var participantTeam = await _teamService.AddParticipantToTeamAsync(teamID, participant, inviterUsername);
                return CreatedAtAction("AddParticipantToTeam", participantTeam);
            }
            else
                return UnprocessableEntity($"Participant {participant.Username} already is a part of a team in this activity");
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
            
                participant.RoleTypeID = participant.RoleTypeID ?? 3;
                var participantTeam = await _teamService.AddParticipantToTeamAsync(teamID, participant);
                return CreatedAtAction("AddParticipantToTeam", participantTeam);
            
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
        }

        /// <summary>
        /// Get all team invites of the user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("invites")]
        public ActionResult<IEnumerable<TeamExtendedViewModel>> GetTeamInvites()
        {
            var username = AuthUtils.GetUsername(User);
            try
            {
                var teamInvites = _teamService.GetTeamInvitesByParticipantUsername(username);
                return Ok(teamInvites);
            }
            catch(Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets number of games a participant has participated in for a team
        /// </summary>
        /// <param name="teamID"></param>
        /// <param name="username"></param>
        /// <returns>number of games a participant has attended for a team</returns>
        [HttpGet]
        [Route("{teamID}/attendance")]
        public async Task<ActionResult<int>> NumberOfGamesParticipatedByParticipant(int teamID, [FromBody] string username)
        {
            var res = _teamService.NumberOfGamesParticipatedByParticipant(teamID, username);
            return Ok(res);
        }
        /// <summary>
        /// Accept one specified team invite and true delete others from the same activity if there's any
        /// </summary>
        /// <param name="teamID"></param>
        /// <param name="response"></param>
        /// <returns>The accepted TeamInviteViewModel</returns>
        [HttpPatch]
        [Route("{teamID}/invite/status")]
        public async Task<ActionResult<ParticipantTeamViewModel?>> AcceptTeamInvite(int teamID, [FromBody]string response)
        {
            var username = AuthUtils.GetUsername(User);
            try
            {
                var invite = _teamService.GetParticipantTeam(teamID, username);
                if (invite is null)
                    return NotFound("You were not invited by this team.");
                if (username != invite.ParticipantUsername)
                    return Forbid($"You are not permitted to accept invitations for another participant.");

                switch (response)
                {
                    case "accepted":
                        var joinedParticipantTeam = await _teamService.UpdateParticipantRoleAsync(invite.TeamID,
                            new ParticipantTeamUploadViewModel { Username = username, RoleTypeID = 3 }
                            );
                        return CreatedAtAction("AcceptTeamInvite", joinedParticipantTeam);
                    case "rejected":
                        await _teamService.DeleteParticipantTeamAsync(invite.TeamID, username);
                        return Ok();
                    default:
                        return BadRequest("Request does not specify valid invite action");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
