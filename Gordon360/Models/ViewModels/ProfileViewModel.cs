using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class ProfileViewModel
    {
        public int Row_ID { get; set; }
        public string ID { get; set; }
        public string SSN { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string MaidenName { get; set; }
        public string NickName { get; set; }
        public string EmailUserName { get; set; }
        public string EmailPassword { get; set; }
        public string EmailServer { get; set; }
        public string EmailProtocol { get; set; }
        public string OnOffCampus { get; set; }
        public string OnCampusBuilding { get; set; }
        public string OnCampusRoom { get; set; }
        public string OnCampusPhone { get; set; }
        public string OnCampusPrivatePhone { get; set; }
        public string OnCampusFax { get; set; }
        public string OffCampusStreet1 { get; set; }
        public string OffCampusStreet2 { get; set; }
        public string OffCampusCity { get; set; }
        public string OffCampusState { get; set; }
        public string OffCampusPostalCode { get; set; }
        public string OffCampusCountry { get; set; }
        public string OffCampusPhone { get; set; }
        public string OffCampusFax { get; set; }
        public string HomeStreet1 { get; set; }
        public string HomeStreet2 { get; set; }
        public string HomeCity { get; set; }
        public string HomeState { get; set; }
        public string HomePostalCode { get; set; }
        public string HomeCountry { get; set; }
        public string HomePhone { get; set; }
        public string HomeFax { get; set; }
        public string Cohort { get; set; }
        public string Class { get; set; }
        public string KeepPrivate { get; set; }
        public string Major { get; set; }
        public string Barcode { get; set; }
        public string AdvisorIDs { get; set; }
        public string Married { get; set; }
        public string Commuter { get; set; }
        public string StatusType { get; set; }
        public string CurrentStatusType { get; set; }
        public string NextStatusType { get; set; }
        public Nullable<byte> CreditHrs { get; set; }
        public string GradFlag { get; set; }
        public string Major2 { get; set; }
        public string Email { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string finance_approved { get; set; }
        public string reg_complete { get; set; }
        public string grad_student { get; set; }
        public string GradDate { get; set; }
        public string AJScholar { get; set; }
        public string Major3 { get; set; }
        public string Minor1 { get; set; }
        public string Minor2 { get; set; }
        public string Minor3 { get; set; }
        public string Class_List { get; set; }
        public string GC_Insurance { get; set; }
        public string MobilePhone { get; set; }
        public Nullable<bool> IsMobilePhonePrivate { get; set; }


        public static implicit operator ProfileViewModel(student_temp stu)
        {
            ProfileViewModel vm = new ProfileViewModel
            {
                Row_ID = stu.Row_ID,
                ID = stu.ID.Trim(),
                FirstName = stu.FirstName.Trim(),
                LastName = stu.LastName.Trim(), 
                NickName = stu.NickName.Trim() ?? "", // Just in case some random record has a null user_name 
                EmailUserName = stu.EmailUserName.Trim() ?? "", // Just in case some random record has a null email field
                OnOffCampus = stu.OnOffCampus.Trim() ?? "",
                HomeStreet1 = stu.HomeStreet1 ?? "",
                HomeStreet2 = stu.HomeStreet2 ?? "",
                HomeCity = stu.HomeCity.Trim() ?? "",
                HomeState = stu.HomeState.Trim() ?? "",
                HomePostalCode = stu.HomePostalCode.Trim() ?? "",
                HomeCountry = stu.HomeCountry.Trim() ?? "",
                HomePhone = stu.HomePhone ?? "",
                Class = stu.Class.Trim() ?? "",
                KeepPrivate = stu.KeepPrivate.Trim() ?? "",
                Major = stu.Major.Trim() ?? "",
                Email = stu.Email ?? "",
                Gender = stu.Gender.Trim() ?? "",
                MobilePhone = stu.MobilePhone ?? "",
                IsMobilePhonePrivate = stu.IsMobilePhonePrivate
            };

            return vm;
        }
    }
}