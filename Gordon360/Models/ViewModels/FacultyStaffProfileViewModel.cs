using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels
{
    public record FacultyStaffProfileViewModel
        (
        string ID,
        string Title,
        string FirstName,
        string MiddleName,
        string LastName,
        string Suffix,
        string MaidenName,
        string NickName,
        string OnCampusDepartment,
        string OnCampusBuilding,
        string OnCampusRoom,
        string OnCampusPhone,
        string OnCampusPrivatePhone,
        string OnCampusFax,
        string HomeStreet1,
        string HomeStreet2,
        string HomeCity,
        string HomeState,
        string HomePostalCode,
        string HomeCountry,
        string HomePhone,
        string HomeFax,
        string KeepPrivate,
        string JobTitle,
        string Dept,
        string SpouseName,
        string Barcode,
        string Gender,
        string Email,
        string Type,
        string AD_Username,
        string office_hours,
        int? preferred_photo,
        int? show_pic,
        string BuildingDescription,
        string Country,
        string Mail_Location)
    {
        public static implicit operator FacultyStaffProfileViewModel?(FacStaff? fac)
        {
            if (fac == null)
            {
                return null;
            }

            return new FacultyStaffProfileViewModel(
                fac.ID.Trim(),
                fac.Title ?? "",
                fac.FirstName ?? "",
                fac.MiddleName ?? "",
                fac.LastName ?? "",
                fac.Suffix ?? "",
                fac.MaidenName ?? "",
                fac.Nickname ?? "",
                fac.OnCampusDepartment ?? "",
                fac.OnCampusBuilding ?? "",
                fac.OnCampusRoom ?? "",
                fac.OnCampusPhone ?? "",
                fac.OnCampusPrivatePhone ?? "",
                fac.OnCampusFax ?? "",
                fac.HomeStreet1 ?? "",
                fac.HomeStreet2 ?? "",
                fac.HomeCity ?? "",
                fac.HomeState ?? "",
                fac.HomePostalCode ?? "",
                fac.HomeCountry ?? "",
                fac.HomePhone ?? "",
                fac.HomeFax ?? "",
                fac.KeepPrivate ?? "",
                fac.JobTitle ?? "",
                fac.Dept ?? "",
                fac.SpouseName ?? "",
                fac.Barcode ?? "",
                fac.Gender ?? "",
                fac.Email ?? "",
                fac.Type ?? "",
                fac.AD_Username ?? "",
                fac.office_hours ?? "",
                fac.preferred_photo,
                fac.show_pic,
                fac.BuildingDescription ?? "",
                fac.Country ?? "",
                fac.Mail_Location ?? "",
                fac.Mail_Description ?? ""
            );
        }
    }
}