﻿//using Gordon360.Models.ViewModels.RecIM;
using System;
using Gordon360.Static.Methods;

namespace Gordon360.Models.ViewModels;

public class CombinedProfileViewModel
{
    const bool PRIVATE = true;
    const bool PUBLIC = false;

    public string ID { get; set; }
    public string Title { get; set; }
    public ProfileItem FirstName { get; set; }
    public ProfileItem MiddleName { get; set; }
    public ProfileItem LastName { get; set; }
    public ProfileItem Suffix { get; set; }
    public ProfileItem MaidenName { get; set; }
    public ProfileItem NickName { get; set; }
    public string Email { get; set; }
    public string Gender { get; set; }
    public ProfileItem HomeStreet1 { get; set; }
    public ProfileItem HomeStreet2 { get; set; }
    public ProfileItem HomeCity { get; set; }
    public ProfileItem HomeState { get; set; }
    public ProfileItem HomePostalCode { get; set; }
    public ProfileItem HomeCountry { get; set; }
    public ProfileItem HomePhone { get; set; }
    public string AD_Username { get; set; } // Leave as string
    public Nullable<int> show_pic { get; set; }
    public Nullable<int> preferred_photo { get; set; }
    public ProfileItem Country { get; set; }
    public string Barcode { get; set; }
    public string Facebook { get; set; }
    public string Twitter { get; set; }
    public string Instagram { get; set; }
    public string LinkedIn { get; set; }
    public string Handshake { get; set; }
    public string Calendar { get; set; }

    // Student Only
    public string OnOffCampus { get; set; }
    public string OffCampusStreet1 { get; set; }
    public string OffCampusStreet2 { get; set; }
    public string OffCampusCity { get; set; }
    public string OffCampusState { get; set; }
    public string OffCampusPostalCode { get; set; }
    public string OffCampusCountry { get; set; }
    public string OffCampusPhone { get; set; }
    public string OffCampusFax { get; set; }
    public string Major3 { get; set; }
    public string Major3Description { get; set; }
    public string Minor1 { get; set; }
    public string Minor1Description { get; set; }
    public string Minor2 { get; set; }
    public string Minor2Description { get; set; }
    public string Minor3 { get; set; }
    public string Minor3Description { get; set; }
    public string GradDate { get; set; }
    public string PlannedGradYear { get; set; }
    public DateTime? Entrance_Date { get; set; }
    public ProfileItem MobilePhone { get; set; }
    public bool IsMobilePhonePrivate { get; set; }
    public Nullable<int> ChapelRequired { get; set; }
    public Nullable<int> ChapelAttended { get; set; }
    public string Cohort { get; set; }
    public string Class { get; set; }
    public string AdvisorIDs { get; set; }
    public string Married { get; set; }
    public string Commuter { get; set; }

    // Alumni Only
    public string WebUpdate { get; set; }
    public ProfileItem HomeEmail { get; set; }
    public string MaritalStatus { get; set; }
    public string College { get; set; }
    public string ClassYear { get; set; }
    public string PreferredClassYear { get; set; }
    public string ShareName { get; set; }
    public string ShareAddress { get; set; }

    // Student And Alumni Only
    public string Major { get; set; }
    public string Major1Description { get; set; }
    public string Major2 { get; set; }
    public string Major2Description { get; set; }
    public string grad_student { get; set; }

    // FacStaff Only
    public string OnCampusDepartment { get; set; }
    public string Type { get; set; }
    public string office_hours { get; set; }
    public string Dept { get; set; }
    public string Mail_Description { get; set; }
    public DateTime? FirstHireDt { get; set; }

    // FacStaff and Alumni Only
    public string JobTitle { get; set; }
    public ProfileItem SpouseName { get; set; }

    // FacStaff and Student Only
    public string BuildingDescription { get; set; }
    public string Mail_Location { get; set; }
    public string OnCampusBuilding { get; set; }
    public string OnCampusRoom { get; set; }
    public string OnCampusPhone { get; set; }
    public string OnCampusPrivatePhone { get; set; }
    public string OnCampusFax { get; set; }
    public string KeepPrivate { get; set; }

    // ProfileViewModel Only
    public string PersonType { get; set; } // Leave as string

