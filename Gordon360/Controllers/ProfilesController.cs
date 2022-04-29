using Gordon360.AuthorizationFilters;
using Gordon360.Database.CCT;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Gordon360.Static.Names;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class ProfilesController : GordonControllerBase
    {
        private readonly IProfileService _profileService;
        private readonly IAccountService _accountService;
        private readonly IRoleCheckingService _roleCheckingService;
        private readonly IConfiguration _config;

        public ProfilesController(CCTContext context, IConfiguration config)
        {
            _profileService = new ProfileService(context);
            _accountService = new AccountService(context);
            _roleCheckingService = new RoleCheckingService(context);
            _config = config;
        }

        /// <summary>Get profile info of currently logged in user</summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public ActionResult<ProfileViewModel> Get()
        {
            var authenticatedUserUsername = AuthUtils.GetAuthenticatedUserUsername(User);

            // search username in cached data
            var student = _profileService.GetStudentProfileByUsername(authenticatedUserUsername);
            var faculty = _profileService.GetFacultyStaffProfileByUsername(authenticatedUserUsername);
            var alumni = _profileService.GetAlumniProfileByUsername(authenticatedUserUsername);
            //get profile links
            var customInfo = _profileService.GetCustomUserInfo(authenticatedUserUsername);

            if (student is null && alumni is null && faculty is null)
            {
                return NotFound();
            }

            // merge the person's info if this person is in multiple tables
            var profile = new JObject();
            var personType = "";

            if (student != null)
            {
                profile.Merge(JObject.FromObject(student), new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
                personType += "stu";
            }

            if (alumni != null)
            {
                profile.Merge(JObject.FromObject(alumni), new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
                personType += "alu";
            }

            if (faculty != null)
            {
                profile.Merge(JObject.FromObject(faculty), new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
                personType += "fac";
            }

            if (customInfo != null)
            {
                profile.Merge(JObject.FromObject(customInfo), new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
            }

            profile.Add("PersonType", personType);

            return Ok(profile.ToObject<ProfileViewModel>());
        }

        /// <summary>Get public profile info for a user</summary>
        /// <param name="username">username of the profile info</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{username}")]
        public ActionResult<ProfileViewModel> GetUserProfile(string username)
        {
            var authenticatedUserUsername = AuthUtils.GetAuthenticatedUserUsername(User);
            var viewerType = _roleCheckingService.GetCollegeRole(authenticatedUserUsername);

            //search this person in cached data.
            var _student = _profileService.GetStudentProfileByUsername(username);
            var _faculty = _profileService.GetFacultyStaffProfileByUsername(username);
            var _alumni = _profileService.GetAlumniProfileByUsername(username);
                
            var _customInfo = _profileService.GetCustomUserInfo(username);

            object? student = null;
            object? faculty = null;
            object? alumni = null;
            object? customInfo = null;

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
                    student = _student == null ? null : (PublicStudentProfileViewModel)_student;
                    faculty = _faculty == null ? null : (PublicFacultyStaffProfileViewModel)_faculty;
                    alumni = null;  //student can't see alumini
                    customInfo = _customInfo;
                    break;
                case Position.FACSTAFF:
                    student = _student;
                    faculty = _faculty == null ? null : (PublicFacultyStaffProfileViewModel)_faculty;
                    alumni = _alumni == null ? null : (PublicAlumniProfileViewModel)_alumni;
                    customInfo = _customInfo;
                    break;
            }

            if (student is null && alumni is null && faculty is null)
            {
                return NotFound();
            }

            // merge the person's info if this person is in multiple tables
            var profile = new JObject();
            var personType = "";
            if (student != null)
            {
                profile.Merge(JObject.FromObject(student), new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
                personType += "stu";
            }

            if (alumni != null)
            {
                profile.Merge(JObject.FromObject(alumni), new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
                personType += "alu";
            }

            if (faculty != null)
            {
                profile.Merge(JObject.FromObject(faculty), new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
                personType += "fac";
            }

            if (customInfo != null)
            {
                profile.Merge(JObject.FromObject(customInfo), new JsonMergeSettings
                {
                    MergeArrayHandling = MergeArrayHandling.Union
                });
            }

            profile.Add("PersonType", personType);

            return Ok(profile.ToObject<ProfileViewModel>());
        }

        ///<summary>Get the advisor(s) of a particular student</summary>
        /// <returns>
        /// All advisors of the given student.  For each advisor,
        /// provides first name, last name, and username.
        /// </returns>
        [HttpGet]
        [Route("Advisors/{username}")]
        public async Task<ActionResult<IEnumerable<AdvisorViewModel>>> GetAdvisorsAsync(string username)
        {
            var advisors = await _profileService.GetAdvisorsAsync(username);

            return Ok(advisors);
        }

        /// <summary> Gets the clifton strengths of a particular user </summary>
        /// <param name="username"> The username for which to retrieve info </param>
        /// <returns> Clifton strengths of the given user. </returns>
        [HttpGet]
        [Route("clifton/{username}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.PROFILE)]
        public ActionResult<string[]> GetCliftonStrengths(string username)
        {
            var id = _accountService.GetAccountByUsername(username).GordonID;
            var strengths = _profileService.GetCliftonStrengths(int.Parse(id));

            return Ok(strengths);

        }

        /// <summary> Gets the emergency contact information of a particular user </summary>
        /// <param name="username"> The username for which to retrieve info </param>
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
            var username = AuthUtils.GetAuthenticatedUserUsername(User);

            var result = _profileService.GetMailboxCombination(username);
            return Ok(result);
        }

        /// <summary>Gets the date of birth of the current logged-in user</summary>
        /// <returns></returns>
        [HttpGet]
        [Route("birthdate")]
        public ActionResult<DateTime> GetBirthdate()
        {
            var username = AuthUtils.GetAuthenticatedUserUsername(User);

            var result = _profileService.GetBirthdate(username);
            return Ok(result);
        }
        /* @TODO: fix images
        /// <summary>Get the profile image of currently logged in user</summary>
        /// <returns></returns>
        [HttpGet]
        [Route("image")]
        public async Task<ActionResult<JObject>> GetMyImgAsync()
        {
            var username = AuthUtils.GetAuthenticatedUserUsername(User);
            var photoModel = await _profileService.GetPhotoPathAsync(username);
            JObject result = new JObject();

            if (photoModel == null) //There is no preferred or ID image
            {
                var unapprovedFileName = username + "_" + _accountService.GetAccountByUsername(username).account_id + ".jpg";
                var unapprovedFilePath = _config["DEFAULT_ID_SUBMISSION_PATH"];
                string unapproved_img = await GetProfileImageOrDefault(unapprovedFilePath + unapprovedFileName);
                result.Add("def", unapproved_img);
                return Ok(result);
            }

            string prefImgPath = _config["PREFERRED_IMAGE_PATH"] + photoModel.Pref_Img_Name;

            if (string.IsNullOrEmpty(photoModel.Pref_Img_Name) || !System.IO.File.Exists(prefImgPath)) //check file existence for prefferred image.
            {
                var defaultImgPath = _config["DEFAULT_IMAGE_PATH"] + photoModel.Img_Name;
                result.Add("def", await GetProfileImageOrDefault(defaultImgPath));
                return Ok(result);
            }
            else
            {
                result.Add("pref", await GetProfileImageOrDefault(prefImgPath));
                return Ok(result);
            }
        }

        /// <summary>Get the profile image of the given user</summary>
        /// <returns>The profile image(s) that the authenticated user is allowed to see, if any</returns>
        [HttpGet]
        [Route("image/{username}")]
        public async Task<ActionResult<JObject>> GetImgAsync(string username)
        {
            var authUsername = AuthUtils.GetAuthenticatedUserUsername(User);
            var viewerType = _roleCheckingService.GetCollegeRole(authUsername);
            var photoInfo = await _profileService.GetPhotoPathAsync(username);

            JObject result = new JObject();

            //return default image if no photo info found for this user.
            if (photoInfo == null)
            {
                result.Add("def", await ImageUtils.DownloadImageFromURL(_config["DEFAULT_PROFILE_IMAGE_PATH"]));
                return Ok(result);
            }

            var preferredImagePath = string.IsNullOrEmpty(photoInfo.Pref_Img_Name) ? null : _config["PREFERRED_IMAGE_PATH"] + photoInfo.Pref_Img_Name;
            var defaultImagePath = _config["DEFAULT_IMAGE_PATH"] + photoInfo.Img_Name;
            //security control depends on viewer type.
            switch (viewerType)
            {
                case Position.SUPERADMIN:
                case Position.FACSTAFF:
                case Position.POLICE:
                    if (preferredImagePath is not null && System.IO.File.Exists(preferredImagePath))
                    {
                        result.Add("pref", await GetProfileImageOrDefault(preferredImagePath));
                    }
                    result.Add("def", await GetProfileImageOrDefault(defaultImagePath));
                    return Ok(result);

                case Position.STUDENT:
                    if (_accountService.GetAccountByUsername(username).show_pic == 1)
                    {
                        if (preferredImagePath is not null && System.IO.File.Exists(preferredImagePath))
                        {
                            result.Add("pref", await GetProfileImageOrDefault(preferredImagePath));
                        }
                        else
                        {
                            result.Add("def", await GetProfileImageOrDefault(defaultImagePath));
                        }
                    }
                    else
                    {
                        result.Add("def", await ImageUtils.DownloadImageFromURL(_config["DEFAULT_PROFILE_IMAGE_PATH"]));
                    }
                    return Ok(result);
                default:
                    return Ok();
            }
        }

        ///// <summary>
        ///// Set an image for profile
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("image")]
        //public async Task<ActionResult> PostImageAsync()
        //{
        //    var authenticatedUserUsername = AuthUtils.GetAuthenticatedUserUsername(User);
        //    string root = _config["PREFERRED_IMAGE_PATH"];
        //    var fileName = _accountService.GetAccountByUsername(authenticatedUserUsername).Barcode + ".jpg";
        //    var pathInfo = _profileService.GetPhotoPath(authenticatedUserUsername);
        //    var provider = new CustomMultipartFormDataStreamProvider(root);

        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //    }

        //    try
        //    {
        //        if (pathInfo == null) // can't upload image if there is no record for this user in the database
        //            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "There was an error uploading the image. Please contact the maintainers");

        //        System.IO.DirectoryInfo di = new DirectoryInfo(root);
        //        foreach (FileInfo file in di.GetFiles(fileName))
        //        {
        //            file.Delete();                   //delete old image file if it exists.
        //        }

        //        // Read the form data.
        //        await Request.Content.ReadAsMultipartAsync(provider);

        //        foreach (MultipartFileData file in provider.FileData)
        //        {
        //            var fileContent = provider.Contents.SingleOrDefault();
        //            var oldFileName = fileContent.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);

        //            di = new DirectoryInfo(root);
        //            System.IO.File.Move(di.FullName + oldFileName, di.FullName + fileName); //rename

        //            _profileService.UpdateProfileImage(id, Defaults.DATABASE_IMAGE_PATH, fileName); //update database
        //        }
        //        return Request.CreateResponse(HttpStatusCode.OK);
        //    }
        //    catch (System.Exception e)
        //    {
        //        System.Diagnostics.Debug.WriteLine(e.Message);
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "There was an error uploading the image. Please contact the maintainers");
        //    }
        //}

        ///// <summary>
        ///// Set an IDimage for a user
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("IDimage")]
        //public async Task<ActionResult> PostIDImageAsync()
        //{
        //    var authenticatedUserUsername = AuthUtils.GetUsername(User);
        //    string root = System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_ID_SUBMISSION_PATH"];
        //    var fileName = username + "_" + _accountService.GetAccountByUsername(authenticatedUserUsername).account_id + ".jpg";
        //    var provider = new CustomMultipartFormDataStreamProvider(root);
        //    JObject result = new JObject();

        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //    }


        //    try
        //    {
        //        System.IO.DirectoryInfo di = new DirectoryInfo(root);

        //        //delete old image file if it exists.
        //        foreach (FileInfo file in di.GetFiles(fileName))
        //        {
        //            file.Delete();
        //        }

        //        // Read the form data.
        //        await Request.Content.ReadAsMultipartAsync(provider);

        //        foreach (MultipartFileData fileData in provider.FileData)
        //        {
        //            Debug.WriteLine(fileData.LocalFileName);
        //            di = new DirectoryInfo(root); //di is declared at beginning of try.

        //            FileInfo f1 = new FileInfo(fileData.LocalFileName);
        //            long size1 = f1.Length;


        //            System.IO.File.Move(fileData.LocalFileName, Path.Combine(di.FullName, fileName)); //upload

        //            FileInfo f2 = new FileInfo(Path.Combine(di.FullName, fileName));
        //            long size2 = f2.Length;



        //            if (size1 < 3000 || size2 < 3000)
        //            {
        //                return BadRequest("The ID image was lost in transit. Resubmission should attempt automatically.");
        //            }
        //        }
        //        return Ok();
        //    }
        //    catch (Exception e)
        //    {
        //        return InternalServerError(e);
        //    }
        //}

        /// <summary>
        /// Reset the profile Image
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("image/reset")]
        public ActionResult ResetImage()
        {
            var authenticatedUserUsername = AuthUtils.GetAuthenticatedUserUsername(User);
            string root = _config["PREFERRED_IMAGE_PATH"];
            var fileName = _accountService.GetAccountByUsername(authenticatedUserUsername).Barcode + ".jpg";
            try
            {
                DirectoryInfo di = new DirectoryInfo(root);
                foreach (FileInfo file in di.GetFiles(fileName))
                {
                    file.Delete();                  //delete old image file if it exists.
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            _profileService.UpdateProfileImageAsync(authenticatedUserUsername, null, null);  //update database
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
        public async Task<ActionResult> UpdateLinkAsync(string type, CUSTOM_PROFILE path)
        {
            var authenticatedUserUsername = AuthUtils.GetAuthenticatedUserUsername(User);

            await _profileService.UpdateProfileLinkAsync(authenticatedUserUsername, type, path);

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
        public async Task<ActionResult> UpdateMobilePrivacyAsync(string value)
        {
            var authenticatedUserUsername = AuthUtils.GetAuthenticatedUserUsername(User);
            await _profileService.UpdateMobilePrivacyAsync(authenticatedUserUsername, value);

            return Ok();
        }

        /// <summary>
        /// Update privacy of profile image
        /// </summary>
        /// <param name="value">Y or N</param>
        /// <returns></returns>
        [HttpPut]
        [Route("image_privacy/{value}")]
        public async Task<ActionResult> UpdateImagePrivacyAsync(string value)
        {
            var authenticatedUserUsername = AuthUtils.GetAuthenticatedUserUsername(User);
            await _profileService.UpdateImagePrivacyAsync(authenticatedUserUsername, value);

            return Ok();
        }

        /// <summary>
        /// Gets the profile image at the given path or, if that file does not exist, the 360 default profile image
        /// </summary>
        /// <remarks>
        /// Note that the 360 default profile image is different from a user's default image.
        /// A given user's default image is simply their approved ID photo.
        /// The 360 default profile image, on the other hand, is a stock image of Scottie Lion.
        /// Hence, the 360 default profile image is only used when no other image exists (or should be displayed) for a user.
        /// </remarks>
        /// <param name="imagePath">Path to the profile image to load</param>
        /// <returns></returns>
        private async Task<string> GetProfileImageOrDefault(string imagePath)
        {
            try
            {
                // User's profile images (both preferred and default) are stored in the GO site's filesystem.
                // Hence, we access them via the network file share, the same way we would access a local file
                return ImageUtils.RetrieveImageFromPath(imagePath);
            }
            catch (FileNotFoundException)
            {
                // The 360 default profile image path is a URL, so we have to download it over an HTTP connection
                return await ImageUtils.DownloadImageFromURL(_config["DEFAULT_PROFILE_IMAGE_PATH"]);
            }
        }
    }
}
