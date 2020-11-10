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
        public IEnumerable<HousingAppToSubmitViewModel>SaveApplication(string username, string sess_cde, string [] appIds)
        {
           //return RawSqlQuery<HousingViewModel>.query("INSERT_AA_APPLICATION", id);
            IEnumerable<HousingAppToSubmitViewModel> result = null;
            IEnumerable<HousingViewModel> result2 = null;
            IEnumerable<HousingAppToSubmitViewModel> result3 = null;


            DateTime now = System.DateTime.Now;

            var timeParam = new SqlParameter("@NOW", username);
            var idParam = new SqlParameter("@MODIFIER_ID", now);
            var sessionParam = new SqlParameter("@SESS_CDE", sess_cde);
            //var programParam = new SqlParameter("@APRT_PROGRAM", now);

            result = RawSqlQuery<HousingAppToSubmitViewModel>.query("INSERT_AA_APPLICATION @NOW, @MODIFIER_ID", timeParam, idParam); //run stored procedure
            result2 = RawSqlQuery<HousingViewModel>.query("GET_AA_APPID_BY_NAME_AND_DATE @NOW, @MODIFIER_ID", timeParam, idParam); //run stored procedure
            foreach(string applicant in appIds){
                result3 = RawSqlQuery<HousingAppToSubmitViewModel>.query("INSERT_AA_APPLICANT @APPLICATION_ID, @ID_NUM, @APRT_PROGRAM,  @SESS_CDE", result2, applicant, null, sessionParam); //run stored procedure
            }


            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            return result;
        }
    }
}
