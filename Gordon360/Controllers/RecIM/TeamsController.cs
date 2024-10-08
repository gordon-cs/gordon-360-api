using Gordon360.Authorization;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services.RecIM;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gordon360.Controllers.RecIM;

[Route("api/recim/[controller]")]
public class TeamsController(ITeamService teamService, IActivityService activityService, IParticipantService participantService) : GordonControllerBase
{

    ///<summary>
    ///Get all team objects stored in rec-im
    ///</summary>
    /// <param name="active"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("")]
    public ActionResult<TeamExtendedViewModel> GetTeams([FromQuery] bool active)
    {
        var team = teamService.GetTeams(active);
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

        var viewerUsername = AuthUtils.GetUsername(User);
        var viewerParticipation = participantService.GetParticipantByUsername(viewerUsername);
        var isAdmin = viewerParticipation?.IsAdmin == true || AuthUtils.UserIsInGroup(User, Enums.AuthGroup.RecIMSuperAdmin);

        var team = teamService.GetTeamByID(teamID, isAdmin);

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
        var res = teamService.GetTeamLookup(type);
        if (res is not null)
        {
            return Ok(res);
        }
        return NotFound();
    }

    /// <summary>
    /// Create a new team with the requesting user set to team captain
    /// </summary>
    /// <param name="newTeam"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("")]
    public async Task<ActionResult<TeamViewModel>> PostTeamAsync(TeamUploadViewModel newTeam)
    {
        var username = AuthUtils.GetUsername(User);
        var activity = activityService.GetActivityByID(newTeam.ActivityID);
        if (activity is null)
            return NotFound($"This activity does not exist");
       
        //redudant check for API as countermeasure against postman navigation around UI check, admins can make any number of teams
        if (teamService.HasUserJoined(newTeam.ActivityID, username) && !participantService.IsAdmin(username))
            return UnprocessableEntity($"Participant {username} already is a part of a team in this activity");
        
        if(activityService.ActivityRegistrationClosed(newTeam.ActivityID) && !participantService.IsAdmin(username))
            return UnprocessableEntity("Activity Registration has closed.");

        /* temporarily deprecated
        if (_activityService.ActivityTeamCapacityReached(newTeam.ActivityID))
            return UnprocessableEntity("Activity capacity has been reached. Try again later.");
        */

        var team = await teamService.PostTeamAsync(newTeam, username);
        // future error handling
        // (cannot implement at the moment as we only have 4 developer accs)
        if (team is null)
        {
            return BadRequest($"Participant {username} already is a part of a team in this activity");
        }
        return CreatedAtAction(nameof(GetTeamByID), new { teamID = team.ID }, team);


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
    public async Task<ActionResult<ParticipantTeamViewModel>> AddParticipantToTeamAsync(int teamID, ParticipantTeamUploadViewModel participant)
    {
        var inviterUsername = AuthUtils.GetUsername(User);
        var activityID = teamService.GetTeamActivityID(teamID);
        if (!teamService.HasUserJoined(activityID, participant.Username) || participantService.IsAdmin(inviterUsername))
        {
            var participantTeam = await teamService.AddParticipantToTeamAsync(teamID, participant, inviterUsername);
            return Ok(participantTeam);
        }

        return UnprocessableEntity($"Participant {participant.Username} already is a part of a team in this activity");
    }

    /// <summary>
    /// Updates Participant role in a team
    /// </summary>
    /// <param name="teamID"></param>
    /// <param name="participant"></param>
    /// <returns></returns>
    [HttpPatch]
    [Route("{teamID}/participants")]
    [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_TEAM)]
    public async Task<ActionResult<ParticipantTeamViewModel>> UpdateParticipantTeamAsync(int teamID, ParticipantTeamUploadViewModel participant)
    {
        participant.RoleTypeID = participant.RoleTypeID ?? 3;
        var participantTeam = await teamService.UpdateParticipantRoleAsync(teamID, participant);
        return Ok(participantTeam);
    }

    /// <summary>
    /// Removes team and all participants in the team
    /// </summary>
    /// <param name="teamID"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{teamID}")]
    [StateYourBusiness(operation = Operation.DELETE, resource = Resource.RECIM_TEAM)]
    public async Task<ActionResult> DeleteTeamAsync(int teamID)
    {
        var res = await teamService.DeleteTeamCascadeAsync(teamID);
        return Ok(res);
    }

    /// <summary>
    /// Removes specified user from a team
    /// </summary>
    /// <param name="teamID"></param>
    /// <param name="username"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{teamID}/participants/{username}")]
    public async Task<ActionResult> DeleteTeamParticipantAsync(int teamID, string username)
    {
        var user_name = AuthUtils.GetUsername(User);
        var participantTeam = teamService.GetParticipantTeam(teamID, username);
        if (participantTeam is null)
            return NotFound("The user is not part of the team.");
        if (user_name != participantTeam.ParticipantUsername)
            return Forbid($"You are not permitted to reject invitations for another participant.");

        var res = await teamService.DeleteParticipantTeamAsync(teamID, username);
        return Ok(res);
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
    public async Task<ActionResult<TeamViewModel>> UpdateTeamInfoAsync(int teamID, TeamPatchViewModel team)
    {
        var updatedTeam = await teamService.UpdateTeamAsync(teamID, team);
        return CreatedAtAction(nameof(GetTeamByID), new { teamID = updatedTeam.ID }, updatedTeam);
;       }

    /// <summary>
    /// Get all team invites of the user
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("invites")]
    public ActionResult<IEnumerable<TeamExtendedViewModel>> GetTeamInvites()
    {
        var username = AuthUtils.GetUsername(User);

        var teamInvites = teamService.GetTeamInvitesByParticipantUsername(username);
        return Ok(teamInvites);
    }

    /// <summary>
    /// Gets number of games a participant has participated in for a team
    /// </summary>
    /// <param name="teamID"></param>
    /// <param name="username"></param>
    /// <returns>number of games a participant has attended for a team</returns>
    [HttpGet]
    [Route("{teamID}/participants/{username}/attendance")]
    public ActionResult<int> NumberOfGamesParticipatedByParticipant(int teamID, string username)
    {
        var res = teamService.ParticipantAttendanceCount(teamID, username);
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
    public async Task<ActionResult<ParticipantTeamViewModel?>> HandleTeamInviteAsync(int teamID, [FromBody]string response)
    {
        var username = AuthUtils.GetUsername(User);
        var invite = teamService.GetParticipantTeam(teamID, username);

        if (invite is null)
            return NotFound("You were not invited by this team.");
        if (username != invite.ParticipantUsername)
            return Forbid($"You are not permitted to accept invitations for another participant.");
        
        switch (response)
        {
            case "accepted":
                var joinedParticipantTeam = await teamService.UpdateParticipantRoleAsync(invite.TeamID,
                    new ParticipantTeamUploadViewModel { Username = username, RoleTypeID = 3 }
                    );
                return Ok(joinedParticipantTeam);
            case "rejected":
                var res = await teamService.DeleteParticipantTeamAsync(invite.TeamID, username);
                return Ok(res);
            default:
                return BadRequest("Request does not specify valid invite action");
        }
    }
}
