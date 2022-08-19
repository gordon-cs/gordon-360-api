using Gordon360.Authorization;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Methods;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class ActivitiesController : GordonControllerBase
    {
        private readonly IActivityService _activityService;
        private readonly CCTContext _context;

        public ActivitiesController(CCTContext context)
        {
            _context = context;
            _activityService = new ActivityService(context);
        }

        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public ActionResult<ActivityInfoViewModel> Get()
        {
            var all = _activityService.GetAll();
            return Ok(all);
        }

        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public ActionResult<ActivityInfoViewModel> Get(string id)
        {
            var result = _activityService.Get(id);

            if (result == null)
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
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ActivityInfoViewModel>>> GetActivitiesForSessionAsync(string id)
        {
            var result = await _activityService.GetActivitiesForSessionAsync(id);

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
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<string>>> GetActivityTypesForSessionAsync(string id)
        {
            var result = await _activityService.GetActivityTypesForSessionAsync(id);

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
        [AllowAnonymous]
        public ActionResult<bool> GetActivityStatus(string sessionCode, string id)
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
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<string>>> GetOpenActivitiesAsync()
        {
            var sessionCode = Helpers.GetCurrentSession(_context);

            var activity_codes = _activityService.GetOpenActivities(sessionCode);

            var activities = new List<ActivityInfoViewModel>();

            foreach (var code in activity_codes)
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
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<string>>> GetOpenActivitiesAsync(int id)
        {
            var sessionCode = Helpers.GetCurrentSession(_context);

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
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<string>>> GetClosedActivitiesAsync()
        {
            var sessionCode = Helpers.GetCurrentSession(_context);

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
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<string>>> GetClosedActivitiesAsync(int id)
        {
            var sessionCode = Helpers.GetCurrentSession(_context);

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
        /// <param name="activity_code">The code of the activity to update</param>
        /// <param name="involvement">The updated involvement details</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{activity_code}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.ACTIVITY_INFO)]
        public ActionResult<ACT_INFO> Put(string activity_code, InvolvementUpdateViewModel involvement)
        {
            var result = _activityService.Update(activity_code, involvement);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPut]
        [Route("{id}/session/{sess_cde}/close")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.ACTIVITY_STATUS)]
        public ActionResult CloseSession(string id, string sess_cde)
        {
            _activityService.CloseOutActivityForSession(id, sess_cde);

            return Ok();
        }

        [HttpPut]
        [Route("{id}/session/{sess_cde}/open")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.ACTIVITY_STATUS)]
        public ActionResult OpenSession(string id, string sess_cde)
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
        [Route("{id}/image")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.ACTIVITY_INFO)]
        public async Task<ActionResult> PostImageAsync(string id)
        {
            // Commenting out until we can build and test rewriting this image code
            // https://www.fatalerrors.org/a/comparison-of-multi-file-upload-between-net-and-net-core-web-api-formdata.html
            // https://stackoverflow.com/questions/43674504/multipart-form-data-file-upload-in-asp-net-core-web-api
            // https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-5.0
            return Ok();
            /*
            var uploadsFolder = "/browseable/uploads/" + id + "/";
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new System.Web.Http.HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var existingFile = "";
            var webRoot = _env.WebRootPath;
            var uploadsFolderPath = System.IO.Path.Combine(webRoot, "~" + uploadsFolder);
            if (!System.IO.Directory.Exists(uploadsFolderPath)) {

            }
            if (!System.IO.Directory.Exists(uploadsFolderPath))
            {
                System.IO.Directory.CreateDirectory(uploadsFolderPath);
            }
            else
            {
                try
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(uploadsFolderPath);
                    
                    foreach (FileInfo file in di.GetFiles())
                    {
                        existingFile = file.Name;
                        file.Delete();
                    }
                }
                catch (System.Exception e) {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }

            var provider = new CustomMultipartFormDataStreamProvider(uploadsFolderPath);

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
                return Ok();
            }
            catch (System.Exception e)
            {
                throw new ResourceCreationException() { ExceptionMessage = "There was an error uploading the image. Please contact the maintainers" };
            }
            */
        }

        /// <summary>
        /// Reset the activity Image
        /// </summary>
        /// <param name="id">The activity code</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/image/reset")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.ACTIVITY_INFO)]
        public ActionResult ResetImage(string id)
        {
            _activityService.ResetActivityImage(id);

            return Ok();
        }

        /// <summary>Update an existing activity to be private or not</summary>
        /// <param name="id">The id of the activity</param>
        /// <param name = "p">the boolean value</param>
        /// <remarks>Calls the server to make a call and update the database with the changed information</remarks>
        [HttpPut]
        [Route("{id}/privacy/{p}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.ACTIVITY_INFO)]
        public ActionResult TogglePrivacy(string id, bool p)
        {
            _activityService.TogglePrivacy(id, p);
            return Ok();
        }

    }
}
