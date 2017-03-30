
namespace Gordon360.Static.Names
{
    public static class Resource
    {
         
        public const string MEMBERSHIP_REQUEST = "A Membership Request Resource";
        public const string MEMBERSHIP = "A Membership Resource";
        public const string STUDENT = "A Student Resource";
        public const string ACCOUNT = "An Account Resource.";
        public const string ADVISOR = "An Advisor Resource";
        public const string GROUP_ADMIN = "A Group Admin Resource";
        public const string ADMIN = "An admininistrator Ressource";
        public const string ACTIVITY_INFO = "An Activity Info Service.";

        // Partial resources, to be targetted by Operation.READ_PARTIAL
        public const string MEMBERSHIP_REQUEST_BY_ACTIVITY = "Membership Request Resources associated with an activity";
        public const string MEMBERSHIP_REQUEST_BY_STUDENT = "Membership Request Resources associated with a student";
        public const string MEMBERSHIP_BY_ACTIVITY = "Membership Resources associated with an activity";
        public const string MEMBERSHIP_BY_STUDENT = "Membership Resources associated with a student";
        public const string EMAILS_BY_ACTIVITY = "Emails for activity members";
        public const string EMAILS_BY_LEADERS = "Emails for activity leaders";
        public const string ADVISOR_BY_ACTIVITY = "Advisor Resources associated with an activity";
        public const string LEADER_BY_ACTIVITY = "Leader Ressources associated with an activity";
        public const string GROUP_ADMIN_BY_ACTIVITY = "Group Admin Resources associated with an activity";


    }

    public static class Operation
    {
        public const string READ_ALL = "Reading all available resources";
        public const string READ_PARTIAL = "Reading a group of related resources";
        public const string READ_ONE = "Reading one resource";
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
    }
    
}