using Gordon360.Exceptions;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gordon360.Services
{
    /// <summary>
    /// Service Class that facilitates data transactions between the AcademicCheckInController and the CheckIn database model.
    /// </summary>
	/// 
    public class AcademicCheckInService : IAcademicCheckInService
    {
        private readonly CCTContext _context;
        public AcademicCheckInService(CCTContext context)
        {
            _context = context;
        }


        /// <summary> Stores the emergency contact information of a particular user </summary>
        /// <param name="data"> The object that stores the contact info </param>
        /// <param name="id"> The students id number</param>
        /// <returns> The stored data </returns>
        public async Task<EmergencyContactViewModel> PutEmergencyContactAsync(EmergencyContactViewModel data, int id, string username)
        {
            var splitUsername = username.Split('.');

            var result = await _context.Procedures.UPDATE_EMRGCONTACTAsync(
                id,
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
        /// <param name="id"> The id of the student to be updated </param>
        /// <returns> The stored data </returns>
        public async Task<AcademicCheckInViewModel> PutCellPhoneAsync(int id, AcademicCheckInViewModel data)
        {
            var result = await _context.Procedures.FINALIZATION_UPDATECELLPHONEAsync(id, FormatNumber(data.PersonalPhone), data.MakePrivate, data.NoPhone);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }
            return data;
        }

        /// <summary> Stores the demographic data (race and ethnicity) of the current user </summary>
        /// <param name="data"> The race and ethnicity data for the user </param>
        /// <param name="id"> The id of the user to be updated </param>
        /// <returns> The stored data </returns>
        public async Task<AcademicCheckInViewModel> PutDemographicAsync(int id, AcademicCheckInViewModel data)
        {
            var result = await _context.Procedures.FINALIZATION_UPDATEDEMOGRAPHICAsync(id, data.Race, data.Ethnicity);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }
            return data;
        }

        /// <summary> Gets the holds of the user with the given ID </summary>
        /// <param name="id"> The id of the user whose holds are to be found </param>
        /// <returns> The stored data </returns>
        public async Task<IEnumerable<AcademicCheckInViewModel>> GetHoldsAsync(int id)
        {
            var result = await _context.Procedures.FINALIZATION_GETHOLDSBYIDAsync(id);

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            return result.Select(x => new AcademicCheckInViewModel
            {
                FinancialHold = x.FinancialHold ?? false,
                HighSchoolHold = x.HighSchoolHold ?? false,
                MedicalHold = x.MedicalHold ?? false,
                MajorHold = x.MajorHold ?? false,
                RegistrarHold = x.RegistrarHold ?? false,
                LaVidaHold = x.LaVidaHold ?? false,
                MustRegisterForClasses = x.MustRegisterForClasses ?? false,
                NewStudent = x.NewStudent,
                FinancialHoldText = x.FinancialHoldText,
                MeetingDate = x.MeetingDate,
                MeetingLocations = x.MeetingLocations
            });
        }

        /// <summary> Sets the user as having been checked in </summary>
        /// <param name="id"> The id of the user who is to be marked as checked in </param>
        public async Task SetStatusAsync(int id)
        {
            var result = await _context.Procedures.FINALIZATION_MARK_AS_CURRENTLY_COMPLETEDAsync(id);

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }
        }

        /// <summary> Gets the whether the user has completed Academic Checkin </summary>
        /// <param name="id"> The id of the user for which the data is to be found for </param>
        public async Task<bool> GetStatusAsync(int id)
        {
            var result = await _context.Procedures.FINALIZATION_GET_FINALIZATION_STATUSAsync(id);

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
            if (Regex.IsMatch(phoneNum, @"\+?[0-9]*"))
            {
                return phoneNum;
            }
            else
            {
                throw new BadInputException() { ExceptionMessage = "Phone Numbers must only be numerical digits." };
            }
        }


    }
}
