using System;
using System.Xml.Linq;
using System.Collections.Generic;
using Gordon360.Models;
using Gordon360.Models.ViewModels;

/// <summary>
/// Namespace with all the Service Interfaces that are to be implemented. I don't think making this interface is required, the services can work find on their own.
/// However, building the interfaces first does give a general sense of structure to their implementations. A certian cohesiveness :p.
/// </summary>
namespace Gordon360.Services
{

    public interface IRoleCheckingService
    {
        string getCollegeRole(string username);
    }
    public interface IProfileService
    {
        StudentProfileViewModel GetStudentProfileByUsername(string username);
        FacultyStaffProfileViewModel GetFacultyStaffProfileByUsername(string username);
        AlumniProfileViewModel GetAlumniProfileByUsername(string username);
        ProfileCustomViewModel GetCustomUserInfo(string username);
        PhotoPathViewModel GetPhotoPath(string id);
        void UpdateProfileLink(string username, string type, CUSTOM_PROFILE path);
        void UpdateMobilePrivacy(string id, string value);
        void UpdateImagePrivacy(string id, string value);
        void UpdateProfileImage(string id, string path, string name);
    }

    public interface IEventService
    {
        IEnumerable<AttendedEventViewModel> GetAllForStudent(string id);
        IEnumerable<AttendedEventViewModel> GetEventsForStudentByTerm(string id, string term);
        IEnumerable<EventViewModel> GetSpecificEvents(string Event_ID, string type);
        IEnumerable<EventViewModel> GetAllEvents(XDocument xmlDoc);
    }

    public interface IDiningService
    {
        DiningViewModel GetDiningPlanInfo(int id, string sessionCode);
    }

    public interface IAccountService
    {
        AccountViewModel Get(string id);
        IEnumerable<AccountViewModel> GetAll();
        AccountViewModel GetAccountByEmail(string email);
        AccountViewModel GetAccountByUsername(string username);
    }

    public interface IActivityService
    {
        ActivityInfoViewModel Get(string id);
        IEnumerable<ActivityInfoViewModel> GetActivitiesForSession(string id);
        IEnumerable<String> GetActivityTypesForSession(string id);
        IEnumerable<ActivityInfoViewModel> GetAll();
        bool IsOpen(string id, string sessionCode);
        IEnumerable<string> GetOpenActivities(string sess_cde);
        IEnumerable<string> GetOpenActivities(string sess_cde, int id);
        IEnumerable<string> GetClosedActivities(string sess_cde);
        IEnumerable<string> GetClosedActivities(string sess_cde, int id);
        ACT_INFO Update(string id, ACT_INFO activity);
        void CloseOutActivityForSession(string id, string sess_cde);
        void OpenActivityForSession(string id, string sess_cde);
        void UpdateActivityImage(string id, string path);
        void ResetActivityImage(string id);
        void TogglePrivacy(string id, bool p);
    }

    public interface IActivityInfoService
    {
        ActivityInfoViewModel Get(string id);
        IEnumerable<ActivityInfoViewModel> GetAll();
    }
    public interface IAdministratorService
    {
        ADMIN Get(int id);
        ADMIN Get(string gordon_id);
        IEnumerable<ADMIN> GetAll();
        ADMIN Add(ADMIN admin);
        ADMIN Delete(int id);
    }
    public interface IEmailService
    {
        // Get emails for the current session.
        IEnumerable<EmailViewModel> GetEmailsForGroupAdmin(string id);
        IEnumerable<EmailViewModel> GetEmailsForActivityLeaders(string id);
        IEnumerable<EmailViewModel> GetEmailsForActivityAdvisors(string id);
        IEnumerable<EmailViewModel> GetEmailsForActivity(string id);
        // Get emails for some other session
        IEnumerable<EmailViewModel> GetEmailsForGroupAdmin(string id, string session_code);
        IEnumerable<EmailViewModel> GetEmailsForActivityLeaders(string activity_code, string session_code);
        IEnumerable<EmailViewModel> GetEmailsForActivityAdvisors(string activity_code, string session_code);
        IEnumerable<EmailViewModel> GetEmailsForActivity(string activity_code, string session_code);
        // Send emails
        void SendEmails(string [] to_emails, string to_email, string subject, string email_content, string password);
        void SendEmailToActivity(string activityCode, string sessionCode, string from_email, string subject, string email_content, string password);

    }
    public interface ISessionService
    {
        SessionViewModel Get(string id);
        IEnumerable<SessionViewModel> GetAll();
    }

    public interface IJenzibarActivityService
    {
        JNZB_ACTIVITIES Get(int id);
        IEnumerable<JNZB_ACTIVITIES> GetAll();
    }

    public interface IMembershipService
    {
        MembershipViewModel Get(int id);
        IEnumerable<MembershipViewModel> GetLeaderMembershipsForActivity(string id);
        IEnumerable<MembershipViewModel> GetAdvisorMembershipsForActivity(string id);
        IEnumerable<MembershipViewModel> GetGroupAdminMembershipsForActivity(string id);
        IEnumerable<MembershipViewModel> GetMembershipsForActivity(string id);
        IEnumerable<MembershipViewModel> GetMembershipsForStudent(string id);
        int GetActivityFollowersCountForSession(string id, string sess_cde);
        int GetActivityMembersCountForSession(string id, string sess_cde);
        IEnumerable<MembershipViewModel> GetAll();
        int GetActivityFollowersCount(string id);
        int GetActivityMembersCount(string id);
        MEMBERSHIP Add(MEMBERSHIP membership);
        MEMBERSHIP Update(int id, MEMBERSHIP membership);
        MEMBERSHIP ToggleGroupAdmin(int id, MEMBERSHIP membership);
        void TogglePrivacy(int id, bool p);
        MEMBERSHIP Delete(int id);
    }

    public interface IParticipationService
    {
        ParticipationViewModel Get(string id);
        IEnumerable<ParticipationViewModel> GetAll();
    }

    public interface IMembershipRequestService
    {
        MembershipRequestViewModel Get(int id);
        IEnumerable<MembershipRequestViewModel> GetAll();
        IEnumerable<MembershipRequestViewModel> GetMembershipRequestsForActivity(string id);
        IEnumerable<MembershipRequestViewModel> GetMembershipRequestsForStudent(string id);
        REQUEST Add(REQUEST membershipRequest);
        REQUEST Update(int id, REQUEST membershipRequest);
        // The ODD one out. When we approve a request, we would like to get back the new membership.
        MEMBERSHIP ApproveRequest(int id);
        REQUEST DenyRequest(int id);
        REQUEST Delete(int id);
    }

    public interface IContentManagementService
    {
        IEnumerable<SliderViewModel> GetSliderContent();
    }

}

