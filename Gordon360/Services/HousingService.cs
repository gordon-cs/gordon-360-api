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
        private readonly IUnitOfWork _unitOfWork;

        private CCTEntities1 _context;

        public HousingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            _context = new CCTEntities1();
        }

        /// <summary>
        /// Calls a stored procedure that returns a row in the staff whitelist which has the given user id,
        /// if it is in the whitelist
        /// </summary>
        /// <param name="userID"> The id of the person using the page </param>
        /// <returns> Whether or not the user is on the staff whitelist </returns>
        public bool CheckIfHousingAdmin(string userID)
        {
            IEnumerable<HousingAdminViewModel> idResult = null;

            SqlParameter idParam = new SqlParameter("@USER_ID", userID);

            idResult = RawSqlQuery<HousingAdminViewModel>.query("GET_AA_ADMIN @USER_ID", idParam);
            if (idResult == null || !idResult.Any())
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
            IEnumerable<HousingAdminViewModel> idResult = null;

            SqlParameter idParam = new SqlParameter("@ADMIN_ID", id);

            idResult = RawSqlQuery<HousingAdminViewModel>.query("INSERT_AA_ADMIN @ADMIN_ID", idParam);
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
            IEnumerable<HousingAdminViewModel> idResult = null;

            SqlParameter idParam = new SqlParameter("@ADMIN_ID", id);

            idResult = RawSqlQuery<HousingAdminViewModel>.query("DELETE_AA_ADMIN @ADMIN_ID", idParam);
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
        /// Calls a stored procedure that gets all names of apartment halls
        /// </summary>
        /// <returns> AN array of hall names </returns>
        public AA_ApartmentHalls[] GetAllApartmentHalls()
        {
            IEnumerable<AA_ApartmentHalls> hallsResult = null;

            hallsResult = RawSqlQuery<AA_ApartmentHalls>.query("GET_AA_APARTMENT_HALLS");
            if (hallsResult == null || !hallsResult.Any())
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The apartment halls could not be found." };
            }

            return hallsResult.ToArray();
        }

        /// <summary>
        /// Calls a stored procedure that tries to get the id of an the application that a given user is 
        /// applicant on for a given session
        /// </summary>
        /// <param name="username"> The student username to look for </param>
        /// <param name="sess_cde"> Session for which the application would be </param>
        /// <returns> 
        /// The id of the application or 
        /// null if the user is not on an application for that session 
        /// </returns>
        public int? GetApplicationID(string username, string sess_cde)
        {
            IEnumerable<ApartmentAppIDViewModel> idResult = null;

            SqlParameter userParam = new SqlParameter("@STUDENT_USERNAME", username);
            SqlParameter sessionParam = new SqlParameter("@SESS_CDE", sess_cde);

            idResult = RawSqlQuery<ApartmentAppIDViewModel>.query("GET_AA_APPID_BY_STU_ID_AND_SESS @SESS_CDE, @STUDENT_USERNAME", sessionParam, userParam); //run stored procedure
            if (idResult == null || !idResult.Any())
            {
                return null;
            }

            int result = idResult.FirstOrDefault().AprtAppID;

            return result;
        }

        /// <summary>
        /// Saves student housing info
        /// - first, it creates a new row in the applications table and inserts the username of the primary applicant and a timestamp
        /// - second, it retrieves the application id of the application with the information we just inserted (because
        /// the database creates the application ID so we have to ask it which number it generated for it)
        /// - third, it inserts each applicant into the applicants table along with the application ID so we know
        /// which application on which they are an applicant
        ///
        /// </summary>
        /// <param name="username"> The student username of the user who is attempting to save the apartment application (retrieved via authentication token) </param>
        /// <param name="sess_cde"> The current session code </param>
        /// <param name="editorUsername"> The student username of the student who is declared to be the editor of this application (retrieved from the JSON from the front end) </param>
        /// <param name="apartmentApplicants"> Array of JSON objects providing apartment applicants </param>
        /// <param name="apartmentChoices"> Array of JSON objects providing apartment hall choices </param>
        /// <returns>Returns the application ID number if all the queries succeeded</returns>
        public int SaveApplication(string username, string sess_cde, string editorUsername, ApartmentApplicantViewModel[] apartmentApplicants, ApartmentChoiceViewModel[] apartmentChoices)
        {
            IEnumerable<ApartmentAppIDViewModel> idResult = null;

            DateTime now = System.DateTime.Now;

            SqlParameter sessionParam = new SqlParameter("@SESS_CDE", sess_cde);
            SqlParameter editorParam = new SqlParameter("@STUDENT_USERNAME", editorUsername);

            // If an application ID was not passed in, then check if an application already exists
            idResult = RawSqlQuery<ApartmentAppIDViewModel>.query("GET_AA_APPID_BY_STU_ID_AND_SESS @SESS_CDE, @STUDENT_USERNAME", sessionParam, editorParam); //run stored procedure
            if (idResult != null && idResult.Any())
            {
                throw new ResourceCreationException() { ExceptionMessage = "An existing application ID was found for this user. Please use 'EditApplication' to update an existing application." };
            }

            //----------------
            // Save the application editor and time

            // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
            SqlParameter timeParam = new SqlParameter("@NOW", now);
            editorParam = new SqlParameter("@EDITOR_USERNAME", editorUsername);

            // If an existing application was not found for this editor, then insert a new application entry in the database
            int? newAppResult = _context.Database.ExecuteSqlCommand("INSERT_AA_APPLICATION @NOW, @EDITOR_USERNAME", timeParam, editorParam); //run stored procedure
            if (newAppResult == null)
            {
                throw new ResourceCreationException() { ExceptionMessage = "The application could not be saved." };
            }

            //----------------
            // Retrieve the application ID number of this new application

            // The following is ODD, I know. It seems you cannot execute the same query with the same sql parameters twice.
            // Thus, these two sql params must be recreated after being used in the last query:

            idResult = null;

            // All SqlParameters must be remade before each SQL Query to prevent errors
            timeParam = new SqlParameter("@NOW", now);
            editorParam = new SqlParameter("@EDITOR_USERNAME", editorUsername);

            idResult = RawSqlQuery<ApartmentAppIDViewModel>.query("GET_AA_APPID_BY_NAME_AND_DATE @NOW, @EDITOR_USERNAME", timeParam, editorParam); //run stored procedure
            if (idResult == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The new application ID could not be found." };
            }

            int applicationID = idResult.FirstOrDefault().AprtAppID;

            //----------------
            // Save applicant information

            SqlParameter appIDParam = null;
            SqlParameter userParam = null;
            SqlParameter programParam = null;

            foreach (ApartmentApplicantViewModel applicant in apartmentApplicants)
            {
                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                appIDParam = new SqlParameter("@APPLICATION_ID", applicationID);
                userParam = new SqlParameter("@USERNAME", applicant.Username);
                programParam = new SqlParameter("@APRT_PROGRAM", applicant.OffCampusProgram ?? "");
                sessionParam = new SqlParameter("@SESS_CDE", sess_cde);

                int? applicantResult = _context.Database.ExecuteSqlCommand("INSERT_AA_APPLICANT @APPLICATION_ID, @USERNAME, @APRT_PROGRAM, @SESS_CDE", appIDParam, userParam, programParam, sessionParam); //run stored procedure
                if (applicantResult == null)
                {
                    throw new ResourceCreationException() { ExceptionMessage = "Applicant " + applicant.Username + " could not be saved." };
                }
            }

            //----------------
            // Save hall information

            SqlParameter rankingParam = null;
            SqlParameter buildingCodeParam = null;

            foreach (ApartmentChoiceViewModel choice in apartmentChoices)
            {
                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                appIDParam = new SqlParameter("@APPLICATION_ID", applicationID);
                rankingParam = new SqlParameter("@RANKING", choice.HallRank);
                buildingCodeParam = new SqlParameter("@HALL_NAME", choice.HallName);
                int? apartmentChoiceResult = _context.Database.ExecuteSqlCommand("INSERT_AA_APARTMENT_CHOICE @APPLICATION_ID, @RANKING, @HALL_NAME", appIDParam, rankingParam, buildingCodeParam); // run stored procedure
                if (apartmentChoiceResult == null)
                {
                    throw new ResourceCreationException() { ExceptionMessage = "The apartment preference could not be saved." };
                }
            }

            return applicationID;
        }

        /// <summary>
        /// Edit an existings apartment application
        /// - first, it gets the EditorUsername from the database for the given application ID and makes sure that the student username of the current user matches that stored username
        /// - second, it gets an array of the applicants that are already stored in the database for the given application ID
        /// - third, it inserts each applicant that is in the 'newApplicantIDs' array but was not yet in the database
        /// - fourth, it removes each applicant that was stored in the database but was not in the 'newApplicantIDs' array
        ///
        /// </summary>
        /// <param name="username"> The student username of the user who is attempting to save the apartment application (retrieved via authentication token) </param>
        /// <param name="sess_cde"> The current session code </param>
        /// <param name="applicationID"> The application ID number of the application to be edited </param>
        /// <param name="newEditorUsername"> The student username of the student who is declared to be the editor of this application (retrieved from the JSON from the front end) </param>
        /// <param name="newApartmentApplicants"> Array of JSON objects providing apartment applicants </param>
        /// <param name="newApartmentChoices"> Array of JSON objects providing apartment hall choices </param>
        /// <returns>Returns the application ID number if all the queries succeeded</returns>
        public int EditApplication(string username, string sess_cde, int applicationID, string newEditorUsername, ApartmentApplicantViewModel[] newApartmentApplicants, ApartmentChoiceViewModel[] newApartmentChoices)
        {
            IEnumerable<string> editorResult = null;

            SqlParameter appIDParam = new SqlParameter("@APPLICATION_ID", applicationID);

            editorResult = RawSqlQuery<string>.query("GET_AA_EDITOR_BY_APPID @APPLICATION_ID", appIDParam);
            if (editorResult == null || !editorResult.Any())
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The application could not be found." };
            }

            string storedEditorUsername = editorResult.FirstOrDefault();

            if (username != storedEditorUsername)
            {
                // This should already be caught by the StateYourBusiness, but I will leave this check here just in case
                throw new Exceptions.CustomExceptions.UnauthorizedAccessException() { ExceptionMessage = "The current user does not match the stored editor of this application" };
            }
            // Only perform the update if the username of the current user matched the 'EditorUsername' stored in the database for the requested application


            //--------
            // Update applicant information

            IEnumerable<GET_AA_APPLICANTS_BY_APPID_Result> existingApplicantResult = null;

            appIDParam = new SqlParameter("@APPLICATION_ID", applicationID);

            // Get the IDs of the applicants that are already stored in the database for this application
            existingApplicantResult = RawSqlQuery<GET_AA_APPLICANTS_BY_APPID_Result>.query("GET_AA_APPLICANTS_BY_APPID @APPLICATION_ID", appIDParam);
            if (existingApplicantResult == null)
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
            if (existingApplicantResult.Any())
            {
                foreach (GET_AA_APPLICANTS_BY_APPID_Result existingApplicant in existingApplicantResult)
                {
                    ApartmentApplicantViewModel newMatchingApplicant = null;
                    newMatchingApplicant = newApartmentApplicants.FirstOrDefault(x => x.Username == existingApplicant.Username);
                    if (newMatchingApplicant != null)
                    {
                        // If the applicant is in both the new applicant list and the existing applicant list, then we do NOT need to add it to the database
                        applicantsToAdd.Remove(newMatchingApplicant);
                        if (newMatchingApplicant.OffCampusProgram != existingApplicant.AprtProgram)
                        {
                            // If the applicant is in both the new and existing applicant lists but has different OffCampusProgram values, then we need to update that in the database
                            applicantsToUpdate.Add(newMatchingApplicant);
                        }
                    }
                    else
                    {
                        ApartmentApplicantViewModel nonMatchingApplicant = new ApartmentApplicantViewModel
                        {
                            ApplicationID = existingApplicant.AprtAppID,
                            Username = existingApplicant.Username, // Code for after we remade the AA_Applicants table
                        };
                        // If the applicant is in the existing list but not in the new list of applicants, then we need to remove it from the database
                        applicantsToRemove.Add(nonMatchingApplicant);
                    }
                }
            }

            SqlParameter userParam = null;
            SqlParameter programParam = null;
            SqlParameter sessionParam = null;

            // Insert new applicants that are not yet in the database
            foreach (ApartmentApplicantViewModel applicant in applicantsToAdd)
            {
                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                appIDParam = new SqlParameter("@APPLICATION_ID", applicationID);
                userParam = new SqlParameter("@USERNAME", applicant.Username);
                programParam = new SqlParameter("@APRT_PROGRAM", applicant.OffCampusProgram ?? "");
                sessionParam = new SqlParameter("@SESS_CDE", sess_cde);

                int? applicantResult = _context.Database.ExecuteSqlCommand("INSERT_AA_APPLICANT @APPLICATION_ID, @USERNAME, @APRT_PROGRAM, @SESS_CDE", appIDParam, userParam, programParam, sessionParam); //run stored procedure
                if (applicantResult == null)
                {
                    throw new ResourceCreationException() { ExceptionMessage = "Applicant " + applicant.Username + " could not be inserted." };
                }
            }

            // Update the info of applicants from the frontend that are already in the database
            foreach (ApartmentApplicantViewModel applicant in applicantsToUpdate)
            {
                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                appIDParam = new SqlParameter("@APPLICATION_ID", applicationID);
                userParam = new SqlParameter("@USERNAME", applicant.Username);
                programParam = new SqlParameter("@APRT_PROGRAM", applicant.OffCampusProgram ?? "");
                sessionParam = new SqlParameter("@SESS_CDE", sess_cde);

                int? applicantResult = _context.Database.ExecuteSqlCommand("UPDATE_AA_APPLICANT @APPLICATION_ID, @USERNAME, @APRT_PROGRAM, @SESS_CDE", appIDParam, userParam, programParam, sessionParam); //run stored procedure
                if (applicantResult == null)
                {
                    throw new ResourceCreationException() { ExceptionMessage = "Applicant " + applicant.Username + " could not be updated." };
                }
            }

            // Remove applicants from the database that were remove from the frontend
            foreach (ApartmentApplicantViewModel applicant in applicantsToRemove)
            {
                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                appIDParam = new SqlParameter("@APPLICATION_ID", applicationID);
                userParam = new SqlParameter("@USERNAME", applicant.Username);
                sessionParam = new SqlParameter("@SESS_CDE", sess_cde);

                int? applicantResult = _context.Database.ExecuteSqlCommand("DELETE_AA_APPLICANT @APPLICATION_ID, @USERNAME, @SESS_CDE", appIDParam, userParam, sessionParam); //run stored procedure
                if (applicantResult == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "Applicant " + applicant.Username + " could not be removed." };
                }
            }

            //--------
            // Update hall information

            IEnumerable<GET_AA_APARTMENT_CHOICES_BY_APP_ID_Result> existingApartmentChoiceResult = null;

            appIDParam = new SqlParameter("@APPLICATION_ID", applicationID);

            // Get the apartment preferences that are already stored in the database for this application
            existingApartmentChoiceResult = RawSqlQuery<GET_AA_APARTMENT_CHOICES_BY_APP_ID_Result>.query("GET_AA_APARTMENT_CHOICES_BY_APP_ID @APPLICATION_ID", appIDParam);
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
                foreach (GET_AA_APARTMENT_CHOICES_BY_APP_ID_Result existingApartmentChoice in existingApartmentChoiceResult)
                {
                    ApartmentChoiceViewModel newMatchingApartmentChoice = null;
                    newMatchingApartmentChoice = newApartmentChoices.FirstOrDefault(x => x.HallName == existingApartmentChoice.HallName);
                    if (newMatchingApartmentChoice != null)
                    {
                        // If the apartment is in both the new apartment list and the existing apartment list, then we do NOT need to add it to the database
                        apartmentChoicesToAdd.Remove(newMatchingApartmentChoice);
                        if (newMatchingApartmentChoice.HallRank != existingApartmentChoice.Ranking)
                        {
                            // If the apartment is in both the new and existing apartment lists but has different ranking values, then we need to update that in the database
                            apartmentChoicesToUpdate.Add(newMatchingApartmentChoice);
                        }
                    }
                    else
                    {
                        ApartmentChoiceViewModel nonMatchingApartmentChoice = new ApartmentChoiceViewModel
                        {
                            ApplicationID = existingApartmentChoice.AprtAppID,
                            HallRank = existingApartmentChoice.Ranking,
                            HallName = existingApartmentChoice.HallName,
                        };
                        // If the apartment is in the existing list but not in the new list of apartments, then we need to remove it from the database
                        apartmentChoicesToRemove.Add(nonMatchingApartmentChoice);
                    }
                }
            }

            SqlParameter rankingParam = null;
            SqlParameter buildingCodeParam = null;

            // Insert new apartment choices that are not yet in the database
            foreach (ApartmentChoiceViewModel apartmentChoice in apartmentChoicesToAdd)
            {
                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                appIDParam = new SqlParameter("@APPLICATION_ID", applicationID);
                rankingParam = new SqlParameter("@RANKING", apartmentChoice.HallRank);
                buildingCodeParam = new SqlParameter("@HALL_NAME", apartmentChoice.HallName);

                int? apartmentChoiceResult = _context.Database.ExecuteSqlCommand("INSERT_AA_APARTMENT_CHOICE @APPLICATION_ID, @RANKING, @HALL_NAME", appIDParam, rankingParam, buildingCodeParam); //run stored procedure
                if (apartmentChoiceResult == null)
                {
                    throw new ResourceCreationException() { ExceptionMessage = "Apartment choice with ID " + applicationID + " and hall name " + apartmentChoice.HallName + " could not be inserted." };
                }
            }

            // Update the info of apartment choices from the frontend that are already in the database
            foreach (ApartmentChoiceViewModel apartmentChoice in apartmentChoicesToUpdate)
            {
                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                appIDParam = new SqlParameter("@APPLICATION_ID", applicationID);
                rankingParam = new SqlParameter("@RANKING", apartmentChoice.HallRank);
                buildingCodeParam = new SqlParameter("@HALL_NAME", apartmentChoice.HallName);

                int? apartmentChoiceResult = _context.Database.ExecuteSqlCommand("UPDATE_AA_APARTMENT_CHOICE @APPLICATION_ID, @RANKING, @HALL_NAME", appIDParam, rankingParam, buildingCodeParam); //run stored procedure
                if (apartmentChoiceResult == null)
                {
                    throw new ResourceCreationException() { ExceptionMessage = "Apartment choice with ID " + applicationID + " and hall name " + apartmentChoice.HallName + " could not be updated." };
                }
            }

            // Remove apartment choices from the database that were removed from the frontend
            foreach (ApartmentChoiceViewModel apartmentChoice in apartmentChoicesToRemove)
            {
                // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                appIDParam = new SqlParameter("@APPLICATION_ID", applicationID);
                buildingCodeParam = new SqlParameter("@HALL_NAME", apartmentChoice.HallName);

                int? apartmentChoiceResult = _context.Database.ExecuteSqlCommand("DELETE_AA_APARTMENT_CHOICE @APPLICATION_ID, @HALL_NAME", appIDParam, buildingCodeParam); //run stored procedure
                if (apartmentChoiceResult == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "Apartment choice with ID " + applicationID + " and hall name " + apartmentChoice.HallName + " could not be removed." };
                }
            }

            //--------
            // Update the date modified (and application editor if necessary)

            DateTime now = System.DateTime.Now;

            appIDParam = new SqlParameter("@APPLICATION_ID", applicationID);

            SqlParameter timeParam = new SqlParameter("@NOW", now);
            if (newEditorUsername != storedEditorUsername)
            {
                SqlParameter editorParam = new SqlParameter("@EDITOR_USERNAME", username);
                SqlParameter newEditorParam = new SqlParameter("@NEW_EDITOR_USERNAME", newEditorUsername);
                int? result = _context.Database.ExecuteSqlCommand("UPDATE_AA_APPLICATION_EDITOR @APPLICATION_ID, @EDITOR_USERNAME, @NOW, @NEW_EDITOR_USERNAME", appIDParam, editorParam, timeParam, newEditorParam); //run stored procedure
                if (result == null)
                {
                    throw new ResourceCreationException() { ExceptionMessage = "The application could not be updated." };
                }
            }
            else
            {
                int? result = _context.Database.ExecuteSqlCommand("UPDATE_AA_APPLICATION_DATEMODIFIED @APPLICATION_ID, @NOW", appIDParam, timeParam); //run stored procedure
                if (result == null)
                {
                    throw new ResourceCreationException() { ExceptionMessage = "The application DateModified could not be updated." };
                }
            }

            return applicationID;
        }

        /// <summary>
        /// Changes the student user who has permission to edit the given application
        ///
        /// </summary>
        /// <returns>Whether or not all the queries succeeded</returns>
        public bool ChangeApplicationEditor(string username, int applicationID, string newEditorUsername)
        {
            IEnumerable<string> editorResult = null;

            SqlParameter appIDParam = new SqlParameter("@APPLICATION_ID", applicationID);

            editorResult = RawSqlQuery<string>.query("GET_AA_EDITOR_BY_APPID @APPLICATION_ID", appIDParam);
            if (editorResult == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The application could not be found." };
            }
            else if (!editorResult.Any())
            {
                return false;
            }

            string storedEditorUsername = editorResult.FirstOrDefault();

            if (username != storedEditorUsername)
            {
                // Throw an error if the current user does not match this application's editor stored in the database
                throw new Exceptions.CustomExceptions.UnauthorizedAccessException() { ExceptionMessage = "The current user does not match the stored editor of this application" };

            }
            // Only perform the update if the username of the current user matched the 'EditorUsername' username stored in the database for the requested application

            DateTime now = System.DateTime.Now;

            appIDParam = new SqlParameter("@APPLICATION_ID", applicationID);
            SqlParameter timeParam = new SqlParameter("@NOW", now);
            if (newEditorUsername != storedEditorUsername)
            {
                SqlParameter editorParam = new SqlParameter("@EDITOR_USERNAME", username);
                SqlParameter newEditorParam = new SqlParameter("@NEW_EDITOR_USERNAME", newEditorUsername);
                int? result = _context.Database.ExecuteSqlCommand("UPDATE_AA_APPLICATION_EDITOR @APPLICATION_ID, @EDITOR_USERNAME, @NOW, @NEW_EDITOR_USERNAME", appIDParam, editorParam, timeParam, newEditorParam); //run stored procedure
                if (result == null)
                {
                    throw new ResourceCreationException() { ExceptionMessage = "The application could not be updated." };
                }
            }
            else
            {
                int? result = _context.Database.ExecuteSqlCommand("UPDATE_AA_APPLICATION_DATEMODIFIED @APPLICATION_ID, @NOW", appIDParam, timeParam); //run stored procedure
                if (result == null)
                {
                    throw new ResourceCreationException() { ExceptionMessage = "The application DateModified could not be updated." };
                }
            }
            return true;
        }

        /// <param name="applicationID">application ID number of the apartment application</param>
        /// <param name="isAdmin">boolean indicating whether the current user is an admin, permits access to restricted information such as birth date</param>
        /// <returns>Object of type ApartmentApplicationViewModel</returns>
        public ApartmentApplicationViewModel GetApartmentApplication(int applicationID, bool isAdmin = false)
        {
            SqlParameter appIDParam = new SqlParameter("@APPLICATION_ID", applicationID);

            IEnumerable<GET_AA_APPLICATIONS_BY_ID_Result> applicationResult = RawSqlQuery<GET_AA_APPLICATIONS_BY_ID_Result>.query("GET_AA_APPLICATIONS_BY_ID @APPLICATION_ID", appIDParam);
            if (applicationResult == null || !applicationResult.Any())
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The application could not be found." };
            }
            else if (applicationResult.Count() > 1)
            {
                // Somehow there was more than one application for this session code and applcation ID.... THis should not be possible
                // We will have to decide what is the best course of action in this case
            }

            GET_AA_APPLICATIONS_BY_ID_Result applicationDBModel = applicationResult.FirstOrDefault(x => x.AprtAppID == applicationID);

            // Assign the values from the database to the custom view model for the frontend
            ApartmentApplicationViewModel apartmentApplicationModel = applicationDBModel; //implicit conversion

            if (apartmentApplicationModel.EditorProfile == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The student information about the editor of this application could not be found." };
            }

            // Get the applicants that match this application ID
            appIDParam = new SqlParameter("@APPLICATION_ID", applicationID);
            IEnumerable<GET_AA_APPLICANTS_BY_APPID_Result> applicantsResult = RawSqlQuery<GET_AA_APPLICANTS_BY_APPID_Result>.query("GET_AA_APPLICANTS_BY_APPID @APPLICATION_ID", appIDParam);
            if (applicantsResult != null && applicantsResult.Any())
            {
                // Only attempt to parse the data if the collection of applicants is not empty
                // It is possible for a valid saved application to not contain any applicants yet, so we do not want to throw an error in the case where no applicants were found
                List<ApartmentApplicantViewModel> applicantsList = new List<ApartmentApplicantViewModel>();
                foreach (GET_AA_APPLICANTS_BY_APPID_Result applicantDBModel in applicantsResult)
                {
                    ApartmentApplicantViewModel applicantModel = applicantDBModel; //implicit conversion

                    // If the student information is found, create a new ApplicationViewModel and fill in its properties
                    if (applicantModel.Profile != null && applicantDBModel.AprtAppID == applicationID)
                    {
                        if (isAdmin) // if the current user is a housing admin or super admin 
                        {
                            // Only add the birthdate, probabtion, and points if the user is authorized to view that information
                            applicantModel.BirthDate = new UnitOfWork().AccountRepository.FirstOrDefault(x => x.AD_Username.ToLower() == applicantDBModel.Username.ToLower()).Birth_Date;

                            // The probation data is already in the database, we just need to write a stored procedure to get it
                            // applicantModel.Probation = ... // TBD

                            // Calculate application points
                            int points = 0;

                            if (!String.IsNullOrEmpty(applicantModel.Class))
                            {
                                points += Int32.Parse(applicantModel.Class);
                            }

                            if (applicantModel.Age >= 23)
                            {
                                points += 1;
                            }

                            if (!String.IsNullOrEmpty(applicantModel.OffCampusProgram))
                            {
                                points += 1;
                            }

                            if (applicantModel.Probation)
                            {
                                points -= 3;
                            }

                            applicantModel.Points = Math.Max(0, points); ; // Set the resulting points to zero if the sum gave a value less than zero
                        }

                        // Add this new ApplicantViewModel object to the list of applicants for this application
                        applicantsList.Add(applicantModel);
                    }
                }

                if (applicantsList.Any())
                {
                    // Add this list of applicants to the application model as an array
                    apartmentApplicationModel.Applicants = applicantsList.OrderBy(x => x.Username).ToArray();
                }
            }

            // Get the apartment choices that match this application ID
            appIDParam = new SqlParameter("@APPLICATION_ID", applicationID);
            IEnumerable<GET_AA_APARTMENT_CHOICES_BY_APP_ID_Result> apartmentChoicesResult = RawSqlQuery<GET_AA_APARTMENT_CHOICES_BY_APP_ID_Result>.query("GET_AA_APARTMENT_CHOICES_BY_APP_ID @APPLICATION_ID", appIDParam);
            if (apartmentChoicesResult != null && apartmentChoicesResult.Any())
            {
                // Only attempt to parse the data if the collection of apartment choices is not empty
                // It is possible for a valid saved application to not contain any apartment choices yet, so we do not want to throw an error in the case where no apartment choices were found
                List<ApartmentChoiceViewModel> apartmentChoicesList = new List<ApartmentChoiceViewModel>();
                foreach (GET_AA_APARTMENT_CHOICES_BY_APP_ID_Result apartmentChoiceDBModel in apartmentChoicesResult)
                {
                    ApartmentChoiceViewModel apartmentChoiceModel = apartmentChoiceDBModel; //implicit conversion

                    // Add this new ApartmentChoiceModel object to the list of apartment choices for this application
                    apartmentChoicesList.Add(apartmentChoiceModel);
                }

                if (apartmentChoicesList.Any())
                {
                    // Sort the apartment choices by their ranking number and Add this list of apartment choices to the application model as an array
                    apartmentApplicationModel.ApartmentChoices = apartmentChoicesList.OrderBy(x => x.HallRank).ThenBy(x => x.HallName).ToArray();
                }
            }

            return apartmentApplicationModel;
        }

        /// <returns>Array of ApartmentApplicationViewModel Objects</returns>
        public ApartmentApplicationViewModel[] GetAllApartmentApplication()
        {
            IEnumerable<ApartmentAppIDViewModel> appIDsResult = null;

            appIDsResult = RawSqlQuery<ApartmentAppIDViewModel>.query("GET_AA_CURRENT_APP_IDS");
            if (appIDsResult == null || !appIDsResult.Any())
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The application could not be found." };
            }

            List<ApartmentApplicationViewModel> applicationList = new List<ApartmentApplicationViewModel>();
            foreach (ApartmentAppIDViewModel appIDModel in appIDsResult)
            {
                ApartmentApplicationViewModel apartmentApplicationModel = null;
                try
                {
                    apartmentApplicationModel = GetApartmentApplication(appIDModel.AprtAppID, true);
                    if (apartmentApplicationModel != null)
                    {
                        applicationList.Add(apartmentApplicationModel);
                    }
                }
                catch
                {
                    // Do nothing, simply skip this application
                    // TODO: condsider how to report this error for the case where the application exists but something went wrong getting the editor student info (See the GetApartmentApplication method for what I am referring to)
                    // We want to report the error, but not stop the entire process. It is better to have some or most applications rather than none of them
                }
            }

            ApartmentApplicationViewModel[] apartmentApplicationArray = null;
            if (applicationList.Any())
            {
                apartmentApplicationArray = applicationList.ToArray();
            }
            return apartmentApplicationArray;
        }
    }
}
