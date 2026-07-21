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
    string HomeFax,
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
            fac.ID.Trim(),
            fac.Title ?? "",
            fac.FirstName ?? "",
            fac.MiddleName ?? "",
            fac.LastName ?? "",
            fac.Suffix ?? "",
            fac.MaidenName ?? "",
            fac.Nickname ?? "",
            fac.OnCampusDepartment ?? "",
            fac.OnCampusBuilding ?? "",
            fac.OnCampusRoom ?? "",
            fac.OnCampusPhone ?? "",
            fac.OnCampusPrivatePhone ?? "",
            fac.OnCampusFax ?? "",
            fac.HomeStreet1 ?? "",
            fac.HomeStreet2 ?? "",
            fac.HomeCity ?? "",
            fac.HomeState ?? "",
            fac.HomePostalCode ?? "",
            fac.HomeCountry ?? "",
            fac.HomePhone ?? "",
            fac.HomeFax ?? "",
            fac.KeepPrivate ?? "",
            fac.JobTitle ?? "",
            fac.Dept ?? "",
            fac.SpouseName ?? "",
            fac.Barcode ?? "",
            fac.Gender ?? "",
            fac.Email ?? "",
            fac.Type ?? "",
            fac.FirstHireDt,
            fac.AD_Username ?? "",
            fac.office_hours ?? "",
            fac.preferred_photo,
            fac.show_pic,
            fac.BuildingDescription ?? "",
            fac.Country ?? "",
            fac.Mail_Location ?? "",
            fac.Mail_Description ?? ""
        );
    }

    public static explicit operator FacultyStaffProfileViewModel?(Account? account)
    {
        if (account is null)
        {
            return null;
        }

        var contact = account.Contacts?.records?.FirstOrDefault() ?? new Contact();

        var homeAddress = account.ContactPointAddresses?.records?.FirstOrDefault(c => c.AddressType == "Home")
                            ?? new ContactPointAddress();

        var onCampusAddress = account.ContactPointAddresses?.records?.FirstOrDefault(c => c.AddressType == "On-Campus")
                                ?? new ContactPointAddress();

        var firstEmployment = account.PersonEmployments?.records?.FirstOrDefault() ?? new PersonEmployment();

        return new FacultyStaffProfileViewModel(
            account.Student_Id__pc ?? "",
            account.PersonTitle ?? "",
            account.FirstName ?? "",
            account.MiddleName ?? "",
            account.LastName ?? "",
            account.Suffix__pc ?? "",
            account.FormerLastName__pc ?? "",
            account.Preferred_First_Name_Formula__pc ?? "",
            "", // TODO: Implement OnCampusDepartment
            onCampusAddress?.gc_On_Campus_Location__r?.ParentLocation?.Name ?? "",
            onCampusAddress?.gc_On_Campus_Location__r?.gc_Jenz_Room_Code__c ?? "",
            onCampusAddress?.gc_On_Campus_Location__r?.Phone ?? "",
            "", // TODO: Implement private phone
            "", // TODO: Implement OnCampusFax
            "", // TODO: It seems like, for a long time, street1 has represented street2 (in the database, frontend and here). We should fix that.
            homeAddress.Street ?? "",
            homeAddress.City ?? "",
            homeAddress.StateCode ?? "",
            homeAddress.PostalCode ?? "",
            homeAddress.CountryCode ?? "",
            homeAddress.PhoneNumber,
            "", // TODO: implement fax
            "", // TODO: Implement private faculty profile
            contact.gc_Current_Positions__c ?? "",
            "", // TODO: implement department
            "", // TODO: Implement spouse name
            "", // TODO: Implement barcode
            account.PersonGenderIdentity ?? "",
            account.PersonEmail ?? "",
            "", // TODO: Implement type
            firstEmployment?.StartDate ?? new DateTime(1900, 1, 1),
            account.AD_Username__pc ?? "",
            "test test fac staff",
            2, // show_pic
            1, // preferred_photo
            onCampusAddress?.gc_On_Campus_Location__r?.ParentLocation?.Name ?? "",
            "", // TODO: Implement country
            "", // TODO: Implement mail_location
            "" // TODO: Implement mail_description
        );
    }
}