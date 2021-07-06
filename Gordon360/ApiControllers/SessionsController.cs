using System;
using System.Collections.Generic;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.Static.Methods;
using Microsoft.AspNetCore.Mvc;

namespace Gordon360.Controllers.Api
{
    [Route("api/sessions")]
    [CustomExceptionFilter]
    //All Routes made public for Guest View (No authorization needed)
    public class SessionsController : ControllerBase
    {
        private readonly ISessionService _sessionService;

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
        //Public Route
        public ActionResult<IEnumerable<SessionViewModel>> Get()
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
        //Public Route
        public ActionResult<SessionViewModel> Get(string id)
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
        //Public Route
        public ActionResult<SessionViewModel> GetCurrentSession()
        {
            var currentSession = Helpers.GetCurrentSession();
            if(currentSession == null)
            {
                return NotFound();
            }

            return Ok(currentSession);
        }

        /// <summary>
        /// Gets the first day in the current session
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("firstDay")]
        //Public Route
        public IHttpActionResult GetFirstDayinSemester()
        {
            var firstDay = Helpers.GetFirstDay();
            if (firstDay == null)
            {
                return NotFound();
            }

            return Ok(firstDay);
        }

        /// <summary>
        /// Gets the last day in the current session
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("lastDay")]
        //Public Route
        public IHttpActionResult GetLastDayinSemester()
        {
            var lastDay = Helpers.GetLastDay();
            if (lastDay == null)
            {
                return NotFound();
            }

            return Ok(lastDay);
        }

        /// <summary>
        /// Gets the days left in the current session
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("daysLeft")]
        //Public Route
        public ActionResult<double[]> GetDaysLeftinSemester()
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
