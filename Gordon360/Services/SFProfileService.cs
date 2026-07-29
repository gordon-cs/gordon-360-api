using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Models.webSQL.Context;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gordon360.Models.Salesforce;

namespace Gordon360.Services;

public class SFProfileService(CCTContext context, IConfiguration config, SFProfiles profileProcedures, IAccountService accountService, webSQLContext webSQLContext) : IProfileService
{
    public async Task<StudentProfileViewModel> GetStudentProfileByUsername(string username)
    {
        if (string.IsNullOrEmpty(username)) throw new ResourceNotFoundException() { ExceptionMessage = "username missing or empty" };
        var student = (StudentProfileViewModel)(await profileProcedures.GetAccountByAdUsernameAsync(username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "Account not found" });;
        student = ComposeStudentProfile(student, context.Student.FirstOrDefault(x => x.AD_Username.ToLower() == username.ToLower()));
        return student;
    }

    public async Task<FacultyStaffProfileViewModel> GetFacultyStaffProfileByUsername(string username)
    {
        if (string.IsNullOrEmpty(username)) throw new ResourceNotFoundException() { ExceptionMessage = "username missing or empty" };
        var facstaff = (FacultyStaffProfileViewModel)(await profileProcedures.GetAccountByAdUsernameAsync(username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "Account not found" });;
        facstaff = ComposeFacStaffProfile(facstaff, context.FacStaff.FirstOrDefault(x => x.AD_Username.ToLower() == username.ToLower()));
        return facstaff;
    }

    public async Task<AlumniProfileViewModel> GetAlumniProfileByUsername(string username)
    {
        if (string.IsNullOrEmpty(username)) throw new ResourceNotFoundException() { ExceptionMessage = "username missing or empty" };
        var alumni = (AlumniProfileViewModel)(await profileProcedures.GetAccountByAdUsernameAsync(username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "Account not found" });;
        alumni = ComposeAlumniProfile(alumni, context.Alumni.FirstOrDefault(x => x.AD_Username.ToLower() == username.ToLower()));
        return alumni;
    }

    public async Task<ProfileViewModel> GetProfileByUsername(string username)
    {
        if (string.IsNullOrEmpty(username)) throw new ResourceNotFoundException() { ExceptionMessage = "username missing or empty" };
        var profile = await profileProcedures.GetAccountByAdUsernameAsync(username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "Account not found" };;
        return (ProfileViewModel)profile;
    }

    public async Task<MailboxCombinationViewModel> GetMailboxCombination(string username)
    {
        if (string.IsNullOrEmpty(username)) throw new ResourceNotFoundException() { ExceptionMessage = "username missing or empty" };
        var mailbox = await profileProcedures.GetMailboxCombinationAsync(username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "Account not found" };;
        if (mailbox.gc_Combination__c is null) throw new ResourceNotFoundException() { ExceptionMessage = "No combination!" };;
        return new MailboxCombinationViewModel(mailbox.gc_Combination__c);
    }

