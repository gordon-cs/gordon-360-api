using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;

namespace Gordon360.Models.ViewModels
{
    // Field: field for which the visibility group will be updated
    //   (HomeStreet1, HomeStreet2, HomeCity, HomeState, HomeCountry, SpouseName,
    //   Country, HomePhone, MobilePhone)
    // VisibilityGroup: The group of users that can see the information corresponding to Field
    //   Currently a subset of Public, FacStaff, or Private
    public class UserPrivacyViewModel
    {
        public static implicit operator UserPrivacyViewModel(UserPrivacy_Settings up_s)
        {
            return new UserPrivacyViewModel
            {
                Field = up_s.Field,
                VisibilityGroup = up_s.Visibility
            };
        }
        public string Field { get; set; }
        public string VisibilityGroup { get; set; }
    }
}