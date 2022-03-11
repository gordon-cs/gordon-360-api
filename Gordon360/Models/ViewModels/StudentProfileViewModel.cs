﻿using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels
{
    public record StudentProfileViewModel
        (
            string ID, string Title, string FirstName, string MiddleName, string LastName, string Suffix, string MaidenName,
            string NickName, string OnOffCampus, string OnCampusBuilding, string OnCampusRoom, string OnCampusPhone,
            string OnCampusPrivatePhone, string OnCampusFax, string OffCampusStreet1, string OffCampusStreet2,
            string OffCampusCity, string OffCampusState, string OffCampusPostalCode, string OffCampusCountry,
            string OffCampusPhone, string OffCampusFax, string HomeStreet1, string HomeStreet2, string HomeCity,
            string HomeState, string HomePostalCode, string HomeCountry, string HomePhone, string HomeFax, string Cohort,
            string Class, string KeepPrivate, string Barcode, string AdvisorIDs, string Married, string Commuter,
            string Major, string Major2, string Major3, string Minor1, string Minor2, string Minor3, string Email,
            string Gender, string grad_student, string GradDate, string MobilePhone, bool IsMobilePhonePrivate,
            string AD_Username, int? show_pic, int? preferred_photo, string Country, string BuildingDescription,
            string Major1Description, string Major2Description, string Major3Description, string Minor1Description,
            string Minor2Description, string Minor3Description, string Mail_Location, int? ChapelRequired,
        int? ChapelAttended) : ProfileViewModel()
    {
        public static implicit operator StudentProfileViewModel?(Student? stu)
        {
            if (stu == null)
            {
                return null;
            }

            return new StudentProfileViewModel(
                stu.ID.Trim(),
                stu.Title ?? "",
                stu.FirstName ?? "",
                stu.MiddleName ?? "",
                stu.LastName ?? "",
                stu.Suffix ?? "",
                stu.MaidenName ?? "",
                stu.NickName ?? "", // Just in case some random record has a null user_name 
                stu.OnOffCampus ?? "",
                stu.OnCampusBuilding ?? "",
                stu.OnCampusRoom ?? "",
                stu.OnCampusPhone ?? "",
                stu.OnCampusPrivatePhone ?? "",
                stu.OnCampusFax ?? "",
                stu.OffCampusStreet1 ?? "",
                stu.OffCampusStreet2 ?? "",
                stu.OffCampusCity ?? "",
                stu.OffCampusState ?? "",
                stu.OffCampusPostalCode ?? "",
                stu.OffCampusCountry ?? "",
                stu.OffCampusPhone ?? "",
                stu.OffCampusFax ?? "",
                stu.HomeStreet1 ?? "",
                stu.HomeStreet2 ?? "",
                stu.HomeCity ?? "",
                stu.HomeState ?? "",
                stu.HomePostalCode ?? "",
                stu.HomeCountry ?? "",
                stu.HomePhone ?? "",
                stu.HomeFax ?? "",
                stu.Cohort ?? "",
                stu.Class ?? "",
                stu.KeepPrivate ?? "",
                stu.Barcode ?? "",
                stu.AdvisorIDs ?? "",
                stu.Married ?? "",
                stu.Commuter ?? "",
                stu.Major ?? "",
                stu.Major2 ?? "",
                stu.Major3 ?? "",
                stu.Minor1 ?? "",
                stu.Minor2 ?? "",
                stu.Minor3 ?? "",
                stu.Email ?? "",
                stu.Gender ?? "",
                stu.grad_student ?? "",
                stu.GradDate ?? "",
                stu.MobilePhone ?? "",
                stu.IsMobilePhonePrivate == 1 ? true : false,
                stu.AD_Username ?? "", // Just in case some random record has a null email field
                stu.show_pic,
                stu.preferred_photo,
                stu.Country ?? "",
                stu.BuildingDescription ?? "",
                stu.Major1Description ?? "",
                stu.Major2Description ?? "",
                stu.Major3Description ?? "",
                stu.Minor1Description ?? "",
                stu.Minor2Description ?? "",
                stu.Minor3Description ?? "",
                stu.Mail_Location ?? "",
                stu.ChapelRequired ?? 0,
                stu.ChapelAttended ?? 0
            );
        }
    }
}





