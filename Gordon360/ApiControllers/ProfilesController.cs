using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using System.Threading.Tasks;
using Gordon360.Models;
using System.Web;
using System.Diagnostics;
using Gordon360.Providers;
using System.IO;
using Gordon360.Static.Methods;
using Gordon360.Models.ViewModels;


namespace Gordon360.Controllers.Api
{

    [RoutePrefix("api/profiles")]
    [CustomExceptionFilter]
    [Authorize]
    public class ProfilesController : ApiController
    {
        private IProfileService _profileService;

        public ProfilesController()
        {
            var _unitOfWork = new UnitOfWork();
            _profileService = new ProfileService(_unitOfWork);
        }

        public ProfilesController(IProfileService profileService)
        {
            _profileService = profileService;
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
            var all = _profileService.GetAll();
            return Ok(all);
        }

        /// <summary>Get the info of currently logged in user</summary>
        /// <param name="username">An identifier for the person</param>
        /// <returns></returns>
        // GET api/<controller>/5
        [HttpGet]
        [Route("{username}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.PROFILE)]    //make sure the requested person is the same as the logged in user
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

            // search username in three tables
            var student = _profileService.GetStudentProfileByUsername(username);
            var faculty = _profileService.GetFacultyStaffProfileByUsername(username);
            var alumni = _profileService.GetAlumniProfileByUsername(username);
            var customInfo = _profileService.GetCustomUserInfo(username);

            // merge the person's info if this person is in multiple tables and return result 
            if (student != null)
            {
                if (faculty != null)
                {
                    if (alumni != null)
                    {
                        JObject stualufac = JObject.FromObject(student);                                 //convert into JSON object in order to use JSON.NET library 
                        stualufac.Merge(JObject.FromObject(alumni), new JsonMergeSettings                // user Merge function to merge two json object
                        {
                            MergeArrayHandling = MergeArrayHandling.Union
                        });
                        stualufac.Merge(JObject.FromObject(faculty), new JsonMergeSettings
                        {
                            MergeArrayHandling = MergeArrayHandling.Union
                        });
                        stualufac.Merge(JObject.FromObject(customInfo), new JsonMergeSettings
                        {
                            MergeArrayHandling = MergeArrayHandling.Union
                        });
                        stualufac.Add("PersonType", "stualufac");                                         // assign a type to the json object 
                        return Ok(stualufac);
                    }
                    JObject stufac = JObject.FromObject(student);
                    stufac.Merge(JObject.FromObject(faculty), new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                    stufac.Merge(JObject.FromObject(customInfo), new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                    stufac.Add("PersonType", "stufac");
                    return Ok(stufac);
                }
                else if (alumni != null)
                {
                    JObject stualu = JObject.FromObject(student);
                    stualu.Merge(JObject.FromObject(alumni), new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                    stualu.Merge(JObject.FromObject(customInfo), new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                    stualu.Add("PersonType", "stualu");
                    return Ok(stualu);
                }
                JObject stu = JObject.FromObject(student);
                stu.Merge(JObject.FromObject(customInfo), new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
                stu.Add("PersonType", "stu");
                return Ok(stu);
            }
            else if (faculty != null)
            {
                if (alumni != null)
                {
                    JObject alufac = JObject.FromObject(alumni);
                    alufac.Merge(JObject.FromObject(faculty), new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                    alufac.Merge(JObject.FromObject(customInfo), new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Union
                    });
                    alufac.Add("PersonType", "alufac");
                    return Ok(alufac);
                }
                JObject fac = JObject.FromObject(faculty);
                fac.Merge(JObject.FromObject(customInfo), new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
                fac.Add("PersonType", "fac");
                return Ok(fac);
            }
            else if (alumni != null)
            {
                JObject alu = JObject.FromObject(alumni);
                alu.Merge(JObject.FromObject(customInfo), new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
                alu.Add("PersonType", "alu");
                return Ok(alu);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Set an image for profile
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{username}/image")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.PROFILE)]
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
                    _profileService.UpdateProfileImage(username, imagePath);
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
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.PROFILE)]
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

            _profileService.ResetProfileImage(username);

            return Ok();

        }

        /// <summary>
        /// Update the profile social media links
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="type">The type of social media</param>
        /// <param name="path">The path of the links</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{username}/{type}/{path}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.PROFILE)]
        public IHttpActionResult UpdateLink(string username, string type, string path)
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

            _profileService.UpdateProfileLink(username, type, path);

            return Ok();

        }
        /// <summary>
        /// Update privacy of mobile phone number
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="p">private or not(1 or 0)</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{username}/mobile_privacy/{p}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.PROFILE)]
        public IHttpActionResult UpdateMobilePrivacy(string username, bool p)
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

            _profileService.UpdateMobilePrivacy(username, p);

            return Ok();

        }
    }
}
