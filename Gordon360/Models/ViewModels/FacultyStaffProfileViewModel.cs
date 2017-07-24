using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class FacultyStaffProfileViewModel
    {
        public string ID { get; set; }
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
        public string Barcode { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public string AD_Username { get; set; }
        public string office_hours { get; set; }
        public Nullable<int> preferred_photo { get; set; }
        public Nullable<int> show_pic { get; set; }
        public string BuildingDescription { get; set; }
        public string Country { get; set; }


        public static implicit operator FacultyStaffProfileViewModel(FacStaff fac)
        {
            FacultyStaffProfileViewModel vm = new FacultyStaffProfileViewModel
            {
                ID = fac.ID.Trim(),
                Title = fac.Title ?? "",
                Suffix = fac.Suffix ?? "",
                MaidenName = fac.MaidenName ?? "",
                FirstName = fac.FirstName,
                LastName = fac.LastName,
                MiddleName = fac.MiddleName ?? "",
                NickName = fac.Nickname ?? "", 
                AD_Username = fac.AD_Username ?? "",
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
                Dept = fac.Dept ?? "",
                SpouseName = fac.SpouseName ?? "",
                Barcode = fac.Barcode ?? "",
                Email = fac.Email ?? "",
                Type = fac.Type ?? "",
                Gender = fac.Gender ?? "",
                office_hours = fac.office_hours ?? "",
                show_pic = fac.show_pic,
                preferred_photo = fac.preferred_photo,
                BuildingDescription = fac.BuildingDescription ?? "",
                Country = fac.Country ?? ""
            };

            return vm;
        }
    }
}