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
    [RoutePrefix("api/profile")]
    [CustomExceptionFilter]
    [Authorize]

    public class ProfileImageController : ApiController
    {
        private IProfileImageService _profileImageService;

        public ProfileImageController()
        {
            var _unitOfWork = new UnitOfWork();
            _profileImageService = new ProfileImageService(_unitOfWork);
        }

        public ProfileImageController(IProfileImageService profileService)
        {
            _profileImageService = profileService;
        }

        /// <summary>
        /// Get all available users
        /// </summary>
        /// <returns>All the users in the databse</returns>
        /// <remarks></remarks>
        // GET api/<controller>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var all = _profileImageService.GetAll();
            return Ok(all);
        }

        /// <summary>Get a single profile based upon the string username entered in the URL</summary>
        /// <param name="username">An identifier for a single person's profile</param>
        /// <returns></returns>
        /// <remarks>Get a single profile from the database</remarks>
        // GET api/<controller>/5
        [HttpGet]
        [Route("{username}")]
        public IHttpActionResult Get(string username)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(username))
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
            var result = _profileImageService.Get(username);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Set an image for profile
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{username}/image")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.PROFILE_IMAGE)]
        public async Task<HttpResponseMessage> PostImage(string username)
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
            var profileFolder = "/browseable/profile/" + username + "/";
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var existingFile = "";

            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~" + profileFolder)))
            {
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~" + profileFolder));
            }
            else
            {
                try
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath("~" + profileFolder));

                    foreach (FileInfo file in di.GetFiles())
                    {
                        existingFile = file.Name;
                        file.Delete();
                    }
                }
                catch (System.Exception e) { }
            }

            string root = HttpContext.Current.Server.MapPath("~" + profileFolder);
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
                        System.IO.DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Server.MapPath("~" + profileFolder));
                        System.IO.File.Move(di.FullName + oldFileName, di.FullName + fileName);
                    }

                    var uploadPath = profileFolder + fileName;
                    var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
                    var imagePath = baseUrl + uploadPath;
                    _profileImageService.UpdateProfileImage(username, imagePath);
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "There was an error uploading the image. Please contact the maintainers");
            }
        }

        /// <summary>
        /// Reset the profile Image
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{username}/image/reset")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.PROFILE_IMAGE)]
        public IHttpActionResult ResetImage(string username)
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

            _profileImageService.ResetProfileImage(username);

            return Ok();

        }
    }
}


