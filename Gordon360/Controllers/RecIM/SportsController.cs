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
    public class SportsController : GordonControllerBase
    {
        private readonly ISportService _sportService;

        public SportsController(ISportService sportService)
        {
            _sportService = sportService;
        }

        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<SportViewModel>> GetSports()
        {
            var res = _sportService.GetSports();
            return Ok(res);
        }

        [HttpGet]
        [Route("{sportID}")]
        public ActionResult<SportViewModel> GetSportByID(int sportID)
        {
            var res = _sportService.GetSportByID(sportID);
            return Ok(res);
        }
      
        [HttpPatch]
        [Route("{sportID}")]
        public async Task<ActionResult<SportViewModel>> UpdateSport(int sportID, SportViewModel updatedSport)
        {
            var sport = await _sportService.UpdateSport(updatedSport);
            return CreatedAtAction("UpdateSport", sport);
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<SportViewModel>> CreateSport(SportUploadViewModel newSport)
        {
            var sport = await _sportService.PostSport(newSport);
            return CreatedAtAction("CreateSport", sport);
        }

    }
}
