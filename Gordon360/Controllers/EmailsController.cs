using Gordon360.AuthorizationFilters;
using Gordon360.Database.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class EmailsController : GordonControllerBase
    {
        private readonly EmailService _emailService;

        public EmailsController(CCTContext context)
        {
            _emailService = new EmailService(context);
        }

        [Route("activity/{id}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_ACTIVITY)]
        public ActionResult<IEnumerable<EmailViewModel>> GetEmailsForActivity(string id)
        {
            var result = _emailService.GetEmailsForActivityAsync(id);

            if (result == null)
            {
                NotFound();
            }
            return Ok(result);

        }

        [Route("activity/{id}/session/{session}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_ACTIVITY)]
        public ActionResult<IEnumerable<EmailViewModel>> GetEmailsForActivity(string id, string session)
        {
            var result = _emailService.GetEmailsForActivityAsync(id, session);

            if (result == null)
            {
                NotFound();
            }
            return Ok(result);

        }

        [Route("activity/{id}/leaders")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_LEADERS)]
        public ActionResult<IEnumerable<EmailViewModel>> GetEmailsForleader(string id)
        {
            var result = _emailService.GetEmailsForActivityLeadersAsync(id);

            if (result == null)
            {
                NotFound();
            }
            return Ok(result);
        }

        [Route("activity/{id}/group-admin/session/{session}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_GROUP_ADMIN)]
        public ActionResult<IEnumerable<EmailViewModel>> GetEmailsForGroupAdmin(string id, string session)
        {
            var result = _emailService.GetEmailsForGroupAdminAsync(id, session);

            if (result == null)
            {
                NotFound();
            }
            return Ok(result);
        }

        [Route("activity/{id}/leaders/session/{session}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_LEADERS)]
        public ActionResult<IEnumerable<EmailViewModel>> GetEmailsForleader(string id, string session)
        {
            var result = _emailService.GetEmailsForActivityLeadersAsync(id, session);

            if (result == null)
            {
                NotFound();
            }
            return Ok(result);
        }

        [Route("activity/{id}/advisors")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_LEADERS)]
        public ActionResult<IEnumerable<EmailViewModel>> GetEmailsForAdvisor(string id)
        {
            var result = _emailService.GetEmailsForActivityAdvisorsAsync(id);

            if (result == null)
            {
                NotFound();
            }
            return Ok(result);
        }

        [Route("activity/{id}/advisors/session/{session}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_LEADERS)]
        public ActionResult<IEnumerable<EmailViewModel>> GetEmailsForAdvisor(string id, string session)
        {
            var result = _emailService.GetEmailsForActivityAdvisorsAsync(id, session);

            if (result == null)
            {
                NotFound();
            }
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

        /*
        [HttpPut]
        [Route("activity/{id}/leaders/session/{session}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_LEADERS)]
        public IHttpActionResult SendEmailsToleaders(string id, string session, [FromBody] EmailContentViewModel email)
        {
            var emails = _emailService.GetEmailsForActivityLeaders(id, session);
            var toAddress = emails.Select(x => x.Email).ToArray();
            _emailService.SendEmails(toAddress, email.FromAddress, email.Subject, email.Content, email.Password);
            return Ok();
        }
        */

        [HttpPut]
        [Route("activity/{id}/session/{session}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_ACTIVITY)]
        public ActionResult SendEmailToActivity(string id, string session, [FromBody] EmailContentViewModel email)
        {
            _emailService.SendEmailToActivityAsync(id, session, email.FromAddress, email.Subject, email.Content, email.Password);

            //if (result == null)
            //{
            //    NotFound();
            //}
            //return Ok(result);
            return Ok();

        }
    }
}
