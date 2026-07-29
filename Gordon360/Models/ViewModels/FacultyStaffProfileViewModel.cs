using Gordon360.Models.CCT;
using Gordon360.Models.Salesforce;
using System;
using System.Linq;

namespace Gordon360.Models.ViewModels;

public record FacultyStaffProfileViewModel
    (
    string ID,
    string Title,
    string FirstName,
    string MiddleName,
    string LastName,
    string Suffix,
    string MaidenName,
    string NickName,
    string OnCampusDepartment,
    string OnCampusBuilding,
    string OnCampusRoom,
    string OnCampusPhone,
    string OnCampusPrivatePhone,
    string OnCampusFax,
    string HomeStreet1,
    string HomeStreet2,
    string HomeCity,
    string HomeState,
    string HomePostalCode,
    string HomeCountry,
    string HomePhone,
    string MobilePhone,
    string KeepPrivate,
    string JobTitle,
    string Dept,
    string SpouseName,
    string Barcode,
    string Gender,
    string Email,
    string Type,
    DateTime? FirstHireDt,
    string AD_Username,
    string office_hours,
    int? preferred_photo,
    int? show_pic,
    string BuildingDescription,
    string Country,
    string Mail_Location,
    string Mail_Description)
{
    public static implicit operator FacultyStaffProfileViewModel?(FacStaff? fac)
    {
        if (fac == null)
        {
            return null;
        }

        return new FacultyStaffProfileViewModel(
            ID: fac.ID.Trim(),
            Title: fac.Title ?? "",
            FirstName: fac.FirstName ?? "",
            MiddleName: fac.MiddleName ?? "",
            LastName: fac.LastName ?? "",
            Suffix: fac.Suffix ?? "",
            MaidenName: fac.MaidenName ?? "",
            NickName: fac.Nickname ?? "",
            OnCampusDepartment: fac.OnCampusDepartment ?? "",
            OnCampusBuilding: fac.OnCampusBuilding ?? "",
            OnCampusRoom: fac.OnCampusRoom ?? "",
            OnCampusPhone: fac.OnCampusPhone ?? "",
            OnCampusPrivatePhone: fac.OnCampusPrivatePhone ?? "",
            OnCampusFax: fac.OnCampusFax ?? "",
            HomeStreet1: fac.HomeStreet1 ?? "",
            HomeStreet2: fac.HomeStreet2 ?? "",
            HomeCity: fac.HomeCity ?? "",
            HomeState: fac.HomeState ?? "",
            HomePostalCode: fac.HomePostalCode ?? "",
            HomeCountry: fac.HomeCountry ?? "",
            HomePhone: fac.HomePhone ?? "",
            MobilePhone: fac.MobilePhone ?? "",
            KeepPrivate: fac.KeepPrivate ?? "",
            JobTitle: fac.JobTitle ?? "",
            Dept: fac.Dept ?? "",
            SpouseName: fac.SpouseName ?? "",
            Barcode: fac.Barcode ?? "",
            Gender: fac.Gender ?? "",
            Email: fac.Email ?? "",
            Type: fac.Type ?? "",
            FirstHireDt: fac.FirstHireDt,
            AD_Username: fac.AD_Username ?? "",
            office_hours: fac.office_hours ?? "",
            preferred_photo: fac.preferred_photo,
            show_pic: fac.show_pic,
            BuildingDescription: fac.BuildingDescription ?? "",
            Country: fac.Country ?? "",
            Mail_Location: fac.Mail_Location ?? "",
            Mail_Description: fac.Mail_Description ?? ""
        );
    }

    public static explicit operator FacultyStaffProfileViewModel(Account account)
    {
        var contact = account.Contacts?.records?.FirstOrDefault() ?? new Contact();

        var homeAddress = account.ContactPointAddresses?.records?.FirstOrDefault(c => c.AddressType == "Home")
                            ?? new ContactPointAddress();

        var onCampusAddress = account.ContactPointAddresses?.records?.FirstOrDefault(c => c.AddressType == "On-Campus")
                                ?? new ContactPointAddress();

        var firstEmployment = account.PersonEmployments?.records?.FirstOrDefault() ?? new PersonEmployment();

        return new FacultyStaffProfileViewModel(
            ID: account.Student_Id__pc ?? "",
            Title: account.PersonTitle ?? "",
            FirstName: account.FirstName ?? "",
            MiddleName: account.MiddleName ?? "",
            LastName: account.LastName ?? "",
            Suffix: account.Suffix__pc ?? "",
            MaidenName: account.FormerLastName__pc ?? "",
            NickName: account.Preferred_First_Name_Formula__pc ?? "",
            OnCampusDepartment: "", // TODO: Implement OnCampusDepartment
            OnCampusBuilding: onCampusAddress?.gc_On_Campus_Location__r?.ParentLocation?.Name ?? "",
            OnCampusRoom: onCampusAddress?.gc_On_Campus_Location__r?.gc_Jenz_Room_Code__c ?? "",
            OnCampusPhone: onCampusAddress?.gc_On_Campus_Location__r?.Phone ?? "",
            OnCampusPrivatePhone: "", // TODO: Implement private phone
            OnCampusFax: "", // TODO: Implement OnCampusFax
            HomeStreet1: "", // TODO: It seems like, for a long time, street1 has represented street2 (in the database, frontend and here). We should fix that.
            HomeStreet2: homeAddress.Street ?? "",
            HomeCity: homeAddress.City ?? "",
            HomeState: homeAddress.StateCode ?? "",
            HomePostalCode: homeAddress.PostalCode ?? "",
            HomeCountry: homeAddress.CountryCode ?? "",
            HomePhone: homeAddress.PhoneNumber,
            MobilePhone: "", // TODO: implement fax
            KeepPrivate: "", // TODO: Implement private faculty profile
            JobTitle: contact.gc_Current_Positions__c ?? "",
            Dept: "", // TODO: implement department
            SpouseName: "", // TODO: Implement spouse name
            Barcode: "", // TODO: Implement barcode
            Gender: account.PersonGenderIdentity ?? "",
            Email: account.PersonEmail ?? "",
            Type: "", // TODO: Implement Type
            FirstHireDt: firstEmployment?.StartDate ?? new DateTime(1900, 1, 1),
            AD_Username: account.AD_Username__pc ?? "",
            office_hours: "test test fac staff",
            preferred_photo: 2, // show_pic
            show_pic: 1, // preferred_photo
            BuildingDescription: onCampusAddress?.gc_On_Campus_Location__r?.ParentLocation?.Name ?? "",
            Country: "", // TODO: Implement country
            Mail_Location: "", // TODO: Implement mail_location
            Mail_Description: "" // TODO: Implement mail_description
        );
    }
}