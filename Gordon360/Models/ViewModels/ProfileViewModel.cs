namespace Gordon360.Models.ViewModels;

public record ProfileViewModel(
    // All Profiles
    string ID,
    string Title,
    string FirstName,
    string MiddleName,
    string LastName,
    string Suffix,
    string MaidenName,
    string NickName,
    string Email,
    string Gender,
    string HomeStreet1,
    string HomeStreet2,
    string HomeCity,
    string HomeState,
    string HomePostalCode,
    string HomeCountry, // Abbreviation of Country
    string HomePhone,
    string HomeFax,
    string AD_Username,
    int? show_pic,
    int? preferred_photo,
    string Country,
    string Barcode,
    string Facebook,
    string Twitter,
    string Instagram,
    string LinkedIn,
    string Handshake,
    string Calendar,

    // Student Only
    string OnOffCampus,
    string OffCampusStreet1,
    string OffCampusStreet2,
    string OffCampusCity,
    string OffCampusState,
    string OffCampusPostalCode,
    string OffCampusCountry,
    string OffCampusPhone,
    string OffCampusFax,
    string Major3,
    string Major3Description,
    string Minor1,
    string Minor1Description,
    string Minor2,
    string Minor2Description,
    string Minor3,
    string Minor3Description,
    string GradDate,
    string PlannedGradYear,
    string MobilePhone,
    bool IsMobilePhonePrivate,
    int? ChapelRequired,
    int? ChapelAttended,
    string Cohort,
    string Class,
    string AdvisorIDs,
    string Married,
    string Commuter,

    // Alumni Only
    string? WebUpdate,
    string HomeEmail,
    string MaritalStatus,
    string College,
    string ClassYear,
    string? PreferredClassYear,
    string ShareName,
    string? ShareAddress,

    // Student And Alumni Only
    string Major,
    string Major1Description,
    string Major2,
    string Major2Description,
    string grad_student,

    // FacStaff Only
    string? OnCampusDepartment,
    string? Type,
    string? office_hours,
    string Dept,
    string Mail_Description,

    // FacStaff and Alumni Only
    string JobTitle,
    string SpouseName,

    // FacStaff and Student Only
    string BuildingDescription,
    string Mail_Location,
    string OnCampusBuilding,
    string OnCampusRoom,
    string OnCampusPhone,
    string OnCampusPrivatePhone,
    string OnCampusFax,
    string KeepPrivate,

    // ProfileViewModel Only
    string PersonType
    );