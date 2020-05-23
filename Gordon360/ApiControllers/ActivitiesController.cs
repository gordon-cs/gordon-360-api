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
using Gordon360.Static.Methods;
using System.Collections.Generic;
using Gordon360.Models.ViewModels;

namespace Gordon360.Controllers.Api
{
    
    [RoutePrefix("api/activities")]
    [CustomExceptionFilter]
    //All GET routes are public (No Authorization Needed) 
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

        [HttpGet]
        [Route("")]
        //Public Route
        public IHttpActionResult Get()
        {
            var all = _activityService.GetAll();
            return Ok(all);
        }

        [HttpGet]
        [Route("{id}")]
        //Public Route 
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
        //Public Route
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

        /// <summary>Gets the different types of activities taking place during a given session</summary>
        /// <param name="id">The session identifier</param>
        /// <returns>A list of all the different types of activities that are active during the given session determined by the id parameter</returns>
        /// <remarks>Queries the database to find the distinct activities type of activities that are active during the session desired</remarks>
        [HttpGet]
        [Route("session/{id}/types")]
        //Public Route 
        public IHttpActionResult GetActivityTypesForSession(string id)
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

            var result = _activityService.GetActivityTypesForSession(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

        }

        /// <summary>
        /// Get the status (open or closed) of an activity for a given session
        /// </summary>
        /// <param name="sessionCode">The session code that we want to check the status for</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{sessionCode}/{id}/status")]
        //Public Route 
        public IHttpActionResult GetActivityStatus(string sessionCode, string id)
        {
            var result = _activityService.IsOpen(id, sessionCode) ? "OPEN" : "CLOSED";

            return Ok(result);
        }

        /// <summary>
        /// Get all the activities that have not yet been closed out for the current session
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("open")]
        //Public Route 
        public IHttpActionResult GetOpenActivities()
        {
            var sessionCode = Helpers.GetCurrentSession().SessionCode;

            var activity_codes = _activityService.GetOpenActivities(sessionCode);

            var activities = new List<ActivityInfoViewModel>();

            foreach( var code in activity_codes)
            {
                activities.Add(_activityService.Get(code));
            }

            return Ok(activities);
        }

        /// <summary>
        /// Get all the activities that have not yet been closed out for the current session for 
        /// which a given user is the group admin
        /// </summary>
        /// <param name="id">The id of the user who is group admin</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/open")]
        //Public Route 
        public IHttpActionResult GetOpenActivities(int id)
        {
            var sessionCode = Helpers.GetCurrentSession().SessionCode;

            var activity_codes = _activityService.GetOpenActivities(sessionCode, id);

            var activities = new List<ActivityInfoViewModel>();

            foreach (var code in activity_codes)
            {
                activities.Add(_activityService.Get(code));
            }

            return Ok(activities);
        }

        /// <summary>
        /// Get all the activities that are already closed out for the current session
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("closed")]
        //Public Route 
        public IHttpActionResult GetClosedActivities()
        {
            var sessionCode = Helpers.GetCurrentSession().SessionCode;

            var activity_codes = _activityService.GetClosedActivities(sessionCode);

            var activities = new List<ActivityInfoViewModel>();

            foreach (var code in activity_codes)
            {
                activities.Add(_activityService.Get(code));
            }

            return Ok(activities);
        }

        /// <summary>
        /// Get all the activities that are already closed out for the current session for
        /// which a given user is group admin
        /// </summary>
        /// <param name="id">The id of the user who is group admin</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/closed")]
        //Public Route 
        public IHttpActionResult GetClosedActivities(int id)
        {
            var sessionCode = Helpers.GetCurrentSession().SessionCode;

            var activity_codes = _activityService.GetClosedActivities(sessionCode, id);

            var activities = new List<ActivityInfoViewModel>();

            foreach (var code in activity_codes)
            {
                activities.Add(_activityService.Get(code));
            }

            return Ok(activities);
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
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

        [HttpPut]
        [Authorize]
        [Route("{id}/session/{sess_cde}/close")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.ACTIVITY_STATUS)]
        public IHttpActionResult CloseSession(string id, string sess_cde)
        {
            _activityService.CloseOutActivityForSession(id, sess_cde);

            return Ok();
        }

        [HttpPut]
        [Authorize]
        [Route("{id}/session/{sess_cde}/open")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.ACTIVITY_STATUS)]
        public IHttpActionResult OpenSession(string id, string sess_cde)
        {
            _activityService.OpenActivityForSession(id, sess_cde);

            return Ok();
        }

        /// <summary>
        /// Set an image for the activity
        /// </summary>
        /// <param name="id">The activity Code</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
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

            var existingFile = "";
            
            if(!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~" + uploadsFolder)))
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~" + uploadsFolder));
            }
            else
            {
                try
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath("~" + uploadsFolder));
                    
                    foreach (FileInfo file in di.GetFiles())
                    {
                        existingFile = file.Name;
                        file.Delete();
                    }
                }
                catch (System.Exception e) {}
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
                    var fileName = file.Headers.ContentDisposition.FileName.Replace("\"", "");

                    // If the file has the same name as the previous one, add a 1 at the end to make it different (so that the browser does not cache it)
                    if (fileName.Equals(existingFile))
                    {
                        var oldFileName = fileName;

                        // Add "1" before the extension
                        fileName = fileName.Substring(0, fileName.LastIndexOf('.')) + "1" + fileName.Substring(fileName.LastIndexOf('.'));

                        // Rename existing File
                        System.IO.DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath("~" + uploadsFolder));
                        System.IO.File.Move(di.FullName + oldFileName, di.FullName + fileName);
                    }

                    var uploadPath = uploadsFolder + fileName;
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
        [Authorize]
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
        /// <summary>Update an existing activity to be private or not</summary>
        /// <param name="id">The id of the activity</param>
        /// <param name = "p">the boolean value</param>
        /// <remarks>Calls the server to make a call and update the database with the changed information</remarks>
        [HttpPut]
        [Authorize]
        [Route("{id}/privacy/{p}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.ACTIVITY_INFO)]
        public IHttpActionResult TogglePrivacy(string id, bool p)
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

            _activityService.TogglePrivacy(id, p);
            return Ok();
        }

    }
}