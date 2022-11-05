using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services.RecIM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gordon360.Controllers.RecIM
{
    [Route("api/recim/[controller]")]
    [AllowAnonymous]
    public class ParticipantsController : GordonControllerBase
    {
        private readonly IParticipantService _participantService;

        public ParticipantsController(IParticipantService participantService)
        {
            _participantService = participantService;
        }

        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<ParticipantViewModel>> GetParticipants()
        {
            var res = _participantService.GetParticipants();
            return Ok(res);
        }

        [HttpGet]
        [Route("{username}/StatusHistory")]
        public ActionResult<IEnumerable<ParticipantStatusViewModel>> GetParticipantStatushistory(string username)
        {
            var res = _participantService.GetParticipantStatusHistory(username);
            return Ok(res);
        }

        [HttpGet]
        [Route("{username}")]
        public ActionResult<ParticipantViewModel> GetParticipantByUsername(string username)
        {
            var res = _participantService.GetParticipantByUsername(username);
            return Ok(res);
        }

        //this will be handled eventually by team routes
        [HttpGet]
        [Route("{username}/teams")]
        public ActionResult<IEnumerable<TeamViewModel>> GetParticipantTeams(string username)
        {
            var res = _participantService.GetParticipantTeams(username);
            return Ok(res);
        }

        [HttpPost]
        [Route("{participantID}")]
        public async Task<ActionResult> AddParticipant(int participantID)
        {
            await _participantService.PostParticipant(participantID);
            return (Ok(participantID));
        }

        [HttpPatch]
        [Route("{username}")]
        public async Task<ActionResult> UpdateParticipant(string username, ParticipantPatchViewModel updatedParticipant)
        {

            await _participantService.UpdateParticipant(updatedParticipant);
            return Ok(username);
        }

        [HttpPatch]
        [Route("Status/{username}")]
        public async Task<ActionResult> UpdateParticipantStatus(string username, ParticipantStatusPatchViewModel updatedParticipant)
        {
            await _participantService.UpdateParticipantStatus(updatedParticipant);
            return Ok(username);
        }

    }
}
