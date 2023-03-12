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
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Gordon360.Controllers.RecIM
{
    [Route("api/recim/[controller]")]
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
       /// Get's current match attendance for a specified match
       /// </summary>
       /// <param name="matchID"></param>
       /// <returns></returns>
        [HttpGet]
        [Route("{matchID}/attendance")]
        public ActionResult<IEnumerable<ParticipantAttendanceViewModel>> GetMatchAttendance(int matchID)
        {
            var res = _matchService.GetMatchAttendance(matchID);
            return Ok(res);
        }

        /// <summary>
        /// Fetches Match by MatchID
        /// </summary>
        /// <param name="matchID"></param>
        /// <returns>The match with the requested matchID (or null)</returns>
        [HttpGet]
        [Route("{matchID}")]
        public ActionResult<MatchExtendedViewModel> GetMatchByID(int matchID)
        {
            var match = _matchService.GetMatchByID(matchID);
            return Ok(match);
        }

        /// <summary>
        /// Match lookup
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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
        /// Gets all surfaces
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("surfaces")]
        public ActionResult<IEnumerable<SurfaceViewModel>> GetSurfaces()
        {
            return Ok(_matchService.GetSurfaces());
        }

        /// <summary>
        /// Creates a new match/series surface
        /// </summary>
        /// <param name="newSurface"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("surfaces")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.RECIM_SURFACE)]
        public async Task<ActionResult<SurfaceViewModel>> PostSurface(SurfaceUploadViewModel newSurface)
        {
            if (newSurface.Name is null && newSurface.Description is null) return BadRequest("Surface has to have name or description filled out");
            var res = await _matchService.PostSurfaceAsync(newSurface);
            return CreatedAtAction(nameof(UpdateSurface), new { surfaceID = res.ID }, res);
        }

        /// <summary>
        /// Updates a given surface
        /// </summary>
        /// <param name="surfaceID"></param>
        /// <param name="updatedSurface"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("surfaces/{surfaceID}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_SURFACE)]
        public async Task<ActionResult<SurfaceViewModel>> UpdateSurface(int surfaceID, SurfaceUploadViewModel updatedSurface)
        {
            if (updatedSurface.Name is null && updatedSurface.Description is null) return BadRequest("Surface has to have name or description filled out");
            var res = await _matchService.UpdateSurfaceAsync(surfaceID, updatedSurface);
            return CreatedAtAction(nameof(UpdateSurface), new { surfaceID = res.ID }, res);
        }

        /// <summary>
        /// Deletes surface, points all foreign keys to an unknown surface
        /// to prevent corrupted data
        /// </summary>
        /// <param name="surfaceID"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("surfaces/{surfaceID}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.RECIM_SURFACE)]
        public async Task<ActionResult> DeleteSurface(int surfaceID)
        {
            await _matchService.DeleteSurfaceAsync(surfaceID);
            return NoContent();
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
            return CreatedAtAction(nameof(UpdateStats), new { matchTeamID = stats.ID }, stats);
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
            return CreatedAtAction(nameof(UpdateMatch), new { matchID = match.ID }, match);
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
            return CreatedAtAction(nameof(CreateMatch), new { matchID = match.ID }, match);
        }

        /// <summary>
        /// Cascade deletes all DBobjects related to given Match ID
        /// </summary>
        /// <param name="matchID"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{matchID}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.RECIM_MATCH)]
        public async Task<ActionResult> DeleteMatchCascade(int matchID)
        {
            var res = await _matchService.DeleteMatchCascadeAsync(matchID);
            return Ok(res);
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
        public async Task<ActionResult<IEnumerable<MatchAttendance>>> AddParticipantAttendance(int matchID, ParticipantAttendanceViewModel teamAttendanceList)
        {
            var attendance = await _teamService.AddParticipantAttendanceAsync(matchID, teamAttendanceList);
            return CreatedAtAction(nameof(AddParticipantAttendance), attendance);
        }
    }
}
