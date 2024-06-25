using System;

namespace Gordon360.Models.ViewModels;

public class CombinedProfileViewModel
{
    public string ID { get; set; }
    public string Title { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string Suffix { get; set; }
    public string MaidenName { get; set; }
    public string NickName { get; set; }
    public string Email { get; set; }
    public string Gender { get; set; }
    public string HomeStreet1 { get; set; }
    public string HomeStreet2 { get; set; }
    public string HomeCity { get; set; }
    public string HomeState { get; set; }
    public string HomePostalCode { get; set; }
    public string HomeCountry { get; set; }
    public string HomePhone { get; set; }
    public string HomeFax { get; set; }
    public string AD_Username { get; set; }
    public Nullable<int> show_pic { get; set; }
    public Nullable<int> preferred_photo { get; set; }
    public string Country { get; set; }
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
    public string MobilePhone { get; set; }
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
    public string SpouseName { get; set; }

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
            ID = vm.ID ?? null,
            Title = vm.Title ?? null,
            FirstName = vm.FirstName ?? null,
            MiddleName = vm.MiddleName ?? null,
            LastName = vm.LastName ?? null,
            Suffix = vm.Suffix ?? null,
            MaidenName = vm.MaidenName ?? null,
            NickName = vm.NickName ?? null,
            Email = vm.Email ?? null,
            Gender = vm.Gender ?? null,
            HomeStreet1 = vm.HomeStreet1 ?? null,
            HomeStreet2 = vm.HomeStreet2 ?? null,
            HomeCity = vm.HomeCity ?? null,
            HomeState = vm.HomeState ?? null,
            HomePostalCode = vm.HomePostalCode ?? null,
            HomeCountry = vm.HomeCountry ?? null,
            HomePhone = vm.HomePhone ?? null,
            HomeFax = vm.HomeFax ?? null,
            AD_Username = vm.AD_Username ?? null,
            show_pic = vm.show_pic,
            preferred_photo = vm.preferred_photo,
            Country = vm.Country ?? null,
            Barcode = vm.Barcode ?? null,
            Facebook = vm.Facebook ?? null,
            Twitter = vm.Twitter ?? null,
            Instagram = vm.Instagram ?? null,
            LinkedIn = vm.LinkedIn ?? null,
            Handshake = vm.Handshake ?? null,
            Calendar = vm.Calendar ?? null,

            // Student Only
            OnOffCampus = vm.OnOffCampus ?? null,
            OffCampusStreet1 = vm.OffCampusStreet1 ?? null,
            OffCampusStreet2 = vm.OffCampusStreet2 ?? null,
            OffCampusCity = vm.OffCampusCity ?? null,
            OffCampusState = vm.OffCampusState ?? null,
            OffCampusPostalCode = vm.OffCampusPostalCode ?? null,
            OffCampusCountry = vm.OffCampusCountry ?? null,
            OffCampusPhone = vm.OffCampusPhone ?? null,
            OffCampusFax = vm.OffCampusFax ?? null,
            Major3 = vm.Major3 ?? null,
            Major3Description = vm.Major3Description ?? null,
            Minor1 = vm.Minor1 ?? null,
            Minor1Description = vm.Minor1Description ?? null,
            Minor2 = vm.Minor2 ?? null,
            Minor2Description = vm.Minor2Description ?? null,
            Minor3 = vm.Minor3 ?? null,
            Minor3Description = vm.Minor3Description ?? null,
            GradDate = vm.GradDate ?? null,
            PlannedGradYear = vm.PlannedGradYear ?? null,
            MobilePhone = vm.MobilePhone ?? null,
            IsMobilePhonePrivate = vm.IsMobilePhonePrivate,
            ChapelRequired = vm.ChapelRequired,
            ChapelAttended = vm.ChapelAttended,
            Cohort = vm.Cohort ?? null,
            Class = vm.Class ?? null,
            AdvisorIDs = vm.AdvisorIDs ?? null,
            Married = vm.Married ?? null,
            Commuter = vm.Commuter ?? null,

            // Alumni Only
            WebUpdate = vm. WebUpdate ?? null,
            HomeEmail = vm.HomeEmail ?? null,
            MaritalStatus = vm.MaritalStatus ?? null,
            College = vm.College ?? null,
            ClassYear = vm.ClassYear ?? null,
            PreferredClassYear = vm. PreferredClassYear ?? null,
            ShareName = vm.ShareName ?? null,
            ShareAddress = vm. ShareAddress ?? null,

            // Student And Alumni Only
            Major = vm.Major ?? null,
            Major1Description = vm.Major1Description ?? null,
            Major2 = vm.Major2 ?? null,
            Major2Description = vm.Major2Description ?? null,
            grad_student = vm.grad_student ?? null,

            // FacStaff Only
            OnCampusDepartment = vm. OnCampusDepartment ?? null,
            Type = vm. Type ?? null,
            office_hours = vm. office_hours ?? null,
            Dept = vm.Dept ?? null,
            Mail_Description = vm.Mail_Description ?? null,

            // FacStaff and Alumni Only
            JobTitle = vm.JobTitle ?? null,
            SpouseName = vm.SpouseName ?? null,

            // FacStaff and Student Only
            BuildingDescription = vm.BuildingDescription ?? null,
            Mail_Location = vm.Mail_Location ?? null,
            OnCampusBuilding = vm.OnCampusBuilding ?? null,
            OnCampusRoom = vm.OnCampusRoom ?? null,
            OnCampusPhone = vm.OnCampusPhone ?? null,
            OnCampusPrivatePhone = vm.OnCampusPrivatePhone ?? null,
            OnCampusFax = vm.OnCampusFax ?? null,
            KeepPrivate = vm.KeepPrivate ?? null,

            // ProfileViewModel Only
            PersonType = vm.PersonType ?? null
        };
        return new_vm;
    }
}
