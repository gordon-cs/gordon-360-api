using Gordon360.Models;
using Gordon360.Models.ViewModels;
using static Gordon360.Controllers.Api.WellnessController;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

// <summary>
// Namespace with all the Service Interfaces that are to be implemented. I don't think making this interface is required, the services can work find on their own.
// However, building the interfaces first does give a general sense of structure to their implementations. A certain cohesiveness :p.
// </summary>
namespace Gordon360.Services
{

    public interface IRoleCheckingService
    {
        string getCollegeRole(string username);
    }
    public interface IProfileService
    {
        StudentProfileViewModel GetStudentProfileByID(string id);
        StudentProfileViewModel GetStudentProfileByUsername(string username);
        FacultyStaffProfileViewModel GetFacultyStaffProfileByUsername(string username);
        AlumniProfileViewModel GetAlumniProfileByUsername(string username);
        IEnumerable<AdvisorViewModel> GetAdvisors(string id);
        CliftonStrengthsViewModel GetCliftonStrengths(int id);
        ProfileCustomViewModel GetCustomUserInfo(string username);
        PhotoPathViewModel GetPhotoPath(string id);
        void UpdateProfileLink(string username, string type, CUSTOM_PROFILE path);
        void UpdateMobilePhoneNumber(string id, string newPhoneNumber);
        void UpdateMobilePrivacy(string id, string value);
        void UpdateImagePrivacy(string id, string value);
        void UpdateProfileImage(string id, string path, string name);
    }

    public interface IEventService
    {
        IEnumerable<AttendedEventViewModel> GetEventsForStudentByTerm(string id, string term);
        IEnumerable<EventViewModel> GetAllEvents();
        IEnumerable<EventViewModel> GetPublicEvents();
        IEnumerable<EventViewModel> GetCLAWEvents();
    }

    public interface IDiningService
    {
        DiningViewModel GetDiningPlanInfo(int id, string sessionCode);
        string GetBalance(int cardHolderID, string planID);
    }

    public interface IAccountService
    {
        AccountViewModel Get(string id);
        IEnumerable<AccountViewModel> GetAll();
        AccountViewModel GetAccountByEmail(string email);
        AccountViewModel GetAccountByUsername(string username);
    }

    public interface IWellnessService
    {
        WellnessViewModel GetStatus(string id);
        WellnessQuestionViewModel GetQuestion();
        Health_Status PostStatus(WellnessStatusColor status, string id);
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
    public interface IVictoryPromiseService
    {
        IEnumerable<VictoryPromiseViewModel> GetVPScores(string id);
    }
    public interface IStudentEmploymentService
    {
        IEnumerable<StudentEmploymentViewModel> GetEmployment(string id);
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
        void SendEmails(string[] to_emails, string to_email, string subject, string email_content, string password);
        void SendEmailToActivity(string activityCode, string sessionCode, string from_email, string subject, string email_content, string password);
    }

    public interface IErrorLogService
    {
        ERROR_LOG Add(ERROR_LOG error_log);
        ERROR_LOG Log(string error_message);
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
        IEnumerable<MembershipViewModel> GetLeaderMembershipsForActivity(string id);
        IEnumerable<MembershipViewModel> GetAdvisorMembershipsForActivity(string id);
        IEnumerable<MembershipViewModel> GetGroupAdminMembershipsForActivity(string id);
        IEnumerable<MembershipViewModel> GetMembershipsForActivity(string id);
        IEnumerable<MembershipViewModel> GetMembershipsForStudent(string id);
        int GetActivityFollowersCountForSession(string id, string sess_cde);
        int GetActivityMembersCountForSession(string id, string sess_cde);
        IEnumerable<MembershipViewModel> GetAll();
        MEMBERSHIP GetSpecificMembership(int id);
        int GetActivityFollowersCount(string id);
        int GetActivityMembersCount(string id);
        MEMBERSHIP Add(MEMBERSHIP membership);
        MEMBERSHIP Update(int id, MEMBERSHIP membership);
        MEMBERSHIP ToggleGroupAdmin(int id, MEMBERSHIP membership);
        void TogglePrivacy(int id, bool p);
        MEMBERSHIP Delete(int id);
        Boolean IsGroupAdmin(int studentID);
    }

