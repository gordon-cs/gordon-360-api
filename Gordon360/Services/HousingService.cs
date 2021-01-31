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

        /*
        /// <summary>
        /// Gets student housing info
        /// TODO list what exactly we mean by houding info
        /// </summary>
        /// <returns>The housing item</returns>
        public IEnumerable<HousingViewModel> GetAll()
        {
            return RawSqlQuery<HousingViewModel>.query("GET_STU_HOUSING_INFO");
        }
        */

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
        /// <param name="editor"> The student ID number of the user who is attempting to save the apartment application </param>  
        /// <param name="sess_cde"> The current session code </param>  
        /// <param name="applicantIds"> Array of student ID numbers for each of the applicants </param>  
        /// <returns>Whether or not all the queries succeeded</returns>
        public int SaveApplication(int apartAppId, string editor, string sess_cde, string [] applicantIds)
        {
            IEnumerable<ApartmentAppIDViewModel> idResult = null;

            DateTime now = System.DateTime.Now;

            var sessionParam = new SqlParameter("@SESS_CDE", sess_cde);
            var modParam = new SqlParameter("@STUDENT_ID", editor);

            int appId = -1;

            if (apartAppId == -1)
            {
                // If an application ID was not passed in, then check if an application already exists
                idResult = RawSqlQuery<ApartmentAppIDViewModel>.query("GET_AA_APPID_BY_STU_ID_AND_SESS @SESS_CDE, @STUDENT_ID", sessionParam, modParam); //run stored procedure
                if (idResult == null || !idResult.Any())
                {
                    IEnumerable<ApartmentAppSaveViewModel> newAppResult = null;

                    // All SqlParameters must be remade before being reused in an SQL Query to prevent errors
                    var timeParam = new SqlParameter("@NOW", now);
                    modParam = new SqlParameter("@MODIFIER_ID", editor);

                    // If an existing application was not found for this modifier, then insert a new application entry in the database
                    newAppResult = RawSqlQuery<ApartmentAppSaveViewModel>.query("INSERT_AA_APPLICATION @NOW, @MODIFIER_ID", timeParam, modParam); //run stored procedure
                    if (newAppResult == null)
                    {
                        throw new ResourceNotFoundException() { ExceptionMessage = "The application could not be saved." };
                    }

                    // The following is ODD, I know. It seems you cannot execute the same query with the same sql parameters twice.
                    // Thus, these two sql params must be recreated after being used in the last query:

                    // All SqlParameters must be remade before each SQL Query to prevent errors
                    timeParam = new SqlParameter("@NOW", now);
                    modParam = new SqlParameter("@MODIFIER_ID", editor);

                    idResult = RawSqlQuery<ApartmentAppIDViewModel>.query("GET_AA_APPID_BY_NAME_AND_DATE @NOW, @MODIFIER_ID", timeParam, modParam); //run stored procedure
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

        // Sudo code:
        // Call a stored procedure that gets all data from all apartmentapp tables
        // - Get the first element in the query result (This will be a view model)
        // - Then, 
        // foreach element in result {
        // csv += element.User_ID
        // + ","
        // /...
        // csv += "\n"
        // 1,50197937,0,0
        // 1,5027658,0,0
        // 1,5078654,0,0

        /// <summary>
        /// Exports the database table into a CSV file and allow the user to save it locally.
        /// </summary>
        public string CreateCSV()
        {
            string csv = string.Empty;
            var result = RawSqlQuery<ApartmentApplicationsTableViewModel>.query("GET_AA_APPLICATIONS"); //run stored procedure
            foreach (var element in result)
            {
                csv += element.AprtAppID + ","
                     + element.DateSubmitted + ","
                     + element.DateModified.ToString("MM/dd/yyyy HH:mm:ss") + ","
                     + element.EditorID;
                const string V = "\r\n";
                csv += V;
            }

            return csv;
        }
    }
}
