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
        public StudentEmploymentViewModel GetEmployment(string id)
        {
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }
            var idParam = new SqlParameter("@GORDON_ID", int.Parse(id));
            var result = RawSqlQuery<StudentEmploymentViewModel>.query("STUDENT_JOBS_PER_ID_NUM @GORDON_ID", idParam).FirstOrDefault(); //run stored procedure

            StudentEmploymentViewModel vm = new StudentEmploymentViewModel
            {
                Job_Title = result.Job_Title ?? "",
                Job_Department = result.Job_Department ?? "",
                Job_Department_Name = result.Job_Department_Name ?? "",
                Job_Start_Date = result.Job_Start_Date ?? DateTime.Now,
                //Need to fix when it is null 
                Job_End_Date = result.Job_End_Date ?? DateTime.Now,
                Job_Expected_End_Date = result.Job_Expected_End_Date ?? DateTime.Now
            
            };
            return vm;
        }
    }
}