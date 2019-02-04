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
    public class VictoryPromiseService : IVictoryPromiseService
    {
        private IUnitOfWork _unitOfWork;

        public VictoryPromiseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// get victory promise scores
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>PhotoPathViewModel if found, null if not found</returns>
        public VictoryPromiseViewModel GetVPScores(string id)
        {
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }
            var idParam = new SqlParameter("@ID", Int32.Parse(id));
            var result = RawSqlQuery<VictoryPromiseViewModel>.query("VICTORY_PROMISE_BY_STUDENT_ID @ID", idParam).FirstOrDefault(); //run stored procedure
            Debug.WriteLine("RESULTTTTTTT: " + result.cc + result.im + result.lv + result.lw); // debug message

            if (result == null)
            {
                return null;
            }

            return result;
        }
    }
}