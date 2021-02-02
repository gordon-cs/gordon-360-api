using System;
using System.Collections.Generic;
using System.Linq;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Exceptions.CustomExceptions;
using System.Data.SqlClient;
using Gordon360.Services.ComplexQueries;

namespace Gordon360.Services
{
    public class HousingService : IHousingService
    {
        private IUnitOfWork _unitOfWork;


        public HousingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Saves student housing info
        /// - first, it creates a new row in the applications table and inserts the id of the primary applicant and a timestamp
        /// - second, it retrieves the application id of the application with the information we just inserted (because
        /// the database creates the application ID so we have to ask it which number it generated for it)
        /// - third, it inserts each applicant into the applicants table along with the apartment ID so we know
        /// which application on which they are an applicant
        ///
        /// </summary>
        /// <param name="editorId"> The student ID number of the user who is attempting to save the apartment application </param>
        /// <param name="sess_cde"> The current session code </param>
        /// <param name="applicantIds"> Array of student ID numbers for each of the applicants </param>
        /// <param name="apartmentChoices"> Array of JSON objects providing apartment hall choices </param>
        /// <returns>The application ID number if all the queries succeeded, otherwise returns -1</returns>
        public int SaveApplication(string editorId, string sess_cde, string [] applicantIds, ApartmentChoiceViewModel[] apartmentChoices)
        {
            IEnumerable<ApartmentAppIDViewModel> idResult = null;

            DateTime now = System.DateTime.Now;

            SqlParameter sessionParam = new SqlParameter("@SESS_CDE", sess_cde);
            SqlParameter editorParam = new SqlParameter("@STUDENT_ID", editorId);

            // If an application ID was not passed in, then check if an application already exists
            idResult = RawSqlQuery<ApartmentAppIDViewModel>.query("GET_AA_APPID_BY_STU_ID_AND_SESS @SESS_CDE, @STUDENT_ID", sessionParam, editorParam); //run stored procedure
            if (idResult != null && idResult.Any())
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "An existing application ID was found for this user. Please use 'EditApplication' to update an existing application." };
            }

            //----------------
            // Save the application editor and time

            IEnumerable<ApartmentAppSaveResultViewModel> newAppResult = null;

            // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
            SqlParameter timeParam = new SqlParameter("@NOW", now);
            editorParam = new SqlParameter("@EDITOR_ID", editorId);

            // If an existing application was not found for this editor, then insert a new application entry in the database
            newAppResult = RawSqlQuery<ApartmentAppSaveResultViewModel>.query("INSERT_AA_APPLICATION @NOW, @EDITOR_ID", timeParam, editorParam); //run stored procedure
            if (newAppResult == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The application could not be saved." };
            }

            //----------------
            // Retrieve the application ID number of this new application

            // The following is ODD, I know. It seems you cannot execute the same query with the same sql parameters twice.
            // Thus, these two sql params must be recreated after being used in the last query:

            idResult = null;

            // All SqlParameters must be remade before each SQL Query to prevent errors
            timeParam = new SqlParameter("@NOW", now);
            editorParam = new SqlParameter("@EDITOR_ID", editorId);

            idResult = RawSqlQuery<ApartmentAppIDViewModel>.query("GET_AA_APPID_BY_NAME_AND_DATE @NOW, @EDITOR_ID", timeParam, editorParam); //run stored procedure
            if (idResult == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The new application ID could not be found." };
            }

            ApartmentAppIDViewModel idModel = idResult.ElementAt(0);
            int apartAppId = idModel.AprtAppID;

            //----------------
            // Save applicant information

            SqlParameter appIdParam = null;
            SqlParameter idParam = null;
            SqlParameter programParam = null;

            foreach (string id in applicantIds) {
                IEnumerable<ApartmentApplicantViewModel> applicantResult = null;

                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);
                idParam = new SqlParameter("@ID_NUM", id);
                programParam = new SqlParameter("@APRT_PROGRAM", "");
                sessionParam = new SqlParameter("@SESS_CDE", sess_cde);

