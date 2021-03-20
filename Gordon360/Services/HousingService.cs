using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services.ComplexQueries;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

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

            SqlParameter idParam = new SqlParameter("@USER_ID", userId);

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

            SqlParameter idParam = new SqlParameter("@ADMIN_ID", id);

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

            SqlParameter idParam = new SqlParameter("@ADMIN_ID", id);

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

            SqlParameter idParam = new SqlParameter("@STUDENT_ID", studentId);
            SqlParameter sessionParam = new SqlParameter("@SESS_CDE", sess_cde);

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
        /// <param name="apartmentApplicants"> Array of JSON objects providing apartment applicants </param>
        /// <param name="apartmentChoices"> Array of JSON objects providing apartment hall choices </param>
        /// <returns>Returns the application ID number if all the queries succeeded</returns>
        public int SaveApplication(string editorId, string sess_cde, ApartmentApplicantViewModel[] apartmentApplicants, ApartmentChoiceViewModel[] apartmentChoices)
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

            foreach (ApartmentApplicantViewModel applicant in apartmentApplicants)
            {
                IEnumerable<AA_Applicants> applicantResult = null;

                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);
                idParam = new SqlParameter("@ID_NUM", applicant.ID);
                programParam = new SqlParameter("@APRT_PROGRAM", "");
                sessionParam = new SqlParameter("@SESS_CDE", sess_cde);

                applicantResult = RawSqlQuery<AA_Applicants>.query("INSERT_AA_APPLICANT @APPLICATION_ID, @ID_NUM, @APRT_PROGRAM, @SESS_CDE", appIdParam, idParam, programParam, sessionParam); //run stored procedure
                if (applicantResult == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "Applicant with ID " + applicant.ID + " could not be saved." };
                }
            }

            //----------------
            // Save hall information

            SqlParameter rankingParam = null;
            SqlParameter buildingCodeParam = null;

            foreach (ApartmentChoiceViewModel choice in apartmentChoices)
            {
                IEnumerable<AA_ApartmentChoices> apartmentChoiceResult = null;

                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);
                rankingParam = new SqlParameter("@RANKING", choice.HallRank);
                buildingCodeParam = new SqlParameter("@BLDG_CDE", choice.HallName);
                apartmentChoiceResult = RawSqlQuery<AA_ApartmentChoices>.query("INSERT_AA_APARTMENT_CHOICE @APPLICATION_ID, @RANKING, @BLDG_CDE", appIdParam, rankingParam, buildingCodeParam); // run stored procedure
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
        /// <param name="newApartmentApplicants"> Array of JSON objects providing apartment applicants </param>
        /// <param name="newApartmentChoices"> Array of JSON objects providing apartment hall choices </param>
        /// <returns>Returns the application ID number if all the queries succeeded</returns>
        public int EditApplication(string editorId, string sess_cde, int apartAppId, ApartmentApplicantViewModel[] newApartmentApplicants, ApartmentChoiceViewModel[] newApartmentChoices)
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

            IEnumerable<GET_AA_APPLICANTS_BY_APPID_Result> applicantsResult = null;

            appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);

            // Get the IDs of the applicants that are already stored in the database for this application
            applicantsResult = RawSqlQuery<GET_AA_APPLICANTS_BY_APPID_Result>.query("GET_AA_APPLICANTS_BY_APPID @APPLICATION_ID", appIdParam);
            if (applicantsResult == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The applicants could not be found." };
            }

            // List of applicants IDs that are in the array recieved from the frontend but not yet in the database
            List<ApartmentApplicantViewModel> applicantsToAdd = new List<ApartmentApplicantViewModel>(newApartmentApplicants);

            // List of applicants IDs that are in both the array recieved from the frontend and the database
            List<ApartmentApplicantViewModel> applicantsToUpdate = new List<ApartmentApplicantViewModel>();

            // List of applicants IDs that are in the database but not in the array recieved from the frontend
            List<ApartmentApplicantViewModel> applicantsToRemove = new List<ApartmentApplicantViewModel>();

            // Check whether any applicants were found matching the given application ID number
            if (applicantsResult.Any())
            {
                foreach (GET_AA_APPLICANTS_BY_APPID_Result applicantResult in applicantsResult)
                {
                    ApartmentApplicantViewModel existingApplicant = null;
                    existingApplicant = newApartmentApplicants.FirstOrDefault(x => x.ID == applicantResult.ID_NUM);  //.Where(x => x.ID == existingApplicantID).First();
                    if (existingApplicant != null)
                    {
                        applicantsToAdd.Remove(existingApplicant);
                        if (existingApplicant.OffCampusProgram != applicantResult.AprtProgram)
                        {
                            applicantsToUpdate.Add(existingApplicant);
                        }
                    }
                    else
                    {
                        applicantsToRemove.Add(existingApplicant);
                    }
                }
            }

            SqlParameter idParam = null;
            SqlParameter programParam = null;
            SqlParameter sessionParam = null;

            // Insert new applicants that are not yet in the database
            foreach (ApartmentApplicantViewModel applicant in applicantsToAdd)
            {
                IEnumerable<AA_Applicants> applicantResult = null;

                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);
                idParam = new SqlParameter("@ID_NUM", applicant.ID);
                programParam = new SqlParameter("@APRT_PROGRAM", "");
                sessionParam = new SqlParameter("@SESS_CDE", sess_cde);

                applicantResult = RawSqlQuery<AA_Applicants>.query("INSERT_AA_APPLICANT @APPLICATION_ID, @ID_NUM, @APRT_PROGRAM, @SESS_CDE", appIdParam, idParam, programParam, sessionParam); //run stored procedure
                if (applicantResult == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "Applicant with ID " + applicant.ID + " could not be inserted." };
                }
            }

            // Update the info of applicants from the frontend that are already in the database
            foreach (ApartmentApplicantViewModel applicant in applicantsToUpdate)
            {
                IEnumerable<AA_Applicants> applicantResult = null;

                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);
                idParam = new SqlParameter("@ID_NUM", applicant.ID);
                programParam = new SqlParameter("@APRT_PROGRAM", ""); // TODO: This will be used to update the off-campus program department once that feature has been made on the frontend
                sessionParam = new SqlParameter("@SESS_CDE", sess_cde);

                applicantResult = RawSqlQuery<AA_Applicants>.query("UPDATE_AA_APPLICANT @APPLICATION_ID, @ID_NUM, @APRT_PROGRAM, @SESS_CDE", appIdParam, idParam, programParam, sessionParam); //run stored procedure
                if (applicantResult == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "Applicant with ID " + applicant.ID + " could not be updated." };
                }
            }

            // Remove applicants from the database that were remove from the frontend
            foreach (ApartmentApplicantViewModel applicant in applicantsToRemove)
            {
                IEnumerable<AA_Applicants> applicantResult = null;

                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);
                idParam = new SqlParameter("@ID_NUM", applicant.ID);
                sessionParam = new SqlParameter("@SESS_CDE", sess_cde);

                applicantResult = RawSqlQuery<AA_Applicants>.query("DELETE_AA_APPLICANT @APPLICATION_ID, @ID_NUM, @SESS_CDE", appIdParam, idParam, sessionParam); //run stored procedure
                if (applicantResult == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "Applicant with ID " + applicant.ID + " could not be removed." };
                }
            }

            //--------
            // Update hall information

            IEnumerable<GET_AA_APARTMENT_CHOICES_BY_APP_ID_Result> existingApartmentChoiceResult = null;

            appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);

            // Get the apartment preferences that are already stored in the database for this application
            existingApartmentChoiceResult = RawSqlQuery<GET_AA_APARTMENT_CHOICES_BY_APP_ID_Result>.query("GET_AA_APARTMENT_CHOICES_BY_APP_ID @APPLICATION_ID", appIdParam);
            if (existingApartmentChoiceResult == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The hall information could not be found." };
            }

            // List of apartment choices that are in the array recieved from the frontend but not yet in the database
            List<ApartmentChoiceViewModel> apartmentChoicesToAdd = new List<ApartmentChoiceViewModel>(newApartmentChoices);

            // List of apartment choices that are in both the array recieved from the frontend and the database
            List<ApartmentChoiceViewModel> apartmentChoicesToUpdate = new List<ApartmentChoiceViewModel>();

            // List of apartment choices that are in the database but not in the array recieved from the frontend
            List<ApartmentChoiceViewModel> apartmentChoicesToRemove = new List<ApartmentChoiceViewModel>();

            // Check whether any apartment choices were found matching the given application ID number
            if (existingApartmentChoiceResult.Any())
            {
                foreach (GET_AA_APARTMENT_CHOICES_BY_APP_ID_Result existingApartmentChoiceModel in existingApartmentChoiceResult)
                {
                    ApartmentChoiceViewModel existingApartmentChoice = null;
                    existingApartmentChoice = newApartmentChoices.FirstOrDefault(x => x.HallName == existingApartmentChoiceModel.BLDG_CDE);
                    if (existingApartmentChoice != null)
                    {
                        apartmentChoicesToAdd.Remove(existingApartmentChoice);
                        apartmentChoicesToUpdate.Add(existingApartmentChoice);
                    }
                    else
                    {
                        apartmentChoicesToRemove.Add(existingApartmentChoice);
                    }
                }
            }

            SqlParameter rankingParam = null;
            SqlParameter buildingCodeParam = null;

            // Insert new apartment choices that are not yet in the database
            foreach (ApartmentChoiceViewModel apartmentChoice in apartmentChoicesToAdd)
            {
                IEnumerable<AA_ApartmentChoices> apartmentChoiceResult = null;

                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);
                rankingParam = new SqlParameter("@RANKING", apartmentChoice.HallRank);
                buildingCodeParam = new SqlParameter("@BLDG_CDE", apartmentChoice.HallName);

                apartmentChoiceResult = RawSqlQuery<AA_ApartmentChoices>.query("INSERT_AA_APARTMENT_CHOICE @APPLICATION_ID, @RANKING, @BLDG_CDE", appIdParam, rankingParam, buildingCodeParam); //run stored procedure
                if (apartmentChoiceResult == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "Apartment choice with ID " + apartAppId + " and hall name " + apartmentChoice.HallName + " could not be inserted." };
                }
            }

            // Update the info of apartment choices from the frontend that are already in the database
            foreach (ApartmentChoiceViewModel apartmentChoice in apartmentChoicesToUpdate)
            {
                IEnumerable<AA_ApartmentChoices> apartmentChoiceResult = null;

                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);
                rankingParam = new SqlParameter("@RANKING", apartmentChoice.HallRank);
                buildingCodeParam = new SqlParameter("@BLDG_CDE", apartmentChoice.HallName);

                apartmentChoiceResult = RawSqlQuery<AA_ApartmentChoices>.query("UPDATE_AA_APARTMENT_CHOICE @APPLICATION_ID, @RANKING, @BLDG_CDE", appIdParam, rankingParam, buildingCodeParam); //run stored procedure
                if (apartmentChoiceResult == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "Apartment choice with ID " + apartAppId + " and hall name " + apartmentChoice.HallName + " could not be updated." };
                }
            }

            // Remove apartment choices from the database that were remove from the frontend
            foreach (ApartmentChoiceViewModel apartmentChoice in apartmentChoicesToRemove)
            {
                IEnumerable<AA_ApartmentChoices> apartmentChoiceResult = null;

                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                appIdParam = new SqlParameter("@APPLICATION_ID", apartAppId);
                rankingParam = new SqlParameter("@RANKING", apartmentChoice.HallRank);
                buildingCodeParam = new SqlParameter("@BLDG_CDE", apartmentChoice.HallName);

                apartmentChoiceResult = RawSqlQuery<AA_ApartmentChoices>.query("DELETE_AA_APARTMENT_CHOICE @APPLICATION_ID, @BLDG_CDE", appIdParam, buildingCodeParam); //run stored procedure
                if (apartmentChoiceResult == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "Apartment choice with ID " + apartAppId + " and hall name " + apartmentChoice.HallName + " could not be removed." };
                }
            }

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
            IEnumerable<GET_AA_APPLICANTS_DETAILS_Result> result = RawSqlQuery<GET_AA_APPLICANTS_DETAILS_Result>.query("GET_AA_APPLICANTS_DETAILS"); //run stored procedure
            foreach (GET_AA_APPLICANTS_DETAILS_Result element in result)
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

        public ApartmentAppViewModel GetApartmentApplication(string sess_cde, int applicationID)
        {
            SqlParameter appIDParam = new SqlParameter("@APPLICATION_ID", applicationID);

            IEnumerable<GET_AA_APPLICATIONS_BY_ID_Result> applicationResult = RawSqlQuery<GET_AA_APPLICATIONS_BY_ID_Result>.query("GET_AA_APPLICATIONS_BY_ID @APPLICATION_ID", applicationID);
            if (applicationResult == null || !applicationResult.Any())
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The application could not be found." };
            }
            else if (applicationResult.Count() > 1)
            {
                // Somehow there was more than one application for this session code and applcation ID.... THis should not be possible
                // We will have to decide what is the best course of action in this case
            }

            GET_AA_APPLICATIONS_BY_ID_Result applicationsDBModel = applicationResult.FirstOrDefault(x => x.AprtAppID == applicationID);

            ApartmentAppViewModel apartmentApplication = new ApartmentAppViewModel();

            SqlParameter sessionParam = new SqlParameter("@SESS_CDE", sess_cde);
            // Assign the values from the database to the corresponding properties
            apartmentApplication.AprtAppID = applicationsDBModel.AprtAppID;



            return null; //Temporary while I work
        }

        public ApartmentAppViewModel[] GetAllApartmentApplication(string sess_cde)
        {
            IEnumerable<GET_AA_APPLICATIONS_Result> applicationResult = null;

            SqlParameter sessionParam = new SqlParameter("@SESS_CDE", sess_cde);

            applicationResult = RawSqlQuery<GET_AA_APPLICATIONS_Result>.query("GET_AA_APPLICATIONS @SESS_CDE", sessionParam);
            if (applicationResult == null || !applicationResult.Any())
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The application could not be found." };
            }
            return null; //Temporary while I work
        }
    }
}
