using System.Collections.Generic;

namespace Gordon360.Models.ViewModels
{
    // ViewModel for updating privacy settings

    // Field: field for which the visibility group will be updated
    //   (HomeStreet1, HomeStreet2, HomeCity, HomeState, HomeCountry, SpouseName,
    //   Country, HomePhone, MobilePhone)
    // VisibilityGroup: The group of users that can see the information corresponding to Field
    //   Currently a subset of Public, FacStaff, or Private
    public class UserPrivacyUpdateViewModel
    {
        public UserPrivacyUpdateViewModel(IEnumerable<string> field, string group)
        {
            Field = field;
            VisibilityGroup = group;
        }
        public IEnumerable<string> Field { get; set; }
        public string VisibilityGroup { get; set; }
    }
}