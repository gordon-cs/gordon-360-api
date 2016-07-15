
namespace Gordon360.Static.Names
{
    public static class Resource
    {
         
        public const string MEMBERSHIP_REQUEST = "A Membership Request Resource";
        public const string MEMBERSHIP = "A Membership Resource";
        public const string STUDENT = "A Student Resource";
        public const string SUPERVISOR = "A Supervisor Resource";

        // Partial resources, to be targetted by Operation.READ_PARTIAL
        public const string MEMBERSHIP_REQUEST_BY_ACTIVITY = "Membership Request Resources associated with an activity";
        public const string MEMBERSHIP_REQUEST_BY_STUDENT = "Membership Request Resources associated with a student";
        public const string MEMBERSHIP_BY_ACTIVITY = "Membership Resources associated with an activity";
        public const string MEMBERSHIP_BY_STUDENT = "Membership Resources associated with a student";
        public const string SUPERVISOR_BY_ACTIVITY = "Supervisor Resources associated with an activity";



    }

    public static class Operation
    {
        public const string READ_ALL = "Reading all available resources";
        public const string READ_PARTIAL = "Reading a group of related resources";
        public const string READ_ONE = "Reading one resource";
        public const string ADD = "Creating a resource";
        public const string UPDATE = "Updating a resource";
        public const string DELETE = "Deleting a resource";

    }


    public static class Position
    {
        public const string STUDENT = "This person is a student";
        public const string FACSTAFF = "This person is either a faculty or staff member";
        public const string GOD = "This person is an admin and has all the permissions";
    }

    public static class Activity_Roles
    {
        public const string MEMBER = "MEMBR";
        public const string GUEST = "GUEST";
        public static readonly string[] LEADER = { "CAPT", "CODIR", "CORD", "DIREC", "PRES", "VICEC", "VICEP" };
    }

    
}