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

namespace Gordon360.ApiControllers
{
    [Authorize]
    [CustomExceptionFilter]
    [RoutePrefix("api/emails")]
    public class EmailsController : ApiController
    {
        EmailService _emailService;

        public EmailsController()
        {
            IUnitOfWork unitOfWork = new UnitOfWork();
            _emailService = new EmailService(unitOfWork);
        }

        [Route("activity/{id}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.EMAILS_BY_ACTIVITY)]
        public IHttpActionResult GetEmailsForActivity(string id)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
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
                return BadRequest();
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
                return BadRequest();
            }
            var result = _emailService.GetEmailsForActivityLeaders(id);

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
                return BadRequest();
            }
            var result = _emailService.GetEmailsForActivityLeaders(id, session);

            if (result == null)
            {
                NotFound();
            }
            return Ok(result);
        }

    }
}
