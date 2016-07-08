using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCT_App.Models;
using CCT_App.Models.ViewModels;
using CCT_App.Repositories;
using CCT_App.Services.ComplexQueries;

namespace CCT_App.Services
{
    /// <summary>
    /// Service Class to faciliatate data transactions between the controller and the database models.
    /// </summary>
    public class StudentService : IStudentService
    {
        private IUnitOfWork _unitOfWork;

        public StudentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Fetches the student record whose id number matches the parameter.
        /// </summary>
        /// <param name="id">The student id</param>
        /// <returns>A student view model if found, null if not found.</returns>
        public StudentViewModel Get(string id)
        {
            var query = _unitOfWork.StudentRepository.GetById(id);
            StudentViewModel result = query; // Implicit conversion happening here, see ViewModels.
            return result;
        }


        /// <summary>
        /// Fetches all the membership information linked to the student whose id appears as a parameter.
        /// </summary>
        /// <param name="id">The student id.</param>
        /// <returns>A MembershipViewModel IEnumerable. If nothing is found, an empty IEnumberable is returned.</returns>
        public IEnumerable<MembershipViewModel> GetActivitiesForStudent(string id)
        {
            var studentExists = _unitOfWork.StudentRepository.Where(x => x.student_id.Trim() == id).Count() > 0;
            if(!studentExists)
            {
                return null;
            }

            var query = Constants.getMembershipsForStudentQuery;
            var result = RawSqlQuery<MembershipViewModel>.query(query, id);

            // The Views that were given to were defined as char(n) instead of varchar(n)
            // They return with whitespace padding which messes up comparisons later on.
            // I'm using the IEnumerable Select method here to help get rid of that.
            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.ActivityCode = x.ActivityCode.Trim();
                trim.ActivityDescription = x.ActivityDescription.Trim();
                trim.SessionCode = x.SessionCode.Trim();
                trim.SessionDescription = x.SessionDescription.Trim();
                trim.Participation = x.Participation.Trim();
                trim.ParticipationDescription = x.ParticipationDescription.Trim();
                trim.FirstName = x.FirstName.Trim();
                trim.LastName = x.LastName.Trim();
                trim.IDNumber = x.IDNumber.Trim();
                return trim;
            });

            return trimmedResult.OrderByDescending(x => x.StartDate);
        }

        /// <summary>
        /// Fetches all the Student records from the database.
        /// </summary>
        /// <returns>StudentViweModel IEnumberable. If nothing is found, an empty IEnumberable is returned.</returns>
        public IEnumerable<StudentViewModel> GetAll()
        {
            var query = _unitOfWork.StudentRepository.GetAll();
            var result = query.Select<Student, StudentViewModel>(x => x);
            return result;
        }

    }
}