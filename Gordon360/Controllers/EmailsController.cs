using Gordon360.Authorization;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Gordon360.Controllers;

[Route("api/[controller]")]
public class EmailsController(IEmailService emailService) : GordonControllerBase
{
    [HttpGet]
    [Route("involvement/{activityCode}")]
    [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_ACTIVITY)]
    public ActionResult<IEnumerable<EmailViewModel>> GetEmailsForActivity(string activityCode, string? sessionCode = null, [FromQuery] List<string>? participationTypes = null)
    {
        var result = emailService.GetEmailsForActivity(activityCode, sessionCode, participationTypes);

        return Ok(result);
    }

    [HttpPut]
    [Route("")]
    public ActionResult SendEmails([FromBody] EmailContentViewModel email)
    {
        var to_emails = email.ToAddress.Split(',');
        emailService.SendEmails(to_emails, email.FromAddress, email.Subject, email.Content, email.Password);
        return Ok();
    }

    [HttpPut]
    [Route("activity/{id}/session/{session}")]
    [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_ACTIVITY)]
    public ActionResult SendEmailToActivity(string id, string session, [FromBody] EmailContentViewModel email)
    {
        emailService.SendEmailToActivity(id, session, email.FromAddress, email.Subject, email.Content, email.Password);

        return Ok();
    }
}
