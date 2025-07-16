using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Models.webSQL.Context;
using Gordon360.Static.Methods;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gordon360.Enums;

namespace Gordon360.Services;

public class ProfileService(CCTContext context, IConfiguration config, IAccountService accountService, webSQLContext webSQLContext) : IProfileService
{
    // These three-character strings are valid substrings for the PersonType
    // field in the profile. These are used in the UI and so cannot be changed
    // here unless the corresponding change is made in the UI.
    const string FACSTAFF_PROFILE = "fac";
    const string STUDENT_PROFILE = "stu";
    const string ALUMNI_PROFILE = "alu";

    /// <summary>
    /// get student profile info
    /// </summary>
    /// <param name="username">username</param>
    /// <returns>StudentProfileViewModel if found, null if not found</returns>
    public StudentProfileViewModel? GetStudentProfileByUsername(string username)
    {
        return context.Student.FirstOrDefault(x => x.AD_Username.ToLower() == username.ToLower());
    }

    /// <summary>
    /// get faculty staff profile info
    /// </summary>
    /// <param name="username">username</param>
    /// <returns>FacultyStaffProfileViewModel if found, null if not found</returns>
    public FacultyStaffProfileViewModel? GetFacultyStaffProfileByUsername(string username)
    {
        return context.FacStaff.FirstOrDefault(x => x.AD_Username.ToLower() == username.ToLower());
    }

    /// <summary>
    /// get alumni profile info
    /// </summary>
    /// <param name="username">username</param>
    /// <returns>AlumniProfileViewModel if found, null if not found</returns>
    public AlumniProfileViewModel? GetAlumniProfileByUsername(string username)
    {
        return context.Alumni.FirstOrDefault(x => x.AD_Username.ToLower() == username.ToLower());
    }

    /// <summary>
    /// get mailbox information (contains box combination)
    /// </summary>
    /// <param name="username">The current user's username</param>
    /// <returns>MailboxCombinationViewModel with the combination</returns>
    public MailboxCombinationViewModel? GetMailboxCombination(string username)
    {
        return context.Mailboxes
            .Where(m => m.HolderUsername == username)
            .Select(m => m.Combination)
            .Select(MailboxCombinationViewModel.From)
            .FirstOrDefault();
    }

