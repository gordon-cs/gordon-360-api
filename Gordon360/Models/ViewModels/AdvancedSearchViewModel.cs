using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels
{
    public class AdvancedSearchViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string MaidenName { get; set; }
        public string HomeCity { get; set; }
        public string HomeState { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string AD_Username { get; set; }
        public string Hall { get; set; }
        public string Class { get; set; }
        public string Major1Description { get; set; }
        public string Major2Description { get; set; }
        public string Major3Description { get; set; }
        public string Minor1Description { get; set; }
        public string Minor2Description { get; set; }
        public string Minor3Description { get; set; }
        public string KeepPrivate { get; set; }
        public string Mail_Location { get; set; }
        public string OnCampusDepartment { get; set; }
        public string BuildingDescription { get; set; }
        public string JobTitle { get; set; }
        public string Type { get; set; }
        public string ShareName { get; set; }
        public string PreferredClassYear { get; set; }
        public string ShareAddress { get; set; }

        public static implicit operator AdvancedSearchViewModel(Student s)
        {
            return new AdvancedSearchViewModel
            {
                FirstName = s.FirstName ?? "",
                LastName = s.LastName ?? "",
                NickName = s.NickName ?? "",
                MaidenName = s.MaidenName ?? "",
                HomeCity = s.HomeCity ?? "",
                HomeState = s.HomeState ?? "",
                Country = s.Country ?? "",
                Email = s.Email ?? "",
                AD_Username = s.AD_Username ?? "",
                Hall = s.BuildingDescription ?? "",
                Class = s.Class ?? "",
                Major1Description = s.Major1Description ?? "",
                Major2Description = s.Major2Description ?? "",
                Major3Description = s.Major3Description ?? "",
                Minor1Description = s.Minor1Description ?? "",
                Minor2Description = s.Minor2Description ?? "",
                Minor3Description = s.Minor3Description ?? "",
                KeepPrivate = s.KeepPrivate ?? "",
                Mail_Location = s.Mail_Location ?? "",
                OnCampusDepartment = "",
                BuildingDescription = s.BuildingDescription ?? "",
                JobTitle = "",
                Type = "Student",
                ShareName = "",
                PreferredClassYear = "",
                ShareAddress = ""
            };
        }

        public static implicit operator AdvancedSearchViewModel(FacStaff fs)
        {
            return new AdvancedSearchViewModel
            {
                FirstName = fs.FirstName ?? "",
                LastName = fs.LastName ?? "",
                NickName = fs.Nickname ?? "",
                MaidenName = fs.MaidenName ?? "",
                HomeCity = fs.HomeCity ?? "",
                HomeState = fs.HomeState ?? "",
                Country = fs.Country ?? "",
                Email = fs.Email ?? "",
                AD_Username = fs.AD_Username ?? "",
                Hall = fs.BuildingDescription ?? "",
                Class = "",
                Major1Description = "",
                Major2Description = "",
                Major3Description = "",
                Minor1Description = "",
                Minor2Description = "",
                Minor3Description = "",
                KeepPrivate = fs.KeepPrivate ?? "",
                Mail_Location = fs.Mail_Location ?? "",
                OnCampusDepartment = fs.OnCampusDepartment ?? "",
                BuildingDescription = fs.BuildingDescription ?? "",
                JobTitle = fs.JobTitle ?? "",
                Type = fs.Type ?? "",
                ShareName = "",
                PreferredClassYear = "",
                ShareAddress = ""
            };
        }

        public static implicit operator AdvancedSearchViewModel(Alumni a)
        {
            return new AdvancedSearchViewModel
            {
                FirstName = a.FirstName ?? "",
                LastName = a.LastName ?? "",
                NickName = a.NickName ?? "",
                MaidenName = a.MaidenName ?? "",
                HomeCity = a.HomeCity ?? "",
                HomeState = a.HomeState ?? "",
                Country = a.Country ?? "",
                Email = a.Email ?? "",
                AD_Username = a.AD_Username ?? "",
                Hall = "",
                Class = "",
                Major1Description = a.Major1Description ?? "",
                Major2Description = a.Major2Description ?? "",
                Major3Description = "",
                Minor1Description = "",
                Minor2Description = "",
                Minor3Description = "",
                KeepPrivate = "",
                Mail_Location = "",
                OnCampusDepartment = "",
                BuildingDescription = "",
                JobTitle = "",
                Type = "Alumni",
                ShareName = a.ShareName ?? "",
                PreferredClassYear = a.PreferredClassYear ?? "",
                ShareAddress = a.ShareAddress ?? ""
            };
        }
    }
}
