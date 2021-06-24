using Gordon360.Static.Methods;

namespace Gordon360.Static.Names
{
    public static class Resource
    {
        public const string PROFILE = "A new profile resource";
        public const string MEMBERSHIP_REQUEST = "A Membership Request Resource";
        public const string MEMBERSHIP = "A Membership Resource";
        public const string MEMBERSHIP_PRIVACY = "A Membership privacy";
        public const string STUDENT = "A Student Resource";
        public const string ACCOUNT = "An Account Resource.";
        public const string ADVISOR = "An Advisor Resource";
        public const string GROUP_ADMIN = "A Group Admin Resource";
        public const string ADMIN = "An admininistrator Resource";
        public const string ACTIVITY_INFO = "An Activity Info Service.";
        public const string ACTIVITY_STATUS = "The open or closed status of an activity";
        public const string ChapelEvent = "The info of chapel events";
        public const string DINING = "Info related to dining service";
        public const string HOUSING = "Info related to housing";
        public const string HOUSING_ADMIN = "A Housing Admin Resource";
        public const string ERROR_LOG = "The error log resource";
        public const string MYSCHEDULE = "A custom schedule resource";
        public const string Save_Rides = "A ride resource";
        public const string SCHEDULE = "A course schedule resource";
        public const string NEWS = "A student news resource";
        public const string SHIFT = "A shift that a student has worked";

        // Partial resources, to be targetted by Operation.READ_PARTIAL
        public const string MEMBERSHIP_REQUEST_BY_ACTIVITY = "Membership Request Resources associated with an activity";
        public const string MEMBERSHIP_REQUEST_BY_STUDENT = "Membership Request Resources associated with a student";
        public const string MEMBERSHIP_BY_ACTIVITY = "Membership Resources associated with an activity";
        public const string MEMBERSHIP_BY_STUDENT = "Membership Resources associated with a student";
        public const string EMAILS_BY_ACTIVITY = "Emails for activity members";
        public const string EMAILS_BY_LEADERS = "Emails for activity leaders";
        public const string EMAILS_BY_GROUP_ADMIN = "Emails for group admin";
        public const string ADVISOR_BY_ACTIVITY = "Advisor Resources associated with an activity";
        public const string LEADER_BY_ACTIVITY = "Leader Ressources associated with an activity";
        public const string GROUP_ADMIN_BY_ACTIVITY = "Group Admin Resources associated with an activity";
        public const string EVENTS_BY_STUDENT_ID = "Every event attended for a specific student";
        public const string ALL_BASIC_INFO = "returns all basic public information for an account";
        // Public resources
        public const string SLIDER = "Slider to be shown on the homepage";

    }

    public static class Operation
    {
        public const string READ_ALL = "Reading all available resources";
        public const string READ_PARTIAL = "Reading a group of related resources";
        public const string READ_ONE = "Reading one resource";
        public const string READ_PUBLIC = "Reading public resources";
        public const string ADD = "Creating a resource";
        public const string UPDATE = "Updating a resource";
        public const string DELETE = "Deleting a resource";

        // Should only be used for a resource of type Membership Request
        public const string DENY_ALLOW = "Denying or allowing a request";

    }


    public static class Position
    {
        public const string STUDENT = "student";
        public const string FACSTAFF = "facstaff";
        public const string ALUMNI = "alumni";
        public const string SUPERADMIN = "god";      // TODO: change in database to something more reverent
        public const string POLICE = "gordon police";
        public const string READONLY = "readonly";
    }

    public static class Activity_Roles
    {
        public const string GUEST = "GUEST";
    }

    public static class Request_Status
    {
        public const string PENDING = "Pending";
        public const string APPROVED = "Approved";
        public const string DENIED = "Denied";
    }


    public static class Defaults
    {
        public const string DEFAULT_ACTIVITY_IMAGE_PATH = "https://360api.gordon.edu/browseable/uploads/Default/activityImage.png";
        public const string DEFAULT_PROFILE_IMAGE_PATH = "https://360api.gordon.edu/browseable/profile/Default/profile.png";
        public const string DEFAULT_PREF_IMAGE_PATH = "\\\\go\\pref_photos\\";
        public const string DEFAULT_IMAGE_PATH = "\\\\go\\photos\\";
        public const string DEFAULT_ID_SUBMISSION_PATH = "\\\\go\\ID_Photo_Submissions\\";
        public const string DATABASE_IMAGE_PATH = "f:\\inetpub\\pref_photos\\";

    }