    public static implicit operator CombinedProfileViewModel(ProfileViewModel vm)
    {
        CombinedProfileViewModel new_vm = new CombinedProfileViewModel
        {
            // All Profiles
            ID = vm.ID,
            Title = vm.Title,
            FirstName = vm.FirstName is null || vm.FirstName == "" ? null : new ProfileItem(vm.FirstName, PUBLIC),
            MiddleName = vm.MiddleName is null || vm.MiddleName == "" ? null : new ProfileItem(vm.MiddleName, PUBLIC),
            LastName = vm.LastName is null || vm.LastName == "" ? null : new ProfileItem(vm.LastName, PUBLIC),
            Suffix = vm.Suffix is null || vm.Suffix == "" ? null : new ProfileItem(vm.Suffix, PUBLIC),
            MaidenName = vm.MaidenName is null || vm.MaidenName == "" ? null : new ProfileItem(vm.MaidenName, PUBLIC),
            NickName = vm.NickName is null || vm.NickName == "" ? null : new ProfileItem(vm.NickName, PUBLIC),
            Email = vm.Email,
            Gender = vm.Gender,
            HomeStreet1 = vm.HomeStreet1 is null || vm.HomeStreet1 == "" ? null : new ProfileItem(vm.HomeStreet1, PUBLIC),
            HomeStreet2 = vm.HomeStreet2 is null || vm.HomeStreet2 == "" ? null : new ProfileItem(vm.HomeStreet2, PUBLIC),
            HomeCity =  vm.HomeCity is null || vm.HomeCity == "" ? null : new ProfileItem(vm.HomeCity, PUBLIC),
            HomeState = vm.HomeState is null || vm.HomeState == "" ? null : new ProfileItem(vm.HomeState, PUBLIC),
            HomePostalCode = vm.HomePostalCode is null || vm.HomePostalCode == "" ? null : new ProfileItem(vm.HomePostalCode, PUBLIC),
            HomeCountry = vm.HomeCountry is null || vm.HomeCountry == "" ? null : new ProfileItem(vm.HomeCountry, PUBLIC),
            HomePhone = vm.HomePhone is null || vm.HomePhone == "" ? null : new ProfileItem(vm.HomePhone, PUBLIC),
            AD_Username = vm.AD_Username,
            show_pic = vm.show_pic,
            preferred_photo = vm.preferred_photo,
            Country = vm.Country is null || vm.Country == "" ? null : new ProfileItem(vm.Country, PUBLIC),
            Barcode = vm.Barcode,
            Facebook = vm.Facebook,
            Twitter = vm.Twitter,
            Instagram = vm.Instagram,
            LinkedIn = vm.LinkedIn,
            Handshake = vm.Handshake,
            Calendar = vm.Calendar,

            // Student Only
            OnOffCampus = vm.OnOffCampus,
            OffCampusStreet1 = vm.OffCampusStreet1,
            OffCampusStreet2 = vm.OffCampusStreet2,
            OffCampusCity = vm.OffCampusCity,
            OffCampusState = vm.OffCampusState,
            OffCampusPostalCode = vm.OffCampusPostalCode,
            OffCampusCountry = vm.OffCampusCountry,
            OffCampusPhone = vm.OffCampusPhone,
            OffCampusFax = vm.OffCampusFax,
            Major3 = vm.Major3,
            Major3Description = vm.Major3Description,
            Minor1 = vm.Minor1,
            Minor1Description = vm.Minor1Description,
            Minor2 = vm.Minor2,
            Minor2Description = vm.Minor2Description,
            Minor3 = vm.Minor3,
            Minor3Description = vm.Minor3Description,
            GradDate = vm.GradDate,
            PlannedGradYear = vm.PlannedGradYear,
            Entrance_Date = vm.Entrance_Date,
            MobilePhone = vm.MobilePhone is null || vm.MobilePhone == "" ? null : new ProfileItem(vm.MobilePhone, PUBLIC),
            IsMobilePhonePrivate = vm.IsMobilePhonePrivate,
            ChapelRequired = vm.ChapelRequired,
            ChapelAttended = vm.ChapelAttended,
            Cohort = vm.Cohort,
            Class = vm.Class,
            AdvisorIDs = vm.AdvisorIDs,
            Married = vm.Married,
            Commuter = vm.Commuter,

            // Alumni Only
            WebUpdate = vm. WebUpdate,
            HomeEmail = vm.HomeEmail is null || vm.HomeEmail == "" ? null : new ProfileItem(vm.HomeEmail, PUBLIC),
            MaritalStatus = vm.MaritalStatus,
            College = vm.College,
            ClassYear = vm.ClassYear,
            PreferredClassYear = vm. PreferredClassYear,
            ShareName = vm.ShareName,
            ShareAddress = vm. ShareAddress,

            // Student And Alumni Only
            Major = vm.Major,
            Major1Description = vm.Major1Description,
            Major2 = vm.Major2,
            Major2Description = vm.Major2Description,
            grad_student = vm.grad_student,

            // FacStaff Only
            OnCampusDepartment = vm. OnCampusDepartment,
            Type = vm. Type,
            office_hours = vm. office_hours,
            Dept = vm.Dept,
            Mail_Description = vm.Mail_Description,
            FirstHireDt = vm.FirstHireDt,

            // FacStaff and Alumni Only
            JobTitle = vm.JobTitle,
            SpouseName = vm.SpouseName is null || vm.SpouseName == "" ? null : new ProfileItem(vm.SpouseName, PUBLIC),

            // FacStaff and Student Only
            BuildingDescription = vm.BuildingDescription,
            Mail_Location = vm.Mail_Location,
            OnCampusBuilding = vm.OnCampusBuilding,
            OnCampusRoom = vm.OnCampusRoom,
            OnCampusPhone = vm.OnCampusPhone,
            OnCampusPrivatePhone = vm.OnCampusPrivatePhone,
            OnCampusFax = vm.OnCampusFax,
            KeepPrivate = vm.KeepPrivate,

            // ProfileViewModel Only
            PersonType = vm.PersonType
        };
        return new_vm;
    }
}
