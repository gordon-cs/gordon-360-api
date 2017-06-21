using System.Web;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services.ComplexQueries;
using Gordon360.Static.Names;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Services
{
    /// <summary>
    /// Service Class that facilitates data transactions between the ProfileController and the PROFILE database model.
    /// </summary>

    public class ProfileImageService : IProfileImageService
    {
        private IUnitOfWork _unitOfWork;

        public ProfileImageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// Fetches a single activity record whose username matches the username provided as an argument
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>ProfileViewModel if found, null if not found</returns>
        public ProfileImageViewModel Get(string username)
        {
            var query = _unitOfWork.ProfileImageRepository.GetByUsername(username);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Profile was not found." };
            }
            ProfileImageViewModel result = query;
            return result;
        }

        /// <summary>
        /// Fetches all profile records from storage.
        /// </summary>
        /// <returns>ProfileImageViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public IEnumerable<ProfileImageViewModel> GetAll()
        {
            var query = _unitOfWork.ProfileImageRepository.GetAll();
            var result = query.Select<PROFILE_IMAGE, ProfileImageViewModel>(x => x);
            return result;
        }

        /// <summary>
        /// Sets the path for the profile image.
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="path"></param>
        public void UpdateProfileImage(string username, string path)
        {
            var original = _unitOfWork.ProfileImageRepository.GetByUsername(username);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The profile was not found." };
            }

            original.Img_Path = path;

            _unitOfWork.Save();
        }

        /// <summary>
        /// Reset the path for the profile image
        /// </summary>
        /// <param name="username">The username</param>
        public void ResetProfileImage(string username)
        {
            var original = _unitOfWork.ProfileImageRepository.GetByUsername(username);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The profile was not found." };
            }

            original.Img_Path = Defaults.DEFAULT_PROFILE_IMAGE_PATH;

            _unitOfWork.Save();
        }

    }
}