    public static class URLs
    {
        /**
         * Retrieve events from the 25Live API. 
         * event_type_id parameter fetches only events of type 14 (Calendar Announcement) and 57 (Event).
         * All other event types are not appropiate for the 360 events feed.
         * end_after parameter  limits to request to events from the current academic year.
         * state parameter fetches only confirmed events
         */
        public static string ALL_EVENTS_REQUEST = "https://25live.collegenet.com/25live/data/gordon/run/events.xml?/&event_type_id=14+57&state=2&end_after=" + Helpers.GetFirstEventDate() + "&scope=extended";
    }

    public static class SQLQuery
    {
        public static string ALL_PUBLIC_STUDENT_REQUEST = "SELECT ISNULL(Mail_Location, '') as Mail_Location, ISNULL(BuildingDescription, '') as Hall, ISNULL(FirstName, '') as FirstName, ISNULL(LastName, '') as LastName, ISNULL(NickName, '') as NickName, ISNULL(Class, '') as Class, ISNULL(Major1Description, '') as Major1Description, ISNULL(Major2Description, '') as Major2Description, ISNULL(Major3Description, '') as Major3Description, ISNULL(Minor1Description, '') as Minor1Description, ISNULL(Minor2Description, '') as Minor2Description, ISNULL(Minor3Description, '') as Minor3Description, ISNULL(HomeCity, '') as HomeCity, ISNULL(HomeState, '') as HomeState, ISNULL(Country, '') as Country, ISNULL(KeepPrivate, '') as KeepPrivate, ISNULL(Email, '') as Email, AD_Username FROM STUDENT S WHERE AD_Username is not null FOR JSON PATH, ROOT('Students')";
        public static string ALL_PUBLIC_FACULTY_STAFF_REQUEST = "SELECT FirstName, LastName, NickName, OnCampusDepartment, BuildingDescription, HomeCity, HomeState, Country, KeepPrivate, JobTitle, Email, Type, AD_Username, Mail_Location FROM FacStaff WHERE AD_Username is not null";
        public static string ALL_PUBLIC_ALUMNI_REQUEST = "SELECT FirstName, LastName, NickName, Major1Description, Major2Description, HomeCity, HomeState, Country, Email, ShareName, PreferredClassYear, AD_Username FROM Alumni WHERE ShareName is null OR ShareName = 'Y';";

        public static string ALL_STUDENT_REQUEST = "SELECT * from Student WHERE AD_Username is not null";
        public static string ALL_FACULTY_STAFF_REQUEST = "SELECT * from FacStaff WHERE AD_Username is not null";
        public static string ALL_ALUMNI_REQUEST = "SELECT * from Alumni WHERE AD_Username is not null";
        public static string ALL_BASIC_INFO_NOT_ALUM = "ALL_BASIC_INFO_NOT_ALUMNI";

        // GoStalk
        public static string ALL_MAJORS = "SELECT DISTINCT MajorDescription FROM Majors ORDER BY MajorDescription ASC";
        public static string ALL_MINORS = "SELECT DISTINCT Minor1Description FROM Student WHERE Minor1Description is not null";
        public static string ALL_STUDENT_HALLS = "SELECT DISTINCT BuildingDescription FROM Student WHERE BuildingDescription is not null ORDER BY BuildingDescription ASC";
        public static string ALL_ACCOUNTS_COUNTRIES = "SELECT DISTINCT Country FROM Student WHERE COUNTRY is not null UNION SELECT Country FROM FacStaff WHERE COUNTRY is not null UNION SELECT Country FROM Alumni WHERE COUNTRY is not null ORDER BY Country ASC";
        public static string ALL_ACCOUNTS_STATES = "SELECT HomeState FROM Student WHERE HomeState is not null UNION SELECT HomeState FROM FacStaff WHERE HomeState is not null UNION SELECT HomeState FROM Alumni WHERE HomeState is not null ORDER BY HomeState ASC";


        public static string ALL_FACSTAFF_BUILDINGS = "SELECT DISTINCT BuildingDescription FROM FacStaff WHERE OnCampusBuilding is not null ORDER BY BuildingDescription ASC";
        public static string ALL_FACSTAFF_DEPARTMENTS = "SELECT DISTINCT OnCampusDepartment FROM FacStaff WHERE OnCampusDepartment is not null ORDER BY OnCampusDepartment ASC";

        // Timesheets
        public static string ALL_STUDENT_TIMESHEETS_INFO = "SELECT * FROM student_timesheets for json path";

    }
}
