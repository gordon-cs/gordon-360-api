using Gordon360.Models.CCT;
using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Gordon360.Controllers.RecIMControllers
{
    [Route("api/recim")]
    public class LeagueController : GordonControllerBase
    {
        private readonly ILeagueService _leagueService;

        public LeagueController(ILeagueService leagueService)
        {
            _leagueService = leagueService;
        }

        ///<summary>Gets a list of all Leagues</summary>
        /// <returns>
        /// All Existing Leagues 
        /// </returns>
        [HttpGet]
        [Route("league")]
        public ActionResult<IEnumerable<League>> GetAllLeagues()
        {
            var result = _leagueService.GetAllLeagues();

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        ///<summary>Gets a League object by ID number</summary>
        /// <param name="leagueID">League ID Number</param>
        /// <returns>
        /// League object
        /// </returns>
        [HttpGet]
        [Route("league/{leagueID}")]
        public ActionResult<League> GetLeagueByID(int leagueID)
        {
            var result = _leagueService.GetLeagueByID(leagueID);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        ///<summary>Gets a list of all Series associated with a League</summary>
        /// <param name="leagueID"> League ID Number</param>
        /// <returns>
        /// list of all Series object associated with a league
        /// </returns>
        [HttpGet]
        [Route("league/{ID}/series")]
        public ActionResult<HashSet<Series>> GetAllLeagueSeries(int leagueID)
        {
            var result = _leagueService.GetAllLeagueSeries(leagueID);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        /// <summary>
        /// Posts league into CCT.RecIM.Leagues
        /// </summary>
        /// <param name="newLeague">League object with appropriate values</param>
        /// <returns></returns>
        [HttpPost]
        [Route("league")]
        public async Task<ActionResult> CreateLeague(League newLeague)
        {
            await _leagueService.PostLeague(newLeague);
            return Ok();
        }

        ///<summary>Creates a new League (currently hard coded)</summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost]
        [Route("league/add_smash")]
        public async Task<ActionResult> CreateSmashLeague()
        {
            var smashLeague = new League
            {
                Name = "Super Smash Bros. Ultimate 1v1",
                RegistrationStart = DateTime.Now,
                RegistrationEnd = DateTime.Now,
                TypeID = 1,
                StatusID = 1,
                SportID = 1,
                MinCapacity = 1,
                MaxCapacity = null,
                SoloRegistration = true,
                Logo = null,
                Completed = false
            };
            await _leagueService.PostLeague(smashLeague);
            return Ok();
        }

    }
}
