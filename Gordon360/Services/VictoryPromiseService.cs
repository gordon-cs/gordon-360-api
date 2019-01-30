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

            var idParam = new SqlParameter("@ID", id);
            var result = RawSqlQuery<VictoryPromiseViewModel>.query("PHOTO_INFO_PER_USER_NAME @ID", idParam).FirstOrDefault(); //run stored procedure

            if (result == null)
            {
                return null;
            }

            return result;
        }
    }
}