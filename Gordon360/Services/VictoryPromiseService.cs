using System;
using System.Collections.Generic;
using System.Linq;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Exceptions.CustomExceptions;
using System.Data.SqlClient;
using Gordon360.Services.ComplexQueries;
using System.Diagnostics;
using Gordon360.Database.CCT;
using System.Threading.Tasks;

namespace Gordon360.Services
{
    public class VictoryPromiseService : IVictoryPromiseService
    {
        private readonly CCTContext _context;

        public VictoryPromiseService(CCTContext context)
        {
            _context = context;
        }

        /// <summary>
        /// get victory promise scores
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>VictoryPromiseViewModel if found, null if not found</returns>
        public async Task<IEnumerable<VictoryPromiseViewModel>> GetVPScores(string id)
        {
            var query = _context.ACCOUNT.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var result = await _context.Procedures.VICTORY_PROMISE_BY_STUDENT_IDAsync(int.Parse(id));
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            return (IEnumerable<VictoryPromiseViewModel>)result;
        }
    }
}