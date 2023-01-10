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

        public MatchesController(IMatchService matchService)
        {
            _matchService = matchService;
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
        public ActionResult<IEnumerable<MatchViewModel>> GetMatches([FromQuery] DateTime? day, bool active)
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
        public ActionResult<MatchViewModel> GetMatchByID(int matchID)
        {
            var match = _matchService.GetMatchByID(matchID);
            return Ok(match);
        }

        [HttpGet]
        [Route("lookup")]
        public ActionResult<IEnumerable<LookupViewModel>> GetMatchTypes(string type)
        {
            var res = _matchService.GetMatchLookup(type);
            return Ok(res);
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
        public async Task<ActionResult<MatchCreatedViewModel>> UpdateMatch(int matchID, MatchPatchViewModel updatedMatch)
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
        public async Task<ActionResult<MatchCreatedViewModel>> CreateMatch(MatchUploadViewModel newMatch)
        {
            var match = await _matchService.PostMatchAsync(newMatch);
            return CreatedAtAction("CreateMatch", match);
        }

        /// <summary>
        /// Creates Match Attendee
        /// </summary>
        /// <param name="username"></param>
        /// <param name="matchID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{matchID}/attendance")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_MATCH)]
        public async Task<ActionResult<MatchParticipantViewModel>> AddAttendance(int matchID, [FromBody] string username)
        {
            var attendance = await _matchService.AddParticipantAttendanceAsync(username, matchID);
            return CreatedAtAction("AddAttendance", attendance);
        }

    }
}
