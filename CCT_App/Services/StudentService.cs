using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCT_App.Models;
using CCT_App.Repositories;

namespace CCT_App.Services
{
    public class StudentService : IStudentService
    {
        private IUnitOfWork _unitOfWork;

        public StudentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Student Get(string id)
        {
            var result = _unitOfWork.StudentRepository.GetById(id);
            return result;
        }

        public IEnumerable<Membership> GetActivitiesForStudent(string id)
        {
            var studentExists = _unitOfWork.StudentRepository.Where(x => x.student_id.Trim() == id).Count() > 0;
            if(!studentExists)
            {
                return null;
            }

            var studentMemberships = _unitOfWork.MembershipRepository.Where(x => x.ID_NUM.Trim() == id);
            return studentMemberships.AsEnumerable();
        }

        public IEnumerable<Student> GetAll()
        {
            var result = _unitOfWork.StudentRepository.GetAll();
            return result;
        }

    }
}