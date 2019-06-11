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
        public StudentEmploymentViewModel Get(int id)
        {
            var query = _unitOfWork.StudentEmploymentRepository.GetById(id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Membership was not found." };
            }

            var idParam = new SqlParameter("@MEMBERSHIP_ID", id);
            var result = RawSqlQuery<StudentEmploymentViewModel>.query("MEMBERSHIPS_PER_MEMBERSHIP_ID @MEMBERSHIP_ID", idParam).FirstOrDefault();

            if (result == null)
            {
                return null;
            }
            // Getting rid of database-inherited whitespace
            result.ActivityCode = result.ActivityCode.Trim();
            result.ActivityDescription = result.ActivityDescription.Trim();
            result.SessionCode = result.SessionCode.Trim();
            result.SessionDescription = result.SessionDescription.Trim();
            result.IDNumber = result.IDNumber;
            result.FirstName = result.FirstName.Trim();
            result.LastName = result.LastName.Trim();
            result.Participation = result.Participation.Trim();
            result.ParticipationDescription = result.ParticipationDescription.Trim();

            return result;
        }

    }
}