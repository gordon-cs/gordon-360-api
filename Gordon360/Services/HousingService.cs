using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Exceptions.CustomExceptions;
using System.Data.SqlClient;
using Gordon360.Services.ComplexQueries;
using System.Diagnostics;

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
        ///       - second, it looks for the application id of the application with the information we just input (because 
        ///       the database creates the application ID so we have to ask it which number it generated for it)
        ///     - else, it saves the application ID of that existing application to the variable "appId"
        /// - third, it inserts each applicant into the applicnts table along with the apartment ID so we know
        /// which application on which they are an applicant
        ///  
        /// </summary>
        /// <param name="apartAppId"> The application ID number of the application to be edited </param>  
        /// <param name="modifierId"> The student ID number of the user who is attempting to save the apartment application </param>  
        /// <param name="sess_cde"> The current session code </param>  
        /// <param name="applicantIds"> Array of student ID numbers for each of the applicants </param>  
        /// <returns>Whether or not all the queries succeeded</returns>
        public int SaveApplication(int apartAppId, string modifierId, string sess_cde, string [] applicantIds)
        {
            IEnumerable<ApartmentAppSaveViewModel> result = null;
            IEnumerable<ApartmentAppIDViewModel> idResult = null;
            IEnumerable<ApartmentApplicantViewModel> result2 = null;

            DateTime now = System.DateTime.Now;

            var timeParam = new SqlParameter("@NOW", now);
            var modParam = new SqlParameter("@MODIFIER_ID", modifierId);
            var sessionParam = new SqlParameter("@SESS_CDE", sess_cde);

            int appId = -1;

            if (apartAppId == -1)
            {
                // If an application ID was not passed in, then check if an application already exists
                idResult = RawSqlQuery<ApartmentAppIDViewModel>.query("GET_AA_APPID_BY_STU_ID_AND_SESS @SESS_CDE, @STUDENT_ID", sessionParam, modParam); //run stored procedure
                if (idResult == null)
                {
                    // If an existing application was not found for this modifier, then insert a new application entry in the database
                    result = RawSqlQuery<ApartmentAppSaveViewModel>.query("INSERT_AA_APPLICATION @NOW, @MODIFIER_ID", timeParam, modParam); //run stored procedure
                    if (result == null)
                    {
                        throw new ResourceNotFoundException() { ExceptionMessage = "The application could not be saved." };
                    }

                    // The following is ODD, I know. It seems you cannot execute the same query with the same sql parameters twice.
                    // Thus, these two sql params must be recreated after being used in the last query:

                    timeParam = new SqlParameter("@NOW", now);
                    modParam = new SqlParameter("@MODIFIER_ID", modifierId);

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

            foreach (string id in applicantIds) {
                // this constructs new SqlParameters each time we iterate (despite that only 1/4 will actually be different
                // on subsequent iterations. See above explanation of this ODD strategy.

                //idParam.Value = id; might need if this ODD solution is not satisfactory
                appIdParam = new SqlParameter("@APPLICATION_ID", appId);
                idParam = new SqlParameter("@ID_NUM", id);
                programParam = new SqlParameter("@APRT_PROGRAM", "");
                sessionParam = new SqlParameter("@SESS_CDE", sess_cde);

                result2 = RawSqlQuery<ApartmentApplicantViewModel>.query("INSERT_AA_APPLICANT @APPLICATION_ID, @ID_NUM, @APRT_PROGRAM, @SESS_CDE", appIdParam, idParam, programParam, sessionParam); //run stored procedure
                if (result2 == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "Applicant with ID " + id + " could not be saved." };   
                }
            }

            return appId;
        }
    }
}
