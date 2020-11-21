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

        /// <summary>
        /// Gets student housing info
        /// TODO list what exactly we mean by houding info
        /// </summary>
        /// <returns>The housing item</returns>
        public IEnumerable<HousingViewModel> GetAll()
        {
            return RawSqlQuery<HousingViewModel>.query("GET_STU_HOUSING_INFO");
        }

        /// <summary>
        /// Saves student housing info
        /// </summary>
        /// <returns>The housing item</returns>
        public bool SaveApplication(string username, string sess_cde, string [] appIds)
        {
            IEnumerable<AA_ApartmentApplications> result = null;
            IEnumerable<AA_ApartmentApplications> result2 = null;
            IEnumerable<AA_Applicants> result3 = null;

            DateTime now = System.DateTime.Now;

            var timeParam = new SqlParameter("@NOW", now);
            var idParam = new SqlParameter("@MODIFIER_ID", username);
            var sessionParam = new SqlParameter("@SESS_CDE", sess_cde);
            //var programParam = new SqlParameter("@APRT_PROGRAM", now);

            bool returnAnswer = true;

            result = RawSqlQuery<AA_ApartmentApplications>.query("INSERT_AA_APPLICATION @NOW, @MODIFIER_ID", timeParam, idParam); //run stored procedure
            if (result == null)
            {
                returnAnswer = false;
                throw new ResourceNotFoundException() { ExceptionMessage = "The application could not be saved." };
            }

            result2 = RawSqlQuery<AA_ApartmentApplications>.query("GET_AA_APPID_BY_NAME_AND_DATE @NOW, @MODIFIER_ID", timeParam, idParam); //run stored procedure
            if (result2 == null)
            {
                returnAnswer = false;
                throw new ResourceNotFoundException() { ExceptionMessage = "The new application ID could not be found." };
            }

            foreach (string applicant in appIds){
                result3 = RawSqlQuery<AA_Applicants>.query("INSERT_AA_APPLICANT @APPLICATION_ID, @ID_NUM, @APRT_PROGRAM,  @SESS_CDE", result2, applicant, null, sessionParam); //run stored procedure
                if (result3 == null)
                {
                    returnAnswer = false; // not sure if this matters since I have heard throwing an exception "returns"
                    throw new ResourceNotFoundException() { ExceptionMessage = "Applicant with ID " + applicant + " could not be saved." };   
                }
            }

            return returnAnswer;
        }
    }
}
