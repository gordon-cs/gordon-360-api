using Gordon360.Authorization;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class EmailsController : GordonControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailsController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet]
        [Route("involvement/{activityCode}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_ACTIVITY)]
        public async Task<ActionResult<IEnumerable<EmailViewModel>>> GetEmailsForActivityAsync(string activityCode, string? sessionCode = null, [FromQuery] List<string>? participationTypes = null)
        {
            var result = await _emailService.GetEmailsForActivityAsync(activityCode, sessionCode, participationTypes);

            return Ok(result);
        }

        [HttpGet]
        [Route("activity/{activityCode}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_ACTIVITY)]
        [Obsolete("Use the new route that accepts a list of participation types instead")]
        public async Task<ActionResult<IEnumerable<EmailViewModel>>> DEPRECATED_GetEmailsForActivityAsync(string activityCode, string? sessionCode, string? participationType)
        {
            var result = await _emailService.GetEmailsForActivityAsync(activityCode, sessionCode, new List<string> { participationType ?? "" });

            return Ok(result);
        }

        [HttpPut]
        [Route("")]
        public ActionResult SendEmails([FromBody] EmailContentViewModel email)
        {
            var to_emails = email.ToAddress.Split(',');
            _emailService.SendEmails(to_emails, email.FromAddress, email.Subject, email.Content, email.Password);
            return Ok();
        }

        [HttpPut]
        [Route("activity/{id}/session/{session}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_ACTIVITY)]
        public async Task<ActionResult> SendEmailToActivityAsync(string id, string session, [FromBody] EmailContentViewModel email)
        {
            await _emailService.SendEmailToActivityAsync(id, session, email.FromAddress, email.Subject, email.Content, email.Password);

            return Ok();

        }
    }
}
