using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels
{
    public record AlumniProfileViewModel
        (
            string ID, string WebUpdate, string Title, string FirstName, string MiddleName, string LastName,
            string Suffix, string MaidenName, string NickName, string HomeStreet1, string HomeStreet2, string HomeCity,
            string HomeState, string HomePostalCode, string HomeCountry, string HomePhone, string HomeFax,
            string HomeEmail, string JobTitle, string MaritalStatus, string SpouseName, string College, string ClassYear,
            string PreferredClassYear, string Major, string Major2, string ShareName, string ShareAddress, string Gender,
            string GradDate, string Email, string grad_student, string Barcode, string AD_Username, int? show_pic,
            int? preferred_photo, string Country, string Major1Description, string Major2Description
        ) : ProfileViewModel()
    {
        public static implicit operator AlumniProfileViewModel?(Alumni? alu)
        {
            if (alu == null)
            {
                return null;
            }

            return new AlumniProfileViewModel
            (
                alu.ID.Trim(),
                alu.WebUpdate ?? "",
                alu.Title ?? "",
                alu.FirstName ?? "",
                alu.MiddleName ?? "",
                alu.LastName ?? "",
                alu.Suffix ?? "",
                alu.MaidenName ?? "",
                alu.NickName ?? "", // Just in case some random record has a null user_name 
                alu.HomeStreet1 ?? "",
                alu.HomeStreet2 ?? "",
                alu.HomeCity ?? "",
                alu.HomeState ?? "",
                alu.HomePostalCode ?? "",
                alu.HomeCountry ?? "",
                alu.HomePhone ?? "",
                alu.HomeFax ?? "",
                alu.HomeEmail ?? "",
                alu.JobTitle ?? "",
                alu.MaritalStatus ?? "",
                alu.SpouseName ?? "",
                alu.College ?? "",
                alu.ClassYear ?? "",
                alu.PreferredClassYear ?? "",
                alu.Major1 ?? "",
                alu.Major2 ?? "",
                alu.ShareName ?? "",
                alu.ShareAddress ?? "",
                alu.Gender ?? "",
                alu.GradDate ?? "",
                alu.Email ?? "",
                alu.grad_student ?? "",
                alu.Barcode ?? "",
                alu.AD_Username ?? "", // Just in case some random record has a null email field
                alu.show_pic,
                alu.preferred_photo,
                alu.Country ?? "",
                alu.Major1Description ?? "",
                alu.Major2Description ?? ""
            );
        }
    }
}