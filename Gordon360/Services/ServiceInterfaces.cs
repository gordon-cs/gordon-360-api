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
        IEnumerable<Countries> GetAllCountries();
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
        IEnumerable<AdvancedSearchViewModel> AdvancedSearch(List<string> accountTypes,
                                                            string firstname,
                                                            string lastname,
                                                            string major,
                                                            string minor,
                                                            string hall,
                                                            string classType,
                                                            string homeCity,
                                                            string state,
                                                            string country,
                                                            string department,
                                                            string building);
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
        ADMIN Get(int id);
        ADMIN Get(string gordon_id);
        IEnumerable<ADMIN> GetAll();
        ADMIN Add(ADMIN admin);
        ADMIN Delete(int id);
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
        public SessionViewModel GetCurrentSession();
        public double[] GetDaysLeft();
        IEnumerable<SessionViewModel> GetAll();
    }

    public interface IJenzibarActivityService
    {
        JNZB_ACTIVITIES Get(int id);
        IEnumerable<JNZB_ACTIVITIES> GetAll();
    }


    public interface IMembershipService
    {
        Task<IEnumerable<MembershipViewModel>> GetLeaderMembershipsForActivityAsync(string activityCode);
        Task<IEnumerable<MembershipViewModel>> GetAdvisorMembershipsForActivityAsync(string activityCode);
        Task<IEnumerable<MembershipViewModel>> GetGroupAdminMembershipsForActivityAsync(string activityCode);
        Task<IEnumerable<MembershipViewModel>> GetMembershipsForActivityAsync(string activityCode, string? sessionCode);
        Task<IEnumerable<MembershipViewModel>> GetMembershipsForStudentAsync(string username);
        Task<int> GetActivityFollowersCountForSessionAsync(string activityCode, string sessionCode);
        Task<int> GetActivityMembersCountForSessionAsync(string activityCode, string sessionCode);
        Task<IEnumerable<MembershipViewModel>> GetAllAsync();
        MEMBERSHIP GetSpecificMembership(int membershipID);
        Task<int> GetActivityFollowersCountAsync(string idactivityCode);
        Task<int> GetActivityMembersCountAsync(string activityCode);
        Task<MEMBERSHIP> AddAsync(MEMBERSHIP membership);
        Task<MEMBERSHIP> UpdateAsync(int membershipID, MEMBERSHIP membership);
        Task<MEMBERSHIP> ToggleGroupAdminAsync(int membershipID, MEMBERSHIP membership);
        void TogglePrivacy(int membershipID, bool isPrivate);
        MEMBERSHIP Delete(int membershipID);
        bool IsGroupAdmin(int gordonID);
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
        Task<MembershipRequestViewModel> GetAsync(int requestID);
        Task<IEnumerable<MembershipRequestViewModel>> GetAllAsync();
        Task<IEnumerable<MembershipRequestViewModel>> GetMembershipRequestsForActivityAsync(string activityCode);
        Task<IEnumerable<MembershipRequestViewModel>> GetMembershipRequestsForStudentAsync(string usernamne);
        REQUEST Add(REQUEST membershipRequest);
        REQUEST Update(int requestID, REQUEST membershipRequest);
        // The ODD one out. When we approve a request, we would like to get back the new membership.
        MEMBERSHIP ApproveRequest(int requestID);
        REQUEST DenyRequest(int requestID);
        REQUEST Delete(int requestID);
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
        IEnumerable<SliderViewModel> DEPRECATED_GetSliderContent();
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
        StudentNewsViewModel EditPosting(int newsID, StudentNews newsItem);
    }

    public interface IHousingService
    {
        bool CheckIfHousingAdmin(string gordonID);
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

    namespace RecIM
    {
        public interface IActivityService
        {
            IEnumerable<Activity> GetActivities();
            Activity GetActivityByID(int ActivityID);
            IEnumerable<Activity> GetActivitiesByTime(DateTime? time);
            Task UpdateActivity(Activity updatedActivity);
            Task PostActivity(Activity newActivity);

        }
        public interface ISeriesService
        {
            IEnumerable<Series> GetSeries();
            Series GetSeriesByID(int seriesID);
            IEnumerable<Match> GetMatches(int seriesID);
            Task PostSeries(Series newSeries);
            Task UpdateSeries(Series updatedSeries);
        }

        public interface ITeamService
        {
            IEnumerable<Team> GetTeamByID(int teamID);
            Task PostTeam(Team team);
            Task AddUserToTeam(int teamID, int participantID);
            Task UpdateParticipantRole(int teamID, int participantID, RoleType participantRole);
            Task UpdateTeam(Team updatedTeam);
        }

        public interface IParticipantService
        {
            IEnumerable<Participant> getParticipants();
            Task PostParticipant(int participantID);
            Participant GetParticipant(int participantID);
            ParticipantTeam GetParticipantTeamHistory(int participantID);
        }

        public interface ISportService
        {
            IEnumerable<Sport> GetSports();
            Sport GetSportByID(int sportID);
            Task PostSport(Sport newSport);
            Task UpdateSport(Sport updatedSport);
        }

        public interface IMatchService
        {
            Match GetMatchByID(int matchID);
            Task PostMatch(Match newMatch);
            Task UpdateMatch(Match updatedMatch);
            Task UpdateTeamScore(int matchID, int teamID, int teamScore);
        }
    }

}
