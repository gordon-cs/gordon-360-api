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

public class ProfileService(CCTContext context, IConfiguration config, SFProfiles profileProcedures, IAccountService accountService, webSQLContext webSQLContext) : IProfileService
{
    public async Task<StudentProfileViewModel?> GetStudentProfileByUsername(string username)
    {
        var student = await profileProcedures.GetStudentProfile(username);
        return student;
    }

    public async Task<FacultyStaffProfileViewModel?> GetFacultyStaffProfileByUsername(string username)
    {
        var facStaff = await profileProcedures.GetFacStaffProfile(username);
        return facStaff; // context.FacStaff.FirstOrDefault(x => x.AD_Username.ToLower() == username.ToLower());
    }

    public async Task<AlumniProfileViewModel?> GetAlumniProfileByUsername(string username)
    {
        var alumni = await profileProcedures.GetAlumniProfile(username);
        return alumni; // context.Alumni.FirstOrDefault(x => x.AD_Username.ToLower() == username.ToLower());
    }

    public MailboxCombinationViewModel? GetMailboxCombination(string username)
    {
        return context.Mailboxes
            .Where(m => m.HolderUsername == username)
            .Select(m => m.Combination)
            .Select(MailboxCombinationViewModel.From)
            .FirstOrDefault();
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

    public async Task<IEnumerable<AdvisorViewModel>?> GetAdvisorsAsync(string username)
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

    public CliftonStrengthsViewModel? GetCliftonStrengths(int id)
    {
        return context.Clifton_Strengths.FirstOrDefault(c => c.ID_NUM == id);
    }

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
        var account = accountService.GetAccountByUsername(username);

        var photoInfoList = await context.Procedures.PHOTO_INFO_PER_USER_NAMEAsync(int.Parse(account.GordonID));
        return photoInfoList.Select(p => new PhotoPathViewModel { Img_Name = p.Img_Name, Img_Path = p.Img_Path, Pref_Img_Name = p.Pref_Img_Name, Pref_Img_Path = p.Pref_Img_Path }).FirstOrDefault();
    }

    public ProfileCustomViewModel? GetCustomUserInfo(string username)
    {
        return context.CUSTOM_PROFILE.Find(username);
    }

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

    public async Task<StudentProfileViewModel> UpdateMobilePhoneNumberAsync(string username, string newMobilePhoneNumber)
    {
        var profile = await GetStudentProfileByUsername(username);
        if (profile == null)
        {
            throw new ResourceNotFoundException { ExceptionMessage = "The account was not found" };
        
        }
        var digitsOnly = Regex.Replace(newMobilePhoneNumber, @"[^\d]", "");
        await context.Procedures.UPDATE_CELL_PHONEAsync(profile.ID, digitsOnly);
        return profile;
    }

    public async Task<FacultyStaffProfileViewModel> UpdateOfficeLocationAsync(string username, string newBuilding, string newRoom)
    {
        var profile = await GetFacultyStaffProfileByUsername(username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found" };
        var user = webSQLContext.accounts.FirstOrDefault(a => a.AD_Username == username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "The webSQL account was not found" };
        user.Building = newBuilding;
        user.Room = newRoom;
        await webSQLContext.SaveChangesAsync();

        // Get updated profile
        profile = await GetFacultyStaffProfileByUsername(username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found" };

        return profile;
    }

    public async Task<FacultyStaffProfileViewModel> UpdateOfficeHoursAsync(string username, string newHours)
    {
        var profile = await GetFacultyStaffProfileByUsername(username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found" };
        var acccount = webSQLContext.accounts.FirstOrDefault(a => a.AD_Username == username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found" };
        var user = webSQLContext.account_profiles.FirstOrDefault(a => a.account_id == acccount.account_id) ?? throw new ResourceNotFoundException() { ExceptionMessage = "The user was not found" };
        user.office_hours = newHours;
        await webSQLContext.SaveChangesAsync();

        // Get updated profile
        profile = await GetFacultyStaffProfileByUsername(username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found" };

        return profile;
    }

    public async Task<FacultyStaffProfileViewModel> UpdateMailStopAsync(string username, string newMail)
    {
        var profile = await GetFacultyStaffProfileByUsername(username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found" };
        var user = webSQLContext.accounts.FirstOrDefault(a => a.AD_Username == username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "The user was not found" };
        user.mail_server = newMail;
        await webSQLContext.SaveChangesAsync();

        // Get updated profile
        profile = await GetFacultyStaffProfileByUsername(username) ?? throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found" };

        return profile;
    }

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
}
