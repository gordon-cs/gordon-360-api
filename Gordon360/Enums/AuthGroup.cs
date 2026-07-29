namespace Gordon360.Enums;

public enum AuthGroup
{
    AcademicInfoView,
    Advisors,
    Alumni,
    FacStaff,
    Faculty,
    HousingAdmin,
    NewsAdmin,
    Police,
    RecIMSuperAdmin,
    SiteAdmin,
    Staff,
    Student,
    RA,
    RD,
    HallInfoViewer,
    HousingDeveloper, //Remove before deployment
    LostAndFoundAdmin,
    LostAndFoundAssist,
    LostAndFoundDevelopers,
}

public static class AuthGroupEnum
{
    public static AuthGroup? FromString(string groupName) => groupName switch
    {
        "360-AcademicInfoView-SG" => AuthGroup.AcademicInfoView,
        "360-Advisors-SG" => AuthGroup.Advisors,
        "360-Alumni-SG" => AuthGroup.Alumni,
        "360-FacStaff-SG" => AuthGroup.FacStaff,
        "360-Faculty-SG" => AuthGroup.Faculty,
        "360-HousingAdmin-SG" => AuthGroup.HousingAdmin,
        "360-NewsAdmin-SG" => AuthGroup.NewsAdmin,
        "360-Police-SG" => AuthGroup.Police,
        "360-RecIMAdmin-SG" => AuthGroup.RecIMSuperAdmin,
        "360-SiteAdmin-SG" => AuthGroup.SiteAdmin,
        "360-Staff-SG" => AuthGroup.Staff,
        "360-Student-SG" => AuthGroup.Student,
        "360-ResLifeStudentWorker-SG" => AuthGroup.RA,
        "360-HallInfoViewer-SG" => AuthGroup.HallInfoViewer,
        "360-ResidentDirector" => AuthGroup.RD,
        "360-HousingDevelopers-SG" => AuthGroup.HousingDeveloper,
        "360-LostAndFoundAdmins-SG" => AuthGroup.LostAndFoundAdmin,
        "360-LostAndFoundAssist-SG" => AuthGroup.LostAndFoundAssist,
        "360-LostAndFound-Developers-SG" => AuthGroup.LostAndFoundDevelopers,
        _ => null
    };
}
