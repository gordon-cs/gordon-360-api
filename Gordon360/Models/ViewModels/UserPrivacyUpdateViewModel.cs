using System.Collections.Generic;

namespace Gordon360.Models.ViewModels
{
    // Field: the field where the user wants to update the privacy setting (HomeCity, HomeState, HomeCountry, SpouseName, Country, HomePhone, MobilePhone)
    // VisibilityGroup: the group that the user wants to be seen by (Public, Private, FacStaff)
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