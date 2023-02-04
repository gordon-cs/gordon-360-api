using Gordon360.Authorization;
using Gordon360.Models.CCT;
using Gordon360.Models.MyGordon;
using Gordon360.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Gordon360.Controllers.WellnessController;
using static Gordon360.Services.MembershipService;

// <summary>
// Namespace with all the Service Interfaces that are to be implemented. I don't think making this interface is required, the services can work fine on their own.
// However, building the interfaces first does give a general sense of structure to their implementations. A certain cohesiveness :p.
// </summary>
namespace Gordon360.Services
{
    public interface IProfileService
    {
        StudentProfileViewModel? GetStudentProfileByUsername(string username);
        FacultyStaffProfileViewModel? GetFacultyStaffProfileByUsername(string username);
        AlumniProfileViewModel? GetAlumniProfileByUsername(string username);
        MailboxViewModel GetMailboxCombination(string username);
        DateTime GetBirthdate(string username);
        Task<IEnumerable<AdvisorViewModel>> GetAdvisorsAsync(string username);
        CliftonStrengthsViewModel? GetCliftonStrengths(int id);
        Task<bool> ToggleCliftonStrengthsPrivacyAsync(int id);
        IEnumerable<EmergencyContactViewModel> GetEmergencyContact(string username);
        ProfileCustomViewModel? GetCustomUserInfo(string username);
        Task<PhotoPathViewModel?> GetPhotoPathAsync(string username);
        Task UpdateProfileLinkAsync(string username, string type, CUSTOM_PROFILE path);
        Task<StudentProfileViewModel> UpdateMobilePhoneNumberAsync(string username, string newMobilePhoneNumber);
        Task UpdateMobilePrivacyAsync(string username, string value);
        Task UpdateImagePrivacyAsync(string username, string value);
        Task UpdateProfileImageAsync(string username, string path, string name);
        ProfileViewModel? ComposeProfile(object? student, object? alumni, object? faculty, object? customInfo);
        Task InformationChangeRequest(string username, ProfileFieldViewModel[] updatedField);
    }

    public interface IAddressesService
    {
        IEnumerable<States> GetAllStates();
        IEnumerable<CountryViewModel> GetAllCountries();
    }

    public interface IEventService
    {
        IEnumerable<AttendedEventViewModel> GetEventsForStudentByTerm(string username, string term);
        IEnumerable<EventViewModel> GetAllEvents();
        IEnumerable<EventViewModel> GetPublicEvents();
        IEnumerable<EventViewModel> GetCLAWEvents();
    }

    public interface IDiningService
    {
        DiningViewModel GetDiningPlanInfo(int id, string sessionCode);
    }

    public interface IAccountService
    {
        AccountViewModel GetAccountByID(string id);
        IEnumerable<AccountViewModel> GetAll();
        AccountViewModel GetAccountByEmail(string email);
        AccountViewModel GetAccountByUsername(string username);
        IEnumerable<AdvancedSearchViewModel> GetAccountsToSearch(List<string> accountTypes, IEnumerable<AuthGroup> authGroups, string? homeCity);
        IEnumerable<AdvancedSearchViewModel> AdvancedSearch(
            IEnumerable<AdvancedSearchViewModel> accounts,
            string? firstname,
            string? lastname,
            string? major,
            string? minor,
            string? hall,
            string? classType,
            string? homeCity,
            string? state,
            string? country,
            string? department,
            string? building);
        Task<IEnumerable<BasicInfoViewModel>> GetAllBasicInfoAsync();
        Task<IEnumerable<BasicInfoViewModel>> GetAllBasicInfoExceptAlumniAsync();
    }

