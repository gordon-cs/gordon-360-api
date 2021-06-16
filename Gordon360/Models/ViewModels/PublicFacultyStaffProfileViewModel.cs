using System;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Models.ViewModels
{
    public class PublicFacultyStaffProfileViewModel
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string MaidenName { get; set; }
        public string NickName { get; set; }
        public string OnCampusDepartment { get; set; }
        public string OnCampusBuilding { get; set; }
        public string OnCampusRoom { get; set; }
        public string OnCampusPhone { get; set; }
        public string OnCampusPrivatePhone { get; set; }
        public string OnCampusFax { get; set; }
        public string HomePhone { get; set; }
        public string HomeCity { get; set; }
        public string HomeState { get; set; }
        public string HomeCountry { get; set; }
        public string KeepPrivate { get; set; }
        public string JobTitle { get; set; }
        public string SpouseName { get; set; }
        public string Dept { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public string AD_Username { get; set; }
        public string office_hours { get; set; }
        public Nullable<int> preferred_photo { get; set; }
        public Nullable<int> show_pic { get; set; }
        public string BuildingDescription { get; set; }
        public string Country { get; set; }
        public string Mail_Location { get; set; }
        public string Mail_Description { get; set; }




        public static implicit operator PublicFacultyStaffProfileViewModel(FacultyStaffProfileViewModel fac)
        {
            PublicFacultyStaffProfileViewModel vm = new PublicFacultyStaffProfileViewModel
            {
                Title = fac.Title ?? "",
                Suffix = fac.Suffix ?? "",
                MaidenName = fac.MaidenName ?? "",
                FirstName = fac.FirstName ?? "",
                LastName = fac.LastName ?? "",
                MiddleName = fac.MiddleName ?? "",
                NickName = fac.NickName ?? "", // Just in case some random record has a null user_name
                AD_Username = fac.AD_Username ?? "", // Just in case some random record has a null email field
                OnCampusDepartment = fac.OnCampusDepartment ?? "",
                OnCampusBuilding = fac.OnCampusBuilding ?? "",
                OnCampusRoom = fac.OnCampusRoom ?? "",
                OnCampusPhone = fac.OnCampusPhone ?? "",
                OnCampusPrivatePhone = fac.OnCampusPrivatePhone ?? "",
                OnCampusFax = fac.OnCampusFax ?? "",
                HomePhone = fac.HomePhone ?? "",
                HomeCity = fac.HomeCity ?? "",
                HomeState = fac.HomeState ?? "",
                HomeCountry = fac.HomeCountry ?? "",
                KeepPrivate = fac.KeepPrivate ?? "",
                JobTitle = fac.JobTitle ?? "",
                SpouseName = fac.SpouseName ?? "",
                Type = fac.Type ?? "",
                Dept = fac.Dept ?? "",
                Email = fac.Email ?? "",
                Gender = fac.Gender ?? "",
                office_hours = fac.office_hours ?? "",
                preferred_photo = fac.preferred_photo,
                show_pic = fac.show_pic,
                BuildingDescription = fac.BuildingDescription ?? "",
                Country = fac.Country ?? "",
                Mail_Location = fac.Mail_Location ?? "",
                Mail_Description = fac.Mail_Description ?? ""
            };
            if (vm.KeepPrivate.Contains("1"))
            {
                vm.HomeCity = "Private as requested.";
                vm.HomeState = "";
                vm.HomeCountry = "";
                vm.SpouseName = "Private as requested.";
                vm.Country = "";
                vm.HomePhone = "";
            }
            return vm;
        }
    }
}
