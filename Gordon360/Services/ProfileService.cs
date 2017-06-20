using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Exceptions.CustomExceptions;

namespace Gordon360.Services
{
    public class ProfileService : IProfileService
    {
        private IUnitOfWork _unitOfWork;

        public ProfileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public StudentProfileViewModel GetStudentProfileByUsername(string username)
        {
            var query = _unitOfWork.StudentTempRepository.FirstOrDefault(x => x.EmailUserName == username);
            if (query == null)
            {
                return null;
            }
            StudentProfileViewModel result = query; // Implicit conversion happening here, see ViewModels.
            return result;
        }

        public FacultyStaffProfileViewModel GetFacultyStaffProfileByUsername(string username)
        {
            var query = _unitOfWork.FacultyStaffRepository.FirstOrDefault(x => x.EmailUserName == username);
            if (query == null)
            {
                return null;
            }
            FacultyStaffProfileViewModel result = query; // Implicit conversion happening here, see ViewModels.
            return result;
        }

        public AlumniProfileViewModel GetAlumniProfileByUsername(string username)
        {
            var query = _unitOfWork.AlumniRepository.FirstOrDefault(x => x.EmailUserName == username);
            if (query == null)
            {
                return null;
            }
            AlumniProfileViewModel result = query; // Implicit conversion happening here, see ViewModels.
            return result;
        }

    }
}