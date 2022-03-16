using Gordon360.Models.CCT;
using Gordon360.Models.MyGordon;
using Gordon360.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Gordon360.Controllers.WellnessController;

// <summary>
// Namespace with all the Service Interfaces that are to be implemented. I don't think making this interface is required, the services can work fine on their own.
// However, building the interfaces first does give a general sense of structure to their implementations. A certain cohesiveness :p.
// </summary>
namespace Gordon360.Services
{
    public interface IRoleCheckingService
    {
        string GetCollegeRole(string username);
    }
    public interface IProfileService
    {
        StudentProfileViewModel? GetStudentProfileByUsername(string username);
        FacultyStaffProfileViewModel? GetFacultyStaffProfileByUsername(string username);
        AlumniProfileViewModel? GetAlumniProfileByUsername(string username);
        MailboxViewModel GetMailboxCombination(string username);
        DateTime GetBirthdate(string username);
        Task<IEnumerable<AdvisorViewModel>> GetAdvisors(string username);
        CliftonStrengthsViewModel GetCliftonStrengths(int id);
        IEnumerable<EmergencyContactViewModel> GetEmergencyContact(string username);
        ProfileCustomViewModel? GetCustomUserInfo(string username);
        Task<PhotoPathViewModel> GetPhotoPath(string username);
        void UpdateProfileLink(string username, string type, CUSTOM_PROFILE path);
        StudentProfileViewModel UpdateMobilePhoneNumber(StudentProfileViewModel profile);
        void UpdateMobilePrivacy(string username, string value);
        void UpdateImagePrivacy(string username, string value);
        void UpdateProfileImage(string username, string path, string name);
    }

    public interface IEventService
    {
        Task<IEnumerable<AttendedEventViewModel>> GetEventsForStudentByTerm(string username, string term);
        IEnumerable<EventViewModel> GetAllEvents();
        IEnumerable<EventViewModel> GetPublicEvents();
        IEnumerable<EventViewModel> GetCLAWEvents();
        IEnumerable<DEPRECATED_AttendedEventViewModel> DEPRECATED_GetEventsForStudentByTerm(string id, string term);
        IEnumerable<DEPRECATED_EventViewModel> DEPRECATED_GetAllEvents();
        IEnumerable<DEPRECATED_EventViewModel> DEPRECATED_GetPublicEvents();
        IEnumerable<DEPRECATED_EventViewModel> DEPRECATED_GetCLAWEvents();
    }

    public interface IDiningService
    {
        DiningViewModel GetDiningPlanInfo(int id, string sessionCode);
        string GetBalance(int cardHolderID, string planID);
    }

    public interface IAccountService
    {
        AccountViewModel? GetAccountByID(string id);
        IEnumerable<AccountViewModel> GetAll();
        AccountViewModel? GetAccountByEmail(string email);
        AccountViewModel? GetAccountByUsername(string username);
    }

    public interface IWellnessService
    {
        WellnessViewModel GetStatus(string username);
        WellnessQuestionViewModel GetQuestion();
        Health_Status PostStatus(WellnessStatusColor status, string username);
    }

    public interface IDirectMessageService
    {
        CreateGroupViewModel CreateGroup(String name, bool group, string image, List<String> usernames, SendTextViewModel initialMessage, string userId);
        bool SendMessage(SendTextViewModel textInfo, string user_id);
        bool StoreUserRooms(String userId, String roomId);
        bool StoreUserConnectionIds(String userId, String connectionId);
        bool DeleteUserConnectionIds(String connectionId);
        List<IEnumerable<ConnectionIdViewModel>> GetUserConnectionIds(List<String> userIds);
        IEnumerable<MessageViewModel> GetMessages(string roomId);
        IEnumerable<GroupViewModel> GetRooms(string userId);
        List<Object> GetRoomById(string userId);
        MessageViewModel GetSingleMessage(string messageID, string roomID);
        object GetSingleRoom(int roomId);
    }

    public interface IActivityService
    {
        ActivityInfoViewModel Get(string activityCode);
        Task<IEnumerable<ActivityInfoViewModel>> GetActivitiesForSession(string sessionCode);
        Task<IEnumerable<string>> GetActivityTypesForSession(string sessionCode);
        IEnumerable<ActivityInfoViewModel> GetAll();
        bool IsOpen(string activityCode, string sessionCode);
        IEnumerable<string> GetOpenActivities(string sess_cde);
        IEnumerable<string> GetOpenActivities(string sess_cde, int gordonID);
        IEnumerable<string> GetClosedActivities(string sess_cde);
        IEnumerable<string> GetClosedActivities(string sess_cde, int gordonID);
        ACT_INFO Update(string activityCode, ACT_INFO activity);
        void CloseOutActivityForSession(string activityCode, string sess_cde);
        void OpenActivityForSession(string activityCode, string sess_cde);
        void UpdateActivityImage(string activityCode, string path);
        void ResetActivityImage(string activityCode);
        void TogglePrivacy(string activityCode, bool isPrivate);
    }
    public interface IVictoryPromiseService
    {
        Task<IEnumerable<VictoryPromiseViewModel>> GetVPScores(string username);
    }
    public interface IStudentEmploymentService
    {
        Task<IEnumerable<StudentEmploymentViewModel>> GetEmployment(string username);
    }

