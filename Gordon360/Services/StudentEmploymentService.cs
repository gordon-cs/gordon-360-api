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
    public class StudentEmploymentService : IStudentEmploymentService
    {
        private CCTContext _context;

        public StudentEmploymentService(CCTContext context)
        {
            _context = context;
        }

        /// <summary>
        /// get victory promise scores
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>VictoryPromiseViewModel if found, null if not found</returns>
        public async Task<IEnumerable<StudentEmploymentViewModel>> GetEmployment(string id)
        {
            var query = _context.ACCOUNT.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var result = await _context.Procedures.STUDENT_JOBS_PER_ID_NUMAsync(int.Parse(id));
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }
            // Transform the ActivityViewModel (ACT_CLUB_DEF) into ActivityInfoViewModel
            var studentEmploymentModel = result.Select(x => new StudentEmploymentViewModel
            {
                Job_Title = x.Job_Title ?? "",
                Job_Department = x.Job_Department ?? "",
                Job_Department_Name = x.Job_Department_Name ?? "",
                Job_Start_Date = x.Job_Start_Date ?? DateTime.MinValue,
                Job_End_Date = x.Job_End_Date ?? DateTime.Now,
                Job_Expected_End_Date = x.Job_Expected_End_Date ?? DateTime.Now,
            });
            return studentEmploymentModel;

        }
    }
}