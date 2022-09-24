using Gordon360.Authorization;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class RecIMController : GordonControllerBase
    {
        private readonly IRecIMService _recimService;

        public RecIMController(IRecIMService recimService)
        {
            _recimService = recimService;
        }

        ///<summary>Gets a list of all Leagues</summary>
        /// <returns>
        /// All Existing Leagues 
        /// </returns>
        [HttpGet]
        [Route("league")]
        public ActionResult<IEnumerable<League>> GetAllLeagues()
        {
            var result = _recimService.GetLeagues();

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
        [Route("league/add")]
        public async Task<ActionResult> CreateLeague(League newLeague)
        {
            await _recimService.PostLeague(newLeague);
            return Ok();
        }

        ///<summary>Creates a new League (currently hard coded)</summary>
        /// <param></param>
        /// <returns></returns>
        [HttpPost]
        [Route("league/add_smash")]
        public async Task<ActionResult> CreateSmashLeague()
        {
            await _recimService.PostSmashLeague();
            return Ok();
        }

    }
}
