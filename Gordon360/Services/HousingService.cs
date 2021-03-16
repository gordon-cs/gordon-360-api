using System;
using System.Collections.Generic;
using System.Linq;
using Gordon360.Models.ViewModels;
using Gordon360.Models;
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
        /// Calls a stored procedure that returns a row in the staff whitelist which has the given user id,
        /// if it is in the whitelist
        /// </summary>
        /// <param name="userId"> The id of the person using the page </param>
        /// <returns> Whether or not the user is on the staff whitelist </returns>
        public bool CheckIfHousingAdmin(string userId)
        {
            IEnumerable<ApartmentAppAdminViewModel> idResult = null;

            var idParam = new SqlParameter("@USER_ID", userId);

            idResult = RawSqlQuery<ApartmentAppAdminViewModel>.query("GET_AA_ADMIN @USER_ID", idParam);
            if (!idResult.Any())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Calls a stored procedure that inserts the given id into the whitelist table
        /// </summary>
        /// <param name="id"> The id to insert </param>
        /// <returns> Whether or not this was successful </returns>
        public bool AddHousingAdmin(string id)
        {
            IEnumerable<ApartmentAppAdminViewModel> idResult = null;

            var idParam = new SqlParameter("@ADMIN_ID", id);

            idResult = RawSqlQuery<ApartmentAppAdminViewModel>.query("INSERT_AA_ADMIN @ADMIN_ID", idParam);
            if (idResult == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The id could not be added." };
            }
            else if (!idResult.Any())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Calls a stored procedure that deletes the given id from the whitelist table
        /// </summary>
        /// <param name="id"> The id to remove </param>
        /// <returns> Whether or not this was successful </returns>
        public bool RemoveHousingAdmin(string id)
        {
            IEnumerable<ApartmentAppAdminViewModel> idResult = null;

            var idParam = new SqlParameter("@ADMIN_ID", id);

            idResult = RawSqlQuery<ApartmentAppAdminViewModel>.query("DELETE_AA_ADMIN @ADMIN_ID", idParam);
            if (idResult == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The id could not be removed." };
            }
            else if (!idResult.Any())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Calls a stored procedure that tries to get the id of an the application that a given user is 
        /// applicant on for a given session
        /// </summary>
        /// <param name="studentId"> The id to look for </param>
        /// /// <param name="sess_cde"> Session for which the application would be </param>
        /// <returns> 
        /// The id of the application or 
        /// null if the user is not on an application for that session 
        /// </returns>
        public int? GetApplicationID(string studentId, string sess_cde)
        {
            IEnumerable<ApartmentAppIDViewModel> idResult = null;

            var idParam = new SqlParameter("@STUDENT_ID", studentId);
            var sessionParam = new SqlParameter("@SESS_CDE", sess_cde);
            
            idResult = RawSqlQuery<ApartmentAppIDViewModel>.query("GET_AA_APPID_BY_STU_ID_AND_SESS @SESS_CDE, @STUDENT_ID", sessionParam, idParam); //run stored procedure
            if (idResult == null || !idResult.Any())
            {
                return null; 
            }

            ApartmentAppIDViewModel idModel = idResult.ElementAt(0);

            int result = idModel.AprtAppID;

            return result;
        }

        /// <summary>
        /// Saves student housing info
        /// - first, it checks if an application ID was passed in as a parameter
        ///   - if not, then it checks the database for an existing application that matches the current semester and the current student user
        ///     - if an existing application is NOT found, then:
        ///       - first, it creates a new row in the applications table and inserts the id of the primary applicant and a timestamp
        ///       - second, it retrieves the application id of the application with the information we just inserted (because
        ///       the database creates the application ID so we have to ask it which number it generated for it)
        ///     - else, it retrieves the application ID of that existing application
        /// - third, it inserts each applicant into the applicants table along with the apartment ID so we know
        /// which application on which they are an applicant
        ///
        /// </summary>
        /// <param name="apartAppId"> The application ID number of the application to be edited </param>
        /// <param name="editorId"> The student ID number of the user who is attempting to save the apartment application </param>
        /// <param name="sess_cde"> The current session code </param>
        /// <param name="applicantIds"> Array of student ID numbers for each of the applicants </param>
        /// <returns>The application ID number if all the queries succeeded, otherwise returns -1</returns>
        public int SaveApplication(int apartAppId, string editorId, string sess_cde, string [] applicantIds)
        {
            IEnumerable<ApartmentAppIDViewModel> idResult = null;

            DateTime now = System.DateTime.Now;

            var sessionParam = new SqlParameter("@SESS_CDE", sess_cde);
            var editorParam = new SqlParameter("@STUDENT_ID", editorId);

            int appId = -1;

            if (apartAppId == -1)
            {
                // If an application ID was not passed in, then check if an application already exists
                idResult = RawSqlQuery<ApartmentAppIDViewModel>.query("GET_AA_APPID_BY_STU_ID_AND_SESS @SESS_CDE, @STUDENT_ID", sessionParam, editorParam); //run stored procedure
                if (idResult == null || !idResult.Any())
                {
                    IEnumerable<ApartmentAppSaveResultViewModel> newAppResult = null;

                    // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                    var timeParam = new SqlParameter("@NOW", now);
                    editorParam = new SqlParameter("@EDITOR_ID", editorId);

                    // If an existing application was not found for this editor, then insert a new application entry in the database
                    newAppResult = RawSqlQuery<ApartmentAppSaveResultViewModel>.query("INSERT_AA_APPLICATION @NOW, @EDITOR_ID", timeParam, editorParam); //run stored procedure
                    if (newAppResult == null)
                    {
                        throw new ResourceNotFoundException() { ExceptionMessage = "The application could not be saved." };
                    }

                    // The following is ODD, I know. It seems you cannot execute the same query with the same sql parameters twice.
                    // Thus, these two sql params must be recreated after being used in the last query:

                    // All SqlParameters must be remade before each SQL Query to prevent errors
                    timeParam = new SqlParameter("@NOW", now);
                    editorParam = new SqlParameter("@EDITOR_ID", editorId);

                    idResult = RawSqlQuery<ApartmentAppIDViewModel>.query("GET_AA_APPID_BY_NAME_AND_DATE @NOW, @EDITOR_ID", timeParam, editorParam); //run stored procedure
                    if (idResult == null)
                    {
                        throw new ResourceNotFoundException() { ExceptionMessage = "The new application ID could not be found." };
                    }
                }
                ApartmentAppIDViewModel idModel = idResult.ElementAt(0);

                appId = idModel.AprtAppID;
            }
            else
            {
                // Use the application ID number that was passed in as a parameter
                appId = apartAppId;
            }

            SqlParameter appIdParam = null;
            SqlParameter idParam = null;
            SqlParameter programParam = null;

            // TODO:
            // use stored procedure to get an array of all applicant IDs that match the application ID
            // - This will be used compare the new applicant IDs with any applicants already stored in the database
            //
            // Then, here is a suggested algorithm for adding and removing the necessary applicants to make the database match the data received from the frontend
            // - currentApplicantIds = array of applicant IDs from the database
            // - newApplicantIds = array of applicant IDs from the frontend
            //
            // We can use `newApplicantIds.Except(currentApplicantIds);` to get array of applicants that are not yet in the database
            // - In other words, an array of applicant that need to be inserted for this application id
            //
            // We can use `currentApplicantIds.Except(newApplicantIds);` to get array of applicants that are in database but not in the array from the frontend
            // - In other words, an array of applicants that should be removed
            //
            // Requires `using System.Linq;`

            foreach (string id in applicantIds) {
                IEnumerable<ApartmentApplicantViewModel> applicantResult = null;

                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                //idParam.Value = id; might need if this ODD solution is not satisfactory
                appIdParam = new SqlParameter("@APPLICATION_ID", appId);
                idParam = new SqlParameter("@ID_NUM", id);
                programParam = new SqlParameter("@APRT_PROGRAM", "");
                sessionParam = new SqlParameter("@SESS_CDE", sess_cde);

                applicantResult = RawSqlQuery<ApartmentApplicantViewModel>.query("INSERT_AA_APPLICANT @APPLICATION_ID, @ID_NUM, @APRT_PROGRAM, @SESS_CDE", appIdParam, idParam, programParam, sessionParam); //run stored procedure
                if (applicantResult == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "Applicant with ID " + id + " could not be saved." };
                }
            }

            return appId;
        }

        /// <summary>
        /// Changes the student user who has permission to edit the given application
        ///
        /// </summary>
        /// <returns>Whether or not all the queries succeeded</returns>
        public bool ChangeApplicationEditor(int apartAppId, string editorId, string newEditorId)
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

            if (editorId == storedEditorId)
            {
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
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a string from the combination of AA_ApartementApplications and AA_Applicants Tables
        /// and returns it to the frontend, so that it can convert it into a csv file.
        /// </summary>
        public string CreateCSV()
        {
            string csv = string.Empty;
            var result = RawSqlQuery<ApartmentApplicantDetailsViewModel>.query("GET_AA_APPLICANTS_DETAILS"); //run stored procedure
            foreach (var element in result)
            {
                csv += element.AprtAppID + ","
                     + element.EditorID + ","
                     + element.DateSubmitted + ","
                     + element.DateModified.ToString("MM/dd/yyyy HH:mm:ss") + ","
                     + element.ID_NUM + ","
                     + element.AprtProgram + ","
                     + element.AprtProgramCredit + ","
                     + element.SESS_CDE;
                const string V = "\r\n";
                csv += V;
            }

            return csv;
        }
    }
}
