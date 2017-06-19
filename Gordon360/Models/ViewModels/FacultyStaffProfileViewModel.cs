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


        public static implicit operator FacultyStaffProfileViewModel(facstaff f)
        {
            FacultyStaffProfileViewModel vm = new FacultyStaffProfileViewModel
            {
                Row_ID = f.Row_ID,
                ID = f.ID.Trim(),
                FirstName = f.FirstName.Trim(),
                LastName = f.LastName.Trim(),
                NickName = f.Nickname.Trim() ?? "", // Just in case some random record has a null user_name 
                EmailUserName = f.EmailUserName.Trim() ?? "", // Just in case some random record has a null email field
                OnCampusDepartment = f.OnCampusDepartment ?? "",
                OnCampusBuilding = f.OnCampusBuilding ?? "",
                OnCampusRoom = f.OnCampusRoom ?? "",
                OnCampusPhone = f.OnCampusPhone ?? "",
                HomeStreet1 = f.HomeStreet1 ?? "",
                HomeStreet2 = f.HomeStreet2 ?? "",
                HomeCity = f.HomeCity ?? "",
                HomeState = f.HomeState ?? "",
                HomePostalCode = f.HomePostalCode ?? "",
                HomeCountry = f.HomeCountry ?? "",
                HomePhone = f.HomePhone ?? "",
                KeepPrivate = f.KeepPrivate ?? "",
                JobTitle = f.JobTitle ?? "",
                Email = f.Email ?? "",
                Gender = f.Gender ?? "",
                StudentID = f.StudentID ?? ""
            };

            return vm;
        }
    }
}