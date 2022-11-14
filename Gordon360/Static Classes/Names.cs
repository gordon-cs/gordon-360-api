namespace Gordon360.Static.Names
{
    public static class Resource
    {
        public const string EMERGENCY_CONTACT = "A new emergency contact resource";
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
        public const string CHECKIN = "Info relating to a student's Academic Check-In";
        public const string SHIFT = "A shift that a student has worked";
        public const string CLIFTON_STRENGTHS = "A student's uploaded clifton strengthsfinder results";

        // Partial resources, to be targetted by Operation.READ_PARTIAL
        public const string MEMBERSHIP_REQUEST_BY_ACTIVITY = "Membership Request Resources associated with an activity";
        public const string MEMBERSHIP_REQUEST_BY_STUDENT = "Membership Request Resources associated with a student";
        public const string MEMBERSHIP_BY_ACTIVITY = "Membership Resources associated with an activity";
        public const string MEMBERSHIP_BY_ACCOUNT = "Membership Resources associated with a student";
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
    }


    public static class Position
    {
        public const string STUDENT = "student";
        public const string FACSTAFF = "facstaff";
        public const string ALUMNI = "alumni";
        public const string SUPERADMIN = "god";      // TODO: change in database to something more reverent
        public const string POLICE = "gordon police";
        public const string READONLY = "readonly";
        public const string DEFAULT = "default";
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
}
