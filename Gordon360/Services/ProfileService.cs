using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Static.Names;
using System.Data.SqlClient;
using Gordon360.Services.ComplexQueries;

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

        /// <summary>
        /// Fetches a single profile whose username matches the username provided as an argument
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>ProfileViewModel if found, null if not found</returns>
        public ProfileCustomViewModel GetCustomUserInfo(string username)
        {
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.AD_Username == username);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var nameParam = new SqlParameter("@USER_NAME", username);
            var result = RawSqlQuery<ProfileCustomViewModel>.query("PHOTO_INFO_PER_USER_NAME @USER_NAME", nameParam).FirstOrDefault();

            if (result == null)
            {
                return null;
            }

            return result;
        }
        /// <summary>
        /// Sets the path for the profile image.
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="path"></param>
        /// <param name="name"></param>
        public void UpdateProfileImage(string username, string path, string name)
        {
            if (_unitOfWork.AccountRepository.FirstOrDefault(x => x.AD_Username == username) == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }
            var authParam = new SqlParameter("@USER_NAME", username);
            var pathParam = new SqlParameter("@FILE_PATH", path);
            if (path == null)
                pathParam = new SqlParameter("@FILE_PATH", DBNull.Value);
            var nameParam = new SqlParameter("@FILE_NAME", name);
            if (name == null)
                nameParam = new SqlParameter("@FILE_NAME", DBNull.Value);
            var context = new CCTEntities1();
          context.Database.ExecuteSqlCommand("UPDATE_PHOTO_PATH @USER_NAME, @FILE_PATH, @FILE_NAME", authParam, pathParam, nameParam);
        }


        /// <summary>
        /// Sets the path for the profile links.
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="type"></param>
        /// <param name="path"></param>
        public void UpdateProfileLink(string username, string type, PROFILE_IMAGE path)
        {
            var original = _unitOfWork.ProfileCustomRepository.GetByUsername(username);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The profile was not found." };
            }

            switch (type)
            {
                case "facebook":
                    original.facebook = path.facebook;
                    break;

                case "twitter":
                    original.twitter = path.twitter;
                    break;

                case "instagram":
                    original.instagram = path.instagram;
                    break;

                case "linkedin":
                    original.linkedin = path.linkedin;
                    break;
            }

            _unitOfWork.Save();
        }

        /// <summary>
        /// Reset the path for the profile image
        /// </summary>
        /// <param name="username">The username</param>
        public void ResetProfileImage(string username)
        {
            var original = _unitOfWork.ProfileCustomRepository.GetByUsername(username);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The profile was not found." };
            }

            original.Img_Path = Defaults.DEFAULT_PROFILE_IMAGE_PATH;

            _unitOfWork.Save();
        }

        /// <summary>
        /// privacy setting of mobile phone.
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="p"></param>
        public void UpdateMobilePrivacy(string username, bool p)
        {
            var original = _unitOfWork.StudentTempRepository.FirstOrDefault(x => x.EmailUserName == username);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The profile was not found." };
            }
            original.IsMobilePhonePrivate = p;
            _unitOfWork.Save();
        }
        /// <summary>
        /// privacy setting user profile photo.
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="p"></param>
        public void UpdateImagePrivacy(string username, int p)
        {
            var original = _unitOfWork.AccountRepository.FirstOrDefault(x => x.AD_Username == username);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }
            original.show_pic = p;
            _unitOfWork.Save();
        }

    }
}