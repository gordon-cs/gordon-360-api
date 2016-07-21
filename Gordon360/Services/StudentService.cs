using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services.ComplexQueries;

namespace Gordon360.Services
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
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Student was not found." };
            }
            StudentViewModel result = query; // Implicit conversion happening here, see ViewModels.
            return result;
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

        /// <summary>
        /// Fetches the student record using the specified email address
        /// </summary>
        /// <param name="email">The Student email</param>
        /// <returns></returns>
        public StudentViewModel GetByEmail(string email)
        {
            var query = _unitOfWork.StudentRepository.FirstOrDefault(x => x.student_email == email);
            if(query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Student was not found." };
            }
            StudentViewModel result = query; // Implicit conversion happening here, see ViewModels.
            return result;
        }
    }
}