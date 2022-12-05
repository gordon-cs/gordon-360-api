﻿using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services.RecIM;
using Gordon360.Authorization;
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
        private readonly IActivityService _activityService;
        private readonly ISeriesService _seriesService;

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
            if ( type == "status" )
            {

            }
            if ( type == "teamstatus" )
            {

            }
            if ( type == "surface")
            {

            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates Match Scores, Sportsmanship Ratings, and Team Status
        /// </summary>
        /// <param name="matchID"></param>
        /// <param name="updatedMatch"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{matchID}/stats")]
        public async Task<ActionResult> UpdateStats(int matchID, MatchStatsPatchViewModel updatedMatch)
        {
            var username = AuthUtils.GetUsername(User);
            var match = _matchService.GetMatchByID(matchID);
            var series = _seriesService.GetSeriesByID(match.SeriesID);
            var isActivityAdmin = _activityService.IsActivityAdmin(username, series.ActivityID);
            if (isActivityAdmin)
            {
                var stats = await _matchService.UpdateTeamStats(matchID, updatedMatch);
                return CreatedAtAction("UpdateStats", stats);
            }
            return Unauthorized();
        }

        /// <summary>
        /// Updates Match Start Times and Surfaces
        /// </summary>
        /// <param name="matchID"></param>
        /// <param name="updatedMatch"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{matchID}")]
        public async Task<ActionResult> UpdateMatch(int matchID, MatchPatchViewModel updatedMatch)
        {
            var username = AuthUtils.GetUsername(User);
            var updatingMatch = _matchService.GetMatchByID(matchID);
            var series = _seriesService.GetSeriesByID(updatingMatch.SeriesID);
            var isActivityAdmin = _activityService.IsActivityAdmin(username, series.ActivityID);
            if (isActivityAdmin)
            {
                var match = await _matchService.UpdateMatch(matchID, updatedMatch);
                return CreatedAtAction("UpdateMatch", match);
            }
            return Unauthorized();
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
            var username = AuthUtils.GetUsername(User);
            var series = _seriesService.GetSeriesByID(newMatch.SeriesID);
            var isActivityAdmin = _activityService.IsActivityAdmin(username, series.ActivityID);
            if (isActivityAdmin)
            {
                var match = await _matchService.PostMatch(newMatch);
                return CreatedAtAction("CreateMatch", match);
            }
            return Unauthorized();
        }

        /// <summary>
        /// Creates Match Attendee
        /// </summary>
        /// <param name="username"></param>
        /// <param name="matchID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{matchID}/attendance")]
        public async Task<ActionResult> AddAttendance(int matchID, [FromBody] string username)
        {
            // We don't have to send username through requests, Microsoft.AspNetCore is able
            // to get/set the user who of this request.
            // We'll refactor the routes in the future PRs
            // var username = AuthUtils.GetUsername(User);

            var match = _matchService.GetMatchByID(matchID);
            var series = _seriesService.GetSeriesByID(match.SeriesID);
            var isActivityAdmin = _activityService.IsActivityAdmin(username, series.ActivityID);
            if (isActivityAdmin)
            {
                var attendance = await _matchService.AddParticipantAttendance(username, matchID);
                return CreatedAtAction("AddAttendance", attendance);
            }
            return Unauthorized();
        }

    }
}
