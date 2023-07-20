using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;

namespace Gordon360.Models.ViewModels
{
    // Field: the field where the user selected the privacy setting
    //        (HomeCity, HomeState, HomeCountry, SpouseName, Country, HomePhone, MobilePhone)
    // VisibilityGroup: the group that the user wanted to be seen by (Public, Private, FacStaff)
    public class UserPrivacyViewModel
    {
        //public UserPrivacyViewModel(string field, string group)
        //{
        //    Field = field;
        //    VisibilityGroup = group;
        //}
        public string Field { get; set; }
        public string VisibilityGroup { get; set; }
        public static implicit operator UserPrivacyViewModel(UserPrivacy_Settings up_s)
        {
            return new UserPrivacyViewModel
            {
                Field = up_s.Field,
                VisibilityGroup = up_s.Visibility
            };
        }
    }
}