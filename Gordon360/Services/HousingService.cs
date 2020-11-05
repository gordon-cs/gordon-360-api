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
        public IEnumerable<HousingAppToSubmitViewModel>SaveApplication(int id)
        {
           //return RawSqlQuery<HousingViewModel>.query("INSERT_AA_APPLICATION", id);
            IEnumerable<HousingAppToSubmitViewModel> result = null;
            DateTime now = System.DateTime.Now;

            var timeParam = new SqlParameter("@NOW", id);
            var idParam = new SqlParameter("@MODIFIER_ID", now);

            result = RawSqlQuery<HousingAppToSubmitViewModel>.query("INSERT_AA_APPLICATION @NOW, @MODIFIER_ID", timeParam, idParam); //run stored procedure
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            return result;
        }
    }
}
