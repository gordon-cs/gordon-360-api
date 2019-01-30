using System.Web.Http;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;

namespace Gordon360.Controllers.API
{
    [RoutePrefix("vpscore")]
    [CustomExceptionFilter]
    [Authorize]
    public class VictoryPromiseController : ApiController
    {
        private IVictoryPromiseService _victoryPromiseService;

        public VictoryPromiseController()
        {
            var _unitOfWork = new UnitOfWork();
            _victoryPromiseService = new VictoryPromiseService(_unitOfWork);
        }
        public VictoryPromiseController(IVictoryPromiseService victoryPromiseService)
        {
            _victoryPromiseService = victoryPromiseService;
        }

        /// <summary>
        ///  Gets current victory promise scores
        /// </summary>
        /// <param name="id">The ID of the student</param>
        /// <returns>A VP object object</returns>
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(string id)
        {
            var result = _victoryPromiseService.GetVPScores(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
        }
}