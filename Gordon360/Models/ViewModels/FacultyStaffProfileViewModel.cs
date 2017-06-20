using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class FacultyStaffProfileViewModel
    {
        public Nullable<int> Row_ID { get; set; }
        public string ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string EmailUserName { get; set; }
        public string OnCampusDepartment { get; set; }
        public string OnCampusBuilding { get; set; }
        public string OnCampusRoom { get; set; }
        public string OnCampusPhone { get; set; }
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
        public Nullable<System.DateTime> AppointDate { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string StudentID { get; set; }


        public static implicit operator FacultyStaffProfileViewModel(facstaff fac)
        {
            FacultyStaffProfileViewModel vm = new FacultyStaffProfileViewModel
            {
                Row_ID = fac.Row_ID,
                ID = fac.ID.Trim(),
                FirstName = fac.FirstName.Trim(),
                LastName = fac.LastName.Trim(),
                NickName = fac.Nickname ?? "", // Just in case some random record has a null user_name 
                EmailUserName = fac.EmailUserName.Trim() ?? "", // Just in case some random record has a null email field
                OnCampusDepartment = fac.OnCampusDepartment ?? "",
                OnCampusBuilding = fac.OnCampusBuilding ?? "",
                OnCampusRoom = fac.OnCampusRoom ?? "",
                OnCampusPhone = fac.OnCampusPhone ?? "",
                HomeStreet1 = fac.HomeStreet1 ?? "",
                HomeStreet2 = fac.HomeStreet2 ?? "",
                HomeCity = fac.HomeCity ?? "",
                HomeState = fac.HomeState ?? "",
                HomePostalCode = fac.HomePostalCode ?? "",
                HomeCountry = fac.HomeCountry ?? "",
                HomePhone = fac.HomePhone ?? "",
                KeepPrivate = fac.KeepPrivate ?? "",
                JobTitle = fac.JobTitle ?? "",
                Email = fac.Email ?? "",
                Gender = fac.Gender ?? "",
                StudentID = fac.StudentID ?? ""
            };

            return vm;
        }
    }
}