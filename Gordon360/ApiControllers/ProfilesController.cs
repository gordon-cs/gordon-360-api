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
using System.Security.Claims;
using System.Net.Http.Headers;

namespace Gordon360.Controllers.Api
{

    [RoutePrefix("api/profiles")]
    [CustomExceptionFilter]
    [Authorize]
    public class ProfilesController : ApiController
    {
        private IProfileService _profileService;
        private IAccountService _accountService;
        private IRoleCheckingService _roleCheckingService;

        public ProfilesController()
        {
            var _unitOfWork = new UnitOfWork();
            _profileService = new ProfileService(_unitOfWork);
            _accountService = new AccountService(_unitOfWork);
            _roleCheckingService = new RoleCheckingService(_unitOfWork);
        }

        public ProfilesController(IProfileService profileService)
        {
            _profileService = profileService;
        }


        /// <summary>Get the info of currently logged in user</summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
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
        /// <summary>Get public profile info for a user</summary>
        /// <param name="username">username of the profile info</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{username}")]
       // [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.PROFILE)]    
        public IHttpActionResult GetUserProfile(string username)
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
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var viewerName = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var viewerType = _roleCheckingService.getViewerRole(viewerName);

            //search this person in three tables.
            var _student = _profileService.GetStudentProfileByUsername(username);
            var _faculty = _profileService.GetFacultyStaffProfileByUsername(username);
            var _alumni = _profileService.GetAlumniProfileByUsername(username);
            var _customInfo = _profileService.GetCustomUserInfo(username);

            object student = null;
            object faculty = null;
            object alumni = null;
            object customInfo = null;