    public interface IWellnessService
    {
        WellnessViewModel GetStatus(string username);
        WellnessQuestionViewModel GetQuestion();
        WellnessViewModel PostStatus(WellnessStatusColor status, string username);
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
        Task<IEnumerable<ActivityInfoViewModel>> GetActivitiesForSessionAsync(string sessionCode);
        Task<IEnumerable<string>> GetActivityTypesForSessionAsync(string sessionCode);
        IEnumerable<ActivityInfoViewModel> GetAll();
        bool IsOpen(string activityCode, string sessionCode);
        IEnumerable<string> GetOpenActivities(string sess_cde);
        IEnumerable<string> GetOpenActivities(string sess_cde, int gordonID);
        IEnumerable<string> GetClosedActivities(string sess_cde);
        IEnumerable<string> GetClosedActivities(string sess_cde, int gordonID);
        ACT_INFO Update(string activityCode, InvolvementUpdateViewModel activity);
        void CloseOutActivityForSession(string activityCode, string sess_cde);
        void OpenActivityForSession(string activityCode, string sess_cde);
        Task<ACT_INFO> UpdateActivityImageAsync(ACT_INFO involvement, IFormFile image);
        void ResetActivityImage(string activityCode);
        void TogglePrivacy(string activityCode, bool isPrivate);
    }
    public interface IVictoryPromiseService
    {
        Task<IEnumerable<VictoryPromiseViewModel>> GetVPScoresAsync(string username);
    }
    public interface IStudentEmploymentService
    {
        Task<IEnumerable<StudentEmploymentViewModel>> GetEmploymentAsync(string username);
    }

    public interface IActivityInfoService
    {
        ActivityInfoViewModel Get(string username);
        IEnumerable<ActivityInfoViewModel> GetAll();
    }

    public interface IAdministratorService
    {
        IEnumerable<AdminViewModel?> GetAll();
        AdminViewModel? GetByUsername(string username);
        AdminViewModel Add(AdminViewModel admin);
        AdminViewModel Delete(string username);
    }

    public interface IEmailService
    {
        Task<IEnumerable<EmailViewModel>> GetEmailsForActivityAsync(string activityCode, string? sessionCode, ParticipationType? participationType);
        void SendEmails(string[] to_emails, string to_email, string subject, string email_content, string password);
        Task SendEmailToActivityAsync(string activityCode, string sessionCode, string from_email, string subject, string email_content, string password);
    }

    public interface IErrorLogService
    {
        ERROR_LOG Add(ERROR_LOG error_log);
        ERROR_LOG Log(string error_message);
    }

    public interface ISessionService
    {
        SessionViewModel Get(string sessionCode);
        SessionViewModel GetCurrentSession();
        double[] GetDaysLeft();
        IEnumerable<SessionViewModel> GetAll();
    }

    public interface IMembershipService
    {
        IEnumerable<MembershipView> GetLeaderMembershipsForActivity(string activityCode);
        IEnumerable<MembershipView> GetAdvisorMembershipsForActivity(string activityCode);
        IEnumerable<MembershipView> GetGroupAdminMembershipsForActivity(string activityCode, string? sessionCode = null);
        IEnumerable<MembershipView> GetMembershipsForActivity(
            string activityCode,
            string? sessionCode = null,
            bool? groupAdmin = null,
            string[]? participationTypes = null
        );
        IEnumerable<MembershipView> GetMembershipsByUser(string username);
        int GetActivitySubscribersCountForSession(string activityCode, string? sessionCode);
        int GetActivityMembersCountForSession(string activityCode, string? sessionCode);
        MembershipView GetSpecificMembership(int membershipID);
        Task<MembershipView> AddAsync(MembershipUploadViewModel membership);
        Task<MembershipView> UpdateAsync(int membershipID, MembershipUploadViewModel membership);
        Task<MembershipView> SetGroupAdminAsync(int membershipID, bool isGroupAdmin);
        Task<MembershipView> SetPrivacyAsync(int membershipID, bool isPrivate);
        MembershipView Delete(int membershipID);
        bool IsGroupAdmin(string username);
        IEnumerable<EmailViewModel> MembershipEmails(string activityCode, string sessionCode, ParticipationType? participationCode = null);
        MembershipView GetMembershipViewById(int membershipId);
        bool ValidateMembership(MembershipUploadViewModel membership);
        bool IsPersonAlreadyInActivity(MembershipUploadViewModel membershipRequest);
    }

