using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels
{
    // ViewModel for retrieving privacy settings

    // Field: field for which the visibility group will be updated
    //   (HomeStreet1, HomeStreet2, HomeCity, HomeState, HomeCountry, SpouseName,
    //   Country, HomePhone, MobilePhone)
    // VisibilityGroup: The group of users that can see the information corresponding to Field
    //   Currently a subset of Public, FacStaff, or Private
    public class UserPrivacyViewModel
    {
        // Constants to match IDs in CCT.dboUserPrivacy_Fields table
        public const int HomeCityID = 1;
        public const int HomeStateID = 2;
        public const int HomeCountryID = 3;
        public const int SpouseNameID = 4;
        public const int CountryID = 5;
        public const int HomePhoneID = 6;
        public const int MobilePhoneID = 7;
        public const int HomeStreet1ID = 8;
        public const int HomeStreet2ID = 9;

        public const int Public_GroupID = 1;
        public const int FacStaff_GroupID = 2;
        public const int Private_GroupID = 3;

        public static implicit operator UserPrivacyViewModel(UserPrivacy_Settings up_s)
        {
            return new UserPrivacyViewModel
            {
                Field = up_s.FieldNavigation.Field,
                VisibilityGroup = up_s.VisibilityNavigation.Group
            };
        }
        public string Field { get; set; }
        public string VisibilityGroup { get; set; }
    }
}