            switch (viewerType)
                {
                    case Position.GOD:
                        student = _student;
                        faculty = _faculty;
                        alumni = _alumni;
                        customInfo = _customInfo;
                        break;
                    case Position.STUDENT:
                        student = (_student == null) ? null : (PublicStudentProfileViewModel)_student;
                        faculty = (_faculty == null) ? null : (PublicFacultyStaffProfileViewModel)_faculty;
                        alumni = (_alumni == null) ? null : (PublicAlumniProfileViewModel)_alumni;
                        customInfo = _customInfo;
                        break;
                    case Position.FACSTAFF:
                        student = _student;
                        faculty = (_faculty == null) ? null : (PublicFacultyStaffProfileViewModel)_faculty;
                        alumni = (_alumni == null) ? null : (PublicAlumniProfileViewModel)_alumni;
                        customInfo = _customInfo;
                        break;
            }


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
                return Ok("Not Found");
            }
        }
        [HttpGet]
        [Route("role/{username}")]
        public IHttpActionResult getRole(string username)
        {
            var role  = _roleCheckingService.getViewerRole(username);
            return Ok(role);
        }

        /// <summary>Get the profile image of currently logged in user</summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Image")]
        public IHttpActionResult getMyImg()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var id = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "id").Value;
            var viewerName = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var viewerType = _roleCheckingService.getViewerRole(viewerName);
            var photoInfo = _profileService.GetPhotoPath(id);
            string pref_img = "";
            string default_img = "";
            var fileName = "";
            var filePath = Defaults.DEFAULT_PREF_IMAGE_PATH;
            byte[] imageBytes;
            JObject result = new JObject();

            if (photoInfo == null)
            {
                var webClient = new WebClient();
                imageBytes = webClient.DownloadData(Defaults.DEFAULT_PROFILE_IMAGE_PATH);
                default_img = Convert.ToBase64String(imageBytes);
                result.Add("def", default_img);
                return Ok(result);
            }
            else
                fileName = photoInfo.Pref_Img_Name;

            if (string.IsNullOrEmpty(fileName) || !File.Exists(filePath + fileName))
            {
                filePath = Defaults.DEFAULT_IMAGE_PATH;
                fileName = photoInfo.Img_Name;
                imageBytes = File.ReadAllBytes(filePath + fileName);
                default_img = Convert.ToBase64String(imageBytes);
                result.Add("def", default_img);
                return Ok(result);
            }
            else
            {
                imageBytes = File.ReadAllBytes(filePath + fileName);
                pref_img = Convert.ToBase64String(imageBytes);
                result.Add("pref", pref_img);
                return Ok(result);  //return image as a base64 string
            }
        }
        /// <summary>Get the profile image of currently logged in user</summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Image/{username}")]
        public IHttpActionResult getImg(string username)
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var viewerName = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var viewerType = _roleCheckingService.getViewerRole(viewerName);
            var id = _accountService.GetAccountByUsername(username).GordonID;
            var photoInfo = _profileService.GetPhotoPath(id);

            var filePath = "";
            var fileName = "";
            byte[] pref_image;
            string pref_img = ""; 
            byte[] default_image;
            string default_img = "";
            JObject result = new JObject();

            if (photoInfo == null)
            {
                var webClient = new WebClient();
                default_image = webClient.DownloadData(Defaults.DEFAULT_PROFILE_IMAGE_PATH);
                default_img = Convert.ToBase64String(default_image);
                result.Add("def", default_img);
                result.Add("pref", default_img);
                return Ok(result);
            }

            switch (viewerType)
            {
                case Position.GOD:
                    filePath = Defaults.DEFAULT_PREF_IMAGE_PATH;
                    fileName = photoInfo.Pref_Img_Name;
                    if (!string.IsNullOrEmpty(fileName) && File.Exists(filePath + fileName))
                    {
                        pref_image = File.ReadAllBytes(filePath + fileName);
                        pref_img = Convert.ToBase64String(pref_image);
                    }
                    filePath = Defaults.DEFAULT_IMAGE_PATH;
                    fileName = photoInfo.Img_Name;
                    default_image = File.ReadAllBytes(filePath + fileName);
                    default_img = Convert.ToBase64String(default_image);
                    result.Add("def", default_img);
                    result.Add("pref", pref_img);
                    return Ok(result);

                case Position.STUDENT:
                    if (_accountService.GetAccountByUsername(username).show_pic == 1)
                    {
                        filePath = Defaults.DEFAULT_PREF_IMAGE_PATH;
                        fileName = photoInfo.Pref_Img_Name;
                        if (string.IsNullOrEmpty(fileName) || !File.Exists(filePath + fileName))
                        {
                            filePath = Defaults.DEFAULT_IMAGE_PATH;
                            fileName = photoInfo.Img_Name;
                            default_image = File.ReadAllBytes(filePath + fileName);
                            default_img = Convert.ToBase64String(default_image);
                            result.Add("def", default_img);
                            return Ok(result);
                        }
                        pref_image = File.ReadAllBytes(filePath + fileName);
                        pref_img = Convert.ToBase64String(pref_image);
                        result.Add("pref", pref_img);
                    }
                    else
                    {
                        var webClient = new WebClient();
                        default_image = webClient.DownloadData(Defaults.DEFAULT_PROFILE_IMAGE_PATH);
                        default_img = Convert.ToBase64String(default_image);
                        result.Add("def", default_img);
                        return Ok(result);
                    }
                    return Ok(result);
                case Position.FACSTAFF:
                    filePath = Defaults.DEFAULT_PREF_IMAGE_PATH;
                    fileName = photoInfo.Pref_Img_Name;
                    if (!string.IsNullOrEmpty(fileName) && File.Exists(filePath + fileName))
                    {
                        pref_image = File.ReadAllBytes(filePath + fileName);
                        pref_img = Convert.ToBase64String(pref_image);
                    }
                    filePath = Defaults.DEFAULT_IMAGE_PATH;
                    fileName = photoInfo.Img_Name;
                    default_image = File.ReadAllBytes(filePath + fileName);
                    default_img = Convert.ToBase64String(default_image);
                    result.Add("def", default_img);
                    result.Add("pref", pref_img);
                    return Ok(result);
                default:
                    return Ok();
            }
        }

        /// <summary>
        /// Set an image for profile
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("image")]
        public async Task<HttpResponseMessage> PostImage()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "id").Value;
            string root = Defaults.DEFAULT_PREF_IMAGE_PATH;
            var fileName = _accountService.GetAccountByUsername(username).Barcode + ".jpg";
            var pathInfo = _profileService.GetPhotoPath(id);
            var provider = new CustomMultipartFormDataStreamProvider(root);

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            try
            {
                if (pathInfo == null)
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "There was an error uploading the image. Please contact the maintainers");

                System.IO.DirectoryInfo di = new DirectoryInfo(root);
                foreach (FileInfo file in di.GetFiles(fileName))
                {
                        file.Delete();                   //delete old image file if it exists.
                }

                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    var fileContent = provider.Contents.SingleOrDefault();
                    var oldFileName = fileContent.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);

                    di = new DirectoryInfo(root);
                    System.IO.File.Move(di.FullName + oldFileName, di.FullName + fileName); //rename

                    _profileService.UpdateProfileImage(id, root, fileName); //update database
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
        /// <returns></returns>
        [HttpPost]
        [Route("image/reset")]
        public IHttpActionResult ResetImage()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "id").Value; ;
            string root = Defaults.DEFAULT_PREF_IMAGE_PATH;
            var fileName = _accountService.GetAccountByUsername(username).Barcode + ".jpg";
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(root);
                foreach (FileInfo file in di.GetFiles(fileName))
                {
                    file.Delete();                  //delete old image file if it exists.
                }
            }
            catch (System.Exception e) { }
           _profileService.UpdateProfileImage(id, null, null);  //update database
            return Ok();
        }


        /// <summary>
        /// Update the profile social media links
        /// </summary>
        /// <param name="type">The type of social media</param>
        /// <param name="path">The path of the links</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{type}")]
        public IHttpActionResult UpdateLink(string type, CUSTOM_PROFILE path)
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
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            _profileService.UpdateProfileLink(username, type, path);

            return Ok();

        }
        /// <summary>
        /// Update privacy of mobile phone number
        /// </summary>
        /// <param name="value">Y or N</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mobile_privacy/{value}")]
        public IHttpActionResult UpdateMobilePrivacy(string value)
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

            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var id = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "id").Value;
            _profileService.UpdateMobilePrivacy(id, value);

            return Ok();

        }
        /// <summary>
        /// Update privacy of profile image
        /// </summary>
        /// <param name="value">Y or N</param>
        /// <returns></returns>
        [HttpPut]
        [Route("image_privacy/{value}")]
        public IHttpActionResult UpdateImagePrivacy(string value)
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

            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var id = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "id").Value;
            _profileService.UpdateImagePrivacy(id, value);

            return Ok();
        }
    }
}
