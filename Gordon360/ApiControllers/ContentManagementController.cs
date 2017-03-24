using System;
using System.Web.Http;
using Gordon360.Services;
using Gordon360.Repositories;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Static.Methods;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/cms")]
    [Authorize]
    [CustomExceptionFilter]
    public class ContentManagementController : ApiController
    {
        private IContentManagementService _contentManagementService;

        public ContentManagementController()
        {
            var _unitOfWork = new UnitOfWork();
            _contentManagementService = new ContentManagementService(_unitOfWork);
        }

        public ContentManagementController(IContentManagementService contentManagementService)
        {
            _contentManagementService = contentManagementService;
        }

        /// <summary>Get all the slider content for the dashboard slider</summary>
        /// <returns>A list of all the slides for the slider</returns>
        /// <remarks>Queries the database for all entries in slider table</remarks>
        // GET: api/cms/slider
        [HttpGet]
        [Route("slider")]
        public IHttpActionResult GetSliderContent()
        {
            var result = _contentManagementService.GetSliderContent();
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
