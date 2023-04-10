using Gordon360.Authorization;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services.RecIM;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gordon360.Controllers.RecIM
{
    [Route("api/recim/[controller]")]
    public class ParticipantsController : GordonControllerBase
    {
        private readonly IParticipantService _participantService;

        public ParticipantsController(IParticipantService participantService)
        {
            _participantService = participantService;
        }

        [HttpGet]
        [Route("")]
        public ActionResult<IEnumerable<ParticipantExtendedViewModel>> GetParticipants()
        {
            var res = _participantService.GetParticipants();
            return Ok(res);
        }

        [HttpGet]
        [Route("{username}/statushistory")]
        public ActionResult<IEnumerable<ParticipantStatusExtendedViewModel>> GetParticipantStatushistory(string username)
        {
            var res = _participantService.GetParticipantStatusHistory(username);
            return Ok(res);
        }

        [HttpGet]
        [Route("{username}")]
        public ActionResult<ParticipantExtendedViewModel> GetParticipantByUsername(string username)
        {
            var res = _participantService.GetParticipantByUsername(username);
            return Ok(res);
        }

        [HttpGet]
        [Route("{username}/teams")]
        public ActionResult<IEnumerable<TeamExtendedViewModel>> GetParticipantTeams(string username)
        {
            var res = _participantService.GetParticipantTeams(username);
            return Ok(res);
        }

        [HttpGet]
        [Route("lookup")]
        public ActionResult<IEnumerable<LookupViewModel>> GetParticipantTypes(string type)
        {
            var res = _participantService.GetParticipantLookup(type);
            if (res is not null)
            {
                return Ok(res);
            }
            return NotFound();
        }

        [HttpPut]
        [Route("{username}")]
        public async Task<ActionResult<ParticipantExtendedViewModel>> AddParticipant(string username)
        {
            var participant = await _participantService.PostParticipantAsync(username);
            return CreatedAtAction(nameof(GetParticipantByUsername), new { username = participant.Username }, participant);
        }

        [HttpPatch]
        [Route("{username}/admin")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_PARTICIPANT_ADMIN)]
        public async Task<ActionResult<ParticipantExtendedViewModel>> SetParticipantAdminStatus(string username, [FromBody] bool isAdmin)
        {
            var participant = await _participantService.SetParticipantAdminStatusAsync(username,isAdmin);
            return CreatedAtAction(nameof(GetParticipantByUsername), new { username = participant.Username }, participant);
        }

        [HttpPatch]
        [Route("{username}/emails")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_PARTICIPANT)]
        public async Task<ActionResult<ParticipantExtendedViewModel>> UpdateParticipant(string username, [FromBody] bool allowEmails)
        {
            var participant = await _participantService.UpdateParticipantAsync(username, allowEmails);
            return CreatedAtAction(nameof(GetParticipantByUsername), new { username = participant.Username }, participant);
        }


        [HttpPost]
        [Route("{username}/notifications")]
        public async Task<ActionResult<ParticipantNotificationViewModel>> SendParticipantNotification(string username, ParticipantNotificationUploadViewModel notificationVM)
        {
            var notification = await _participantService.SendParticipantNotificationAsync(username, notificationVM);
            return CreatedAtAction(nameof(GetParticipantByUsername), new { participantUsername = notification.ParticipantUsername }, notification);
        }


        [HttpPatch]
        [Route("{username}/activities")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_PARTICIPANT)]
        public async Task<ActionResult> UpdateParticipantActivity(string username, ParticipantActivityPatchViewModel updatedParticipantActivity)
        {

            var participant = await _participantService.UpdateParticipantActivityAsync(username, updatedParticipantActivity);
            return CreatedAtAction(nameof(GetParticipantByUsername), new { username = participant.ParticipantUsername }, participant);
        }

        [HttpPatch]
        [Route("{username}/status")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_PARTICIPANT)]
        public async Task<ActionResult<ParticipantStatusHistoryViewModel>> UpdateParticipantStatus(string username, ParticipantStatusPatchViewModel updatedParticipant)
        {
            var status = await _participantService.UpdateParticipantStatusAsync(username, updatedParticipant);
            return CreatedAtAction(nameof(GetParticipantByUsername), new { username = status.ParticipantUsername }, status);
        }

    }
}
