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
    public class StudentEmploymentService : IStudentEmploymentService
    {
        private IUnitOfWork _unitOfWork;

        public StudentEmploymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// get victory promise scores
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>VictoryPromiseViewModel if found, null if not found</returns>
        public IEnumerable<StudentEmploymentViewModel> GetEmployment(string id)
        {
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var idParam = new SqlParameter("@ID", id);
            var result = RawSqlQuery<StudentEmploymentViewModel>.query("STUDENT_JOBS_PER_ID_NUM @ID", idParam); //run stored procedure
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }
            // Transform the ActivityViewModel (ACT_CLUB_DEF) into ActivityInfoViewModel
            var studentEmploymentModel = result.Select(x =>
            {
                StudentEmploymentViewModel y = new StudentEmploymentViewModel();
                y.Job_Title = x.Job_Title ?? "";
                y.Job_Department = x.Job_Department ?? "";
                y.Job_Department_Name = x.Job_Department_Name ?? "";
                y.Job_Start_Date = x.Job_Start_Date ?? DateTime.MinValue;
                y.Job_End_Date = x.Job_End_Date ?? DateTime.Now;
                y.Job_Expected_End_Date = x.Job_Expected_End_Date ?? DateTime.Now;
                return y;
            });
            return studentEmploymentModel;

        }
    }
}