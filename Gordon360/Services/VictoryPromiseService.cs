using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Exceptions.CustomExceptions;
using System.Data.SqlClient;
using Gordon360.Utils.ComplexQueries;
using System.Diagnostics;

namespace Gordon360.Utils
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
        /// <returns>VictoryPromiseViewModel if found, null if not found</returns>
        public IEnumerable<VictoryPromiseViewModel> GetVPScores(string id)
        {
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var idParam = new SqlParameter("@ID", id);
            var result = RawSqlQuery<VictoryPromiseViewModel>.query("VICTORY_PROMISE_BY_STUDENT_ID @ID", idParam); //run stored procedure
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }
            // Transform the ActivityViewModel (ACT_CLUB_DEF) into ActivityInfoViewModel
            var victoryPromiseModel = result.Select(x =>
            {
                VictoryPromiseViewModel y = new VictoryPromiseViewModel();
                y.TOTAL_VP_CC_SCORE = x.TOTAL_VP_CC_SCORE ?? 0;
                y.TOTAL_VP_IM_SCORE = x.TOTAL_VP_IM_SCORE ?? 0;
                y.TOTAL_VP_LS_SCORE = x.TOTAL_VP_LS_SCORE ?? 0;
                y.TOTAL_VP_LW_SCORE = x.TOTAL_VP_LW_SCORE ?? 0;
                return y;
            });
            return victoryPromiseModel;

        }
    }
}