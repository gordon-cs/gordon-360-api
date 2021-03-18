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
        /// - first, it creates a new row in the applications table and inserts the id of the primary applicant and a timestamp
        /// - second, it retrieves the application id of the application with the information we just inserted (because
        /// the database creates the application ID so we have to ask it which number it generated for it)
        /// - third, it inserts each applicant into the applicants table along with the apartment ID so we know
        /// which application on which they are an applicant
        ///
        /// </summary>
        /// <param name="editorId"> The student ID number of the user who is attempting to save the apartment application </param>
        /// <param name="sess_cde"> The current session code </param>
        /// <param name="applicantIDs"> Array of student ID numbers for each of the applicants </param>
        /// <param name="apartmentChoices"> Array of JSON objects providing apartment hall choices </param>
        /// <returns>Returns the application ID number if all the queries succeeded</returns>
        public int SaveApplication(string editorId, string sess_cde, string [] applicantIDs, ApartmentChoiceViewModel[] apartmentChoices)
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

            foreach (string id in applicantIDs) {
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

            SqlParameter rankingParam = null;
            SqlParameter buildingCodeParam = null;

            foreach (ApartmentChoiceViewModel choice in apartmentChoices)
            {
                IEnumerable<ApartmentChoiceSaveViewModel> apartmentChoiceResult = null;

                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);
                rankingParam = new SqlParameter("@RANKING", choice.HallRank);
                buildingCodeParam = new SqlParameter("@BLDG_CDE", choice.HallName);
                apartmentChoiceResult = RawSqlQuery<ApartmentChoiceSaveViewModel>.query("INSERT_AA_APARTMENT_CHOICE @APPLICATION_ID, @RANKING, @BLDG_CDE", appIdParam, rankingParam, buildingCodeParam); // run stored procedure
                if (apartmentChoiceResult == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "The apartment preference could not be saved." };
                }
            }

            return apartAppId;
        }

        /// <summary>
        /// Edit an existings apartment application
        /// - first, it gets the EditorID from the database for the given application ID and makes sure that the student ID of the current user matches that stored ID number
        /// - second, it gets an array of the applicants that are already stored in the database for the given application ID
        /// - third, it inserts each applicant that is in the 'newApplicantIDs' array but was not yet in the database
        /// - fourth, it removes each applicant that was stored in the database but was not in the 'newApplicantIDs' array
        ///
        /// </summary>
        /// <param name="editorId"> The student ID number of the user who is attempting to save the apartment application </param>
        /// <param name="sess_cde"> The current session code </param>
        /// <param name="apartAppId"> The application ID number of the application to be edited </param>
        /// <param name="newApplicantIDs"> Array of student ID numbers for each of the applicants </param>
        /// <param name="newApartmentChoices"> Array of JSON objects providing apartment hall choices </param>
        /// <returns>Returns the application ID number if all the queries succeeded</returns>
        public int EditApplication(string editorId, string sess_cde, int apartAppId, string[] newApplicantIDs, ApartmentChoiceViewModel[] newApartmentChoices)
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

            IEnumerable<ApartmentApplicantIDViewModel> applicantIDsResult = null;

            appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);

            // Get the IDs of the applicants that are already stored in the database for this application
            applicantIDsResult = RawSqlQuery<ApartmentApplicantIDViewModel>.query("GET_AA_APPLICANTS_BY_APPID @APPLICATION_ID", appIdParam);
            if (applicantIDsResult == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The applicants could not be found." };
            }

            // List of applicants IDs that are in the array recieved from the frontend but not yet in the database
            List<string> applicantIDsToAdd = new List<string>(newApplicantIDs);

            // List of applicants IDs that are in both the array recieved from the frontend and the database
            List<string> applicantIDsToUpdate = new List<string>();

            // List of applicants IDs that are in the database but not in the array recieved from the frontend
            List<string> applicantIDsToRemove = new List<string>();

            // Check whether any applicants were found matching the given application ID number
            if (applicantIDsResult.Any())
            {
                foreach(ApartmentApplicantIDViewModel applicantIDModel in applicantIDsResult)
                {
                    string existingApplicantID = applicantIDModel.ID_NUM;
                    if (newApplicantIDs.Contains(existingApplicantID))
                    {
                        applicantIDsToAdd.Remove(existingApplicantID);
                        applicantIDsToUpdate.Add(existingApplicantID);
                    }
                    else
                    {
                        applicantIDsToRemove.Add(existingApplicantID);
                    }
                }
            }

            SqlParameter idParam = null;
            SqlParameter programParam = null;
            SqlParameter sessionParam = null;

            // Insert new applicants that are not yet in the database
            foreach (string id in applicantIDsToAdd)
            {
                IEnumerable<ApartmentApplicantViewModel> applicantResult = null;

                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
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

            // Update the info of applicants from the frontend that are already in the database
            foreach (string id in applicantIDsToUpdate)
            {
                IEnumerable<ApartmentApplicantViewModel> applicantResult = null;

                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);
                idParam = new SqlParameter("@ID_NUM", id);
                programParam = new SqlParameter("@APRT_PROGRAM", ""); // TODO: This will be used to update the off-campus program department once that feature has been made on the frontend
                sessionParam = new SqlParameter("@SESS_CDE", sess_cde);

                applicantResult = RawSqlQuery<ApartmentApplicantViewModel>.query("UPDATE_AA_APPLICANT @APPLICATION_ID, @ID_NUM, @APRT_PROGRAM, @SESS_CDE", appIdParam, idParam, programParam, sessionParam); //run stored procedure
                if (applicantResult == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "Applicant with ID " + id + " could not be updated." };
                }
            }

            // Remove applicants from the database that were remove from the frontend
            foreach (string id in applicantIDsToRemove)
            {
                IEnumerable<ApartmentApplicantViewModel> applicantResult = null;

                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
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

            // Sudo code:
            //first - ApartmentChoiceSaveViewModel list (IEnumerable)
            //    compare the list from front and back and sort the elements into the string lists
            //      list<string> bldg code to add or update
            //      list stinrg to remove

            //    compare the first string list to put the rankings for each buildings

            //    then I will create 2nd viewmodel list to add all of them so that i can send back to db (or call the stored precedure)
            //    loop - add
            //    then 2nd loop update
            //    3rd loop to remove

            IEnumerable<ApartmentChoiceSaveViewModel> apartmentChoicesFromDB = null;

            appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);

            // Get the apartment preferences that are already stored in the database for this application
            apartmentChoicesFromDB = RawSqlQuery<ApartmentChoiceSaveViewModel>.query("GET_AA_APARTMENT_CHOICES_BY_APP_ID @APPLICATION_ID", appIdParam);
            if (apartmentChoicesFromDB == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The hall information could not be found." };
            }

            // Initialize the list to store information from the frontend.
            //List<ApartmentChoiceViewModel> apartmentChoicesFromFrontend = new List<ApartmentChoiceViewModel>(newApartmentChoices);

            // List to store json objects as strings
            List<string> newApartmentChoicesToAddOrUpdate = new List<string>();

            // List to store the existing hall preferences in the database
            List<string> existingApartmentChoices = new List<string>();

            // List of apartment choices to remove from the database
            List<string> apartmentChoicesToRemove = new List<string>();

            // Add the hall preference from the database into the list of strings
            if (apartmentChoicesFromDB.Any())
            {
                foreach (ApartmentChoiceSaveViewModel apartmentChoiceModel in apartmentChoicesFromDB)
                {
                    string oldApartmentChoice = apartmentChoiceModel.BLDG_CDE;
                    existingApartmentChoices.Add(oldApartmentChoice);
                }
            }

            // Add the hall preference from the frontend into the list of strings
            if (newApartmentChoices.Any())
            {
                foreach (ApartmentChoiceViewModel newApartmentChoiceModel in newApartmentChoices)
                {
                    string newApartmentChoice = newApartmentChoiceModel.HallName;
                    newApartmentChoicesToAddOrUpdate.Add(newApartmentChoice);
                }
            }

            // Find a list of apartment choices to remove
            foreach (string bldg in existingApartmentChoices)
            {
                if (!newApartmentChoicesToAddOrUpdate.Contains(bldg))
                {
                    apartmentChoicesToRemove.Add(bldg);
                    newApartmentChoicesToAddOrUpdate.Remove(bldg);
                }
            }

            // List of apartment choices that are in the array recieved from the frontend but not yet in the database
            List<string> apartmentChoicesToAdd = new List<string>();
            // List of apartment choices that are in both the array recieved from the frontend and the database
            List<string> apartmentChoicesToUpdate = new List<string>();

            // Separate the list into two: ToAdd and ToUpdate
            foreach (string bldg in newApartmentChoicesToAddOrUpdate)
            {
                if (existingApartmentChoices.Contains(bldg))
                {
                    apartmentChoicesToUpdate.Add(bldg);
                    newApartmentChoicesToAddOrUpdate.Remove(bldg);
                }
                else
                {
                    apartmentChoicesToAdd.Add(bldg);
                }
            }

            // Insert new apartment choices that are not yet in the database
            SqlParameter rankingParam = null;
            SqlParameter buildingCodeParam = null;

            foreach (string bldg in apartmentChoicesToAdd)
            {
                IEnumerable<ApartmentChoiceSaveViewModel> apartmentChoiceResult = null;

                foreach (ApartmentChoiceViewModel newApartmentChoiceModel in newApartmentChoices)
                {
                    if (bldg == newApartmentChoiceModel.HallName)
                    {
                        // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                        appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);
                        rankingParam = new SqlParameter("@RANKING", newApartmentChoiceModel.HallRank);
                        buildingCodeParam = new SqlParameter("@BLDG_CDE", newApartmentChoiceModel.HallName);
                        apartmentChoiceResult = RawSqlQuery<ApartmentChoiceSaveViewModel>.query("INSERT_AA_APARTMENT_CHOICE @APPLICATION_ID, @RANKING, @BLDG_CDE", appIdParam, rankingParam, buildingCodeParam); // run stored procedure
                        if (apartmentChoiceResult == null)
                        {
                            throw new ResourceNotFoundException() { ExceptionMessage = "The apartment preference could not be saved." };
                        }
                    }
                }
            }

            // Update existing apartment choices that are in both the database and the frontend
            foreach (string bldg in apartmentChoicesToUpdate)
            {
                IEnumerable<ApartmentChoiceSaveViewModel> apartmentChoiceResult = null;

                foreach (ApartmentChoiceViewModel newApartmentChoiceModel in newApartmentChoices)
                {
                    if (bldg == newApartmentChoiceModel.HallName)
                    {
                        // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                        appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);
                        rankingParam = new SqlParameter("@RANKING", newApartmentChoiceModel.HallRank);
                        buildingCodeParam = new SqlParameter("@BLDG_CDE", newApartmentChoiceModel.HallName);
                        apartmentChoiceResult = RawSqlQuery<ApartmentChoiceSaveViewModel>.query("INSERT_AA_APARTMENT_CHOICE @APPLICATION_ID, @RANKING, @BLDG_CDE", appIdParam, rankingParam, buildingCodeParam); // run stored procedure
                        if (apartmentChoiceResult == null)
                        {
                            throw new ResourceNotFoundException() { ExceptionMessage = "The apartment preference could not be saved." };
                        }
                    }
                }
            }

            // Initialize the list of string so that we could perform sorting methods like 'contains', 'add', and 'remove'.
            // List of apartment choices by their BLDG_CDE:
            List<string> apartmentChoicesToAddOrUpdate = new List<string>(); // apartment choices to add or update
            


            
            



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

        /// <summary>
        /// Creates a string from the combination of AA_ApartementApplications and AA_Applicants Tables
        /// and returns it to the frontend, so that it can convert it into a csv file.
        /// </summary>
        public string CreateCSV()
        {
            string csv = string.Empty;
            var result = RawSqlQuery<ApartmentApplicantDataViewModel>.query("GET_AA_APPLICANTS_DETAILS"); //run stored procedure
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
