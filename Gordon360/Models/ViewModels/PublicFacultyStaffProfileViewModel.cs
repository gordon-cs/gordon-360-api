using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public string HomeStreet1 { get; set; }
        public string HomeStreet2 { get; set; }
        public string HomeCity { get; set; }
        public string HomeState { get; set; }
        public string HomePostalCode { get; set; }
        public string HomeCountry { get; set; }
        public string HomePhone { get; set; }
        public string HomeFax { get; set; }
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




        public static implicit operator PublicFacultyStaffProfileViewModel(FacultyStaffProfileViewModel fac)
        {
            PublicFacultyStaffProfileViewModel vm = new PublicFacultyStaffProfileViewModel
            {
                Title = fac.Title ?? "",
                Suffix = fac.Suffix ?? "",
                MaidenName = fac.MaidenName ?? "",
                FirstName = fac.FirstName.Trim(),
                LastName = fac.LastName.Trim(),
                MiddleName = fac.MiddleName ?? "",
                NickName = fac.NickName ?? "", // Just in case some random record has a null user_name 
                AD_Username = fac.AD_Username.Trim() ?? "", // Just in case some random record has a null email field
                OnCampusDepartment = fac.OnCampusDepartment ?? "",
                OnCampusBuilding = fac.OnCampusBuilding ?? "",
                OnCampusRoom = fac.OnCampusRoom ?? "",
                OnCampusPhone = fac.OnCampusPhone ?? "",
                OnCampusPrivatePhone = fac.OnCampusPrivatePhone ?? "",
                OnCampusFax = fac.OnCampusFax ?? "",
                HomeStreet1 = fac.HomeStreet1 ?? "",
                HomeStreet2 = fac.HomeStreet2 ?? "",
                HomeCity = fac.HomeCity ?? "",
                HomeState = fac.HomeState ?? "",
                HomePostalCode = fac.HomePostalCode ?? "",
                HomeCountry = fac.HomeCountry ?? "",
                HomePhone = fac.HomePhone ?? "",
                HomeFax = fac.HomeFax ?? "",
                KeepPrivate = fac.KeepPrivate ?? "",
                JobTitle = fac.JobTitle ?? "",
                SpouseName = fac.SpouseName ?? "",
                Type = fac.Type ?? "",
                Dept = fac.Dept ?? "",
                Email = fac.Email ?? "",
                Gender = fac.Gender ?? "",
                office_hours = fac.office_hours ?? "",
                preferred_photo = fac.preferred_photo,
                show_pic = fac.show_pic
            };
            if (vm.KeepPrivate.Contains("S"))
            {
                vm.HomeStreet1 = "Private as requested.";
                vm.HomeStreet2 = "Private as requested.";
                vm.HomeCity = "Private as requested.";
                vm.HomeState = "Private as requested.";
                vm.HomeCountry = "Private as requested.";
                vm.HomePostalCode = "Private as requested.";
                vm.HomePhone = "Private as requested.";
                vm.HomeFax = "Private as requested.";
                vm.SpouseName = "Private as requested.";
            }
            return vm;
        }
    }
}