    public interface IJobsService
    {
        IEnumerable<StudentTimesheetsViewModel> getSavedShiftsForUser(int ID_NUM);
        IEnumerable<StudentTimesheetsViewModel> saveShiftForUser(int studentID, int jobID, DateTime shiftStart, DateTime shiftEnd, string hoursWorked, string shiftNotes, string lastChangedBy);
        IEnumerable<StudentTimesheetsViewModel> editShift(int rowID, DateTime shiftStart, DateTime shiftEnd, string hoursWorked, string username);
        IEnumerable<StudentTimesheetsViewModel> deleteShiftForUser(int rowID, int studentID);
        IEnumerable<StudentTimesheetsViewModel> submitShiftForUser(int studentID, int jobID, DateTime shiftEnd, int submittedTo, string lastChangedBy);
        IEnumerable<SupervisorViewModel> getsupervisorNameForJob(int supervisorID);
        IEnumerable<ActiveJobViewModel> getActiveJobs(DateTime shiftStart, DateTime shiftEnd, int studentID);
        IEnumerable<OverlappingShiftIdViewModel> editShiftOverlapCheck(int studentID, DateTime shiftStart, DateTime shiftEnd, int rowID);
        IEnumerable<OverlappingShiftIdViewModel> checkForOverlappingShift(int studentID, DateTime shiftStart, DateTime shiftEnd);
        IEnumerable<StaffTimesheetsViewModel> saveShiftForStaff(int staffID, int jobID, DateTime shiftStart, DateTime shiftEnd, string hoursWorked, char hoursType, string shiftNotes, string lastChangedBy);
        IEnumerable<StaffTimesheetsViewModel> getSavedShiftsForStaff(int ID_NUM);
        IEnumerable<StaffTimesheetsViewModel> editShiftStaff(int rowID, DateTime shiftStart, DateTime shiftEnd, string hoursWorked, string username);
        IEnumerable<StaffTimesheetsViewModel> deleteShiftForStaff(int rowID, int staffID);
        IEnumerable<StaffTimesheetsViewModel> submitShiftForStaff(int staffID, int jobID, DateTime shiftEnd, int submittedTo, string lastChangedBy);
        IEnumerable<ActiveJobViewModel> getActiveJobsStaff(DateTime shiftStart, DateTime shiftEnd, int staffID);
        IEnumerable<SupervisorViewModel> getStaffSupervisorNameForJob(int supervisorID);
        IEnumerable<OverlappingShiftIdViewModel> editShiftOverlapCheckStaff(int staffID, DateTime shiftStart, DateTime shiftEnd, int rowID);
        IEnumerable<OverlappingShiftIdViewModel> checkForOverlappingShiftStaff(int staffID, DateTime shiftStart, DateTime shiftEnd);
        IEnumerable<ClockInViewModel> ClockOut(string id);
        ClockInViewModel ClockIn(bool state, string id);
        ClockInViewModel DeleteClockIn(string id);
        IEnumerable<StaffCheckViewModel> CanUsePage(string id);
        IEnumerable<HourTypesViewModel> GetHourTypes();
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
    public interface IScheduleService
    {
        IEnumerable<ScheduleViewModel> GetScheduleStudent(string id);
        IEnumerable<ScheduleViewModel> GetScheduleFaculty(string id);
    }

    public interface IScheduleControlService
    {
        void UpdateSchedulePrivacy(string id, string value);
        void UpdateDescription(string id, string value);
        void UpdateModifiedTimeStamp(string id, DateTime value);
    }


    public interface IMyScheduleService
    {
        MYSCHEDULE GetForID(string event_id, string gordon_id);
        IEnumerable<MYSCHEDULE> GetAllForID(string gordon_id);
        MYSCHEDULE Add(MYSCHEDULE myschedule);
        MYSCHEDULE Update(MYSCHEDULE myschedule);
        MYSCHEDULE Delete(string event_id, string gordon_id);
    }

    public interface ISaveService
    {
        IEnumerable<UPCOMING_RIDES_Result> GetUpcoming(string gordon_id); // done
        IEnumerable<UPCOMING_RIDES_BY_STUDENT_ID_Result> GetUpcomingForUser(string gordon_id); //done
        Save_Rides AddRide(Save_Rides newRide, string gordon_id); //done
        Save_Rides DeleteRide(string rideID, string gordon_id); //done
        int CancelRide(string rideID, string gordon_id);
        Save_Bookings AddBooking(Save_Bookings newBooking); //done
        Save_Bookings DeleteBooking(string rideID, string gordon_id); //done

    }

    public interface IContentManagementService
    {
        IEnumerable<SliderViewModel> GetSliderContent();
    }

    public interface INewsService
    {
        StudentNews Get(int newsID);
        IEnumerable<StudentNewsViewModel> GetNewsNotExpired();
        IEnumerable<StudentNewsViewModel> GetNewsNew();
        IEnumerable<StudentNewsCategoryViewModel> GetNewsCategories();
        IEnumerable<StudentNewsViewModel> GetNewsPersonalUnapproved(string id, string username);
        StudentNews SubmitNews(StudentNews newsItem, string username, string id);
        StudentNews DeleteNews(int newsID);
        StudentNewsViewModel EditPosting(int newsID, StudentNews newsItem);
    }

    public interface IHousingService
    {
        bool CheckIfHousingAdmin(string userID);
        bool AddHousingAdmin(string id);
        bool RemoveHousingAdmin(string id);
        bool DeleteApplication(int applicationID);
        AA_ApartmentHalls[] GetAllApartmentHalls();
        string GetEditorUsername(int applicationID);
        int? GetApplicationID(string username, string sess_cde);
        ApartmentApplicationViewModel GetApartmentApplication(int applicationID, bool isAdmin = false);
        ApartmentApplicationViewModel[] GetAllApartmentApplication();
        int SaveApplication(string username, string sess_cde, string editorUsername, ApartmentApplicantViewModel[] apartmentApplicants, ApartmentChoiceViewModel[] apartmentChoices);
        int EditApplication(string username, string sess_cde, int applicationID, string newEditorUsername, ApartmentApplicantViewModel[] newApartmentApplicants, ApartmentChoiceViewModel[] newApartmentChoices);
        bool ChangeApplicationEditor(string username, int applicationID, string newEditorUsername);
        bool ChangeApplicationDateSubmitted(int applicationID);
    }

}
