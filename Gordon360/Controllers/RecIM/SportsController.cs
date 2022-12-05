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
        /// <summary>
        /// Fetches all Sports in the RecIM database
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<SportViewModel>> GetSports()
        {
            var res = _sportService.GetSports();
            return Ok(res);
        }
        /// <summary>
        /// Gets specific sport by ID
        /// </summary>
        /// <param name="sportID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{sportID}")]
        public ActionResult<SportViewModel> GetSportByID(int sportID)
        {
            var res = _sportService.GetSportByID(sportID);
            return Ok(res);
        }
        /// <summary>
        /// Update Sport in the database by ID
        /// </summary>
        /// <param name="sportID"></param>
        /// <param name="updatedSport"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{sportID}")]
        public async Task<ActionResult> UpdateSport(int sportID, SportPatchViewModel updatedSport)
        {
            var sport = await _sportService.UpdateSport(updatedSport);
            return CreatedAtAction("UpdateSport", sport);
        }
        /// <summary>
        /// Creates new Sport for RecIM
        /// </summary>
        /// <param name="newSport"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreateSport(SportUploadViewModel newSport)
        {
            var sport = await _sportService.PostSport(newSport);
            return CreatedAtAction("UpdateSport", sport);
        }

    }
}
