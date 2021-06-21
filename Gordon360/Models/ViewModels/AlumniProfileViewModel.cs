using System;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Models.ViewModels
{
    public class AlumniProfileViewModel
    {
        public string ID { get; set; }
        public string WebUpdate { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string MaidenName { get; set; }
        public string NickName { get; set; }
        public string HomeStreet1 { get; set; }
        public string HomeStreet2 { get; set; }
        public string HomeCity { get; set; }
        public string HomeState { get; set; }
        public string HomePostalCode { get; set; }
        public string HomeCountry { get; set; }
        public string HomePhone { get; set; }
        public string HomeFax { get; set; }
        public string HomeEmail { get; set; }
        public string JobTitle { get; set; }
        public string MaritalStatus { get; set; }
        public string SpouseName { get; set; }
        public string College { get; set; }
        public string ClassYear { get; set; }
        public string PreferredClassYear { get; set; }
        public string Major { get; set; }
        public string Major2 { get; set; }
        public string ShareName { get; set; }
        public string ShareAddress { get; set; }
        public string Gender { get; set; }
        public string GradDate { get; set; }
        public string Email { get; set; }
        public string grad_student { get; set; }
        public string Barcode { get; set; }
        public string AD_Username { get; set; }
        public Nullable<int> show_pic { get; set; }
        public Nullable<int> preferred_photo { get; set; }
        public string Country { get; set; }
        public string Major2Description { get; set; }
        public string Major1Description { get; set; }


        public static implicit operator AlumniProfileViewModel(Alumni alu)
        {
            AlumniProfileViewModel vm = new AlumniProfileViewModel
            {
                ID = alu.ID.Trim(),
                WebUpdate = alu.WebUpdate ?? "",
                Title = alu.Title ?? "",
                FirstName = alu.FirstName ?? "",
                MiddleName = alu.MiddleName ?? "",
                LastName = alu.LastName ?? "",
                Suffix = alu.Suffix ?? "",
                MaidenName = alu.MaidenName ?? "",
                NickName = alu.NickName ?? "", // Just in case some random record has a null user_name 
                AD_Username = alu.AD_Username ?? "", // Just in case some random record has a null email field
                HomeStreet1 = alu.HomeStreet1 ?? "",
                HomeStreet2 = alu.HomeStreet2 ?? "",
                HomeCity = alu.HomeCity ?? "",
                HomeState = alu.HomeState ?? "",
                HomePostalCode = alu.HomePostalCode ?? "",
                HomeCountry = alu.HomeCountry ?? "",
                HomePhone = alu.HomePhone ?? "",
                HomeFax = alu.HomeFax ?? "",
                HomeEmail = alu.HomeEmail ?? "",
                JobTitle = alu.JobTitle ?? "",
                MaritalStatus = alu.MaritalStatus ?? "",
                SpouseName = alu.SpouseName ?? "",
                College = alu.College ?? "",
                ClassYear = alu.ClassYear ?? "",
                PreferredClassYear = alu.PreferredClassYear ?? "",
                Major = alu.Major1 ?? "",
                Major2 = alu.Major2 ?? "",
                ShareName = alu.ShareName ?? "",
                ShareAddress = alu.ShareAddress ?? "",
                Gender = alu.Gender ?? "",
                GradDate = alu.GradDate ?? "",
                Email = alu.Email ?? "",
                grad_student = alu.grad_student ?? "",
                Barcode = alu.Barcode ?? "",
                show_pic = alu.show_pic,
                preferred_photo = alu.preferred_photo,
                Country = alu.Country ?? "",
                Major1Description = alu.Major1Description ?? "",
                Major2Description = alu.Major2Description ?? ""
            };

            return vm;
        }
    }
}