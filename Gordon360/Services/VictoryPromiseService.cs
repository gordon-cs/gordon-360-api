﻿using System;
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
        /// <returns>VictoryPromiseViewModel if found, null if not found</returns>
        public VictoryPromiseViewModel GetVPScores(string id)
        {
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }
            var idParam = new SqlParameter("@ID", int.Parse(id));
            var result = RawSqlQuery<VictoryPromiseViewModel>.query("VICTORY_PROMISE_BY_STUDENT_ID @ID", idParam).FirstOrDefault(); //run stored procedure


            VictoryPromiseViewModel vm = new VictoryPromiseViewModel
            {
                TOTAL_VP_CC_SCORE = result.TOTAL_VP_CC_SCORE ?? 0,
                TOTAL_VP_IM_SCORE = result.TOTAL_VP_IM_SCORE ?? 0,
                TOTAL_VP_LS_SCORE = result.TOTAL_VP_LS_SCORE ?? 0,
                TOTAL_VP_LW_SCORE = result.TOTAL_VP_LW_SCORE ?? 0
            };
            return vm;
        }
    }
}