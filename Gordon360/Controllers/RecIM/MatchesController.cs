using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services.RecIM;
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

        /// <summary>
        /// Updates Match Scores, Sportsmanship Ratings, and Team Status
        /// </summary>
        /// <param name="updatedMatch"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("")]
        public async Task<ActionResult> UpdateStats(MatchTeamPatchViewModel updatedMatch)
        {
            await _matchService.UpdateTeamStats(updatedMatch);
            return Ok();
        }
        /// <summary>
        /// Add Team to Match
        /// </summary>
        /// <param name="teamID"></param>
        /// <param name="matchID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{matchID}")]
        public async Task<ActionResult> AddTeamToMatch(int teamID, int matchID)
        {
            await _matchService.AddTeamToMatch(teamID, matchID);
            return Ok();
        }
        /// <summary>
        /// Creates Match
        /// </summary>
        /// <param name="newMatch"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreateMatch(MatchUploadViewModel newMatch)
        {
            await _matchService.PostMatch(newMatch);
            return Ok();
        }

    }
}
