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
        public const string ADMIN = "An admininistrator Ressource";
        public const string ACTIVITY_INFO = "An Activity Info Service.";
        public const string ACTIVITY_STATUS = "The open or closed status of an activity";
        public const string ChapelEvent = "The info of chapel events";
        public const string DINING = "Info related to dining service";


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
        public const string GOD = "god";
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
        public const string DEFAULT_PROFILE_IMAGE_PATH = "http://www.sessionlogs.com/media/icons/defaultIcon.png";
        //public const string DEFAULT_PROFILE_IMAGE_PATH = "https://360apitrain.gordon.edu/browseable/profile/Default/profile.png";
        public const string DEFAULT_PREF_IMAGE_PATH = "\\\\gotrain\\pref_photos\\";
        public const string DEFAULT_IMAGE_PATH = "\\\\go\\photos\\";
    }
    
    public static class URLs
    {
        // This url makes use of the 25Live API to retrieve events based on the "event_type" parameter. 
        // We also make use of the "end after" field to get only events from this academic year.
        public static string ALL_EVENTS_REQUEST = "https://25live.collegenet.com/25live/data/gordon/run/events.xml?/&event_type_id=10+12+13+14+16+17+18+19+51+20+21+22+23+24+25+29+30+31+33+35&state=2&end_after=" + Helpers.GetDay() + "0810&scope=extended";

    }
   
    public static class SQLQuery
    {
        public static string ALL_STUDENT_REQUEST = "SELECT * from Student WHERE AD_Username is not null";
        public static string ALL_FACULTY_STAFF_REQUEST = "SELECT * from FacStaff WHERE AD_Username is not null";
        public static string ALL_ALUMNI_REQUEST = "SELECT * from Alumni WHERE AD_Username is not null";
    }
}