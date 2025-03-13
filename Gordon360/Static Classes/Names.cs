using System;

namespace Gordon360.Static.Names;

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
    public const string HOUSING_ROOM_RANGE = "Information related to room ranges in housing";
    public const string HOUSING_RA_ASSIGNMENT = "Resident Advisor assignments in housing";
    public const string RA_CHECKIN = "Info relating to an RA Checkin";
    public const string HOUSING_CONTACT_PREFERENCE = "Resident Advisor preferred contact methods";
    public const string HOUSING_ON_CALL_RA = "Information about on-call Resident Advisors";
    public const string HOUSING_RD_ON_CALL = "Information about on-call Resident Directors";
    public const string HOUSING_HALL_TASK = "Info related to tasks for a Hall";
    public const string HOUSING_HALL_TASK_COMPLETE = "Info related to a tasks status";
    public const string HOUSING_RA_STATUS_EVENT = "Info related to status events for an RA";
    public const string ERROR_LOG = "The error log resource";
    public const string NEWS = "A student news resource";
    public const string NEWS_APPROVAL = "The approval of a student news resource";
    public const string CHECKIN = "Info relating to a student's Academic Check-In";
    public const string CLIFTON_STRENGTHS = "A student's uploaded clifton strengthsfinder results";
    public const string RECIM = "A general or admin RecIM resource";
    public const string RECIM_ACTIVITY = "A RecIM activity resource";
    public const string RECIM_AFFILIATION = "A RecIM Affiliation (Halls,Clubs...etc.)";
    public const string RECIM_SERIES = "A RecIM series resource";
    public const string RECIM_MATCH = "A RecIM match resource";
    public const string RECIM_TEAM = "A RecIM team resource";
    public const string RECIM_SPORT = "A RecIM sport resource";
    public const string RECIM_PARTICIPANT = "A RecIM participating user resource";
    public const string RECIM_PARTICIPANT_ADMIN = "The admin status of a RecIM participating user";
    public const string RECIM_SUPER_ADMIN = "A RecIM director level resource";
    public const string RECIM_SURFACE = "RecIM Surfaces/Playing fields/Locations";
    public const string LOST_AND_FOUND_MISSING_REPORT = "Lost and Found missing item reports";
    public const string STUDENT_SCHEDULE = "A student's schedule events";

    // Partial resources, to be targetted by Operation.READ_PARTIAL
    public const string MEMBERSHIP_REQUEST_BY_ACTIVITY = "Membership Request Resources associated with an activity";
    public const string MEMBERSHIP_REQUEST_BY_STUDENT = "Membership Request Resources associated with a student";
    [Obsolete("Unused once obsolete routes are removed")]
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

public static class Time_Zones
{
    public const string EST = "Eastern Standard Time";
    //fill out when needed
}

public static class RecIM_Resources
{
    public const string CUSTOM_PARTICIPANT_USERNAME_SUFFIX = ".custom";
}
