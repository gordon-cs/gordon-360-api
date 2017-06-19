using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;

namespace Gordon360.Services
{
    public class ProfileService : IProfileService
    {
        private IUnitOfWork _unitOfWork;

        public ProfileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// Fetches a single activity record whose id matches the id provided as an argument
        /// </summary>
        /// <param name="username">The activity code</param>
        /// <returns>ActivityViewModel if found, null if not found</returns>
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.PROFILE)]
        public ProfileViewModel GetProfileByUsername(string username)
        {
            var query = _unitOfWork.StudentTempRepository.FirstOrDefault(x => x.EmailUserName == username);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
            }
            ProfileViewModel result = query; // Implicit conversion happening here, see ViewModels.
            return result;
        }
        /// <summary>
        /// Fetches all the account records from storage.
        /// </summary>
        /// <returns>AccountViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.PROFILE)]
        public IEnumerable<ProfileViewModel> GetAll()
        {
            var query = _unitOfWork.StudentTempRepository.GetAll();
            var result = query.Select<student_temp, ProfileViewModel>(x => x); //Map the database model to a more presentable version (a ViewModel)
            return result;
        }
    }
}