    /// <summary>
    /// get a user's birthday
    /// </summary>
    /// <param name="username">The username of the person to get the birthdate of</param>
    /// <returns>Date the user's date of birth, if available, or a default of 1/1/1800.</returns>
    public DateTime GetBirthdate(string username)
    {
        var birthdate = context.ACCOUNT.FirstOrDefault(a => a.AD_Username == username)?.Birth_Date;
        var impossible_birthdate = new DateTime(1800, 1, 1);

        if (birthdate == null)
        {
            return impossible_birthdate;
        }

        // Test accounts always have current date and time as birthday, so
        // treat this the same as no birthday
        // Comment this out to see "happy birthday" banner in test accounts
        var lifetime = DateTime.Now - (DateTime)birthdate;
        if (lifetime.Days < 1) // no valid user was born within the last 24 hours
        {
            return impossible_birthdate;
        }

        try
        {
            return (DateTime)(birthdate);
        }
        catch
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The user's birthdate was invalid." };
        }
    }

    /// <summary>
    /// get advisors for particular student
    /// </summary>
    /// <param name="username">AD username</param>
    /// <returns></returns>
    public async Task<IEnumerable<AdvisorViewModel>> GetAdvisorsAsync(string username)
    {
        var account = accountService.GetAccountByUsername(username);

        // Stored procedure returns row containing advisor1 ID, advisor2 ID, advisor3 ID 
        var advisorIDsEnumerable = await context.Procedures.ADVISOR_SEPARATEAsync(int.Parse(account.GordonID));
        var advisorIDs = advisorIDsEnumerable.FirstOrDefault();

        if (advisorIDs == null)
        {
            return null;
        }

        List<AdvisorViewModel> resultList = new();

        foreach (var advisorID in new[] { advisorIDs.Advisor1, advisorIDs.Advisor2, advisorIDs.Advisor3 })
        {
            if (!string.IsNullOrEmpty(advisorID))
            {
                var advisor = accountService.GetAccountByID(advisorID);
                resultList.Add(new AdvisorViewModel(advisor.FirstName, advisor.LastName, advisor.ADUserName));
            }
        }

        return resultList;
    }

    /// <summary> Gets the clifton strengths of a particular user </summary>
    /// <param name="id"> The id of the user for which to retrieve info </param>
    /// <returns> Clifton strengths of the given user. </returns>
    public CliftonStrengthsViewModel? GetCliftonStrengths(int id)
    {
        return context.Clifton_Strengths.FirstOrDefault(c => c.ID_NUM == id);
    }

    /// <summary>
    /// Toggles the privacy of the Clifton Strengths data associated with the given id
    /// </summary>
    /// <param name="id">ID of the user whose Clifton Strengths privacy is toggled</param>
    /// <returns>The new privacy value</returns>
    /// <exception cref="ResourceNotFoundException">Thrown when the given ID doesn't match any Clifton Strengths rows</exception>
    public async Task<bool> ToggleCliftonStrengthsPrivacyAsync(int id)
    {
        var strengths = context.Clifton_Strengths.FirstOrDefault(cs => cs.ID_NUM == id);
        if (strengths is null)
        {
            throw new ResourceNotFoundException { ExceptionMessage = "No Strengths found" };
        }

        strengths.Private = !strengths.Private;
        await context.SaveChangesAsync();

        return strengths.Private;
    }

    /// <summary> Gets the emergency contact information of a particular user </summary>
    /// <param name="username"> The username of the user for which to retrieve info </param>
    /// <returns> Emergency contact information of the given user. </returns>
    public IEnumerable<EmergencyContactViewModel> GetEmergencyContact(string username)
    {
        var result = context.EmergencyContact.Where(x => x.AD_Username == username).Select(x => (EmergencyContactViewModel)x);

        if (result == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "No emergency contacts found." };
        }

        return result;
    }

    /// <summary>
    /// Get photo path for profile
    /// </summary>
    /// <param name="username">AD username</param>
    /// <returns>PhotoPathViewModel if found, null if not found</returns>
    public async Task<PhotoPathViewModel?> GetPhotoPathAsync(string username)
    {
        var account = accountService.GetAccountByUsername(username);

        var photoInfoList = await context.Procedures.PHOTO_INFO_PER_USER_NAMEAsync(int.Parse(account.GordonID));
        return photoInfoList.Select(p => new PhotoPathViewModel { Img_Name = p.Img_Name, Img_Path = p.Img_Path, Pref_Img_Name = p.Pref_Img_Name, Pref_Img_Path = p.Pref_Img_Path }).FirstOrDefault();
    }

    /// <summary>
    /// Fetches a single profile whose username matches the username provided as an argument
    /// </summary>
    /// <param name="username">The username</param>
    /// <returns>ProfileViewModel if found, null if not found</returns>
    public ProfileCustomViewModel? GetCustomUserInfo(string username)
    {
        return context.CUSTOM_PROFILE.Find(username);
    }

    /// <summary>
    /// Sets the path for the profile image.
    /// </summary>
    /// <param name="username">AD Username</param>
    /// <param name="path"></param>
    /// <param name="name"></param>
    public async Task UpdateProfileImageAsync(string username, string? path, string? name)
    {
        var account = accountService.GetAccountByUsername(username);

        await context.Procedures.UPDATE_PHOTO_PATHAsync(int.Parse(account.GordonID), path, name);
        // Update value in cached data
        var student = context.Student.FirstOrDefault(x => x.ID == account.GordonID);
        var facStaff = context.FacStaff.FirstOrDefault(x => x.ID == account.GordonID);
        var alum = context.Alumni.FirstOrDefault(x => x.ID == account.GordonID);
        if (student != null)
        {
            student.preferred_photo = (path == null ? 0 : 1);
        }
        else if (facStaff != null)
        {
            facStaff.preferred_photo = (path == null ? 0 : 1);
        }
        else if (alum != null)
        {
            alum.preferred_photo = (path == null ? 0 : 1);
        }
    }


    /// <summary>
    /// Sets the component of the Custom_profile.
    /// </summary>
    /// <param name="username">The username</param>
    /// <param name="type"></param>
    /// <param name="content"></param>
    public async Task UpdateCustomProfileAsync(string username, string type, CUSTOM_PROFILE content)
    {
        var original = await context.CUSTOM_PROFILE.FindAsync(username);

        if (original == null)
        {
            await context.CUSTOM_PROFILE.AddAsync(new CUSTOM_PROFILE
            {
                username = username,
                calendar = content.calendar,
                facebook = content.facebook,
                twitter = content.twitter,
                instagram = content.instagram,
                linkedin = content.linkedin,
                handshake = content.handshake,
                PlannedGradYear = content.PlannedGradYear,
                SMSOptedIn = content.SMSOptedIn,
            });
        }
        else
        {
            switch (type)
            {
                case "calendar":
                    original.calendar = content.calendar;
                    break;
                case "facebook":
                    original.facebook = content.facebook;
                    break;
                case "twitter":
                    original.twitter = content.twitter;
                    break;
                case "instagram":
                    original.instagram = content.instagram;
                    break;
                case "linkedin":
                    original.linkedin = content.linkedin;
                    break;
                case "handshake":
                    original.handshake = content.handshake;
                    break;
                case "plannedGradYear":
                    original.PlannedGradYear = content.PlannedGradYear;
                    break;
                case "SMSOptedIn":
                    original.SMSOptedIn = content.SMSOptedIn;
                    break;
                default:
                    throw new NotSupportedException($"Unrecognized custom profile setting {type}");
            }
        }

        await context.SaveChangesAsync();
    }


    /// <summary>
    /// convert combined profile to public profile based on individual privacy settings
    /// </summary>
    /// <param name="viewerGroups">list of AuthGroups the logged-in user belongs to</param>
    /// <param name="profile">combined profile of the person being searched</param>
    /// <returns>public profile of the person based on individual privacy settings</returns>
    public CombinedProfileViewModel ImposePrivacySettings
        (IEnumerable<AuthGroup> viewerGroups, ProfileViewModel profile)
    {
        // Convert profile from record to class so we can modify its elements
        CombinedProfileViewModel restricted_profile = (CombinedProfileViewModel) profile;

        // Privacy settings are generally heirarchical from bypassing all privacy settings
        // to honoring all privacy settings:
        //   SiteAdmin & Police -> FacStaff -> Students -> Alumni

        // Find the account belonging to person whose profile we are accessing and get their
        // privacy settings
        var account = accountService.GetAccountByUsername(restricted_profile.AD_Username);
        var privacy = context.UserPrivacy_Settings.Where(up_s => up_s.gordon_id == account.GordonID);

        // Determing the viewer and profile user types
        bool viewerIsSiteAdmin = viewerGroups.Contains(AuthGroup.SiteAdmin);
        bool viewerIsPolice = viewerGroups.Contains(AuthGroup.Police);
        bool viewerIsFacStaff = viewerGroups.Contains(AuthGroup.FacStaff);
        bool viewerIsStudent = viewerGroups.Contains(AuthGroup.Student);
        bool viewerIsAlumni = viewerGroups.Contains(AuthGroup.Alumni);
        bool profileIsFacStaff = restricted_profile.PersonType.Contains(FACSTAFF_PROFILE);
        bool profileIsStudent = restricted_profile.PersonType.Contains(STUDENT_PROFILE);
        bool profileIsAlumni = restricted_profile.PersonType.Contains(ALUMNI_PROFILE);

        // Loop over all privacy fields (MobilePhone, HomePhone, HomeCity, etc.) and use
        // visibility data in UserPrivacy_Settings table if exists otherwise use old-style
        // privacy settings in profile.
        
        // NOTE: The "old-style" privacy settings in the profile (e.g. IsMobilePhonePrivate)
        // are set by HR or the student matriciulation process.  These settings will be used
        // for 360 until the user chooses privacy settings in their profile.

        foreach (int fieldID in context.UserPrivacy_Fields.Select(s => s.ID))
        {
            // Determine the visibility for the current privacy field
            //var privacy.Where(f => f.Field == )
            var visibilityID = privacy.Where(x => x.Field == fieldID).FirstOrDefault()?.Visibility;
            if (visibilityID is null)
            {
                if (profileIsStudent)
                {
                    visibilityID = ((restricted_profile.KeepPrivate == "Y" || restricted_profile.KeepPrivate == "P")
                                    || (fieldID == UserPrivacyViewModel.MobilePhoneID && restricted_profile.IsMobilePhonePrivate))
                        ? UserPrivacyViewModel.Private_GroupID
                        : UserPrivacyViewModel.Public_GroupID;
                }
                else if (profileIsFacStaff)
                {
                    visibilityID = restricted_profile.KeepPrivate == "1"
                        ? UserPrivacyViewModel.Private_GroupID
                        : UserPrivacyViewModel.Public_GroupID;
                }
                else if (profileIsAlumni)
                {
                    visibilityID = (restricted_profile.ShareName == "N" 
                                    || restricted_profile.ShareAddress == "N")
                        ? UserPrivacyViewModel.Private_GroupID
                        : UserPrivacyViewModel.Public_GroupID;
                }
            }

            var field = context.UserPrivacy_Fields.Where(s => s.ID == fieldID).FirstOrDefault()?.Field;

            // Enforce the visibility for the current privacy field
            if ((viewerIsSiteAdmin || viewerIsPolice) && visibilityID != UserPrivacyViewModel.Public_GroupID)
            {
                MarkAsPrivate(restricted_profile, field);
            }
            else if (viewerIsFacStaff)
            {
                if (profileIsFacStaff)
                {
                    if (visibilityID == UserPrivacyViewModel.Private_GroupID)
                    {
                        MakePrivate(restricted_profile, field);
                    }
                    else if (visibilityID == UserPrivacyViewModel.FacStaff_GroupID)
                    {
                        MarkAsPrivate(restricted_profile, field);
                    }
                }
                else if ((profileIsStudent || profileIsAlumni) && visibilityID != UserPrivacyViewModel.Public_GroupID)
                {
                    MarkAsPrivate(restricted_profile, field);
                }
            } 
            else if ((viewerIsStudent || viewerIsAlumni) && visibilityID != UserPrivacyViewModel.Public_GroupID)
            {
                MakePrivate(restricted_profile, field);
            }
        }

        return restricted_profile;
    }

    /// <summary>
    /// Get profile fields and visibility settings for a specific user
    /// </summary>
    /// <param name="username">AD username</param>
    /// <returns>List of field and visibility privacy settings for a specific user</returns>
    public IEnumerable<UserPrivacyViewModel> GetPrivacySettingsAsync(string username)
    {
        var account = accountService.GetAccountByUsername(username);

        // select all privacy settings
        var privacy = context.UserPrivacy_Settings
            .Include(up_s => up_s.VisibilityNavigation)
            .Include(up_s => up_s.FieldNavigation)
            .Where(up_s => up_s.gordon_id == account.GordonID)
            .Select(up_s => (UserPrivacyViewModel)up_s);

        return privacy;
    }

    /// <summary>
    /// Set privacy setting of some piece of personal data for user.
    /// </summary>
    /// <param name="username">AD Username</param>
    /// <param name="userPrivacy">User Privacy Update View Model</param>
    public async Task UpdateUserPrivacyAsync(string username, UserPrivacyUpdateViewModel userPrivacy)
    {
        var account = accountService.GetAccountByUsername(username);
        foreach (string field in userPrivacy.Field)
        {
            var fieldID = context.UserPrivacy_Fields.FirstOrDefault(f => f.Field == field).ID;
            var groupID = context.UserPrivacy_Visibility_Groups.FirstOrDefault(v => v.Group == userPrivacy.VisibilityGroup).ID;
            var user = context.UserPrivacy_Settings
                .Include(up_s => up_s.FieldNavigation)
                .Include(up_s => up_s.VisibilityNavigation)
                .FirstOrDefault(up_s => up_s.gordon_id == account.GordonID && up_s.Field == fieldID);
            if (user is null)
            {
                var privacy = new UserPrivacy_Settings
                {
                    gordon_id = account.GordonID,
                    Field = fieldID,
                    Visibility = groupID
                };
                await context.UserPrivacy_Settings.AddAsync(privacy);
            }
            else
            {
                user.Visibility = groupID;
            }
        }

        context.SaveChanges();
    }


    /// <summary>
    /// privacy setting of mobile phone.
    /// </summary>
    /// <param name="username">AD Username</param>
    /// <param name="value">Y or N</param>
    public async Task UpdateMobilePrivacyAsync(string username, string value)
    {
        var account = accountService.GetAccountByUsername(username);
        await context.Procedures.UPDATE_PHONE_PRIVACYAsync(int.Parse(account.GordonID), value);
        // Update value in cached data
        var student = context.Student.FirstOrDefault(x => x.ID == account.GordonID);
        if (student != null)
        {
            student.IsMobilePhonePrivate = (value == "Y" ? 1 : 0);
        }

        context.SaveChanges();
    }

    /// <summary>
    /// Updates mobile phone number 
    /// </summary>
    /// <param name="username">The username for the user whose number is updated</param>
    /// <param name="newMobilePhoneNumber">The new mobile phone number to update for the user's phone number</param>
    /// <returns>updated student profile by there username</returns>
    public async Task<StudentProfileViewModel> UpdateMobilePhoneNumberAsync(string username, string newMobilePhoneNumber)
    {
        var profile = GetStudentProfileByUsername(username);
        if (profile == null)
        {
            throw new ResourceNotFoundException { ExceptionMessage = "The account was not found" };
        
        }
        var digitsOnly = Regex.Replace(newMobilePhoneNumber, @"[^\d]", "");
        await context.Procedures.UPDATE_CELL_PHONEAsync(profile.ID, digitsOnly);
        var student = await context.Student.FirstOrDefaultAsync(x => x.ID == profile.ID);
        if (student != null)
        {
            student.MobilePhone = digitsOnly;
        }
        return profile;
    }

    /// <summary>
    /// office location setting
    /// </summary>
    /// <param name="username"> The username for the user whose office location is to be updated </param>
    /// <param name="newBuilding">The new building location to update the user's office location to</param> 
    /// <param name="newRoom">The new room to update the user's office room to</param>
    /// <returns>updated fac/staff profile if found</returns>
    public async Task<FacultyStaffProfileViewModel> UpdateOfficeLocationAsync(string username, string newBuilding, string newRoom)
    {
        var profile = GetFacultyStaffProfileByUsername(username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found" };
        var user = webSQLContext.accounts.FirstOrDefault(a => a.AD_Username == username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "The webSQL account was not found" };
        user.Building = newBuilding;
        user.Room = newRoom;
        await webSQLContext.SaveChangesAsync();

        return profile;
    }

    /// <summary>
    /// office hours setting
    /// </summary>
    /// <param name="username"> The username for the user whose office hours is to be updated </param>
    /// <param name="newHours">The new hours to update the user's office hours to</param>
    /// <returns>updated fac/staff profile if found</returns>
    public async Task<FacultyStaffProfileViewModel> UpdateOfficeHoursAsync(string username, string newHours)
    {
        var profile = GetFacultyStaffProfileByUsername(username);
        if (profile == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found" };
        }
        var acccount = webSQLContext.accounts.FirstOrDefault(a => a.AD_Username == username);
        var user = webSQLContext.account_profiles.FirstOrDefault(a => a.account_id == acccount.account_id);
        user.office_hours = newHours;
        await webSQLContext.SaveChangesAsync();

        return profile;
    }

    /// <summary>
    /// mail location setting
    /// </summary>
    /// <param name="username"> The username for the user whose mail location is to be updated </param>
    /// <param name="newMail">The new mail location to update the user's mail location to</param>
    /// <returns>updated fac/staff profile if found</returns>
    public async Task<FacultyStaffProfileViewModel> UpdateMailStopAsync(string username, string newMail)
    {
        var profile = GetFacultyStaffProfileByUsername(username);
        if (profile == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found" };
        }
        var user = webSQLContext.accounts.FirstOrDefault(a => a.AD_Username == username);
        user.mail_server = newMail;
        await webSQLContext.SaveChangesAsync();

        return profile;
    }

    /// <summary>
    /// privacy setting user profile photo.
    /// </summary>
    /// <param name="username">AD Username</param>
    /// <param name="value">Y or N</param>
    public async Task UpdateImagePrivacyAsync(string username, string value)
    {
        var account = accountService.GetAccountByUsername(username);

        await context.Procedures.UPDATE_SHOW_PICAsync(account.account_id, value);
        // Update value in cached data
        var student = context.Student.FirstOrDefault(x => x.ID == account.GordonID);
        var facStaff = context.FacStaff.FirstOrDefault(x => x.ID == account.GordonID);
        var alum = context.Alumni.FirstOrDefault(x => x.ID == account.GordonID);
        if (student != null)
        {
            student.show_pic = (value == "Y" ? 1 : 0);
        }
        else if (facStaff != null)
        {
            facStaff.show_pic = (value == "Y" ? 1 : 0);
        }
        else if (alum != null)
        {
            alum.show_pic = (value == "Y" ? 1 : 0);
        }

        context.SaveChanges();
    }

    /// <summary>
    /// Get graduation information for a student
    /// </summary>
    /// <param name="username">The username of the student</param>
    /// <returns>GraduationViewModel containing graduation details</returns>
    public GraduationViewModel? GetGraduationInfo(string username)
    {
        // Find the graduation record directly by AD_Username
        var graduation = context.Graduation.FirstOrDefault(g => g.AD_Username.ToLower() == username.ToLower());
        if (graduation == null)
        {
            return null; // Graduation info might not exist for all students
        }

        // Map the graduation data to a ViewModel
        return new GraduationViewModel
        {
            WhenGraduated = graduation.WHEN_GRAD,
            HasGraduated = graduation.HAS_GRADUATED == "Y",
            GraduationFlag = graduation.GRAD_FLAG
        };
    }

    public ProfileViewModel? ComposeProfile(object? student, object? alumni, object? faculty, object? customInfo)
    {
        var profile = new JObject();
        var personType = "";

        if (student != null)
        {
            MergeProfile(profile, JObject.FromObject(student));
            personType += STUDENT_PROFILE;
        }

        if (alumni != null)
        {
            MergeProfile(profile, JObject.FromObject(alumni));
            personType += ALUMNI_PROFILE;
        }

        if (faculty != null)
        {
            MergeProfile(profile, JObject.FromObject(faculty));
            personType += FACSTAFF_PROFILE;
        }

        if (customInfo != null)
        {
            MergeProfile(profile, JObject.FromObject(customInfo));
        }

        profile.Add("PersonType", personType);

        return profile.ToObject<ProfileViewModel>();
    }

    public async Task InformationChangeRequest(string username, ProfileFieldViewModel[] updatedFields)
    {
        var account = accountService.GetAccountByUsername(username);

        string from_email = config["Emails:Sender:Username"];
        string to_email = config["Emails:AlumniProfileUpdateRequestApprover"];
        string messageBody = $"{account.FirstName} {account.LastName} ({account.GordonID}) has requested the following updates: \n\n";

        var requestNumber = await context.GetNextValueForSequence(Sequence.InformationChangeRequest);
        foreach (var element in updatedFields)
        {
            var itemToSubmit = new Information_Change_Request
            {
                RequestNumber = requestNumber,
                ID_Num = account.GordonID,
                FieldName = element.Field,
                FieldValue = element.Value
            };
            context.Information_Change_Request.Add(itemToSubmit);
            messageBody += $"{element.Label} : {element.Value} \n\n";
        }
        context.SaveChanges();

        using var smtpClient = new SmtpClient()
        {
            Credentials = new NetworkCredential
            {
                UserName = from_email,
                Password = config["Emails:Sender:Password"]
            },
            Host = config["SmtpHost"],
            EnableSsl = true,
            Port = 587,
        };

        var message = new MailMessage(from_email, to_email)
        {
            Subject = $"Information Update Request for {account.FirstName} {account.LastName}",
            Body = messageBody,
        };
        if (account.Email != null)
        {
            message.Bcc.Add(new MailAddress(account.Email));
        }

        smtpClient.Send(message);
    }

    private static JObject MergeProfile(JObject profile, JObject profileInfo)
    {
        profile.Merge(profileInfo, new JsonMergeSettings
        {
            MergeArrayHandling = MergeArrayHandling.Union
        });
        return profile;
    }

    public IEnumerable<string> GetMailStopsAsync()
    {
        return webSQLContext.Mailstops.Select(m => m.code)
                       .OrderBy(d => d);
    }

    /// <summary>
    /// Change a ProfileItem's privacy setting to true
    /// </summary>
    /// <param name="profile">Combined profile containing element to update</param>
    /// <param name="fieldID">The ID of the profile element of which to update IsPrivate</param>
    private static void MarkAsPrivate(CombinedProfileViewModel profile, string field)
    {
        // Profile element will be returned to UI, but should be marked as private
        // since the authenticated user is only seeing because they are authorized
        // to do so.
        Type cpvm = new CombinedProfileViewModel().GetType();
        try
        {
            PropertyInfo prop = cpvm.GetProperty(field);
            ProfileItem<string> profile_item = (ProfileItem<string>) prop.GetValue(profile);
            if (profile_item != null)
            {
                prop.SetValue(profile, new ProfileItem<string>(profile_item.Value, true));
            }
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
        }
    }

    /// <summary>
    /// Change a ProfileItem to be null (remove it from the profile)
    /// </summary>
    /// <param name="profile">Combined profile containing element to make null</param>
    /// <param name="fieldID">The ID of the profile element to make null</param>
    private static void MakePrivate(CombinedProfileViewModel profile, string field)
    {
        // remove profile element if it should not be sent to the UI
        try
        {
            Type cpvm = new CombinedProfileViewModel().GetType();
            cpvm.GetProperty(field).SetValue(profile, null);
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine(e.Message);
        }
    }
}
