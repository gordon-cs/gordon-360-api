using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Gordon360.Static.Names;
using System.Threading.Tasks;
using Gordon360.Models;
using System.Diagnostics;
using Gordon360.Providers;
using System.IO;
using Gordon360.Models.ViewModels;
using System.Security.Claims;
using Gordon360.AuthorizationFilters;

namespace Gordon360.Controllers.Api
{

    [RoutePrefix("api/profiles")]
    [CustomExceptionFilter]
    [Authorize]
    public class ProfilesController : ApiController
    {
        //declare services we are going to use.
        private IProfileService _profileService;
        private IAccountService _accountService;
        private IRoleCheckingService _roleCheckingService;

        public ProfilesController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _profileService = new ProfileService(_unitOfWork);
            _accountService = new AccountService(_unitOfWork);
            _roleCheckingService = new RoleCheckingService(_unitOfWork);
        }

        public ProfilesController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        /// <summary>Get profile info of currently logged in user</summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get()
        {
            //get token data from context, username is the username of current logged in person
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            // search username in cached data
            var student = _profileService.GetStudentProfileByUsername(username);
            var faculty = _profileService.GetFacultyStaffProfileByUsername(username);
            var alumni = _profileService.GetAlumniProfileByUsername(username);

            //get profile links
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
            //get token data from context, username is the username of current logged in person
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var viewerName = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var viewerType = _roleCheckingService.getCollegeRole(viewerName);

            //search this person in cached data.
            var _student = _profileService.GetStudentProfileByUsername(username);
            var _faculty = _profileService.GetFacultyStaffProfileByUsername(username);
            var _alumni = _profileService.GetAlumniProfileByUsername(username);
                
            var _customInfo = _profileService.GetCustomUserInfo(username);

            object student = null;
            object faculty = null;
            object alumni = null;
            object customInfo = null;

            //security control depends on viewer type. apply different views to different viewers.
            switch (viewerType)
            {
                case Position.SUPERADMIN:
                    student = _student;
                    faculty = _faculty;
                    alumni = _alumni;
                    customInfo = _customInfo;
                    break;
                case Position.POLICE:
                    student = _student;
                    faculty = _faculty;
                    alumni = _alumni;
                    customInfo = _customInfo;
                    break;
                case Position.STUDENT:
                    student = (_student == null) ? null : (PublicStudentProfileViewModel)_student;         //implicit conversion
                    faculty = (_faculty == null) ? null : (PublicFacultyStaffProfileViewModel)_faculty;
                    alumni = null;  //student can't see alumini
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
                return NotFound();
            }
        }

        ///<summary>Get the advisor(s) of a particular student</summary>
        /// <returns>
        /// All advisors of the given student.  For each advisor,
        /// provides first name, last name, and username.
        /// </returns>
        [HttpGet]
        [Route("Advisors/{username}")]
        public IHttpActionResult GetAdvisors(string username)
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;

            var _student = new StudentProfileViewModel();
            var id = "";
            try
            {
                _student = _profileService.GetStudentProfileByUsername(username);
                id = _accountService.GetAccountByUsername(username).GordonID;
            }
            catch (ResourceNotFoundException)
            {
                NotFound();
            }

            var advisors = _profileService.GetAdvisors((id == username ? username : id));

