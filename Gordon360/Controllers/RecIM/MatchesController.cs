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


        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<MatchViewModel>> GetMatches([FromQuery] DateTime? day, bool active)
        {
            return null;
        }

        [HttpGet]
        [Route("{matchID}")]
        public ActionResult<MatchViewModel> GetMatchByID(int matchID)
        {
            return null;
        }

        [HttpPut]
        [Route("score")]
        public Task<ActionResult> UpdateTeamScore(MatchTeamViewModel m)
        {
            return null;
        }

        [HttpPut]
        [Route("sportmanship")]
        public Task<ActionResult> UpdateTeamSportmanship(MatchTeamViewModel m)
        {
            return null;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreateMatch(CreateMatchViewModel m)
        {
            return null;
        }

    }
}
