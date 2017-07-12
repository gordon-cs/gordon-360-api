using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class PublicStudentProfileViewModel
    {
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
        public string OffCampusStreet1 { get; set; }
        public string OffCampusStreet2 { get; set; }
        public string OffCampusCity { get; set; }
        public string OffCampusState { get; set; }
        public string OffCampusPostalCode { get; set; }
        public string OffCampusCountry { get; set; }
        public string OffCampusPhone { get; set; }
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
        public string Email { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string MobilePhone { get; set; }
        public bool IsMobilePhonePrivate { get; set; }


        public static implicit operator PublicStudentProfileViewModel(StudentProfileViewModel stu)
        {
            PublicStudentProfileViewModel vm = new PublicStudentProfileViewModel
            {
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
                OffCampusStreet1 = stu.OffCampusStreet1 ?? "",
                OffCampusStreet2 = stu.OffCampusStreet2 ?? "",
                OffCampusCity = stu.OffCampusCity ?? "",
                OffCampusState = stu.OffCampusState ?? "",
                OffCampusCountry = stu.OffCampusCountry ?? "",
                OffCampusPostalCode = stu.OffCampusPostalCode ?? "",
                OffCampusPhone = stu.OffCampusPhone ?? "",
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
                IsMobilePhonePrivate = stu.IsMobilePhonePrivate ?? false,
                MobilePhone = stu.MobilePhone ?? ""

            };
            if (vm.IsMobilePhonePrivate)
            {
                vm.MobilePhone = "Private as requested.";
            }
            if (vm.KeepPrivate.Contains("S"))
            {
                vm.OnOffCampus = "Private as requested.";
                vm.OnCampusBuilding = "Private as requested.";
                vm.OnCampusRoom = "Private as requested.";
                vm.OnCampusPhone = "Private as requested.";
                vm.OnCampusPrivatePhone = "Private as requested.";
                vm.OffCampusStreet1 = "Private as requested.";
                vm.OffCampusStreet2 = "Private as requested.";
                vm.OffCampusCity = "Private as requested.";
                vm.OffCampusState = "Private as requested.";
                vm.OffCampusCountry = "Private as requested.";
                vm.OffCampusPostalCode = "Private as requested.";
                vm.OffCampusPhone = "Private as requested.";
                vm.HomeStreet1 = "Private as requested.";
                vm.HomeStreet2 = "Private as requested.";
                vm.HomeCity = "Private as requested.";
                vm.HomeState = "Private as requested.";
                vm.HomePostalCode = "Private as requested.";
                vm.HomeCountry = "Private as requested.";
                vm.HomePhone = "Private as requested.";
            }
            return vm;
        }
    }
}