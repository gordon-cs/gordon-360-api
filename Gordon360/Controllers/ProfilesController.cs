using Gordon360.Authorization;
using Gordon360.Enums;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Extensions.System;
using Gordon360.Static.Names;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Gordon360.Models.CCT.Context;

namespace Gordon360.Controllers;

[Route("api/[controller]")]
public class ProfilesController(IProfileService profileService,
                                IAccountService accountService,
                                IMembershipService membershipService,
                                IConfiguration config,
                                CCTContext context) : GordonControllerBase
{

    /// <summary>Get profile info of currently logged in user</summary>
    /// <returns></returns>
    [HttpGet]
    [Route("")]
    public ActionResult<ProfileViewModel?> Get()
    {
        var authenticatedUserUsername = AuthUtils.GetUsername(User);

        var student = profileService.GetStudentProfileByUsername(authenticatedUserUsername);
        var faculty = profileService.GetFacultyStaffProfileByUsername(authenticatedUserUsername);
        var alumni = profileService.GetAlumniProfileByUsername(authenticatedUserUsername);
        var customInfo = profileService.GetCustomUserInfo(authenticatedUserUsername);

        if (student is null && alumni is null && faculty is null)
        {
            return Ok(null);
        }

        //var profile = profileService.ComposeProfile(student, alumni, faculty, customInfo);
        var profile = (CombinedProfileViewModel) profileService.ComposeProfile(student, alumni, faculty, customInfo);

        return Ok(profile);
    }

    /// <summary>Indicates whether the user making the request is authorized to see
    /// profile information for students.</summary>
    /// <returns>True if the user making the request is authorized to see
    /// profile information for students, and false otherwise.</returns>
    public bool CanISeeStudents()
    {
        var viewerGroups = AuthUtils.GetGroups(User);

        if (viewerGroups.Contains(AuthGroup.SiteAdmin) || 
            viewerGroups.Contains(AuthGroup.Police) ||
            viewerGroups.Contains(AuthGroup.FacStaff) ||
            viewerGroups.Contains(AuthGroup.Student))
        {
            //TODO: take "KeepPrivate" into account, to enforce FERPA restrictions
            return true;
        }
        return false;
    }

    /// <summary>Indicates whether the user making the request is authorized to see
    /// profile information for this particular student.  Some students are not shown
    /// because of FERPA protections.</summary>
    /// <returns>True if the user making the request is authorized to see
    /// profile information for this student, and false otherwise.</returns>
    public bool CanISeeThisStudent(StudentProfileViewModel? student)
    {
        if (!CanISeeStudents())
        {
            return false;
        }

        var viewerGroups = AuthUtils.GetGroups(User);

        if (viewerGroups.Contains(AuthGroup.SiteAdmin) ||
            viewerGroups.Contains(AuthGroup.Police) ||
            viewerGroups.Contains(AuthGroup.FacStaff))
        {
            return true;
        }
        if (viewerGroups.Contains(AuthGroup.Student))
        {
            //TODO: take "KeepPrivate" into account, to enforce FERPA restrictions
            return (student == null) ? false : student.KeepPrivate != "Y";
        }
        return false;
    }

    /// <summary>Indicates whether the user making the request is authorized to see
    /// profile information for faculty and staff (facstaff).</summary>
    /// <returns>True if the user making the request is authorized to see
    /// profile information for facstaff, and false otherwise.</returns>
    public bool CanISeeFacstaff()
    {
        return true;
    }

    /// <summary>Indicates whether the user making the request is authorized to see
    /// profile information for alumni.</summary>
    /// <returns>True if the user making the request is authorized to see
    /// profile information for alumni, and false otherwise.</returns>
    public bool CanISeeAlumni()
    {
        var viewerGroups = AuthUtils.GetGroups(User);

        return viewerGroups.Contains(AuthGroup.SiteAdmin) ||
               viewerGroups.Contains(AuthGroup.Police) ||
               viewerGroups.Contains(AuthGroup.FacStaff) ||
               viewerGroups.Contains(AuthGroup.Alumni);
    }

    /// <summary>Restrict info about a student to those fields which are potentially
    /// viewable by the user making the request.  Actual visibility may also depend
    /// on privacy choices made by the user whose data is being viewed.  Note that 
    /// this takes FERPA restrictions into account in determining whether this student
    /// is visible to the requesting user.</summary>
    /// <returns>Information the requesting user is potentially authorized to see.
    /// Null if the requesting user is never allowed to see data about students.</returns>
    /// 
    public object? VisibleToMeStudent(StudentProfileViewModel? student)
    {
        var viewerGroups = AuthUtils.GetGroups(User);

        if (viewerGroups.Contains(AuthGroup.SiteAdmin) ||
            viewerGroups.Contains(AuthGroup.Police) ||
            viewerGroups.Contains(AuthGroup.FacStaff))
        {
            return student;
        }
        else if (CanISeeThisStudent(student))
        {
            return (student == null) ? null : (PublicStudentProfileViewModel)student;
        }
        return null;
    }

    /// <summary>Restrict info about a facstaff person to those fields which are potentially
    /// viewable by the user making the request.  Actual visibility may also depend
    /// on privacy choices made by the user whose data is being viewed.</summary>
    /// <returns>Information the requesting user is potentially authorized to see.
    /// Null if the requesting user is never allowed to see data about facstaff.</returns>
    /// 
    public object? VisibleToMeFacstaff(FacultyStaffProfileViewModel? facstaff)
    {
        var viewerGroups = AuthUtils.GetGroups(User);

        if (viewerGroups.Contains(AuthGroup.SiteAdmin) ||
            viewerGroups.Contains(AuthGroup.Police))
        {
            return facstaff;
        }
        else if (CanISeeFacstaff())
        {
            return (facstaff == null) ? null : (PublicFacultyStaffProfileViewModel)facstaff;
        }
        return null;
    }

    /// <summary>Restrict info about an alumni person to those fields which are potentially
    /// viewable by the user making the request.  Actual visibility may also depend
    /// on privacy choices made by the user whose data is being viewed.</summary>
    /// <returns>Information the requesting user is potentially authorized to see.
    /// Null if the requesting user is never allowed to see data about alumni.</returns>
    /// 
    public object? VisibleToMeAlumni(AlumniProfileViewModel? alumni)
    {
        var viewerGroups = AuthUtils.GetGroups(User);

        if (viewerGroups.Contains(AuthGroup.SiteAdmin) ||
            viewerGroups.Contains(AuthGroup.Police))
        {
            return alumni;
        }
        else if (CanISeeAlumni())
        {
            return (alumni == null) ? null : (PublicAlumniProfileViewModel)alumni;
        }
        return null;
    }

    /// <summary>Get another user's profile info.  The info returned depends
    /// on the permissions of the current users, who is making the request.</summary>
    /// <param name="username">username of the profile info</param>
    /// <returns></returns>
    [HttpGet]
    [Route("{username}")]
    public ActionResult<ProfileViewModel?> GetUserProfileAsync(string username)
    {
        var viewerGroups = AuthUtils.GetGroups(User);

        StudentProfileViewModel? _student = profileService.GetStudentProfileByUsername(username);
        FacultyStaffProfileViewModel? _facstaff = profileService.GetFacultyStaffProfileByUsername(username);
        AlumniProfileViewModel? _alumni = profileService.GetAlumniProfileByUsername(username);
        var _customInfo = profileService.GetCustomUserInfo(username);

        var student = VisibleToMeStudent(_student);
        var facstaff = VisibleToMeFacstaff(_facstaff);
        var alumni = VisibleToMeAlumni(_alumni);

        if (student is null && alumni is null && facstaff is null)
        {
            return Ok(null);
        }
        var profile = profileService.ComposeProfile(student, alumni, facstaff, _customInfo);
        var visible_profile = profileService.ImposePrivacySettings(viewerGroups, profile);
        return Ok(visible_profile);
    }

    ///<summary>Get the advisor(s) of a particular student</summary>
    /// <returns>
    /// All advisors of the given student.  For each advisor,
    /// provides first name, last name, and username.
    /// </returns>
    [HttpGet]
    [Route("Advisors/{username}")]
    [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.ADVISOR)]
    public async Task<ActionResult<IEnumerable<AdvisorViewModel>>> GetAdvisorsAsync(string username)
    {
        var advisors = await profileService.GetAdvisorsAsync(username);

        return Ok(advisors);
    }

    ///<summary>Get the privacy settings of a particular user</summary>
    /// <returns>
    /// All privacy settings of the given user.
    /// </returns>
    [HttpGet]
    [Route("{username}/privacy_setting")]
    [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.PROFILE)]
    public ActionResult<IEnumerable<UserPrivacyGetViewModel>> GetPrivacySettingAsync(string username)
    {
        var privacy = profileService.GetPrivacySettingAsync(username);

        return Ok(privacy);
    }

    /// <summary> Gets the clifton strengths of a particular user </summary>
    /// <param name="username"> The username for which to retrieve info </param>
    /// <returns> Clifton strengths of the given user. </returns>
    [HttpGet]
    [Route("clifton/{username}")]
    [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.PROFILE)]
    public ActionResult<string[]> GetCliftonStrengths_DEPRECATED(string username)
    {
        var id = accountService.GetAccountByUsername(username).GordonID;
        var strengths = profileService.GetCliftonStrengths(int.Parse(id));
        if (strengths is null)
        {
            return Ok(Array.Empty<string>());
        }

        var authenticatedUserName = AuthUtils.GetUsername(User);
        return strengths.Private is false || authenticatedUserName.EqualsIgnoreCase(username)
            ? Ok(strengths.Themes)
            : Ok(Array.Empty<string>());
    }


    /// <summary> Gets the clifton strengths of a particular user </summary>
    /// <param name="username"> The username for which to retrieve info </param>
    /// <returns> Clifton strengths of the given user. </returns>
    [HttpGet]
    [Route("{username}/clifton")]
    [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.PROFILE)]
    public ActionResult<CliftonStrengthsViewModel?> GetCliftonStrengths(string username)
    {
        var id = accountService.GetAccountByUsername(username).GordonID;
        var strengths = profileService.GetCliftonStrengths(int.Parse(id));
        if (strengths is null)
        {
            return Ok(null);
        }

        var authenticatedUserName = AuthUtils.GetUsername(User);
        return strengths.Private is false || authenticatedUserName.EqualsIgnoreCase(username)
            ? Ok(strengths)
            : Ok(null);
    }

    /// <summary>Toggle privacy of the current user's Clifton Strengths</summary>
    /// <returns>New privacy value</returns>
    [HttpGet]
    [Route("clifton/privacy")]
    public async Task<ActionResult<bool>> ToggleCliftonStrengthsPrivacyAsync()
    {
        var username = AuthUtils.GetUsername(User);
        var id = accountService.GetAccountByUsername(username).GordonID;
        var privacy = await profileService.ToggleCliftonStrengthsPrivacyAsync(int.Parse(id));

        return Ok(privacy);
    }

    /// <summary> Gets the emergency contact information of a particular user </summary>
    /// <param name="username"> The username for which to retrieve info </param>
    /// <returns> Emergency contact information of the given user. </returns>
    [HttpGet]
    [Route("emergency-contact/{username}")]
    [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.EMERGENCY_CONTACT)]
    public ActionResult<EmergencyContactViewModel> GetEmergencyContact(string username)
    {
        try
        {
            var emrg = profileService.GetEmergencyContact(username);
            return Ok(emrg);
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
            return NotFound();
        }
    }

    /// <summary>Gets the mailbox information of currently logged in user</summary>
    /// <returns></returns>
    [HttpGet]
    [Route("mailbox-information")]
    [Route("mailbox-combination")] // 2024-06-26: Route Deprecated - remove once UI has been updated
    public ActionResult<MailboxViewModel> GetMailInfo()
    {
        var username = AuthUtils.GetUsername(User);

        var result = profileService.GetMailboxInformation(username);
        return Ok(result);
    }

    /// <summary>Gets the date of birth of the current logged-in user</summary>
    /// <returns></returns>
    [HttpGet]
    [Route("birthdate")]
    public ActionResult<DateTime> GetBirthdate()
    {
        var username = AuthUtils.GetUsername(User);

        var result = profileService.GetBirthdate(username);
        return Ok(result);
    }

    /// <summary>Get the profile image of currently logged in user</summary>
    /// <returns></returns>
    [HttpGet]
    [Route("image")]
    public async Task<ActionResult<JObject>> GetMyImgAsync()
    {
        var username = AuthUtils.GetUsername(User);
        var photoModel = await profileService.GetPhotoPathAsync(username);
        JObject result = new JObject();

        if (photoModel == null) //There is no preferred or ID image
        {
            var unapprovedFileName = username + "_" + accountService.GetAccountByUsername(username).account_id;
            var unapprovedFilePath = config["DEFAULT_ID_SUBMISSION_PATH"];
            string extension = "";
            foreach (var file in Directory.GetFiles(unapprovedFilePath, unapprovedFileName + ".*"))
            {
                extension = Path.GetExtension(file);
            }
            string unapproved_img = await GetProfileImageOrDefault(unapprovedFilePath + unapprovedFileName + extension);
            result.Add("def", unapproved_img);
            return Ok(result);
        }

        string prefImgPath = config["PREFERRED_IMAGE_PATH"] + photoModel.Pref_Img_Name;

        if (string.IsNullOrEmpty(photoModel.Pref_Img_Name) || !System.IO.File.Exists(prefImgPath)) //check file existence for prefferred image.
        {
            var defaultImgPath = config["DEFAULT_IMAGE_PATH"] + photoModel.Img_Name;
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
        var photoInfo = await profileService.GetPhotoPathAsync(username);
        JObject result = new JObject();

        //return default image if no photo info found for this user.
        if (photoInfo == null)
        {
            result.Add("def", await ImageUtils.DownloadImageFromURL(config["DEFAULT_PROFILE_IMAGE_PATH"]));
            return Ok(result);
        }

        var preferredImagePath = string.IsNullOrEmpty(photoInfo.Pref_Img_Name) ? null : config["PREFERRED_IMAGE_PATH"] + photoInfo.Pref_Img_Name;
        var defaultImagePath = config["DEFAULT_IMAGE_PATH"] + photoInfo.Img_Name;

        var viewerGroups = AuthUtils.GetGroups(User);
        if (viewerGroups.Contains(AuthGroup.FacStaff))
        {
            if (preferredImagePath is not null && System.IO.File.Exists(preferredImagePath))
            {
                result.Add("pref", await GetProfileImageOrDefault(preferredImagePath));
            }
            result.Add("def", await GetProfileImageOrDefault(defaultImagePath));
            return Ok(result);

        }
        else
        if (viewerGroups.Contains(AuthGroup.Student))
        {
            if (accountService.GetAccountByUsername(username).show_pic == 1)
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
                result.Add("def", await ImageUtils.DownloadImageFromURL(config["DEFAULT_PROFILE_IMAGE_PATH"]));
            }
            return Ok(result);
        }
        else
        {
            return Ok();
        }
    }

    /// <summary>
    /// Set an image for profile
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("image")]
    public async Task<ActionResult> PostImageAsync([FromForm] IFormFile image)
    {
        var username = AuthUtils.GetUsername(User);
        var account = accountService.GetAccountByUsername(username);
        var pathInfo = await profileService.GetPhotoPathAsync(username);

        if (pathInfo == null) // can't upload image if there is no record for this user in the database
            return NotFound("No photo record was found for this user.");

        var (extension, _) = ImageUtils.GetImageFormat(image);
        var fileName = $"{account.Barcode}.{extension}";

        // If there is an old photo that won't get overwritten, delete the old photo
        if (pathInfo.Pref_Img_Name is string oldName
            && oldName != fileName
            && pathInfo.Pref_Img_Path is string oldPath
            && Path.Combine(oldPath, oldName) is string oldFile
            && System.IO.File.Exists(oldFile))
        {
            System.IO.File.Delete(oldFile);
        }

        var filePath = Path.Combine(config["PREFERRED_IMAGE_PATH"], fileName);

        ImageUtils.UploadImageAsync(filePath, image);

        await profileService.UpdateProfileImageAsync(username, config["DATABASE_IMAGE_PATH"], fileName);

        return Ok();
    }

    /// <summary>
    /// Set an IDimage for a user
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("IDimage")]
    public async Task<ActionResult> PostIDImageAsync([FromForm] IFormFile image)
    {
        if (image.Length < 3000)
        {
            return BadRequest("The ID image was lost in transit. Resubmission should attempt automatically.");
        }

        var username = AuthUtils.GetUsername(User);
        var root = config["DEFAULT_ID_SUBMISSION_PATH"];
        var account = accountService.GetAccountByUsername(username);

        //delete old image file if it exists.
        DirectoryInfo di = new DirectoryInfo(root);
        foreach (FileInfo file in di.GetFiles($"{username}_{account.account_id}.*"))
        {
            file.Delete();
        }

        var (extension, _) = ImageUtils.GetImageFormat(image);
        var fileName = $"{username}_{account.account_id}.{extension}";
        var filePath = Path.Combine(root, fileName);

        using var stream = System.IO.File.Create(filePath);
        await image.CopyToAsync(stream);

        return Ok();
    }

    /// <summary>
    /// Reset the profile Image
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("image/reset")]
    public async Task<ActionResult> ResetImage()
    {
        var authenticatedUserUsername = AuthUtils.GetUsername(User);
        var photoInfo = await profileService.GetPhotoPathAsync(authenticatedUserUsername);

        if (!string.IsNullOrEmpty(photoInfo?.Pref_Img_Name))
        {
            System.IO.File.Delete(Path.Combine(config["PREFERRED_IMAGE_PATH"], photoInfo.Pref_Img_Name));
        }

        await profileService.UpdateProfileImageAsync(authenticatedUserUsername, null, null);
        return Ok();
    }

    /// <summary>
    /// Update CUSTOM_PROFILE component
    /// </summary>
    /// <param name="type">The type of component</param>
    /// <param name="value">The value to change the component to</param>
    /// <returns></returns>
    [HttpPut]
    [Route("{type}")]
    public async Task<ActionResult> UpdateCustomProfile(string type, [FromBody] CUSTOM_PROFILE value)
    {
        var authenticatedUserUsername = AuthUtils.GetUsername(User);

        await profileService.UpdateCustomProfileAsync(authenticatedUserUsername, type, value);

        return Ok();
    }

    /// <summary>
    /// Update mobile phone number
    /// </summary>
    /// <param name="value">phoneNumber</param>
    /// <returns></returns>
    [HttpPut]
    [Route("mobile_phone_number/{value}")]
    public async Task<ActionResult<StudentProfileViewModel>> UpdateMobilePhoneNumber(string value)
    {
        var username = AuthUtils.GetUsername(User);
        var result = await profileService.UpdateMobilePhoneNumberAsync(username, value);

        return Ok(result);
    }

    /// <summary>
    /// Update office location (building description and room number)
    /// </summary>
    /// <returns></returns>
    [HttpPut]
    [Route("office_location")]
    public async Task<ActionResult<FacultyStaffProfileViewModel>> UpdateOfficeLocation(OfficeLocationPatchViewModel officeLocation)
    {
        var username = AuthUtils.GetUsername(User);
        var result = await profileService.UpdateOfficeLocationAsync(username, officeLocation.BuildingCode, officeLocation.RoomNumber);
        return Ok(result);
    }

    /// <summary>
    /// Update office hours
    /// </summary>
    /// <param name="value">office hours</param>
    /// <returns></returns>
    [HttpPut]
    [Route("office_hours")]
    public async Task<ActionResult<FacultyStaffProfileViewModel>> UpdateOfficeHours([FromBody] string value)
    {
        var username = AuthUtils.GetUsername(User);
        var result = await profileService.UpdateOfficeHoursAsync(username, value);
        return Ok(result);
    }

    /// <summary>
    /// Set visibility of some piece of personal data for user.
    /// </summary>
    /// <param name="userPrivacy">Faculty Staff Privacy Decisions (see UserPrivacyUpdateViewModel)</param>
    /// <returns></returns>
    [HttpPut]
    [Route("user_privacy")]
    // [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.PROFILE_PRIVACY)]
    public async Task<ActionResult<UserPrivacyUpdateViewModel>> UpdateUserPrivacyAsync(UserPrivacyUpdateViewModel userPrivacy)
    {
        var authenticatedUserUsername = AuthUtils.GetUsername(User);
        await profileService.UpdateUserPrivacyAsync(authenticatedUserUsername, userPrivacy);
        return Ok();
    }

    /// <summary>
    /// Return a list visibility groups.
    /// </summary>
    /// <returns> All visibility groups (Public, FacStaff, Private)</returns>
    [HttpGet]
    [Route("visibility_groups")]
    public ActionResult<IEnumerable<string>> GetVisibilityGroup()
    {
        var groups = context.UserPrivacy_Visibility_Groups.Select(up_v_g => up_v_g.Group)
                               .Distinct()
                               .Where(g => g != null);
        return Ok(groups);
    }

    /// <summary>
    /// Update mail location
    /// </summary>
    /// <param name="value">mail location</param>
    /// <returns></returns>
    [HttpPut]
    [Route("mailstop")]
    public async Task<ActionResult<FacultyStaffProfileViewModel>> UpdateMailStop([FromBody] string value)
    {
        var username = AuthUtils.GetUsername(User);
        var result = await profileService.UpdateMailStopAsync(username, value);
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
        var authenticatedUserUsername = AuthUtils.GetUsername(User);
        await profileService.UpdateMobilePrivacyAsync(authenticatedUserUsername, value);

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
        var authenticatedUserUsername = AuthUtils.GetUsername(User);
        await profileService.UpdateImagePrivacyAsync(authenticatedUserUsername, value);

        return Ok();
    }

    /// <summary>
    /// Posts fields into CCT.dbo.Information_Change_Request 
    /// Sends Alumni Profile Update Email to "devrequest@gordon.edu"
    /// </summary>
    /// <param name="updatedFields">Object with Field's Name and Field's Value, unused Field's Label</param>
    /// <returns></returns>
    [HttpPost]
    [Route("update")]
    public async Task<ActionResult> RequestUpdate(ProfileFieldViewModel[] updatedFields)
    {
        var authenticatedUserUsername = AuthUtils.GetUsername(User);
        await profileService.InformationChangeRequest(authenticatedUserUsername, updatedFields);
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
            return await ImageUtils.DownloadImageFromURL(config["DEFAULT_PROFILE_IMAGE_PATH"]);
        }
    }

    /// <summary>
    /// Fetch memberships that a specific student has been a part of
    /// @TODO: Move security checks to state your business? Or consider changing implementation here
    /// </summary>
    /// <param name="username">The Student Username</param>
    /// <param name="sessionCode">Optional session code or "current". If passed, only memberships from that session will be included. </param>
    /// <param name="participationTypes">Optional participation type. If passed, only memberships of those participation types will be inlcuded</param>
    /// <returns>The membership information that the student is a part of</returns>
    [Route("{username}/memberships")]
    [HttpGet]
    [Obsolete("Use /api/memberships with username query param instead")]
    public ActionResult<List<MembershipView>> GetMembershipsByUser(string username, string? sessionCode = null, [FromQuery] List<string>? participationTypes = null)
    {
        var memberships = membershipService.GetMemberships(
            username: username,
            sessionCode: sessionCode,
            participationTypes: participationTypes);

        var authenticatedUserUsername = AuthUtils.GetUsername(User);
        var viewerGroups = AuthUtils.GetGroups(User);

        // User can see all their own memberships. SiteAdmin and Police can see all of anyone's memberships
        if (username == authenticatedUserUsername
            || viewerGroups.Contains(AuthGroup.SiteAdmin)
            || viewerGroups.Contains(AuthGroup.Police)
            )
        {
            return Ok(memberships);
        }

        var visibleMemberships = membershipService.RemovePrivateMemberships(memberships, authenticatedUserUsername);

        return Ok(visibleMemberships);
    }

    /// <summary>
    /// Fetch the history of a user's memberships
    /// </summary>
    /// <param name="username">The Student Username</param>
    /// <returns>The history of that user's membership in involvements</returns>
    [Route("{username}/memberships-history")]
    [HttpGet]
    public ActionResult<IEnumerable<MembershipHistoryViewModel>> GetMembershipHistory(string username)
    {
        var memberships = membershipService
            .GetMemberships(username: username, sessionCode: "*")
            .Where(m => m.Participation != Participation.Guest.GetCode());

        var authenticatedUserUsername = AuthUtils.GetUsername(User);
        var viewerGroups = AuthUtils.GetGroups(User);

        // User can see all their own memberships. SiteAdmin and Police can see all of anyone's memberships
        if (!(username == authenticatedUserUsername
            || viewerGroups.Contains(AuthGroup.SiteAdmin)
            || viewerGroups.Contains(AuthGroup.Police)
            ))
        {
            memberships = membershipService.RemovePrivateMemberships(memberships, authenticatedUserUsername);
        }

        var membershipHistories = memberships.GroupBy(m => m.ActivityCode).Select(group => MembershipHistoryViewModel.FromMembershipGroup(group));

        return Ok(membershipHistories);
    }

    /// <summary>
    /// Return a list of mail destinations' descriptions.
    /// </summary>
    /// <returns> All Mail Destinations</returns>
    [HttpGet]
    [Route("mailstops")]
    public ActionResult<IEnumerable<string>> GetMailStops()
    {
        var mail_stops = profileService.GetMailStopsAsync();
        return Ok(mail_stops);
    }
}
