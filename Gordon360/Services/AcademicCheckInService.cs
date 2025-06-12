using Gordon360.Exceptions;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gordon360.Services;

/// <summary>
/// Service Class that facilitates data transactions between the AcademicCheckInController and the CheckIn database model.
/// </summary>
	/// 
public partial class AcademicCheckInService(CCTContext context, IProfileService profileService, IAccountService accountService) : IAcademicCheckInService
{
    /// <summary> Stores the emergency contact information of a particular user </summary>
    /// <param name="data"> The object that stores the contact info </param>
    /// <param name="id"> The students id number</param>
    /// <returns> The stored data </returns>
    public async Task<EmergencyContactViewModel> PutEmergencyContactAsync(EmergencyContactViewModel data, string id, string username)
    {
        var splitUsername = username.Split('.');

        var result = await context.Procedures.UPDATE_EMRGCONTACTAsync(
            int.Parse(id),
            data.SEQ_NUMBER,
            data.LastName,
            data.FirstName,
            FormatNumber(data.HomePhone),
            FormatNumber(data.MobilePhone),
            data.Relationship,
            CreateNotesValue(data.MobilePhone, data.HomePhone),
            $"360Web ({splitUsername[1]}, {splitUsername[0]})",
            "Enrollment-Checkin");

        if (result == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
        }
        return data;
    }

    /// <summary>
    /// Create the notes value for the database to be passed in with the rest of the data.
    /// The reason for this is that the notes column in the database is only made up of what phone numbers a contact has that are international
    /// </summary>
    /// <param name="MobilePhone"> The mobile phone of the contact</param>
    /// <param name="HomePhone"> The home phone of the contact </param>
    /// <returns> The formatted notes parameter to be passed to the database </returns>
    private static string CreateNotesValue(string MobilePhone, string HomePhone)
    {
        bool HomePhoneINTL = HomePhone.StartsWith("+");
        bool MobilePhoneINTL = MobilePhone.StartsWith("+");
        string result = "";
        if (HomePhoneINTL)
        {
            result = "Intl Home: " + HomePhone;
        }
        else if (MobilePhoneINTL)
        {
            result = "Intl Mobile: " + MobilePhone;
        }
        else if (MobilePhoneINTL && HomePhoneINTL)
        {
            result = "Intl Home: " + HomePhone + " " + "Intl Mobile: " + MobilePhone;
        }
        return result;
    }

    /// <summary> Stores the cellphone preferences for the current user </summary>
    /// <param name="data"> The phone number object for the user </param>
    /// <param name="username"> The username of the student to be updated </param>
    /// <returns> The stored data </returns>
    public async Task PutCellPhoneAsync(string username, MobilePhoneUpdateViewModel data)
    {
        await profileService.UpdateCustomProfileAsync(username, "SMSOptedIn", new Models.CCT.CUSTOM_PROFILE { username = username, SMSOptedIn = data.SMSOptedIn });

        string id = accountService.GetAccountByUsername(username).GordonID;
        var result = await context.Procedures.FINALIZATION_UPDATECELLPHONEAsync(id, FormatNumber(data.PersonalPhone), data.MakePrivate, NoneProvided: false);

        if (result == null || result.Any(r => r.Success != true))
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
        }
    }

    /// <summary> Stores the demographic data (race and ethnicity) of the current user </summary>
    /// <param name="data"> The race and ethnicity data for the user </param>
    /// <param name="id"> The id of the user to be updated </param>
    /// <returns> The stored data </returns>
    public async Task<AcademicCheckInViewModel> PutDemographicAsync(string id, AcademicCheckInViewModel data)
    {
        var result = await context.Procedures.FINALIZATION_UPDATEDEMOGRAPHICAsync(id, data.Race, data.Ethnicity);
        if (result == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
        }
        return data;
    }

    /// <summary> Gets the holds of the user with the given ID </summary>
    /// <param name="id"> The id of the user whose holds are to be found </param>
    /// <returns>Data about any holds the user has</returns>
    public async Task<EnrollmentCheckinHolds> GetHoldsAsync(string id)
    {
        var result = await context.Procedures.GetEnrollmentCheckinHoldsAsync(int.Parse(id));

        if (result == null || result.Count != 1)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
        }

        return new EnrollmentCheckinHolds(result.Single());
    }

    /// <summary> Sets the user as having been checked in </summary>
    /// <param name="id"> The id of the user who is to be marked as checked in </param>
    public async Task SetStatusAsync(string id)
    {
        var result = await context.Procedures.FINALIZATION_MARK_AS_CURRENTLY_COMPLETEDAsync(id);

        if (result == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
        }
    }

    /// <summary> Gets the whether the user has completed Academic Checkin </summary>
    /// <param name="id"> The id of the user for which the data is to be found for </param>
    public async Task<bool> GetStatusAsync(string id)
    {
        var result = await context.Procedures.FINALIZATION_GET_FINALIZATION_STATUSAsync(int.Parse(id));

        if (result.Count() == 0)
        {
            return true; //This is due to the fact that the database returns nothing if the user is checked in
        }
        else
        {
            return result.First().FinalizationCompleted;
        }
    }

    /// <summary> Formats a phone number for insertion into the database </summary>
    /// <param name="phoneNum"> The phone number to be formatted </param>
    /// <returns> The formatted number </returns>
    private static string FormatNumber(string phoneNum)
    {
        phoneNum = phoneNum.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "");
        if (PhoneNumberRegex().IsMatch(phoneNum))
        {
            return phoneNum;
        }
        else
        {
            throw new BadInputException() { ExceptionMessage = "Phone Numbers must only be numerical digits." };
        }
    }

    [GeneratedRegex(@"^\+?\d*$")]
    private static partial Regex PhoneNumberRegex();
}
