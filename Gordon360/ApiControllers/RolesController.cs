using System;
using System.Web.Http;
using Gordon360.Services;
using Gordon360.Repositories;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Static.Methods;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/participations")]
    [Authorize]
    [CustomExceptionFilter]
    public class ParticipationsController : ApiController
    {
        private IParticipationService _participationService;

        public ParticipationsController()
        {
            var _unitOfWork = new UnitOfWork();
            _participationService = new ParticipationService(_unitOfWork);
        }
        public ParticipationsController(IParticipationService roleservice)
        {
            _participationService = roleservice; ;
        }

        /// <summary>Get all the roles a person may have within an activity</summary>
        /// <returns>A list of all the roles and their coresponding acronyms</returns>
        /// <remarks>Queries the database for all the roles that are valid</remarks>
        // GET: api/participations
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var all = _participationService.GetAll();
            return Ok(all);
        }

        /// <summary>Get a single role and the information about it</summary>
        /// <param name="id">The identifier for a single role</param>
        /// <returns>The information about the specified role</returns>
        /// <remarks>Queries the database and returns information about one particular role</remarks>
        // GET: api/PART_DEF/5
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
            var result = _participationService.Get(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


        /// <summary>
        /// Get the participation levels that are considered to be leadership positions.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("leaders")]
        public IHttpActionResult GetLeaderRoles()
        {
            if (!ModelState.IsValid)
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
            var result = Helpers.GetLeaderRoleCodes();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


        /// <summary>
        /// GEt the participation levels that are considered to be leadership positions.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("transcript-worthy")]
        public IHttpActionResult GetTranscriptWorthyRoles()
        {
            if (!ModelState.IsValid)
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
            var result = Helpers.GetTranscriptWorthyRoles();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


    }
}
