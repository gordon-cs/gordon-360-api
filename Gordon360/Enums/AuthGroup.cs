namespace Gordon360.Enums
{
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
        SiteAdmin,
        Staff,
        Student
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
            "360-SiteAdmin-SG" => AuthGroup.SiteAdmin,
            "360-Staff-SG" => AuthGroup.Staff,
            "360-Student-SG" => AuthGroup.Student,
            _ => null
        };
    }
}