    public interface IActivityInfoService
    {
        ActivityInfoViewModel Get(string username);
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
        Task<IEnumerable<EmailViewModel>> GetEmailsForGroupAdmin(string activityCode);
        Task<IEnumerable<EmailViewModel>> GetEmailsForActivityLeaders(string id);
        Task<IEnumerable<EmailViewModel>> GetEmailsForActivityAdvisors(string id);
        Task<IEnumerable<EmailViewModel>> GetEmailsForActivity(string activityCode);
        // Get emails for some other session
        Task<IEnumerable<EmailViewModel>> GetEmailsForGroupAdmin(string activityCode, string sessionCode);
        Task<IEnumerable<EmailViewModel>> GetEmailsForActivityLeaders(string activityCode, string sessionCode);
        Task<IEnumerable<EmailViewModel>> GetEmailsForActivityAdvisors(string activityCode, string sessionCode);
        Task<IEnumerable<EmailViewModel>> GetEmailsForActivity(string activityCode, string sessionCode);
        // Send emails
        void SendEmails(string[] to_emails, string to_email, string subject, string email_content, string password);
        Task SendEmailToActivity(string activityCode, string sessionCode, string from_email, string subject, string email_content, string password);
    }

    public interface IErrorLogService
    {
        ERROR_LOG Add(ERROR_LOG error_log);
        ERROR_LOG Log(string error_message);
    }

    public interface ISessionService
    {
        SessionViewModel Get(string sessionCode);
        IEnumerable<SessionViewModel> GetAll();
    }

    public interface IJenzibarActivityService
    {
        JNZB_ACTIVITIES Get(int id);
        IEnumerable<JNZB_ACTIVITIES> GetAll();
    }


    public interface IMembershipService
    {
        Task<IEnumerable<MembershipViewModel>> GetLeaderMembershipsForActivity(string activityCode);
        Task<IEnumerable<MembershipViewModel>> GetAdvisorMembershipsForActivity(string activityCode);
        Task<IEnumerable<MembershipViewModel>> GetGroupAdminMembershipsForActivity(string activityCode);
        Task<IEnumerable<MembershipViewModel>> GetMembershipsForActivity(string activityCode);
        Task<IEnumerable<MembershipViewModel>> GetMembershipsForStudent(string username);
        Task<int> GetActivityFollowersCountForSession(string activityCode, string sessionCode);
        Task<int> GetActivityMembersCountForSession(string activityCode, string sessionCode);
        Task<IEnumerable<MembershipViewModel>> GetAll();
        MEMBERSHIP GetSpecificMembership(int membershipID);
        Task<int> GetActivityFollowersCount(string idactivityCode);
        Task<int> GetActivityMembersCount(string activityCode);
        MEMBERSHIP Add(MEMBERSHIP membership);
        MEMBERSHIP Update(int membershipID, MEMBERSHIP membership);
        MEMBERSHIP ToggleGroupAdmin(int membershipID, MEMBERSHIP membership);
        void TogglePrivacy(int membershipID, bool isPrivate);
        MEMBERSHIP Delete(int membershipID);
        Boolean IsGroupAdmin(int gordonID);
    }

    public interface IJobsService
    {
        IEnumerable<StudentTimesheetsViewModel> getSavedShiftsForUser(int ID_NUM);
        void saveShiftForUser(int studentID, int jobID, DateTime shiftStart, DateTime shiftEnd, string hoursWorked, string shiftNotes, string lastChangedBy);
        StudentTimesheetsViewModel editShift(int rowID, DateTime shiftStart, DateTime shiftEnd, string hoursWorked, string username);
        void deleteShiftForUser(int rowID, int studentID);
        void submitShiftForUser(int studentID, int jobID, DateTime shiftEnd, int submittedTo, string lastChangedBy);
        Task<IEnumerable<SupervisorViewModel>> getsupervisorNameForJob(int supervisorID);
        Task<IEnumerable<ActiveJobViewModel>> getActiveJobs(DateTime shiftStart, DateTime shiftEnd, int studentID);
        IEnumerable<OverlappingShiftIdViewModel> editShiftOverlapCheck(int studentID, DateTime shiftStart, DateTime shiftEnd, int rowID);
        IEnumerable<OverlappingShiftIdViewModel> checkForOverlappingShift(int studentID, DateTime shiftStart, DateTime shiftEnd);
        IEnumerable<ClockInViewModel> ClockOut(string id);
        ClockInViewModel ClockIn(bool state, string id);
        ClockInViewModel DeleteClockIn(string id);
    }

