using System;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Models.ViewModels
{
    public class StudentProfileViewModel
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string MaidenName { get; set; }
        public string NickName { get; set; }
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
        public string Major2 { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string grad_student { get; set; }
        public string GradDate { get; set; }
        public string Major3 { get; set; }
        public string Minor1 { get; set; }
        public string Minor2 { get; set; }
        public string Minor3 { get; set; }
        public string MobilePhone { get; set; }
        public int IsMobilePhonePrivate { get; set; }
        public string AD_Username { get; set; }
        public int? show_pic { get; set; }
        public int? preferred_photo { get; set; }
        public string Country { get; set; }
        public string BuildingDescription { get; set; }
        public string Major1Description { get; set; }
        public string Major2Description { get; set; }
        public string Major3Description { get; set; }
        public string Minor1Description { get; set; }
        public string Minor2Description { get; set; }
        public string Minor3Description { get; set; }
        public string Mail_Location { get; set; }
        public int? ChapelRequired { get; set; }
        public int? ChapelAttended { get; set; }


        public static implicit operator StudentProfileViewModel(Student stu)
        {
            StudentProfileViewModel vm = new StudentProfileViewModel
            {
                Title = stu.Title ?? "",
                ID = stu.ID.Trim(),
                FirstName = stu.FirstName ?? "",
                MiddleName = stu.MiddleName ?? "",
                LastName = stu.LastName ?? "",
                Suffix = stu.Suffix ?? "",
                MaidenName = stu.MaidenName ?? "",
                NickName = stu.NickName ?? "", // Just in case some random record has a null user_name 
                AD_Username = stu.AD_Username ?? "", // Just in case some random record has a null email field
                Cohort = stu.Cohort ?? "",
                Class = stu.Class ?? "",
                Commuter = stu.Commuter ?? "",
                grad_student = stu.grad_student ?? "",
                GradDate = stu.GradDate ?? "",
                OnOffCampus = stu.OnOffCampus ?? "",
                OnCampusBuilding = stu.OnCampusBuilding ?? "",
                OnCampusRoom = stu.OnCampusRoom ?? "",
                OnCampusPhone = stu.OnCampusPhone ?? "",
                OnCampusPrivatePhone = stu.OnCampusPrivatePhone ?? "",
                OnCampusFax = stu.OnCampusFax ?? "",
                OffCampusStreet1 = stu.OffCampusStreet1 ?? "",
                OffCampusStreet2 = stu.OffCampusStreet2 ?? "",
                OffCampusCity = stu.OffCampusCity ?? "",
                OffCampusState = stu.OffCampusState ?? "",
                OffCampusCountry = stu.OffCampusCountry ?? "",
                OffCampusPostalCode = stu.OffCampusPostalCode ?? "",
                OffCampusPhone = stu.OffCampusPhone ?? "",
                OffCampusFax = stu.OffCampusFax ?? "",
                HomeStreet1 = stu.HomeStreet1 ?? "",
                HomeStreet2 = stu.HomeStreet2 ?? "",
                HomeCity = stu.HomeCity ?? "",
                HomeState = stu.HomeState ?? "",
                HomePostalCode = stu.HomePostalCode ?? "",
                HomeCountry = stu.HomeCountry ?? "",
                HomePhone = stu.HomePhone ?? "",
                HomeFax = stu.HomeFax ?? "",
                Barcode = stu.Barcode ?? "",
                AdvisorIDs = stu.AdvisorIDs ?? "",
                Married = stu.Married ?? "",
                KeepPrivate = stu.KeepPrivate ?? "",
                Major = stu.Major ?? "",
                Major2 = stu.Major2 ?? "",
                Major3 = stu.Major3 ?? "",
                Minor1 = stu.Minor1 ?? "",
                Minor2 = stu.Minor2 ?? "",
                Minor3 = stu.Minor3 ?? "",
                Email = stu.Email ?? "",
                Gender = stu.Gender ?? "",
                MobilePhone = stu.MobilePhone ?? "",
                IsMobilePhonePrivate = stu.IsMobilePhonePrivate,
                show_pic = stu.show_pic,
                preferred_photo = stu.preferred_photo,
                Country = stu.Country ?? "",
                BuildingDescription = stu.BuildingDescription ?? "",
                Major1Description = stu.Major1Description ?? "",
                Major2Description = stu.Major2Description ?? "",
                Major3Description = stu.Major3Description ?? "",
                Minor1Description = stu.Minor1Description ?? "",
                Minor2Description = stu.Minor2Description ?? "",
                Minor3Description = stu.Minor3Description ?? "",
                Mail_Location = stu.Mail_Location ?? "",
                ChapelRequired = stu.ChapelRequired ?? 0,
                ChapelAttended = stu.ChapelAttended ?? 0
            };

            return vm;
        }
    }
}