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
    public class StudentService : IStudentService
    {
        private IUnitOfWork _unitOfWork;

        public StudentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public StudentViewModel Get(string id)
        {
            var query = _unitOfWork.StudentRepository.GetById(id);
            StudentViewModel result = query;
            return result;
        }

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
                trim.Description = x.Description.Trim();
                return trim;
            });

            return trimmedResult;
        }

        public IEnumerable<StudentViewModel> GetAll()
        {
            var query = _unitOfWork.StudentRepository.GetAll();
            var result = query.Select<Student, StudentViewModel>(x => x);
            return result;
        }

    }
}