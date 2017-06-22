using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class StudentProfileViewModel
    {
        public int Row_ID { get; set; }
        public string ID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string MaidenName { get; set; }
        public string NickName { get; set; }
        public string EmailUserName { get; set; }
        public string OnOffCampus { get; set; }
        public string OnCampusBuilding { get; set; }
        public string OnCampusRoom { get; set; }
        public string OnCampusPhone { get; set; }
        public string OnCampusPrivatePhone { get; set; }
        public string HomeStreet1 { get; set; }
        public string HomeStreet2 { get; set; }
        public string HomeCity { get; set; }
        public string HomeState { get; set; }
        public string HomePostalCode { get; set; }
        public string HomeCountry { get; set; }
        public string HomePhone { get; set; }
        public string Class { get; set; }
        public string KeepPrivate { get; set; }
        public string Major { get; set; }
        public string Major2 { get; set; }
        public string Major3 { get; set; }
        public string Minor1 { get; set; }
        public string Minor2 { get; set; }
        public string Minor3 { get; set; }
        public string Email { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string MobilePhone { get; set; }
        public Nullable<bool> IsMobilePhonePrivate { get; set; }


        public static implicit operator StudentProfileViewModel(student_temp stu)
        {
            StudentProfileViewModel vm = new StudentProfileViewModel
            {
                Row_ID = stu.Row_ID,
                ID = stu.ID.Trim(),
                FirstName = stu.FirstName.Trim(),
                MiddleName = stu.MiddleName ?? "",
                LastName = stu.LastName.Trim(),
                Suffix = stu.Suffix ?? "",
                MaidenName = stu.MaidenName ?? "",
                NickName = stu.NickName ?? "", // Just in case some random record has a null user_name 
                EmailUserName = stu.EmailUserName.Trim() ?? "", // Just in case some random record has a null email field
                OnOffCampus = stu.OnOffCampus ?? "",
                OnCampusBuilding = stu.OnCampusBuilding ?? "",
                OnCampusRoom = stu.OnCampusRoom ?? "",
                OnCampusPhone = stu.OnCampusPhone ?? "",
                OnCampusPrivatePhone = stu.OnCampusPrivatePhone ?? "",
                HomeStreet1 = stu.HomeStreet1 ?? "",
                HomeStreet2 = stu.HomeStreet2 ?? "",
                HomeCity = stu.HomeCity ?? "",
                HomeState = stu.HomeState ?? "",
                HomePostalCode = stu.HomePostalCode ?? "",
                HomeCountry = stu.HomeCountry ?? "",
                HomePhone = stu.HomePhone ?? "",
                Class = stu.Class ?? "",
                KeepPrivate = stu.KeepPrivate ?? "",
                Major = stu.Major ?? "",
                Major2 = stu.Major2 ?? "",
                Major3 = stu.Major3 ?? "",
                Email = stu.Email ?? "",
                Gender = stu.Gender ?? "",
                BirthDate = stu.BirthDate ?? "",
                MobilePhone = stu.MobilePhone ?? "",
                IsMobilePhonePrivate = stu.IsMobilePhonePrivate
            };

            return vm;
        }
    }
}