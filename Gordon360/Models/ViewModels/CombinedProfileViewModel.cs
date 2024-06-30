//using Gordon360.Models.ViewModels.RecIM;
using System;
using Gordon360.Static.Methods;

namespace Gordon360.Models.ViewModels;

public class CombinedProfileViewModel
{
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
    public string HomeStreet1 { get; set; }
    public string HomeStreet2 { get; set; }
    public ProfileItem HomeCity { get; set; }
    public ProfileItem HomeState { get; set; }
    public string HomePostalCode { get; set; }
    public ProfileItem HomeCountry { get; set; }
    public ProfileItem HomePhone { get; set; }
    public string HomeFax { get; set; }
    public string AD_Username { get; set; }
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
    public string HomeEmail { get; set; }
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
    public string PersonType { get; set; }

    public static implicit operator CombinedProfileViewModel(ProfileViewModel vm)
    {
        CombinedProfileViewModel new_vm = new CombinedProfileViewModel
        {
            // All Profiles
            ID = vm.ID,
            Title = vm.Title,
            //FirstName = vm.FirstName,
            FirstName = vm.FirstName is null || vm.FirstName == "" ? null : new ProfileItem(vm.FirstName, false),
            MiddleName = vm.MiddleName is null || vm.MiddleName == "" ? null : new ProfileItem(vm.MiddleName, false),
            LastName = vm.LastName is null || vm.LastName == "" ? null : new ProfileItem(vm.LastName, false),
            Suffix = vm.Suffix is null || vm.Suffix == "" ? null : new ProfileItem(vm.Suffix, false),
            MaidenName = vm.MaidenName is null || vm.MaidenName == "" ? null : new ProfileItem(vm.MaidenName, false),
            NickName = vm.NickName is null || vm.NickName == "" ? null : new ProfileItem(vm.NickName, false),
            Email = vm.Email,
            Gender = vm.Gender,
            HomeStreet1 = vm.HomeStreet1,
            HomeStreet2 = vm.HomeStreet2,
            HomeCity =  vm.HomeCity is null || vm.HomeCity == "" ? null : new ProfileItem(vm.HomeCity, false),
            HomeState = vm.HomeState is null || vm.HomeState == "" ? null : new ProfileItem(vm.HomeState, false),
            HomePostalCode = vm.HomePostalCode,
            HomeCountry = vm.HomeCountry is null || vm.HomeCountry == "" ? null : new ProfileItem(vm.HomeCountry, false),
            HomePhone = vm.HomePhone is null || vm.HomePhone == "" ? null : new ProfileItem(vm.HomePhone, false),
            HomeFax = vm.HomeFax,
            AD_Username = vm.AD_Username,
            show_pic = vm.show_pic,
            preferred_photo = vm.preferred_photo,
            Country = vm.Country is null || vm.Country == "" ? null : new ProfileItem(vm.Country, false),
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
            MobilePhone = vm.MobilePhone is null || vm.MobilePhone == "" ? null : new ProfileItem(vm.MobilePhone, false),
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
            HomeEmail = vm.HomeEmail,
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

            // FacStaff and Alumni Only
            JobTitle = vm.JobTitle,
            SpouseName = vm.SpouseName is null || vm.SpouseName == "" ? null : new ProfileItem(vm.SpouseName, false),

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
