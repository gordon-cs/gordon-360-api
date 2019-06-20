using System;
using System.Web.Http;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.Static.Methods;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/sessions")]
    [CustomExceptionFilter]
    [Authorize]
    public class SessionsController : ApiController
    {

        private ISessionService _sessionService;

        public SessionsController()
        {
            var _unitOfWork = new UnitOfWork();
            _sessionService = new SessionService(_unitOfWork);
        }
        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        /// <summary>Get a list of all sessions</summary>
        /// <returns>All sessions within the database</returns>
        /// <remarks>Queries the database for all sessions, current and past</remarks>
        // GET: api/Sessions
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var all = _sessionService.GetAll();
            return Ok(all);
        }

        /// <summary>Get one specific session specified by the id in the URL string</summary>
        /// <param name="id">The identifier for one specific session</param>
        /// <returns>The information about one specific session</returns>
        /// <remarks>Queries the database regarding a specific session with the given identifier</remarks>
        // GET: api/Sessions/5
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            if (!ModelState.IsValid || String.IsNullOrWhiteSpace(id))
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

            var result = _sessionService.Get(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets the current active session
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("current")]
        public IHttpActionResult GetCurrentSession()
        {
            var currentSession = Helpers.GetCurrentSession();
            if(currentSession == null)
            {
                return NotFound();
            }

            return Ok(currentSession);
        }
        /// <summary>
        /// Gets the days left in the current semester
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("daysLeft")]
        public IHttpActionResult GetDaysLeftinSemester()
        {
            var days = Helpers.GetDaysLeft();
            if ((days[1] == 0 && days[2] == 0) || days == null)
            {
                return NotFound();
            }

            return Ok(days);
        }
    }
}
