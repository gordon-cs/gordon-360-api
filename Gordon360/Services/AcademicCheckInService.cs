using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services.ComplexQueries;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gordon360.Services
{
    /// <summary>
    /// Service Class that facilitates data transactions between the AcademicCheckInController and the CheckIn database model.
    /// </summary>
	/// 
    public class AcademicCheckInService : IAcademicCheckInService
    {
        private IUnitOfWork _unitOfWork;
        public AcademicCheckInService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        /// <summary> Stores the emergency contact information of a particular user </summary>
        /// <param name="data"> The object that stores the contact info </param>
        /// <param name="id"> The students id number</param>
        /// <returns> The stored data </returns>
        public EmergencyContact PutEmergencyContact(EmergencyContact data, string id)
        {
            var studentIDParam = new SqlParameter("@StudentID", Int32.Parse(id));
            var contactIDParam = new SqlParameter("@ContactNum", data.SEQ_NUM);
            var contactLastNameParam = new SqlParameter("@ContactLastName", data.lastname);
            var contactFirstNameParam = new SqlParameter("@ContactFirstName", data.firstname);
            var contactHomePhoneParam = new SqlParameter("@ContactHomePhone", FormatNumber(data.HomePhone));
            var contactMobilePhoneParam = new SqlParameter("@ContactMobilePhone", FormatNumber(data.MobilePhone));
            var contactRelationshipParam = new SqlParameter("@ContactRelationship", data.relationship);
            var notesParam = new SqlParameter("@Notes", "");
            var usernameParam = new SqlParameter("@Username", "360Web (" + data.lastname + ", " + data.firstname + ")");
            var jobNameParam = new SqlParameter("@JobName", "Enrollment-Checkin");

            // Run stored procedure
            var result = RawSqlQuery<EmergencyContactViewModel>.query("UPDATE_EMRGCONTACT @StudentID, @ContactNum, @ContactLastName, @ContactFirstName, @ContactHomePhone, @ContactMobilePhone, @ContactRelationship, @Notes, @Username, @JobName", studentIDParam, contactIDParam, contactLastNameParam, contactFirstNameParam, contactHomePhoneParam, contactMobilePhoneParam, contactRelationshipParam, notesParam, usernameParam, jobNameParam);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }
            return data;
        }

        // need user ID, unformatted number, whether they would like their phone number private, and whether or not they have a phone number


        
        /// <summary> Stores the cellphone preferences for the current user </summary>
        /// <param name="data"> The phone number object for the user </param>
        /// <param name="id"> The id of the student to be updated </param>
        /// <returns> The stored data </returns>
        public AcademicCheckInViewModel PutCellPhone(string id, AcademicCheckInViewModel data)
        {
            var studentIDParam = new SqlParameter("@UserID", id);
            var personalPhoneParam = new SqlParameter("@PhoneUnformatted", FormatNumber(data.PersonalPhone));
            var makePrivateParam = new SqlParameter("@DoNotPublish", data.MakePrivate);
            var noPhoneParam = new SqlParameter("@NoneProvided", data.NoPhone);

            // Run stored procedure
            var result = RawSqlQuery<AcademicCheckInViewModel>.query("FINALIZATION_UPDATECELLPHONE @UserID, @PhoneUnformatted, @DoNotPublish, @NoneProvided", studentIDParam, personalPhoneParam, makePrivateParam, noPhoneParam);
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
        public AcademicCheckInViewModel PutDemographic(string id, AcademicCheckInViewModel data)
        {
            var studentIDParam = new SqlParameter("@UserID", id);
            var RaceValueParam = new SqlParameter("@RaceValue", data.Race);
            var EthnicityValueParam = new SqlParameter("@EthnicityValue", data.Ethnicity);

            // Run stored procedure
            var result = RawSqlQuery<AcademicCheckInViewModel>.query("FINALIZATION_UPDATEDEMOGRAPHIC @UserID, @RaceValue, @EthnicityValue", studentIDParam, RaceValueParam, EthnicityValueParam);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }
            return data;
        }

        /// <summary> Gets the holds of the user with the given ID </summary>
        /// <param name="id"> The id of the user whose holds are to be found </param>
        /// <returns> The stored data </returns>
        public IEnumerable<AcademicCheckInViewModel> GetHolds(string id)
        {
            var studentIDParam = new SqlParameter("@ID_NUM", id);

            // Run stored procedure
            var result = RawSqlQuery<AcademicCheckInViewModel>.query("FINALIZATION_GETHOLDSBYID @ID_NUM", studentIDParam);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }
            return result;
        }

        /// <summary> Sets the user as having been checked in </summary>
        /// <param name="id"> The id of the user who is to be marked as checked in </param>
        public void SetStatus(string id)
        {
            var studentIDParam = new SqlParameter("@UserID", id);

            // Run stored procedure
            var result = RawSqlQuery<AcademicCheckInViewModel>.query("FINALIZATION_MARK_AS_CURRENTLY_COMPLETED @UserID", studentIDParam);

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }
        }

        /// <summary> Gets the users check in status </summary>
        /// <param name="id"> The id of the user for which the data is to be found for </param>
        public Boolean GetStatus(string id)
        {
            var studentIDParam = new SqlParameter("@UserID", id);

            // Run stored procedure
            var result = RawSqlQuery<AcademicCheckInViewModel>.query("FINALIZATION_GET_FINALIZATION_STATUS @UserID", studentIDParam);

            if (result.Count() == 0)
            {
                return false; //This is due to the fact that the database returns nothing if the user is checked in
            } else {
                return result.First().FinalizationCompleted;
            }
        }

        /// <summary> Formats a phone number for insertion into the database </summary>
        /// <param name="phoneNum"> The phone number to be formatted </param>
        /// <returns> The formatted number </returns>
        public string FormatNumber(string phoneNum)
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
