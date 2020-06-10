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
    public class WellnessService : IWellnessService
    {
       

        public WellnessService()
        {
        }

        /// <summary>
        /// gets status of wellness check
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>boolean if found, null if not found</returns>

        public WellnessViewModel GetStatus(string id)
        {
            // var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            // if (query == null)
            // {
            //     throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            // }

            // var idParam = new SqlParameter("@ID", id);
            // var result = true //RawSqlQuery<WellnessViewModel>.query("VICTORY_PROMISE_BY_STUDENT_ID @ID", idParam); //run stored procedure
            // if (result == null)
            // {
            //     throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            // }

            // var wellnessModel = result.Select(x =>
            // {
            //     WellnessViewModel y = new WellnessViewModel();
            //     y.currentStatus = x.currentStatus ?? null;
            //     return y;
            // });


            WellnessViewModel y = new WellnessViewModel()
            {
                currentStatus = true
            };
                
            

            return y;

        }

       //WellnessViewModel IWellnessService.GetStatus(string id)
       // {
       //     throw new NotImplementedException();
       // }
    }
}