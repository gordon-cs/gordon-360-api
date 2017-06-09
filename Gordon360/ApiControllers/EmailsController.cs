using Gordon360.Repositories;
using Gordon360.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Models.ViewModels;

namespace Gordon360.ApiControllers
{
    [Authorize]
    [CustomExceptionFilter]
    [RoutePrefix("api/emails")]
    public class EmailsController : ApiController
    {
        EmailService _emailService;
        AccountService _accountService;

        public EmailsController()
        {
            IUnitOfWork unitOfWork = new UnitOfWork();
            _emailService = new EmailService(unitOfWork);
            _accountService = new AccountService(unitOfWork);
        }

        [Route("activity/{id}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_ACTIVITY)]
        public IHttpActionResult GetEmailsForActivity(string id)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
            {
                string errors = "";
                foreach (var modelstate in ModelState.Values)
                {
                    foreach (var error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                throw new BadInputException() { ExceptionMessage = errors };
            }
            var result = _emailService.GetEmailsForActivity(id);

            if (result == null)
            {
                NotFound();
            }
            return Ok(result);

        }

        [Route("activity/{id}/session/{session}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_ACTIVITY)]
        public IHttpActionResult GetEmailsForActivity(string id, string session)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
            {
                string errors = "";
                foreach (var modelstate in ModelState.Values)
                {
                    foreach (var error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                throw new BadInputException() { ExceptionMessage = errors };
            }
            var result = _emailService.GetEmailsForActivity(id, session);

            if (result == null)
            {
                NotFound();
            }
            return Ok(result);

        }

        [Route("activity/{id}/leaders")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_LEADERS)]
        public IHttpActionResult GetEmailsForleader(string id)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
            {
                string errors = "";
                foreach (var modelstate in ModelState.Values)
                {
                    foreach (var error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                throw new BadInputException() { ExceptionMessage = errors };
            }
            var result = _emailService.GetEmailsForActivityLeaders(id);

            if (result == null)
            {
                NotFound();
            }
            return Ok(result);
        }

        [Route("activity/{id}/group-admin/session/{session}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_GROUP_ADMIN)]
        public IHttpActionResult GetEmailsForGroupAdmin(string id, string session)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
            {
                string errors = "";
                foreach (var modelstate in ModelState.Values)
                {
                    foreach (var error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                throw new BadInputException() { ExceptionMessage = errors };
            }
            var result = _emailService.GetEmailsForGroupAdmin(id, session);

            if (result == null)
            {
                NotFound();
            }
            return Ok(result);
        }

        [Route("activity/{id}/leaders/session/{session}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_LEADERS)]
        public IHttpActionResult GetEmailsForleader(string id, string session)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
            {
                string errors = "";
                foreach (var modelstate in ModelState.Values)
                {
                    foreach (var error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                throw new BadInputException() { ExceptionMessage = errors };
            }
            var result = _emailService.GetEmailsForActivityLeaders(id, session);

            if (result == null)
            {
                NotFound();
            }
            return Ok(result);
        }

        [Route("activity/{id}/advisors")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_LEADERS)]
        public IHttpActionResult GetEmailsForAdvisor(string id)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
            {
                string errors = "";
                foreach (var modelstate in ModelState.Values)
                {
                    foreach (var error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                throw new BadInputException() { ExceptionMessage = errors };
            }
            var result = _emailService.GetEmailsForActivityAdvisors(id);

            if (result == null)
            {
                NotFound();
            }
            return Ok(result);
        }

        [Route("activity/{id}/advisors/session/{session}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_LEADERS)]
        public IHttpActionResult GetEmailsForAdvisor(string id, string session)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
            {
                string errors = "";
                foreach (var modelstate in ModelState.Values)
                {
                    foreach (var error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                throw new BadInputException() { ExceptionMessage = errors };
            }
            var result = _emailService.GetEmailsForActivityAdvisors(id, session);

            if (result == null)
            {
                NotFound();
            }
            return Ok(result);
        }

        [HttpPut]
        [Route("activity/{id}/session/{session}/send/{gordon_id}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAIL_CONTENT)]
        public IHttpActionResult SendEmailsToGroup(string id, string session, string gordon_id, [FromBody]EmailContentViewModel email)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
            {
                string errors = "";
                foreach (var modelstate in ModelState.Values)
                {
                    foreach (var error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                throw new BadInputException() { ExceptionMessage = errors };
            }
            var to_emails = _emailService.GetEmailsForActivityLeaders(id, session);
            var from_email = _accountService.Get(gordon_id).Email;
            _emailService.SendEmails(to_emails, from_email, email.Subject, email.Content);

            return Ok();
        }

    }
}
