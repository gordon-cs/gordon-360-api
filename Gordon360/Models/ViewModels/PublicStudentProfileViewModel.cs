﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gordon360.Models.ViewModels
{
    public class PublicStudentProfileViewModel
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string MaidenName { get; set; }
        public string NickName { get; set; }
        public string OnOffCampus { get; set; }
        public string HomeCity { get; set; }
        public string HomeState { get; set; }
        public string HomeCountry { get; set; }
        public string Cohort { get; set; }
        public string Class { get; set; }
        public string KeepPrivate { get; set; }
        public string Major { get; set; }
        public string Major2 { get; set; }
        public string Major3 { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string grad_student { get; set; }
        public string MobilePhone { get; set; }
        public string AD_Username { get; set; }
        public int IsMobilePhonePrivate { get; set; }
        public Nullable<int> show_pic { get; set; }
        public Nullable<int> preferred_photo { get; set; }
        public string Country { get; set; }
        public string Major1Description { get; set; }
        public string Major2Description { get; set; }
        public string Major3Description { get; set; }
        public string Minor1Description { get; set; }
        public string Minor2Description { get; set; }
        public string Minor3Description { get; set; }


        public static implicit operator PublicStudentProfileViewModel(StudentProfileViewModel stu)
        {
            PublicStudentProfileViewModel vm = new PublicStudentProfileViewModel
            {
                Title = stu.Title ?? "",
                FirstName = stu.FirstName,
                MiddleName = stu.MiddleName ?? "",
                LastName = stu.LastName,
                Suffix = stu.Suffix ?? "",
                MaidenName = stu.MaidenName ?? "",
                NickName = stu.NickName ?? "", // Just in case some random record has a null user_name 
                AD_Username = stu.AD_Username ?? "", // Just in case some random record has a null email field
                OnOffCampus = stu.OnOffCampus ?? "",
                HomeCity = stu.HomeCity ?? "",
                HomeState = stu.HomeState ?? "",
                HomeCountry = stu.HomeCountry ?? "",
                Class = stu.Class ?? "",
                Cohort = stu.Cohort ?? "",
                grad_student = stu.grad_student ?? "",
                KeepPrivate = stu.KeepPrivate ?? "",
                Major = stu.Major ?? "",
                Major2 = stu.Major2 ?? "",
                Major3 = stu.Major3 ?? "",
                Email = stu.Email ?? "",
                Gender = stu.Gender ?? "",
                IsMobilePhonePrivate = stu.IsMobilePhonePrivate,
                MobilePhone = stu.MobilePhone ?? "",
                show_pic = stu.show_pic,
                preferred_photo = stu.preferred_photo,
                Country = stu.Country ?? "",
                Major1Description = stu.Major1Description ?? "",
                Major2Description = stu.Major2Description ?? "",
                Major3Description = stu.Major3Description ?? "",
                Minor1Description = stu.Minor1Description ?? "",
                Minor2Description = stu.Minor2Description ?? "",
                Minor3Description = stu.Minor3Description ?? ""

            };
            if (vm.IsMobilePhonePrivate==1)
            {
                vm.MobilePhone = "Private as requested.";
            }
            if (vm.KeepPrivate.Contains("S"))
            {
                vm.OnOffCampus = "Private as requested.";
                vm.HomeCity = "Private as requested.";
                vm.HomeState = "Private as requested.";
                vm.HomeCountry = "Private as requested.";
                vm.Country = "Private as requested.";
            }
            if (vm.KeepPrivate.Contains("Y") || vm.KeepPrivate.Contains("P"))
            {
                return null;
            }
            return vm;
        }
    }
}