    public interface IJobsService
    {
        IEnumerable<StudentTimesheetsViewModel> getSavedShiftsForUser(int ID_NUM);
        Task SaveShiftForUserAsync(int studentID, int jobID, DateTime shiftStart, DateTime shiftEnd, string hoursWorked, string shiftNotes, string lastChangedBy);
        StudentTimesheetsViewModel EditShift(int rowID, DateTime shiftStart, DateTime shiftEnd, string hoursWorked, string username);
        void DeleteShiftForUser(int rowID, int studentID);
        Task SubmitShiftForUserAsync(int studentID, int jobID, DateTime shiftEnd, int submittedTo, string lastChangedBy);
        Task<IEnumerable<SupervisorViewModel>> GetsupervisorNameForJobAsync(int supervisorID);
        Task<IEnumerable<ActiveJobViewModel>> GetActiveJobsAsync(DateTime shiftStart, DateTime shiftEnd, int studentID);
        Task<IEnumerable<OverlappingShiftIdViewModel>> EditShiftOverlapCheckAsync(int studentID, DateTime shiftStart, DateTime shiftEnd, int rowID);
        Task<IEnumerable<OverlappingShiftIdViewModel>> CheckForOverlappingShiftAsync(int studentID, DateTime shiftStart, DateTime shiftEnd);
        Task<IEnumerable<ClockInViewModel>> ClockOutAsync(string id);
        Task<ClockInViewModel> ClockInAsync(bool state, string id);
        Task<ClockInViewModel> DeleteClockInAsync(string id);
    }

    public interface IParticipationService
    {
        ParticipationViewModel Get(string id);
        IEnumerable<ParticipationViewModel> GetAll();
    }

    public interface IMembershipRequestService
    {
        RequestView Get(int requestID);
        IEnumerable<RequestView> GetAll();
        IEnumerable<RequestView> GetMembershipRequests(string activityCode, string? sessionCode, string? requestStatus);
        IEnumerable<RequestView> GetMembershipRequestsByUsername(string usernamne);
        Task<RequestView> AddAsync(RequestUploadViewModel membershipRequest);
        Task<RequestView?> UpdateAsync(int requestID, RequestUploadViewModel membershipRequest);
        Task<RequestView> ApproveAsync(int requestID);
        Task<RequestView> DenyAsync(int requestID);
        Task<RequestView> SetPendingAsync(int requestID);
        Task<RequestView> DeleteAsync(int requestID);
    }
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleViewModel>> GetScheduleStudentAsync(string username);
        Task<IEnumerable<ScheduleViewModel>> GetScheduleFacultyAsync(string username);
    }

    public interface IScheduleControlService
    {
        Task UpdateSchedulePrivacyAsync(string username, string value);
        Task UpdateDescriptionAsync(string username, string value);
        Task UpdateModifiedTimeStampAsync(string username, DateTime value);
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
        Task<Save_Rides> AddRideAsync(Save_Rides newRide, string gordon_id);
        Task<Save_Rides> DeleteRideAsync(string rideID, string gordon_id);
        Task<int> CancelRideAsync(string rideID, string gordon_id);
        Task<Save_Bookings> AddBookingAsync(Save_Bookings newBooking);
        Task<Save_Bookings> DeleteBookingAsync(string rideID, string gordon_id);
    }

    public interface IContentManagementService
    {
        IEnumerable<Slider_Images> GetBannerSlides();
        Slider_Images AddBannerSlide(BannerSlidePostViewModel slide, string serverURL, string contentRootPath);
        Slider_Images DeleteBannerSlide(int slideID);
    }

    public interface INewsService
    {
        StudentNews Get(int newsID);
        Task<IEnumerable<StudentNewsViewModel>> GetNewsNotExpiredAsync();
        Task<IEnumerable<StudentNewsViewModel>> GetNewsNewAsync();
        IEnumerable<StudentNewsCategoryViewModel> GetNewsCategories();
        Task<IEnumerable<StudentNewsViewModel>> GetNewsPersonalUnapprovedAsync(string username);
        StudentNews SubmitNews(StudentNews newsItem, string username);
        StudentNews DeleteNews(int newsID);
        StudentNewsViewModel EditPosting(int newsID, StudentNewsUploadViewModel newsItem);
        StudentNewsViewModel ApprovePosting(int newsID);
    }

    public interface IHousingService
    {
        bool CheckIfHousingAdmin(string username);
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
        Task<AcademicCheckInViewModel> PutCellPhoneAsync(string id, AcademicCheckInViewModel data);
        Task<EmergencyContactViewModel> PutEmergencyContactAsync(EmergencyContactViewModel data, string id, string username);
        Task<IEnumerable<AcademicCheckInViewModel>> GetHoldsAsync(string id);
        Task SetStatusAsync(string id);
        Task<AcademicCheckInViewModel> PutDemographicAsync(string id, AcademicCheckInViewModel data);
        Task<bool> GetStatusAsync(string id);
    }

}
