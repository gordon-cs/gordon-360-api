namespace Gordon360.Models.ViewModels
{
    public record ProfileViewModel
    {
        record FacultyStaffProfileViewModel
        (
            string ID, string Title, string FirstName, string MiddleName, string LastName, string Suffix,
            string MaidenName, string NickName, string OnCampusDepartment, string OnCampusBuilding, string OnCampusRoom,
            string OnCampusPhone, string OnCampusPrivatePhone, string OnCampusFax, string HomeStreet1,
            string HomeStreet2, string HomeCity, string HomeState, string HomePostalCode, string HomeCountry,
            string HomePhone, string HomeFax, string KeepPrivate, string JobTitle, string SpouseName, string Dept,
            string Barcode, string Gender, string Email, string Type, string AD_Username, string office_hours,
            int? preferred_photo, int? show_pic, string BuildingDescription, string Country, string Mail_Location
        ) : ProfileViewModel();
    }
}
