using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services.ComplexQueries;
using System.Data.SqlClient;
using System.Data;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Static.Methods;
using System.Diagnostics;

namespace Gordon360.Services
{
    /// <summary>
    /// Service Class that facilitates data transactions between the StudentEmploymentController and the StudentEmployment database model.
    /// </summary>
    public class StudentEmploymentService : IStudentEmploymentService
    {
        private IUnitOfWork _unitOfWork;

        public StudentEmploymentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        /// <summary>
        /// Fetch the membership whose id is specified by the parameter
        /// </summary>
        /// <param name="id">The membership id</param>
        /// <returns>MembershipViewModel if found, null if not found</returns>
        public StudentEmploymentViewModel GetEmployment(string id)
        {
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Student was not found." };
            }

            var idParam = new SqlParameter("@GORDON_ID", Int32.Parse(id));
            var result = RawSqlQuery<StudentEmploymentViewModel>.query("STUDENT_JOBS_PER_ID_NUM @GORDON_ID", idParam).FirstOrDefault();

            if (result == null)
            {
                return null;
            }
            // Getting rid of database-inherited whitespace
            result.Job_Title = result.Job_Title.Trim();
            result.Job_Department = result.Job_Department.Trim();
            result.Job_Department_Name = result.Job_Department_Name.Trim();

            return result;
        }

    }
}