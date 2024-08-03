using System.Text.Json.Serialization;

namespace Gordon360.Models.ViewModels;

public record ProfileViewModel
{
    // All Profiles
    public string ID { get; set; }
    public string Title { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string Suffix { get; set; }
    public string MaidenName { get; set; }
    public string NickName { get; set; }
    public string Email { get; set; }
    public string Gender { get; set; }
    public string HomeStreet1 { get; set; }
    public string HomeStreet2 { get; set; }
    public string HomeCity { get; set; }
    public string HomeState { get; set; }
    public string HomePostalCode { get; set; }
    public string HomeCountry { get; set; }
    public string HomePhone { get; set; }
    public string HomeFax { get; set; }
    public string AD_Username { get; set; }
    public int? show_pic { get; set; }
    public int? preferred_photo { get; set; }
    public string Country { get; set; }
    public string Barcode { get; set; }
    public string Facebook { get; set; }
    public string Twitter { get; set; }
    public string Instagram { get; set; }
    public string LinkedIn { get; set; }
    public string Handshake { get; set; }
    public string Calendar { get; set; }

    // Student Only
    public string OnOffCampus { get; set; }
    public string OffCampusStreet1 { get; set; }
    public string OffCampusStreet2 { get; set; }
    public string OffCampusCity { get; set; }
    public string OffCampusState { get; set; }
    public string OffCampusPostalCode { get; set; }
    public string OffCampusCountry { get; set; }
    public string OffCampusPhone { get; set; }
    public string OffCampusFax { get; set; }
    public string Major3 { get; set; }
    public string Major3Description { get; set; }
    public string Minor1 { get; set; }
    public string Minor1Description { get; set; }
    public string Minor2 { get; set; }
    public string Minor2Description { get; set; }
    public string Minor3 { get; set; }
    public string Minor3Description { get; set; }
    public string GradDate { get; set; }
    public string PlannedGradYear { get; set; }
    public string MobilePhone { get; set; }
    public bool IsMobilePhonePrivate { get; set; }
    public int? ChapelRequired { get; set; }
    public int? ChapelAttended { get; set; }
    public string Cohort { get; set; }
    public string Class { get; set; }
    public string AdvisorIDs { get; set; }
    public string Married { get; set; }
    public string Commuter { get; set; }

    // Alumni Only
    public int? WebUpdate { get; set; }
    public string HomeEmail { get; set; }
    public string MaritalStatus { get; set; }
    public string College { get; set; }
    public string ClassYear { get; set; }
    public string? PreferredClassYear { get; set; }
    public string ShareName { get; set; }
    public string? ShareAddress { get; set; }

    // Student And Alumni Only
    public string Major { get; set; }
    public string Major1Description { get; set; }
    public string Major2 { get; set; }
    public string Major2Description { get; set; }
    public string grad_student { get; set; }

    // FacStaff Only
    public string? OnCampusDepartment { get; set; }
    public string? Type { get; set; }
    public string? office_hours { get; set; }
    public string Dept { get; set; }
    public string Mail_Description { get; set; }

    // FacStaff and Alumni Only
    public string JobTitle { get; set; }
    public string SpouseName { get; set; }

    // FacStaff and Student Only
    public string BuildingDescription { get; set; }
    public string Mail_Location { get; set; }
    public string OnCampusBuilding { get; set; }
    public string OnCampusRoom { get; set; }
    public string OnCampusPhone { get; set; }
    public string OnCampusPrivatePhone { get; set; }
    public string OnCampusFax { get; set; }
    public string KeepPrivate { get; set; }

    // ProfileViewModel Only
    public string PersonType { get; set; }
}