                applicantResult = RawSqlQuery<ApartmentApplicantViewModel>.query("INSERT_AA_APPLICANT @APPLICATION_ID, @ID_NUM, @APRT_PROGRAM, @SESS_CDE", appIdParam, idParam, programParam, sessionParam); //run stored procedure
                if (applicantResult == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "Applicant with ID " + id + " could not be saved." };
                }
            }

            //----------------
            // Save hall information

            // PLACEHOLDER
            //
            // The update hall info code will go here once we get a chance to implement it
            //
            // PLACEHOLDER

            return apartAppId;
        }

        /// <summary>
        /// Edit an existings apartment application
        /// - first, it gets the EditorID from the database for the given application ID and makes sure that the student ID of the current user matches that stored ID number
        /// - second, it gets an array of the applicants that are already stored in the database for the given application ID
        /// - third, it inserts each applicant that is in the 'newApplicantIds' array but was not yet in the database
        /// - fourth, it removes each applicant that was stored in the database but was not in the 'newApplicantIds' array
        ///
        /// </summary>
        /// <param name="editorId"> The student ID number of the user who is attempting to save the apartment application </param>
        /// <param name="sess_cde"> The current session code </param>
        /// <param name="apartAppId"> The application ID number of the application to be edited </param>
        /// <param name="newApplicantIds"> Array of student ID numbers for each of the applicants </param>
        /// <param name="newApartmentChoices"> Array of JSON objects providing apartment hall choices </param>
        /// <returns>Returns true if all the queries succeeded, otherwise returns false</returns>
        public int EditApplication(string editorId, string sess_cde, int apartAppId, string[] newApplicantIds, ApartmentChoiceViewModel[] newApartmentChoices)
        {
            IEnumerable<ApartmentAppEditorViewModel> editorResult = null;

            SqlParameter appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);

            editorResult = RawSqlQuery<ApartmentAppEditorViewModel>.query("GET_AA_EDITOR_BY_APPID @APPLICATION_ID", appIdParam);
            if (editorResult == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The application could not be found." };
            }
            else if (!editorResult.Any())
            {
                return -1;
            }

            ApartmentAppEditorViewModel editorModel = editorResult.ElementAt(0);
            string storedEditorId = editorModel.EditorID;

            if (editorId != storedEditorId)
            {
                // Return -1 if the current user does not match this application's editor stored in the database
                return -1;
            }
            // Only perform the update if the student ID of the current user matched the 'EditorID' ID stored in the database for the requested application

            //--------
            // Update applicant information

            IEnumerable<ApartmentApplicantIDViewModel> applicantIdsResult = null;

            appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);

            // Get the IDs of the applicants that are already stored in the database for this application
            applicantIdsResult = RawSqlQuery<ApartmentApplicantIDViewModel>.query("GET_AA_APPLICANTS_BY_APPID @APPLICATION_ID", appIdParam);
            if (applicantIdsResult == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The applicants could not be found." };
            }

            string[] oldApplicantIds =  null;
            // Check whether any applicants were found matching the given application ID number
            if (applicantIdsResult.Any())
            {
                oldApplicantIds = new string[applicantIdsResult.Count()];
                for (int i = 0; i < applicantIdsResult.Count(); i++)
                {
                    ApartmentApplicantIDViewModel applicantModel = applicantIdsResult.ElementAt(i);
                    oldApplicantIds[i] = applicantModel.ID_NUM;
                }
            } else {
                oldApplicantIds = new string[0];
            }

            // Get an array of applicants IDs that are in the array recieved from the frontend but not yet in the database
            IEnumerable<string> applicantIdsToAdd = newApplicantIds.Except(oldApplicantIds);

            // Get an array of appliants IDs that in the database but not in the array recieved from the frontend
            IEnumerable<string> applicantIdsToRemove = oldApplicantIds.Except(newApplicantIds);

            SqlParameter idParam = null;
            SqlParameter programParam = null;
            SqlParameter sessionParam = null;

            foreach (string id in applicantIdsToAdd)
            {
                IEnumerable<ApartmentApplicantViewModel> applicantResult = null;

                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                //idParam.Value = id; might need if this ODD solution is not satisfactory
                appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);
                idParam = new SqlParameter("@ID_NUM", id);
                programParam = new SqlParameter("@APRT_PROGRAM", "");
                sessionParam = new SqlParameter("@SESS_CDE", sess_cde);

                applicantResult = RawSqlQuery<ApartmentApplicantViewModel>.query("INSERT_AA_APPLICANT @APPLICATION_ID, @ID_NUM, @APRT_PROGRAM, @SESS_CDE", appIdParam, idParam, programParam, sessionParam); //run stored procedure
                if (applicantResult == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "Applicant with ID " + id + " could not be inserted." };
                }
            }

            foreach (string id in applicantIdsToRemove)
            {
                IEnumerable<ApartmentApplicantViewModel> applicantResult = null;

                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                //idParam.Value = id; might need if this ODD solution is not satisfactory
                appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);
                idParam = new SqlParameter("@ID_NUM", id);
                sessionParam = new SqlParameter("@SESS_CDE", sess_cde);

                applicantResult = RawSqlQuery<ApartmentApplicantViewModel>.query("DELETE_AA_APPLICANT @APPLICATION_ID, @ID_NUM, @SESS_CDE", appIdParam, idParam, sessionParam); //run stored procedure
                if (applicantResult == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "Applicant with ID " + id + " could not be removed." };
                }
            }

            //--------
            // Update hall information

            // PLACEHOLDER
            //
            // The update hall info code will go here once we get a chance to implement it
            //
            // PLACEHOLDER

            //--------
            // Update the date modified

            IEnumerable<ApartmentAppSaveResultViewModel> result = null;

            DateTime now = System.DateTime.Now;

            appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);
            SqlParameter timeParam = new SqlParameter("@NOW", now);

            result = RawSqlQuery<ApartmentAppSaveResultViewModel>.query("UPDATE_AA_APPLICATION_DATEMODIFIED @APPLICATION_ID, @NOW", appIdParam, timeParam); //run stored procedure
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The application DateModified could not be updated." };
            }

            return apartAppId;
        }

        /// <summary>
        /// Changes the student user who has permission to edit the given application
        ///
        /// </summary>
        /// <returns>Whether or not all the queries succeeded</returns>
        public bool ChangeApplicationEditor(string editorId, int apartAppId, string newEditorId)
        {
            IEnumerable<ApartmentAppEditorViewModel> editorResult = null;
            IEnumerable<ApartmentAppSaveResultViewModel> result = null;

            DateTime now = System.DateTime.Now;

            SqlParameter appIdParam = null;
            SqlParameter editorParam = null;
            SqlParameter timeParam = null;
            SqlParameter newEditorParam = null;

            appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);

            editorResult = RawSqlQuery<ApartmentAppEditorViewModel>.query("GET_AA_EDITOR_BY_APPID @APPLICATION_ID", appIdParam);
            if (editorResult == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The application could not be found." };
            }
            else if (!editorResult.Any())
            {
                return false;
            }

            ApartmentAppEditorViewModel editorModel = editorResult.ElementAt(0);
            string storedEditorId = editorModel.EditorID;

            if (editorId != storedEditorId)
            {
                // Return false if the current user does not match this application's editor stored in the database
                return false;
            }
            // Only perform the update if the ID of the current user matched the 'EditorID' ID stored in the database for the requested application

            appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);
            editorParam = new SqlParameter("@EDITOR_ID", editorId);
            timeParam = new SqlParameter("@NOW", now);
            newEditorParam = new SqlParameter("@NEW_EDITOR_ID", newEditorId);

            result = RawSqlQuery<ApartmentAppSaveResultViewModel>.query("UPDATE_AA_APPLICATION_EDITOR @APPLICATION_ID, @EDITOR_ID, @NOW, @NEW_EDITOR_ID", appIdParam, editorParam, timeParam, newEditorParam); //run stored procedure
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The application could not be updated." };
            }

            return true;
        }
    }
}