    public interface IParticipationService
    {
        ParticipationViewModel Get(string id);
        IEnumerable<ParticipationViewModel> GetAll();
    }

    public interface IMembershipRequestService
    {
        Task<MembershipRequestViewModel> Get(int requestID);
        Task<IEnumerable<MembershipRequestViewModel>> GetAll();
        Task<IEnumerable<MembershipRequestViewModel>> GetMembershipRequestsForActivity(string activityCode);
        Task<IEnumerable<MembershipRequestViewModel>> GetMembershipRequestsForStudent(string gordonID);
        REQUEST Add(REQUEST membershipRequest);
        REQUEST Update(int requestID, REQUEST membershipRequest);
        // The ODD one out. When we approve a request, we would like to get back the new membership.
        MEMBERSHIP ApproveRequest(int requestID);
        REQUEST DenyRequest(int requestID);
        REQUEST Delete(int requestID);
    }
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleViewModel>> GetScheduleStudent(string username);
        Task<IEnumerable<ScheduleViewModel>> GetScheduleFaculty(string username);
        bool CanReadStudentSchedules(string username);
    }

    public interface IScheduleControlService
    {
        Task UpdateSchedulePrivacy(string username, string value);
        Task UpdateDescription(string username, string value);
        Task UpdateModifiedTimeStamp(string username, DateTime value);
    }


    public interface IMyScheduleService
    {
        MYSCHEDULE GetForID(string eventID, string username);
        IEnumerable<MYSCHEDULE> GetAllForUser(string username);
        MYSCHEDULE Add(MYSCHEDULE myschedule);
        MYSCHEDULE Update(MYSCHEDULE myschedule);
        MYSCHEDULE Delete(string eventID, string username);
    }

    public interface ISaveService
    {
        IEnumerable<UPCOMING_RIDESResult> GetUpcoming(string gordon_id);
        IEnumerable<UPCOMING_RIDES_BY_STUDENT_IDResult> GetUpcomingForUser(string gordon_id);
        Task<Save_Rides> AddRide(Save_Rides newRide, string gordon_id);
        Task<Save_Rides> DeleteRide(string rideID, string gordon_id);
        Task<int> CancelRide(string rideID, string gordon_id);
        Task<Save_Bookings> AddBooking(Save_Bookings newBooking);
        Task<Save_Bookings> DeleteBooking(string rideID, string gordon_id);
    }

    public interface IContentManagementService
    {
        IEnumerable<SliderViewModel> DEPRECATED_GetSliderContent();
        IEnumerable<Slider_Images> GetBannerSlides();
        Slider_Images AddBannerSlide(BannerSlidePostViewModel slide, string serverURL);
        Slider_Images DeleteBannerSlide(int slideID);
    }

    public interface INewsService
    {
        StudentNews Get(int newsID);
        Task<IEnumerable<StudentNewsViewModel>> GetNewsNotExpired();
        Task<IEnumerable<StudentNewsViewModel>> GetNewsNew();
        IEnumerable<StudentNewsCategoryViewModel> GetNewsCategories();
        Task<IEnumerable<StudentNewsViewModel>> GetNewsPersonalUnapproved(string username);
        StudentNews SubmitNews(StudentNews newsItem, string username);
        StudentNews DeleteNews(int newsID);
        StudentNewsViewModel EditPosting(int newsID, StudentNews newsItem);
    }

    public interface IHousingService
    {
        bool CheckIfHousingAdmin(string userID);
        bool AddHousingAdmin(string id);
        bool RemoveHousingAdmin(string id);
        bool DeleteApplication(int applicationID);
        string[] GetAllApartmentHalls();
        string GetEditorUsername(int applicationID);
        int? GetApplicationID(string username, string sess_cde);
        ApartmentApplicationViewModel GetApartmentApplication(int applicationID, bool isAdmin = false);
        ApartmentApplicationViewModel[] GetAllApartmentApplication();
        int SaveApplication(string sess_cde, string editorUsername, List<ApartmentApplicantViewModel> apartmentApplicants, List<ApartmentChoiceViewModel> apartmentChoices);
        int EditApplication(string username, string sess_cde, int applicationID, string newEditorUsername, List<ApartmentApplicantViewModel> newApartmentApplicants, List<ApartmentChoiceViewModel> newApartmentChoices);
        bool ChangeApplicationEditor(string username, int applicationID, string newEditorUsername);
        bool ChangeApplicationDateSubmitted(int applicationID);
    }

    public interface IAcademicCheckInService
    {
        AcademicCheckInViewModel PutCellPhone(string id, AcademicCheckInViewModel data);
        EmergencyContactViewModel PutEmergencyContact(EmergencyContactViewModel data, string id, string username);
        IEnumerable<AcademicCheckInViewModel> GetHolds(string id);
        void SetStatus(string id);
        AcademicCheckInViewModel PutDemographic(string id, AcademicCheckInViewModel data);
        Boolean GetStatus(string id);
    }

}
