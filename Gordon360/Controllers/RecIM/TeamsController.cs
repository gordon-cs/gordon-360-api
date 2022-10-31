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
    public class TeamsController : GordonControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        ///<summary>Get a Team object by ID number</summary>
        /// <param name="teamID"> Team ID Number</param>
        /// <returns>Team object</returns>
        [HttpGet]
        [Route("{teamID}")]
        public ActionResult<TeamViewModel> GetTeamByID(int teamID)
        {
            var team = _teamService.GetTeamByID(teamID);
            return Ok(team);
        }

        /*/// <summary>
        /// Posts Team into CCT.RecIM.Team
        /// </summary>
        /// <param name="t">CreateTeamViewModel object with appropriate values</param>
        /// <returns>Posted Team ID</returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreateTeam(CreateTeamViewModel t)
        {
            var teamID = await _teamService.PostTeam(t);
            return Ok(teamID);
        }*/
    }
}
