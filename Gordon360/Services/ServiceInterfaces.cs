﻿using Gordon360.Enums;
using Gordon360.Models.CCT;
using Gordon360.Models.MyGordon;
using Gordon360.Models.ViewModels;
using Gordon360.Models.ViewModels.Housing;
using Gordon360.Models.ViewModels.RecIM;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RecIMActivityViewModel = Gordon360.Models.ViewModels.RecIM.ActivityViewModel;

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
        MailboxCombinationViewModel? GetMailboxCombination(string username);
        DateTime GetBirthdate(string username);
        Task<IEnumerable<AdvisorViewModel>> GetAdvisorsAsync(string username);
        CliftonStrengthsViewModel? GetCliftonStrengths(int id);
        GraduationViewModel? GetGraduationInfo(string username);
        Task<bool> ToggleCliftonStrengthsPrivacyAsync(int id);
        IEnumerable<EmergencyContactViewModel> GetEmergencyContact(string username);
        ProfileCustomViewModel? GetCustomUserInfo(string username);
        Task<PhotoPathViewModel?> GetPhotoPathAsync(string username);
        Task UpdateCustomProfileAsync(string username, string type, CUSTOM_PROFILE content);
        Task<StudentProfileViewModel> UpdateMobilePhoneNumberAsync(string username, string newMobilePhoneNumber);
        Task<FacultyStaffProfileViewModel> UpdateOfficeLocationAsync(string username, string newBuilding, string newRoom);
        Task<FacultyStaffProfileViewModel> UpdateOfficeHoursAsync(string username, string newHours);
        Task<FacultyStaffProfileViewModel> UpdateMailStopAsync(string username, string newMail);
        Task UpdateMobilePrivacyAsync(string username, string value);
        Task UpdateImagePrivacyAsync(string username, string value);
        Task UpdateProfileImageAsync(string username, string path, string name);
        ProfileViewModel? ComposeProfile(object? student, object? alumni, object? faculty, object? customInfo);
        Task InformationChangeRequest(string username, ProfileFieldViewModel[] updatedField);
        IEnumerable<string> GetMailStopsAsync();
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
        Task<IEnumerable<EventViewModel>> GetFinalExamsForUserByTermAsync(string username, DateTime termStart, DateTime termEnd, string yearCode, string termCode);
        Task<IEnumerable<EventViewModel>> GetFinalExamsForInstructorByTermAsync(string username, DateTime termStart, DateTime termEnd, string yearCode, string termCode);
        Task<IEnumerable<EventViewModel>> FetchEventsAsync();
        Task<IEnumerable<EventViewModel>> GetFinalExamsForTermAsync(DateTime termStart, DateTime termEnd, string yearCode, string termCode);
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
            string? preferredClassYear,
            int? initialYear,
            int? finalYear,
            string? homeCity,
            string? state,
            string? country,
            string? department,
            string? building,
            string? involvement,
            string? gender);
        Task<IEnumerable<BasicInfoViewModel>> GetAllBasicInfoAsync();
        Task<IEnumerable<BasicInfoViewModel>> GetAllBasicInfoExceptAlumniAsync();
        ParallelQuery<BasicInfoViewModel> Search(string searchString, IEnumerable<BasicInfoViewModel> accounts);
        ParallelQuery<BasicInfoViewModel> Search(string firstName, string lastName, IEnumerable<BasicInfoViewModel> accounts);

    }

    public interface IActivityService
    {
        ActivityInfoViewModel Get(string activityCode);
        Task<IEnumerable<ActivityInfoViewModel>> GetActivitiesForSessionAsync(string sessionCode);
        Task<IEnumerable<string>> GetActivityTypesForSessionAsync(string sessionCode);
        IEnumerable<ActivityInfoViewModel> GetAll();
        bool IsOpen(string activityCode, string sessionCode);
        IQueryable<ActivityInfoViewModel> GetActivitiesByStatus(string sess_cde, bool getOpen);
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

    public interface IEmailService
    {
        IEnumerable<EmailViewModel> GetEmailsForActivity(string activityCode, string? sessionCode = null, List<string>? participationTypes = null);
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
        SessionViewModel Get(string sessionCode);
        SessionViewModel GetCurrentSession();
        SessionViewModel GetCurrentSessionForFinalExams();
        double[] GetDaysLeft();
        IEnumerable<SessionViewModel> GetAll();
    }

    public interface IMembershipService
    {
        IEnumerable<MembershipView> GetMemberships(
            string? activityCode = null,
            string? username = null,
            string? sessionCode = null,
            List<string>? participationTypes = null
        );
        MembershipView GetSpecificMembership(int membershipID);
        Task<MembershipView> AddAsync(MembershipUploadViewModel membership);
        Task<MembershipView> UpdateAsync(int membershipID, MembershipUploadViewModel membership);
        Task<MembershipView> SetGroupAdminAsync(int membershipID, bool isGroupAdmin);
        Task<MembershipView> SetPrivacyAsync(int membershipID, bool isPrivate);
        MembershipView Delete(int membershipID);
        bool IsGroupAdmin(string username);
        MembershipView GetMembershipViewById(int membershipId);
        bool ValidateMembership(MembershipUploadViewModel membership);
        bool IsPersonAlreadyInActivity(MembershipUploadViewModel membershipRequest);
        IEnumerable<MembershipView> RemovePrivateMemberships(IEnumerable<MembershipView> memberships, string viewerUsername);
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
        Task<IEnumerable<CoursesBySessionViewModel>> GetAllCoursesAsync(string username);

        Task<IEnumerable<CoursesBySessionViewModel>> GetAllInstructorCoursesAsync(string username);
        Task<IEnumerable<CoursesByTermViewModel>> GetAllCoursesByTermAsync(string username);
        Task<IEnumerable<CoursesByTermViewModel>> GetAllInstructorCoursesByTermAsync(string username);
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
        IEnumerable<StudentNewsViewModel> GetNewsUnapproved();
        Task<IEnumerable<StudentNewsViewModel>> GetNewsPersonalUnapprovedAsync(string username);
        StudentNews SubmitNews(StudentNews newsItem, string username);
        StudentNews DeleteNews(int newsID);
        StudentNewsViewModel EditPosting(int newsID, StudentNewsUploadViewModel newsItem);
        StudentNewsViewModel EditImage(int newsID, string newImageData);
        StudentNewsViewModel AlterPostAcceptStatus(int newsID, bool isAccepted);
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
        Task<Hall_Assignment_Ranges> CreateRoomRangeAsync(HallAssignmentRangeViewModel model);
        Task<bool> DeleteRoomRangeAsync(int rangeId);
        Task<Hall_Assignment_Ranges> AssignRaToRoomRangeAsync(int rangeId, string raId);
        Task<bool> DeleteAssignmentAsync(int rangeId);
        Task<RD_StudentsViewModel> GetResidentRDAsync(string hallId);
        Task<List<RD_StudentsViewModel>> GetRDsAsync();
        Task<RdOnCallGetView> CreateRdOnCallAsync(RD_On_Call_Create OnCall);
        Task<bool> DeleteRDOnCallById(int recordId);
        Task<RdOnCallGetView> UpdateRdOnCallAsync(int recordId, RD_On_Call_Create updatedOnCall);
        Task<RD_StudentsViewModel> GetRDOnCall();
        Task<List<RdOnCallGetView>> GetActiveRDOnCallsAsync();
        Task<RA_StudentsViewModel> GetResidentRAAsync(string hallId, string roomNumber);
        Task<List<HallAssignmentRangeViewModel>> GetAllRoomRangesAsync();
        Task<List<RA_StudentsViewModel>> GetAllRAsAsync();
        Task<List<RA_Assigned_RangesViewModel>> GetRangeAssignmentsAsync();
        Task<List<RA_Assigned_RangesViewModel>> GetRangeAssignmentsByRAIdAsync(string raId);
        Task<List<MissedRoomsViewModel>> GetMissedRoomsAsync();
        Task<bool> SetPreferredContactMethodAsync(string raId, string preferredContactMethod);
        Task<RA_ContactPreference> GetPreferredContactAsync(string raId);
        Task<bool> RA_CheckinAsync(string[] HallIDs, string raId);
        Task<RA_On_Call_GetViewModel> GetOnCallRAAsync(string hallId);
        Task<List<RA_On_Call_GetViewModel>> GetOnCallRAAllHallsAsync();
        Task<List<string>> GetOnCallRAHallsAsync(string userName);
        Task<bool> IsRAOnCallAsync(string raId);
        Task<bool> IsStudentResidentialAsync(int idNum);
        Task<HallTaskViewModel> CreateTaskAsync(HallTaskViewModel task);
        Task<HallTaskViewModel> UpdateTaskAsync(int taskID, HallTaskViewModel task);
        Task<bool> DisableTaskAsync(int taskID);
        Task<bool> CompleteTaskAsync(int taskID, string CompletedBy);
        Task<bool> IncompleteTaskAsync(int taskID);
        Task<List<HallTaskViewModel>> GetActiveTasksForHallAsync(string hallId);
        Task<List<DailyTaskViewModel>> GetTasksForHallAsync(string hallId);
        Task<RA_StatusEventsViewModel> CreateStatusEventAsync(RA_StatusEventsViewModel status);
        Task<bool> DeleteStatusEventAsync(int statusID);
        Task<RA_StatusEventsViewModel> UpdateStatusEventAsync(int statusID, RA_StatusEventsViewModel status);
        Task<List<DailyStatusEventsViewModel>> GetStatusEventsForRAAsync(string raID);
        Task<List<RA_StatusEventsViewModel>> GetActiveStatusesByRAIdAsync(string raId);
        Task<string> GetStuLifeContactByPhoneNameAsync(string phoneName);
        Task<bool> SetStuLifePhoneNumberAsync(string PhoneName, string PhoneNumber);

    }

    public interface ILostAndFoundService
    {
        public int CreateMissingItemReport(MissingItemReportViewModel reportDetails, string username);
        public int CreateActionTaken(int id, ActionsTakenViewModel ActionsTaken, string username);
        IEnumerable<MissingItemReportViewModel> GetMissingItems(string requestedUsername, string requestorUsername);
        IEnumerable<MissingItemReportViewModel> GetMissingItemsAll(string username,
                                                                   int? lastId,
                                                                   int? pageSize,
                                                                   string? status,
                                                                   string? color,
                                                                   string? category,
                                                                   string? keywords,
                                                                   DateTime? lastCheckedDate);
        Task UpdateMissingItemReportAsync(int id, MissingItemReportViewModel reportDetails, string username);
        Task UpdateReportStatusAsync(int id, string status, string username);
        Task UpdateReportAssociatedFoundItemAsync(int id, string? foundID, string username);
        MissingItemReportViewModel? GetMissingItem(int id, string username);
        IEnumerable<ActionsTakenViewModel> GetActionsTaken(int id, string username, bool getPublicOnly = false, bool elevatedPermissions = false);
        public int GetMissingItemsCount(
            string username,
            string? status,
            string? color,
            string? category,
            string? keywords
        );
        public string CreateFoundItem(FoundItemViewModel reportDetails, string username);
        public int CreateFoundActionTaken(string foundItemId, FoundActionsTakenViewModel FoundActionsTaken, string username);
        IEnumerable<FoundItemViewModel> GetFoundItemsAll(string username,
                                                         DateTime? latestDate,
                                                         string? status,
                                                         string? color,
                                                         string? category,
                                                         string? ID,
                                                         string? keywords);
        public FoundItemViewModel GetFoundItem(string foundItemID, string username);
        Task UpdateFoundItemAsync(string id, FoundItemViewModel itemDetails, string username);
        Task UpdateFoundStatusAsync(string id, string status, string username);
        Task UpdateFoundAssociatedMissingReportAsync(string foundItemID, int? missingReportID, string username);
        public int GetFoundItemsCount(
             string username,
             DateTime? latestDate,
             string? status,
             string? color,
             string? category,
             string? ID,
             string? keywords
         );

        IEnumerable<FoundItemViewModel> GetFoundItemsByOwner(string requestedUsername, string requestorUsername);

    }


    public interface IAcademicCheckInService
    {
        Task PutCellPhoneAsync(string id, MobilePhoneUpdateViewModel data);
        Task<EmergencyContactViewModel> PutEmergencyContactAsync(EmergencyContactViewModel data, string id, string username);
        Task<EnrollmentCheckinHolds> GetHoldsAsync(string id);
        Task SetStatusAsync(string id);
        Task<AcademicCheckInViewModel> PutDemographicAsync(string id, AcademicCheckInViewModel data);
        Task<bool> GetStatusAsync(string username);
    }

    public interface IPosterService
    {
        IEnumerable<PosterViewModel> GetPosters();
        IEnumerable<PosterViewModel> GetCurrentPosters();
        IEnumerable<PosterViewModel> GetCurrentPostersByActivityCode(string activityCode);
        IEnumerable<PosterViewModel> GetPersonalizedPostersByUsername(string username);
        IEnumerable<string> GetPosterStatuses();
        IEnumerable<PosterViewModel> GetPostersByActivityCode(string activityCode);
        PosterViewModel GetPosterByID(int posterID);
        Task<PosterViewModel> PostPosterAsync(PosterUploadViewModel newPoster);
        Task<PosterViewModel> UpdatePosterAsync(int posterID, PosterPatchViewModel updatedPoster);
        Task<PosterViewModel> DeletePosterAsync(int posterID);
        Task<PosterViewModel> HidePosterAsync(int posterID);
    }

    public interface IAcademicTermService
    {
        Task<YearTermTableViewModel?> GetCurrentTermAsync();
        Task<IEnumerable<YearTermTableViewModel>> GetAllTermsAsync();
        Task<YearTermTableViewModel?> GetCurrentTermForFinalExamsAsync();
        Task<DaysLeftViewModel> GetDaysLeftAsync();
    }

    namespace RecIM
    {
        public interface IRecIMService
        {
            RecIMGeneralReportViewModel GetReport(DateTime start, DateTime end);
        }

        public interface IAffiliationService
        {
            IEnumerable<AffiliationExtendedViewModel> GetAllAffiliationDetails();
            AffiliationExtendedViewModel GetAffiliationDetailsByName(string name);
            Task DeleteAffiliationAsync(string affiliationName);
            Task<string> CreateAffiliation(string affiliationName);
            Task<string> AddPointsToAffilliationAsync(string affiliationName, AffiliationPointsUploadViewModel vm);
            Task<string> UpdateAffiliationAsync(string affiliationName, AffiliationPatchViewModel update);
        }
        public interface IActivityService
        {
            IEnumerable<LookupViewModel>? GetActivityLookup(string type);
            IEnumerable<ActivityExtendedViewModel> GetActivities();
            ActivityExtendedViewModel? GetActivityByID(int activityID);
            IEnumerable<ActivityExtendedViewModel> GetActivitiesByCompletion(bool isActive);
            Task<RecIMActivityViewModel> UpdateActivityAsync(int activytID, ActivityPatchViewModel updatedActivity);
            Task<RecIMActivityViewModel> PostActivityAsync(ActivityUploadViewModel newActivity);
            Task<ParticipantActivityViewModel> AddParticipantActivityInvolvementAsync(string username, int activityID, int privTypeID, bool isFreeAgent);
            bool IsReferee(string username, int activityID);
            bool ActivityTeamCapacityReached(int activityID);
            bool ActivityRegistrationClosed(int activityID);
            Task<RecIMActivityViewModel> DeleteActivityCascade(int activityID);
        }
        public interface ISeriesService
        {
            IEnumerable<LookupViewModel>? GetSeriesLookup(string type);
            IEnumerable<SeriesExtendedViewModel> GetSeries(bool active);
            IEnumerable<SeriesExtendedViewModel> GetSeriesByActivityID(int activityID);
            SeriesExtendedViewModel GetSeriesByID(int seriesID);
            Task<SeriesViewModel> PostSeriesAsync(SeriesUploadViewModel newSeries, int? referenceSeriesID);
            Task<SeriesViewModel> UpdateSeriesAsync(int seriesID, SeriesPatchViewModel series);
            IEnumerable<AffiliationPointsViewModel> GetSeriesWinners(int seriesID);
            Task HandleAdditionalSeriesWinnersAsync(AffiliationPointsUploadViewModel vm);
            Task<SeriesScheduleViewModel> PutSeriesScheduleAsync(SeriesScheduleUploadViewModel seriesSchedule);
            Task<SeriesViewModel> DeleteSeriesCascadeAsync(int seriesID);
            Task<IEnumerable<MatchViewModel>?> ScheduleMatchesAsync(int seriesID, UploadScheduleRequest request);
            SeriesAutoSchedulerEstimateViewModel GetScheduleMatchesEstimateAsync(int seriesID, UploadScheduleRequest request);
            SeriesScheduleExtendedViewModel GetSeriesScheduleByID(int seriesID);
            IEnumerable<MatchBracketViewModel> GetSeriesBracketInformation(int seriesID);
            Task<TeamRecordViewModel> UpdateSeriesTeamRecordAsync(int seriesID, TeamRecordPatchViewModel teamRecord);
        }

        public interface ITeamService
        {
            IEnumerable<LookupViewModel>? GetTeamLookup(string type);
            double GetTeamSportsmanshipScore(int teamID);
            IEnumerable<TeamExtendedViewModel> GetTeams(bool active);
            TeamExtendedViewModel GetTeamByID(int teamID, bool isAdminView = false);
            IEnumerable<TeamExtendedViewModel> GetTeamInvitesByParticipantUsername(string username);
            ParticipantTeamViewModel GetParticipantTeam(int teamID, string username);
            Task<TeamViewModel> PostTeamAsync(TeamUploadViewModel newTeam, string username);
            Task<ParticipantTeamViewModel> AddParticipantToTeamAsync(int teamID, ParticipantTeamUploadViewModel participant, string? inviterUsername = null);
            Task<TeamViewModel> UpdateTeamAsync(int teamID, TeamPatchViewModel updatedTeam);
            Task<ParticipantTeamViewModel> DeleteParticipantTeamAsync(int teamID, string username);
            Task<TeamViewModel> DeleteTeamCascadeAsync(int teamID);
            Task<ParticipantTeamViewModel> UpdateParticipantRoleAsync(int teamID, ParticipantTeamUploadViewModel participant);
            bool HasUserJoined(int activityID, string username);
            bool HasTeamNameTaken(int activityID, string teamName);
            bool IsTeamCaptain(string username, int teamID);
            int GetTeamActivityID(int teamID);
            Task<IEnumerable<MatchAttendance>> PutParticipantAttendanceAsync(int matchID, ParticipantAttendanceViewModel attendance);
            public int ParticipantAttendanceCount(int teamID, string username);
        }

        public interface IParticipantService
        {
            IEnumerable<LookupViewModel>? GetParticipantLookup(string type);
            IEnumerable<ParticipantExtendedViewModel> GetParticipants();
            bool GetParticipantIsCustom(string username);
            IEnumerable<BasicInfoViewModel> GetAllCustomParticipantsBasicInfo();
            IEnumerable<ParticipantStatusExtendedViewModel> GetParticipantStatusHistory(string username);
            ParticipantExtendedViewModel? GetParticipantByUsername(string username, string? roleType = null, bool isAdminView = false);
            AccountViewModel GetUnaffiliatedAccountByUsername(string username);
            IEnumerable<MatchExtendedViewModel> GetParticipantMatches(string username);
            IEnumerable<TeamExtendedViewModel> GetParticipantTeams(string username);
            Task<ParticipantExtendedViewModel> PostParticipantAsync(string username, int? statusID = 4);
            Task<ParticipantExtendedViewModel> PostCustomParticipantAsync(string username, CustomParticipantViewModel newCustomParticipant);
            Task<ParticipantExtendedViewModel> SetParticipantAdminStatusAsync(string username, bool isAdmin);
            Task<ParticipantNotificationViewModel> SendParticipantNotificationAsync(string username, ParticipantNotificationUploadViewModel notificationVM);
            Task<ParticipantActivityViewModel> UpdateParticipantActivityAsync(string username, ParticipantActivityPatchViewModel updatedParticipant);
            Task<ParticipantStatusHistoryViewModel> UpdateParticipantStatusAsync(string username, ParticipantStatusPatchViewModel participantStatus);
            Task<ParticipantExtendedViewModel> UpdateParticipantAllowEmailsAsync(string username, bool allowEmails);
            Task<ParticipantExtendedViewModel> UpdateCustomParticipantAsync(string username, CustomParticipantPatchViewModel updatedParticipant);
            bool IsParticipant(string username);
            bool IsAdmin(string username);
        }

        public interface ISportService
        {
            IEnumerable<SportViewModel> GetSports();
            SportViewModel GetSportByID(int sportID);
            Task<SportViewModel> DeleteSportAsync(int sportID);
            Task<SportViewModel> PostSportAsync(SportUploadViewModel newSport);
            Task<SportViewModel> UpdateSportAsync(int sportID, SportPatchViewModel updatedSport);
        }

        public interface IMatchService
        {
            IEnumerable<MatchExtendedViewModel> GetAllMatches();
            MatchViewModel GetSimpleMatchViewByID(int matchID);
            IEnumerable<ParticipantAttendanceViewModel> GetMatchAttendance(int matchID);
            IEnumerable<LookupViewModel>? GetMatchLookup(string type);
            Task<SurfaceViewModel> PostSurfaceAsync(SurfaceUploadViewModel newSurface);
            Task<SurfaceViewModel> UpdateSurfaceAsync(int surfaceID, SurfaceUploadViewModel updatedSurface);
            IEnumerable<SurfaceViewModel> GetSurfaces();
            Task DeleteSurfaceAsync(int surfaceID);
            MatchExtendedViewModel GetMatchForTeamByMatchID(int matchID);
            MatchExtendedViewModel GetMatchByID(int matchID);
            IEnumerable<MatchExtendedViewModel> GetMatchesBySeriesID(int seriesID);
            Task<MatchViewModel> PostMatchAsync(MatchUploadViewModel match);
            Task<MatchTeamViewModel> UpdateTeamStatsAsync(int matchID, MatchStatsPatchViewModel match);
            Task<MatchViewModel> UpdateMatchAsync(int matchID, MatchPatchViewModel match);
            Task CreateMatchTeamMappingAsync(int teamID, int matchID);
            Task<MatchViewModel> DeleteMatchCascadeAsync(int matchID);
            Task DeleteParticipantAttendanceAsync(int matchID, MatchAttendance attendee);
            Task<MatchAttendance> AddParticipantAttendanceAsync(int matchID, MatchAttendance attendee);
        }
    }

    public interface IMarketplaceService
    {
        IEnumerable<MarketplaceListingViewModel> GetAllListings();
        IEnumerable<MarketplaceListingViewModel> GetUserListings(string username);
        MarketplaceListingViewModel? GetListingById(int listingId);
        Task<MarketplaceListingViewModel> CreateListingAsync(MarketplaceListingUploadViewModel newListing, string username);
        Task<MarketplaceListingViewModel> UpdateListingAsync(int listingId, MarketplaceListingUpdateViewModel updatedListing);
        Task DeleteListingAsync(int listingId);
        Task<MarketplaceListingViewModel> ChangeListingStatusAsync(int listingId, string status);
        IEnumerable<MarketplaceListingViewModel> GetFilteredListings(
            int? categoryId, int? statusId, decimal? minPrice, decimal? maxPrice,
            string? search, string? sortBy, bool desc = false,
            int page = 1, int pageSize = 20);
        int GetFilteredListingsCount(
            int? categoryId, int? statusId, decimal? minPrice, decimal? maxPrice,
            string? search);
        Task<ItemCategory> AddCategoryAsync(string categoryName);
        Task<ItemCondition> AddConditionAsync(string conditionName);
        Task<ItemCategory> UpdateCategoryVisibilityAsync(string categoryName, bool visibility);
        Task<ItemCondition> UpdateConditionVisibilityAsync(string conditionName, bool visibility);
        IEnumerable<MarketplaceAdminViewModel> GetAdminThreads(
            int? id, int? categoryId, int? statusId, decimal? minPrice,
            decimal? maxPrice, string? search, string? sortBy, bool desc = false,
            int page = 1, int pageSize = 20);
        int GetAdminThreadsCount(
            int? categoryId, int? statusId, decimal? minPrice, decimal? maxPrice,
            string? search);
        IEnumerable<MarketplaceListingViewModel> GetThreadEditHistory(int threadId);
    }
}
