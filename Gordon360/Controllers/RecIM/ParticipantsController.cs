using Gordon360.Authorization;
using Gordon360.Models.ViewModels;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Services;
using Gordon360.Services.RecIM;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Controllers.RecIM;

[Route("api/recim/[controller]")]
public class ParticipantsController : GordonControllerBase
{
    private readonly IParticipantService _participantService;
    private readonly IAccountService _accountService;

    public ParticipantsController(IParticipantService participantService, IAccountService accountService)
    {
        _participantService = participantService;
        _accountService = accountService;
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

    [HttpGet]
    [Route("search/{searchString}")]
    public async Task<ActionResult<IEnumerable<BasicInfoViewModel>>> SearchAsync(string searchString)
    {
        var username = AuthUtils.GetUsername(User);
        var isAdmin = _participantService.IsAdmin(username);

        var accounts = await _accountService.GetAllBasicInfoExceptAlumniAsync();
        if (isAdmin)
        {
            var customAccounts = _participantService.GetAllCustomParticipantsBasicInfo();
            accounts = accounts.Union(customAccounts);
        }

        var searchResults = _accountService.Search(searchString, accounts);

        return Ok(searchResults);
    }

    [HttpPut]
    [Route("{username}")]
    public async Task<ActionResult<ParticipantExtendedViewModel>> AddParticipantAsync(string username)
    {
        var participant = await _participantService.PostParticipantAsync(username);
        return CreatedAtAction(nameof(GetParticipantByUsername), new { username = participant.Username }, participant);
    }

    [HttpPost]
    [Route("{username}/custom")]
    [StateYourBusiness(operation = Operation.ADD, resource = Resource.RECIM_PARTICIPANT_ADMIN)]
    public async Task<ActionResult<ParticipantExtendedViewModel>> AddCustomParticipantAsync(string username, [FromBody] CustomParticipantViewModel newCustomParticipant)
    {
        var participant = await _participantService.PostCustomParticipantAsync(username, newCustomParticipant);
        return CreatedAtAction(nameof(GetParticipantByUsername), new { username = participant.Username }, participant);
    }

    [HttpPatch]
    [Route("{username}/custom")]
    [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_PARTICIPANT_ADMIN)]
    public async Task<ActionResult<ParticipantExtendedViewModel>> SetCustomParticipantAsync(string username, [FromBody] CustomParticipantPatchViewModel updatedCustomParticipant)
    {
        var isCustom = _participantService.GetParticipantIsCustom(username);
        if (!isCustom)
            return UnprocessableEntity("This is not a custom participant");

        var participant = await _participantService.UpdateCustomParticipantAsync(username, updatedCustomParticipant);
        return CreatedAtAction(nameof(GetParticipantByUsername), new { username = participant.Username, isCustom = participant.IsCustom }, participant);
    }

    [HttpPatch]
    [Route("{username}/admin")]
    [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_PARTICIPANT_ADMIN)]
    public async Task<ActionResult<ParticipantExtendedViewModel>> SetParticipantAdminStatusAsync(string username, [FromBody] bool isAdmin)
    {
        var participant = await _participantService.SetParticipantAdminStatusAsync(username, isAdmin);
        return CreatedAtAction(nameof(GetParticipantByUsername), new { username = participant.Username }, participant);
    }

    [HttpPatch]
    [Route("{username}/emails")]
    [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_PARTICIPANT)]
    public async Task<ActionResult<ParticipantExtendedViewModel>> SetParticipantAllowEmailsAsync(string username, [FromBody] bool allowEmails)
    {
        var participant = await _participantService.UpdateParticipantAllowEmailsAsync(username, allowEmails);
        return CreatedAtAction(nameof(GetParticipantByUsername), new { username = participant.Username }, participant);
    }


    [HttpPost]
    [Route("{username}/notifications")]
    public async Task<ActionResult<ParticipantNotificationViewModel>> SendParticipantNotificationAsync(string username, ParticipantNotificationUploadViewModel notificationVM)
    {
        var notification = await _participantService.SendParticipantNotificationAsync(username, notificationVM);
        return CreatedAtAction(nameof(GetParticipantByUsername), new { participantUsername = notification.ParticipantUsername }, notification);
    }


    [HttpPatch]
    [Route("{username}/activities")]
    [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_PARTICIPANT)]
    public async Task<ActionResult> UpdateParticipantActivityAsync(string username, ParticipantActivityPatchViewModel updatedParticipantActivity)
    {

        var participant = await _participantService.UpdateParticipantActivityAsync(username, updatedParticipantActivity);
        return CreatedAtAction(nameof(GetParticipantByUsername), new { username = participant.ParticipantUsername }, participant);
    }

    [HttpPatch]
    [Route("{username}/status")]
    [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.RECIM_PARTICIPANT_ADMIN)]
    public async Task<ActionResult<ParticipantStatusHistoryViewModel>> UpdateParticipantStatusAsync(string username, ParticipantStatusPatchViewModel updatedParticipant)
    {
        var status = await _participantService.UpdateParticipantStatusAsync(username, updatedParticipant);
        return CreatedAtAction(nameof(GetParticipantByUsername), new { username = status.ParticipantUsername }, status);
    }
}
