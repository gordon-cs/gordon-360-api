using Gordon360.Database.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Methods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gordon360.ApiControllers
{
    [Route("api/[controller]")]
    public class SessionsController : GordonControllerBase
    {
        private readonly ISessionService _sessionService;

        public SessionsController(CCTContext context)
        {
            _sessionService = new SessionService(context);
        }

        /// <summary>Get a list of all sessions</summary>
        /// <returns>All sessions within the database</returns>
        /// <remarks>Queries the database for all sessions, current and past</remarks>
        // GET: api/Sessions
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
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
        [AllowAnonymous]
        public ActionResult<SessionViewModel> Get(string id)
        {
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
        [AllowAnonymous]
        public ActionResult<SessionViewModel> GetCurrentSession()
        {
            var currentSession = Helpers.GetCurrentSession();
            if (currentSession == null)
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
        [AllowAnonymous]
        public async Task<ActionResult<double[]>> GetDaysLeftinSemester()
        {
            var days = await Helpers.GetDaysLeft();
            if (days[1] == 0 && days[2] == 0 || days == null)
            {
                return NotFound();
            }

            return Ok(days);
        }
    }
}
