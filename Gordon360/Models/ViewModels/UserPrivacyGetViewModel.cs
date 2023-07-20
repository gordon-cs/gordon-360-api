namespace Gordon360.Models.ViewModels
{
    // Field: the field where the user selected the privacy setting
    //        (HomeCity, HomeState, HomeCountry, SpouseName, Country, HomePhone, MobilePhone)
    // VisibilityGroup: the group that the user wanted to be seen by (Public, Private, FacStaff)
    public class UserPrivacyGetViewModel
    {
        public UserPrivacyGetViewModel(string field, string group)
        {
            Field = field;
            VisibilityGroup = group;
        }
        public string Field { get; set; }
        public string VisibilityGroup { get; set; }
    }
}