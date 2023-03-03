using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services.RecIM;
using Gordon360.Authorization;
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
    public class MatchesController : GordonControllerBase
    {
        private readonly IMatchService _matchService;
        private readonly ITeamService _teamService;

        public MatchesController(IMatchService matchService, ITeamService teamService)
        {
            _matchService = matchService;
            _teamService = teamService;
        }

        /// <summary>
        /// Not implemented (yet) might be removed
        /// </summary>
        /// <param name="day"></param>
        /// <param name="active"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<MatchExtendedViewModel>> GetMatches([FromQuery] DateTime? day, bool active)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fetches Match by MatchID
        /// </summary>
        /// <param name="matchID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{matchID}")]
        public ActionResult<MatchExtendedViewModel> GetMatchByID(int matchID)
        {
            var match = _matchService.GetMatchByID(matchID);
            return Ok(match);
        }

        [HttpGet]
        [Route("lookup")]
        public ActionResult<IEnumerable<LookupViewModel>> GetMatchTypes(string type)
        {
            var res = _matchService.GetMatchLookup(type);
            if (res is not null)
            {
                return Ok(res);
            }
            return NotFound();
        }

        /// <summary>
        /// Updates Match Scores, Sportsmanship Ratings, and Team Status
        /// </summary>
        /// <param name="matchID"></param>
        /// <param name="updatedMatch"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{matchID}/stats")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_MATCH)]
        public async Task<ActionResult<MatchTeamViewModel>> UpdateStats(int matchID, MatchStatsPatchViewModel updatedMatch)
        {
            var stats = await _matchService.UpdateTeamStatsAsync(matchID, updatedMatch);
            return CreatedAtAction("UpdateStats", stats);
        }

        /// <summary>
        /// Updates Match Start Times and Surfaces
        /// </summary>
        /// <param name="matchID"></param>
        /// <param name="updatedMatch"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{matchID}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_MATCH)]
        public async Task<ActionResult<MatchViewModel>> UpdateMatch(int matchID, MatchPatchViewModel updatedMatch)
        {
            var match = await _matchService.UpdateMatchAsync(matchID, updatedMatch);
            return CreatedAtAction("UpdateMatch", match);
        }

        /// <summary>
        /// Creates Match
        /// </summary>
        /// <param name="newMatch"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.RECIM_MATCH)]
        public async Task<ActionResult<MatchViewModel>> CreateMatch(MatchUploadViewModel newMatch)
        {
            var match = await _matchService.PostMatchAsync(newMatch);
            return CreatedAtAction("CreateMatch", match);
        }

        /// <summary>
        /// Cascade deletes all DBobjects related to given Match ID
        /// </summary>
        /// <param name="matchID"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{matchID}")]
        public async Task<ActionResult> DeleteMatchCascade(int matchID)
        {
            await _matchService.DeleteMatchCascadeAsync(matchID);
            return NoContent();
        }


        /// <summary>
        /// creates match attendance
        /// </summary>
        /// <param name="matchID"></param>
        /// <param name="teamID"></param>
        /// <param name="teamAttendanceList">List of attendees for a team</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{matchID}/attendance")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_MATCH)]
        public async Task<ActionResult<IEnumerable<Individual>>> AddParticipantAttendance(int matchID, ParticipantAttendanceViewModel teamAttendanceList)
        {
            var attendance = await _teamService.AddParticipantAttendanceAsync(matchID, teamAttendanceList);
            return CreatedAtAction("AddParticipantAttendance", attendance);
        }
    }
}
