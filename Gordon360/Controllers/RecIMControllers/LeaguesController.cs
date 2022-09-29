using Gordon360.Models.CCT;
using Gordon360.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Gordon360.Controllers.RecIMControllers
{
    [Route("api/recim/[controller]")]
    public class LeaguesController : GordonControllerBase
    {
        private readonly ILeagueService _leagueService;

        public LeaguesController(ILeagueService leagueService)
        {
            _leagueService = leagueService;
        }

        ///<summary>Gets a list of all Leagues</summary>
        /// <returns>
        /// All Existing Leagues 
        /// </returns>
        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<League>> GetAllLeagues()
        {
            var result = _leagueService.GetAllLeagues();
            return Ok(result);
        }

        ///<summary>Gets a League object by ID number</summary>
        /// <param name="leagueID">League ID Number</param>
        /// <returns>
        /// League object
        /// </returns>
        [HttpGet]
        [Route("/{leagueID}")]
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
        /// list of all Series objects associated with a league
        /// </returns>
        [HttpGet]
        [Route("/{leagueID}/series")]
        public ActionResult<List<Series>> GetLeagueSeries(int leagueID)
        {
            var result = _leagueService.GetLeagueSeries(leagueID);
            return Ok(result);
        }

        ///<summary>Gets a list of all Teams associated with a League</summary>
        /// <param name="leagueID"> League ID Number</param>
        /// <returns>
        /// list of all team objects associated with a league
        /// </returns>
        [HttpGet]
        [Route("/{leagueID}/team")]
        public ActionResult<List<Team>> GetAllLeaguTeams(int leagueID)
        {
            var result = _leagueService.GetLeagueTeams(leagueID);
            return Ok(result);
        }

        ///<summary>Gets a list of all Users associated with a League</summary>
        /// <param name="leagueID"> League ID Number</param>
        /// <returns>
        /// list of all user objects associated with a league
        /// </returns>
        [HttpGet]
        [Route("/{leagueID}/user")]
        public ActionResult<List<User>> GetAllLeagueUsers(int leagueID)
        {
            var result = _leagueService.GetLeagueUsers(leagueID);
            return Ok(result);
        }

        ///<summary>Gets League Type</summary>
        /// <param name="leagueID"> League ID Number</param>
        /// <returns>
        /// League Type Object
        /// </returns>
        [HttpGet]
        [Route("/{leagueID}/type")]
        public ActionResult<List<User>> GetLeagueType(int leagueID)
        {
            var result = _leagueService.GetLeagueType(leagueID);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        ///<summary>Gets League Status</summary>
        /// <param name="leagueID"> League ID Number</param>
        /// <returns>
        /// League Status Object
        /// </returns>
        [HttpGet]
        [Route("/{leagueID}/status")]
        public ActionResult<List<User>> GetLeagueStatus(int leagueID)
        {
            var result = _leagueService.GetLeagueStatus(leagueID);
            if ( result == null )
            {
                return NotFound();
            }
            return Ok(result);
        }

        ///<summary>Gets League Sport</summary>
        /// <param name="leagueID"> League ID Number</param>
        /// <returns>
        /// Sport Object
        /// </returns>
        [HttpGet]
        [Route("/{leagueID}/sport")]
        public ActionResult<List<User>> GetLeagueSport(int leagueID)
        {
            var result = _leagueService.GetLeagueSport(leagueID);
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
        [Route("")]
        public async Task<ActionResult> CreateLeague(League newLeague)
        {
            await _leagueService.PostLeague(newLeague);
            return Ok();
        }

        ///<summary>Creates a new League (currently hard coded)</summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/add_smash")]
        public async Task<ActionResult> CreateSmashLeague()
        {
            var smashLeague = new League
            {
                Name = "Super Smash Bros. Ultimate 1v1",
                //Name = null,
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
