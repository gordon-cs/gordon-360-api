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
            var result = _leagueService.GetLeagues();
            return Ok(result);
        }

        ///<summary>Gets a list of all Leagues after given time</summary>
        /// <returns>
        /// All Existing Leagues 
        /// </returns>
        [HttpGet]
        [Route("{time}")]
        public ActionResult<IEnumerable<League>> GetAllLeagues(DateTime time)
        {
            return Ok();
        }

        ///<summary>Gets a League object by ID number</summary>
        /// <param name="leagueID">League ID Number</param>
        /// <returns>
        /// League object
        /// </returns>
        [HttpGet]
        [Route("{leagueID}")]
        public ActionResult<League> GetLeagueByID(int leagueID)
        {
            var result = _leagueService.GetLeagueByID(leagueID);

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
        /// <summary>
        /// Updates league based on input
        /// </summary>
        /// <param name="updatedLeague"> Updated League Object </param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        public async Task<ActionResult> UpdateLeague(League updatedLeague)
        {
            await _leagueService.UpdateLeague(updatedLeague);
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
