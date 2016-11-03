using System.Web.Http;
using Gordon360.Services;
using Gordon360.Repositories;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using System;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Models;
using Gordon360.Exceptions.CustomExceptions;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Diagnostics;
using Gordon360.Providers;
using System.IO;

namespace Gordon360.Controllers.Api
{
    
    [RoutePrefix("api/activities")]
    [CustomExceptionFilter]
    [Authorize]
    public class ActivitiesController : ApiController
    {
        private IActivityService _activityService;
        
        public ActivitiesController()
        {
            var _unitOfWork = new UnitOfWork();
            _activityService = new ActivityService(_unitOfWork);
        }

        public ActivitiesController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        /// <summary>
        /// Get all available activities
        /// </summary>
        /// <returns>All the activities in the databse</returns>
        /// <remarks></remarks>
        // GET api/<controller>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var all = _activityService.GetAll();
            return Ok(all);
        }

        /// <summary>Get a single activity based upon the string id entered in the URL</summary>
        /// <param name="id">An identifier for a single activity</param>
        /// <returns></returns>
        /// <remarks>Get a single activity from the database</remarks>
        // GET api/<controller>/5
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult Get(string id)
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
            var result = _activityService.Get(id);

            if ( result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>Gets the activities taking place during a given session</summary>
        /// <param name="id">The session identifier</param>
        /// <returns>A list of all activities that are active during the given session determined by the id parameter</returns>
        /// <remarks>Queries the database to find which activities are active during the session desired</remarks>
        // GET: api/sessions/id/activities
        [HttpGet]
        [Route("session/{id}")]
        public IHttpActionResult GetActivitiesForSession(string id)
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

            var result = _activityService.GetActivitiesForSession(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.ACTIVITY_INFO)]
        public IHttpActionResult Put(string id, ACT_INFO activity)
        {
            if(!ModelState.IsValid)
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

            var result = _activityService.Update(id, activity);

            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Set an image for the activity
        /// </summary>
        /// <param name="id">The activity Code</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/image")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.ACTIVITY_INFO)]
        public async Task<HttpResponseMessage> PostImage(string id)
        {
            // Verify Input
            if(!ModelState.IsValid)
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
            var uploadsFolder = "/browseable/uploads/" + id + "/";
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            
            if(!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~" + uploadsFolder)))
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~" + uploadsFolder));
            }

            string root = HttpContext.Current.Server.MapPath("~" + uploadsFolder);
            var provider = new CustomMultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    var uploadPath = uploadsFolder + file.Headers.ContentDisposition.FileName.Replace("\"", "");
                    var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
                    var imagePath = baseUrl + uploadPath;
                    _activityService.UpdateActivityImage(id, imagePath);
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "There was an error uploading the image. Please contact the maintainers");
            }
        }

        /// <summary>
        /// Reset the activity Image
        /// </summary>
        /// <param name="id">The activity code</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/image/reset")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.ACTIVITY_INFO)]
        public IHttpActionResult ResetImage(string id)
        {
            // Verify Input
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

            _activityService.ResetActivityImage(id);

            return Ok();

        }

    }
}