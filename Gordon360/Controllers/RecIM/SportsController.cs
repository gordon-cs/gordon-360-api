using Gordon360.Authorization;
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
        private readonly IParticipantService _participantService;

        public SportsController(ISportService sportService, IParticipantService participantService)
        {
            _sportService = sportService;
            _participantService = participantService;
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
        public async Task<ActionResult> UpdateSport(int sportID, SportViewModel updatedSport)
        {
            var username = AuthUtils.GetUsername(User);
            var isAdmin = _participantService.IsAdmin(username);
            if (isAdmin)
            {
                var sport = await _sportService.UpdateSport(updatedSport);
                return CreatedAtAction("UpdateSport", sport);
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreateSport(SportUploadViewModel newSport)
        {
            var username = AuthUtils.GetUsername(User);
            var isAdmin = _participantService.IsAdmin(username);
            if (isAdmin)
            {
                var sport = await _sportService.PostSport(newSport);
                return CreatedAtAction("UpdateSport", sport);
            }
            return Unauthorized();
        }

    }
}
