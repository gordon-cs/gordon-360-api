﻿using Gordon360.Models.CCT;
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
            var participant = await _participantService.PostParticipant(participantID);
            return CreatedAtAction("AddParticipant", participant);
        }

        [HttpPost]
        [Route("{username}/notifications")]
        public async Task<IActionResult> SendParticipantNotification(string username, ParticipantNotificationUploadViewModel notificationVM)
        {
            var notification = await _participantService.SendParticipantNotification(username, notificationVM);
            return CreatedAtAction("SendParticipantNotification", notification);
        }


        [HttpPatch]
        [Route("{username}")]
        public async Task<ActionResult> UpdateParticipant(string username, ParticipantActivityPatchViewModel updatedParticipant)
        {

            var participant = await _participantService.UpdateParticipant(username, updatedParticipant);
            return CreatedAtAction("UpdateParticipant", participant);
        }

        [HttpPatch]
        [Route("{username}/status")]
        public async Task<ActionResult> UpdateParticipantStatus(string username, ParticipantStatusPatchViewModel updatedParticipant)
        {
            var status = await _participantService.UpdateParticipantStatus(username, updatedParticipant);
            return CreatedAtAction("UpdateParticipantStatus",status);
        }

    }
}