    public async Task<DateTime> GetBirthdate(string username)
    {
        var birthdate = await profileProcedures.GetBirthday(username);
        var impossible_birthdate = new DateTime(1800, 1, 1);

        // Test accounts always have current date and time as birthday, so
        // treat this the same as no birthday
        // Comment this out to see "happy birthday" banner in test accounts
        var lifetime = DateTime.Now - birthdate;
        if (lifetime.Days < 1) // no valid user was born within the last 24 hours
        {
            return impossible_birthdate;
        }

        try
        {
            return birthdate;
        }
        catch
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The user's birthdate was invalid." };
        }
    }

    public async Task<IEnumerable<AdvisorViewModel>> GetAdvisorsAsync(string username)
    {
        if (string.IsNullOrEmpty(username)) throw new ResourceNotFoundException() { ExceptionMessage = "username missing or empty" };
        var account =  await GetStudentProfileByUsername(username);
        if (account?.AdvisorIDs is null) throw new ResourceNotFoundException() { ExceptionMessage = "AdvisorIDs missing or empty" };

        List<AdvisorViewModel> resultList = [];
        foreach (var advisorID in account.AdvisorIDs.Split(","))
        {
            var advisor = await accountService.GetAccountByID(advisorID);
            if (advisor is null) continue;
            resultList.Add(new AdvisorViewModel(advisor.FirstName, advisor.LastName, advisor.ADUserName));
        }

        return resultList;
    }

    public CliftonStrengthsViewModel? GetCliftonStrengths(int id)
    {
        return context.Clifton_Strengths.FirstOrDefault(c => c.ID_NUM == id);
    }

    public async Task<bool> ToggleCliftonStrengthsPrivacyAsync(int id)
    {
        var strengths = context.Clifton_Strengths.FirstOrDefault(cs => cs.ID_NUM == id) ?? throw new ResourceNotFoundException { ExceptionMessage = "No Strengths found" };
        strengths.Private = !strengths.Private;
        await context.SaveChangesAsync();

        return strengths.Private;
    }

    public IEnumerable<EmergencyContactViewModel> GetEmergencyContact(string username)
    {
        var result = context.EmergencyContact.Where(x => x.AD_Username == username).Select(x => (EmergencyContactViewModel)x);

        if (result == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "No emergency contacts found." };
        }

        return result;
    }

    public async Task<PhotoPathViewModel?> GetPhotoPathAsync(string username)
    {
        var account = await accountService.GetAccountByUsername(username);
        if (account is null) return null;

        var photoInfoList = await context.Procedures.PHOTO_INFO_PER_USER_NAMEAsync(int.Parse(account.GordonID));
        return photoInfoList.Select(p => new PhotoPathViewModel { Img_Name = p.Img_Name, Img_Path = p.Img_Path, Pref_Img_Name = p.Pref_Img_Name, Pref_Img_Path = p.Pref_Img_Path }).FirstOrDefault();
    }

    public ProfileCustomViewModel? GetCustomUserInfo(string username)
    {
        return context.CUSTOM_PROFILE.Find(username);
    }

    public async Task UpdateProfileImageAsync(string username, string? path, string? name)
    {
        var account = await accountService.GetAccountByUsername(username) ?? throw new ResourceNotFoundException { ExceptionMessage = "The account was not found" };
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

    public async Task UpdateMobilePrivacyAsync(string username, string value)
    {
        var account = await accountService.GetAccountByUsername(username) ?? throw new ResourceNotFoundException { ExceptionMessage = "The account was not found" };
        await context.Procedures.UPDATE_PHONE_PRIVACYAsync(int.Parse(account.GordonID), value);
        // Update value in cached data
        var student = context.Student.FirstOrDefault(x => x.ID == account.GordonID);
        if (student != null)
        {
            student.IsMobilePhonePrivate = value == "Y" ? 1 : 0;
        }

        context.SaveChanges();
    }

    public async Task<StudentProfileViewModel> UpdateMobilePhoneNumberAsync(string username, string newMobilePhoneNumber)
    {
        var profile = await GetStudentProfileByUsername(username);
        var digitsOnly = Regex.Replace(newMobilePhoneNumber, @"[^\d]", "");
        await context.Procedures.UPDATE_CELL_PHONEAsync(profile.ID, digitsOnly);
        return profile;
    }

    public async Task<FacultyStaffProfileViewModel> UpdateOfficeLocationAsync(string username, string newBuilding, string newRoom)
    {
        var profile = await GetFacultyStaffProfileByUsername(username);
        var user = webSQLContext.accounts.FirstOrDefault(a => a.AD_Username == username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "The webSQL account was not found" };
        user.Building = newBuilding;
        user.Room = newRoom;
        await webSQLContext.SaveChangesAsync();

        // Get updated profile
        profile = await GetFacultyStaffProfileByUsername(username);

        return profile;
    }

    public async Task<FacultyStaffProfileViewModel> UpdateOfficeHoursAsync(string username, string newHours)
    {
        var profile = await GetFacultyStaffProfileByUsername(username);
        var acccount = webSQLContext.accounts.FirstOrDefault(a => a.AD_Username == username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found" };
        var user = webSQLContext.account_profiles.FirstOrDefault(a => a.account_id == acccount.account_id) ?? throw new ResourceNotFoundException() { ExceptionMessage = "The user was not found" };
        user.office_hours = newHours;
        await webSQLContext.SaveChangesAsync();

        // Get updated profile
        profile = await GetFacultyStaffProfileByUsername(username);

        return profile;
    }

    public async Task<FacultyStaffProfileViewModel> UpdateMailStopAsync(string username, string newMail)
    {
        var profile = await GetFacultyStaffProfileByUsername(username);
        var user = webSQLContext.accounts.FirstOrDefault(a => a.AD_Username == username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "The user was not found" };
        user.mail_server = newMail;
        await webSQLContext.SaveChangesAsync();

        // Get updated profile
        profile = await GetFacultyStaffProfileByUsername(username);

        return profile;
    }

    public async Task UpdateImagePrivacyAsync(string username, string value)
    {
        var account = await accountService.GetAccountByUsername(username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found" };

        await context.Procedures.UPDATE_SHOW_PICAsync(account.account_id, value);
        // Update value in cached data
        var student = context.Student.FirstOrDefault(x => x.ID == account.GordonID);
        var facStaff = context.FacStaff.FirstOrDefault(x => x.ID == account.GordonID);
        var alum = context.Alumni.FirstOrDefault(x => x.ID == account.GordonID);
        if (student != null)
        {
            student.show_pic = value == "Y" ? 1 : 0;
        }
        else if (facStaff != null)
        {
            facStaff.show_pic = value == "Y" ? 1 : 0;
        }
        else if (alum != null)
        {
            alum.show_pic = value == "Y" ? 1 : 0;
        }

        context.SaveChanges();
    }

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
            personType += "stu";
        }

        if (alumni != null)
        {
            MergeProfile(profile, JObject.FromObject(alumni));
            personType += "alu";
        }

        if (faculty != null)
        {
            MergeProfile(profile, JObject.FromObject(faculty));
            personType += "fac";
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
        var account = await accountService.GetAccountByUsername(username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found" };

        string from_email = config["Emails:Sender:Username"] ?? throw new ResourceNotFoundException() { ExceptionMessage = "Email sender not found" };
        string to_email = config["Emails:AlumniProfileUpdateRequestApprover"] ?? throw new ResourceNotFoundException() { ExceptionMessage = "Email recipient not found" };
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
            Host = config["SmtpHost"] ?? throw new ResourceNotFoundException() { ExceptionMessage = "SMTP Host not found" },
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
    /// Fill missing data in salesforce profile using CCT account.
    /// Should become obsolete once migration is complete.
    /// </summary>
    /// <param name="viewModel">Salesforce profile</param>
    /// <param name="dbView">CCT Profile</param>
    /// <returns>Filled-out profile</returns>
    private static StudentProfileViewModel ComposeStudentProfile(StudentProfileViewModel viewModel, Student? dbView)
    {
        var account = viewModel;
        if (dbView == null) return account;
        // cctAccount will only be null if dbView is null
        StudentProfileViewModel cctAccount = dbView!;
        foreach (var prop in account.GetType().GetFields())
        {
            if (prop.GetValue(account) is null) prop.SetValue(account, prop.GetValue(cctAccount));
        }
        return account;
    }

    /// <summary>
    /// Fill missing data in salesforce profile using CCT account.
    /// Should become obsolete once migration is complete.
    /// </summary>
    /// <param name="viewModel">Salesforce profile</param>
    /// <param name="dbView">CCT Profile</param>
    /// <returns>Filled-out profile</returns>
    private static FacultyStaffProfileViewModel ComposeFacStaffProfile(FacultyStaffProfileViewModel viewModel, FacStaff? dbView)
    {
        var account = viewModel;
        if (dbView == null) return account;
        // cctAccount will only be null if dbView is null
        FacStaff cctAccount = dbView!;
        foreach (var prop in account.GetType().GetFields())
        {
            if (prop.GetValue(account) is null) prop.SetValue(account, prop.GetValue(cctAccount));
        }
        return account;
    }

    /// <summary>
    /// Fill missing data in salesforce profile using CCT account.
    /// Should become obsolete once migration is complete.
    /// </summary>
    /// <param name="viewModel">Salesforce profile</param>
    /// <param name="dbView">CCT Profile</param>
    /// <returns>Filled-out profile</returns>
    private static AlumniProfileViewModel ComposeAlumniProfile(AlumniProfileViewModel viewModel, Alumni? dbView)
    {
        var account = viewModel;
        if (dbView == null) return account;
        // cctAccount will only be null if dbView is null
        AlumniProfileViewModel cctAccount = dbView!;
        foreach (var prop in account.GetType().GetFields())
        {
            if (prop.GetValue(account) is null) prop.SetValue(account, prop.GetValue(cctAccount));
        }
        return account;
    }

}