            return Ok(advisors);

        }

        /// <summary> Gets the clifton strengths of a particular user </summary>
        /// <param name="username"> The username for which to retrieve info </param>
        /// <returns> Clifton strengths of the given user. </returns>
        [HttpGet]
        [Route("clifton/{username}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.PROFILE)]
        public IHttpActionResult GetCliftonStrengths(string username)
        {
            var id = _accountService.GetAccountByUsername(username).GordonID;
            var strengths = _profileService.GetCliftonStrengths(int.Parse(id));

            return Ok(strengths);

        }

        /// <summary> Gets the emergency contact information of a particular user </summary>
        /// <param username="AD_Username"> The username for which to retrieve info </param>
        /// <returns> Emergency contact information of the given user. </returns>
        [HttpGet]
        [Route("emergency-contact/{username}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.EMERGENCY_CONTACT)]
        public IHttpActionResult GetEmergencyContact(string username)
        {
            try
            {
                var emrg = _profileService.GetEmergencyContact(username);
                return Ok(emrg);
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "There was an error getting the emergency contact.");
                return NotFound();
            }
            
            
        }
        

        /// <summary>Gets the mailbox information of currently logged in user</summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mailbox-combination")]
        public IHttpActionResult GetMailInfo()
        {
            //get token data from context, username is the username of current logged in person
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;

            var result = _profileService.GetMailboxCombination(username);
            return Ok(result);
        }

        /// <summary>Get the profile image of currently logged in user</summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Image")]
        public IHttpActionResult GetMyImg()
        {
            var authenticatedUser = ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var id = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "id").Value;

            var photoModel = _profileService.GetPhotoPath(id);

            string pref_img = "";
            string default_img = "";

            var fileName = "";
            var filePath = System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_PREF_IMAGE_PATH"];

            var unapprovedFileName = username + "_" + _accountService.GetAccountByUsername(username).account_id + ".jpg";
            var unapprovedFilePath = System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_ID_SUBMISSION_PATH"];

            byte[] imageBytes;
            JObject result = new JObject();

            if (photoModel == null) //There is no preferred or ID image
            {
                if (File.Exists(unapprovedFilePath + unapprovedFileName))
                {

                    var webClient = new WebClient();
                    imageBytes = webClient.DownloadData(unapprovedFilePath + unapprovedFileName);

                    string unapproved_img = Convert.ToBase64String(imageBytes);
                    result.Add("def", unapproved_img);
                    return Ok(result);
                }
                else
                {
                    var webClient = new WebClient();
                    imageBytes = webClient.DownloadData(System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_PROFILE_IMAGE_PATH"]);
                    default_img = Convert.ToBase64String(imageBytes);
                    result.Add("def", default_img);
                    return Ok(result);
                }
            }


            fileName = photoModel.Pref_Img_Name;

            if (string.IsNullOrEmpty(fileName) || !File.Exists(filePath + fileName)) //check file existence for prefferred image.
            {
                filePath = System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_IMAGE_PATH"];
                fileName = photoModel.Img_Name;
                try
                {
                    imageBytes = File.ReadAllBytes(filePath + fileName);
                }
                catch (FileNotFoundException e)
                {
                    Debug.WriteLine(e.Message);
                    var webClient = new WebClient();
                    imageBytes = webClient.DownloadData(System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_PROFILE_IMAGE_PATH"]);
                }
                default_img = Convert.ToBase64String(imageBytes);
                result.Add("def", default_img);
                return Ok(result);
            }
            else
            {
                try
                {
                    imageBytes = File.ReadAllBytes(filePath + fileName);
                }
                catch (FileNotFoundException e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    var webClient = new WebClient();
                    imageBytes = webClient.DownloadData(System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_PROFILE_IMAGE_PATH"]);
                }
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
            var viewerType = _roleCheckingService.getCollegeRole(viewerName);
            var id = "";
            var photoInfo = new PhotoPathViewModel();
            try
            {
                id = _accountService.GetAccountByUsername(username).GordonID;
                photoInfo = _profileService.GetPhotoPath(id);
            }
            catch (ResourceNotFoundException)
            {
                photoInfo = null;
            }

            var filePath = "";
            var fileName = "";
            byte[] pref_image;
            string pref_img = "";
            byte[] default_image;
            string default_img = "";
            JObject result = new JObject();

            //return default image if no photo info found for this user.
            if (photoInfo == null)
            {
                var webClient = new WebClient();
                default_image = webClient.DownloadData(System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_PROFILE_IMAGE_PATH"]);
                default_img = Convert.ToBase64String(default_image);
                result.Add("def", default_img);
                return Ok(result);
            }

            //security control depends on viewer type. return both photos for super admin and gordon police.
            switch (viewerType)
            {
                case Position.SUPERADMIN:
                    filePath = System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_PREF_IMAGE_PATH"];
                    fileName = photoInfo.Pref_Img_Name;
                    if (!string.IsNullOrEmpty(fileName) && File.Exists(filePath + fileName))
                    {
                        try
                        {
                            pref_image = File.ReadAllBytes(filePath + fileName);
                        }
                        catch (FileNotFoundException e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                            var webClient = new WebClient();
                            pref_image = webClient.DownloadData(System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_PROFILE_IMAGE_PATH"]);
                        }
                        pref_img = Convert.ToBase64String(pref_image);
                    }
                    filePath = System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_IMAGE_PATH"];
                    fileName = photoInfo.Img_Name;
                    try
                    {
                        default_image = File.ReadAllBytes(filePath + fileName);
                    }
                    catch (FileNotFoundException e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                        var webClient = new WebClient();
                        default_image = webClient.DownloadData(System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_PROFILE_IMAGE_PATH"]);
                    }
                    default_img = Convert.ToBase64String(default_image);
                    result.Add("def", default_img);
                    result.Add("pref", pref_img);
                    return Ok(result);
                case Position.POLICE:
                    filePath = System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_PREF_IMAGE_PATH"];
                    fileName = photoInfo.Pref_Img_Name;
                    if (!string.IsNullOrEmpty(fileName) && File.Exists(filePath + fileName))
                    {
                        try
                        {
                            pref_image = File.ReadAllBytes(filePath + fileName);
                        }
                        catch (FileNotFoundException e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                            var webClient = new WebClient();
                            pref_image = webClient.DownloadData(System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_PROFILE_IMAGE_PATH"]);
                        }
                        pref_img = Convert.ToBase64String(pref_image);
                    }
                    filePath = System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_IMAGE_PATH"];
                    fileName = photoInfo.Img_Name;
                    try
                    {
                        default_image = File.ReadAllBytes(filePath + fileName);
                    }
                    catch (FileNotFoundException e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                        var webClient = new WebClient();
                        default_image = webClient.DownloadData(System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_PROFILE_IMAGE_PATH"]);
                    }
                    default_img = Convert.ToBase64String(default_image);
                    result.Add("def", default_img);
                    result.Add("pref", pref_img);
                    return Ok(result);
                case Position.STUDENT:
                    if (_accountService.GetAccountByUsername(username).show_pic == 1)                  //check privacy setting of this user.
                    {
                        filePath = System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_PREF_IMAGE_PATH"];
                        fileName = photoInfo.Pref_Img_Name;
                        if (string.IsNullOrEmpty(fileName) || !File.Exists(filePath + fileName))
                        {
                            filePath = System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_IMAGE_PATH"];
                            fileName = photoInfo.Img_Name;
                            try
                            {
                                default_image = File.ReadAllBytes(filePath + fileName);
                            }
                            catch (FileNotFoundException e)
                            {
                                System.Diagnostics.Debug.WriteLine(e.Message);
                                var webClient = new WebClient();
                                default_image = webClient.DownloadData(System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_PROFILE_IMAGE_PATH"]);
                            }
                            default_img = Convert.ToBase64String(default_image);
                            result.Add("def", default_img);
                            return Ok(result);
                        }
                        try
                        {
                            pref_image = File.ReadAllBytes(filePath + fileName);
                        }
                        catch (FileNotFoundException e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                            var webClient = new WebClient();
                            pref_image = webClient.DownloadData(System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_PROFILE_IMAGE_PATH"]);
                        }
                        pref_img = Convert.ToBase64String(pref_image);
                        result.Add("pref", pref_img);
                    }
                    else
                    {
                        var webClient = new WebClient();
                        default_image = webClient.DownloadData(System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_PROFILE_IMAGE_PATH"]);
                        default_img = Convert.ToBase64String(default_image);
                        result.Add("def", default_img);
                        return Ok(result);
                    }
                    return Ok(result);
                case Position.FACSTAFF:
                    filePath = System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_PREF_IMAGE_PATH"];
                    fileName = photoInfo.Pref_Img_Name;
                    if (!string.IsNullOrEmpty(fileName) && File.Exists(filePath + fileName))
                    {
                        try
                        {
                            pref_image = File.ReadAllBytes(filePath + fileName);
                        }
                        catch (FileNotFoundException e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message);
                            var webClient = new WebClient();
                            pref_image = webClient.DownloadData(System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_PROFILE_IMAGE_PATH"]);
                        }
                        pref_img = Convert.ToBase64String(pref_image);
                    }
                    filePath = System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_IMAGE_PATH"];
                    fileName = photoInfo.Img_Name;
                    try
                    {
                        default_image = File.ReadAllBytes(filePath + fileName);
                    }
                    catch (FileNotFoundException e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                        var webClient = new WebClient();
                        default_image = webClient.DownloadData(System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_PROFILE_IMAGE_PATH"]);
                    }
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
            string root = System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_PREF_IMAGE_PATH"];
            var fileName = _accountService.GetAccountByUsername(username).Barcode + ".jpg";
            var pathInfo = _profileService.GetPhotoPath(id);
            var provider = new CustomMultipartFormDataStreamProvider(root);

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            try
            {
                if (pathInfo == null) // can't upload image if there is no record for this user in the database
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "There was an error uploading the image. Please contact the maintainers");

                System.IO.DirectoryInfo di = new DirectoryInfo(root);
                foreach (FileInfo file in di.GetFiles(fileName))
                {
                    file.Delete();                   //delete old image file if it exists.
                }

                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (MultipartFileData file in provider.FileData)
                {
                    var fileContent = provider.Contents.SingleOrDefault();
                    var oldFileName = fileContent.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);

                    di = new DirectoryInfo(root);
                    System.IO.File.Move(di.FullName + oldFileName, di.FullName + fileName); //rename

                    _profileService.UpdateProfileImage(id, Defaults.DATABASE_IMAGE_PATH, fileName); //update database
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "There was an error uploading the image. Please contact the maintainers");
            }
        }

        /// <summary>
        /// Set an IDimage for a user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("IDimage")]
        public async Task<IHttpActionResult> PostIDImage()
        {
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var username = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            string root = System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_ID_SUBMISSION_PATH"];
            var fileName = username + "_" + _accountService.GetAccountByUsername(username).account_id + ".jpg";
            var provider = new CustomMultipartFormDataStreamProvider(root);
            JObject result = new JObject();

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }


            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(root);

                //delete old image file if it exists.
                foreach (FileInfo file in di.GetFiles(fileName))
                {
                    file.Delete();
                }

                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (MultipartFileData fileData in provider.FileData)
                {
                    Debug.WriteLine(fileData.LocalFileName);
                    di = new DirectoryInfo(root); //di is declared at beginning of try.

                    FileInfo f1 = new FileInfo(fileData.LocalFileName);
                    long size1 = f1.Length;


                    System.IO.File.Move(fileData.LocalFileName, Path.Combine(di.FullName, fileName)); //upload

                    FileInfo f2 = new FileInfo(Path.Combine(di.FullName, fileName));
                    long size2 = f2.Length;



                    if (size1 < 3000 || size2 < 3000)
                    {
                        return BadRequest("The ID image was lost in transit. Resubmission should attempt automatically.");
                    }
                }
                return Ok();
            }
            catch (System.Exception e)
            {
                return InternalServerError(e);
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
            string root = System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_PREF_IMAGE_PATH"];
            var fileName = _accountService.GetAccountByUsername(username).Barcode + ".jpg";
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(root);
                foreach (FileInfo file in di.GetFiles(fileName))
                {
                    file.Delete();                  //delete old image file if it exists.
                }
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
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
        /// Update mobile phone number
        /// </summary>
        /// <param name="value">phoneNumber</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mobile_phone_number/{value}")]
        public IHttpActionResult UpdateMobilePhoneNumber(string value)
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
            StudentProfileViewModel profile = _profileService.GetStudentProfileByUsername(username);
            profile.MobilePhone = value;
            StudentProfileViewModel result = _profileService.UpdateMobilePhoneNumber(profile);
            
            return Ok(result);

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
