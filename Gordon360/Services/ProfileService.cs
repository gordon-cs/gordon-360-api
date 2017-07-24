﻿using System;
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
using Gordon360.Static.Data;

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
        /// get student profile info
        /// </summary>
        /// <param name="username">username</param>
        /// <returns>StudentProfileViewModel if found, null if not found</returns>
        public StudentProfileViewModel GetStudentProfileByUsername(string username)
        {
            var all = Data.StudentData;
            StudentProfileViewModel result = null;
            result = all.FirstOrDefault(x => x.AD_Username.ToLower() == username.ToLower());
            return result;
        }
        /// <summary>
        /// get faculty staff profile info
        /// </summary>
        /// <param name="username">username</param>
        /// <returns>FacultyStaffProfileViewModel if found, null if not found</returns>
        public FacultyStaffProfileViewModel GetFacultyStaffProfileByUsername(string username)
        {
            var all = Data.FacultyStaffData;
            FacultyStaffProfileViewModel result = null;
            result = all.FirstOrDefault(x => x.AD_Username.ToLower() == username.ToLower());
            return result;
        }
        /// <summary>
        /// get alumni profile info
        /// </summary>
        /// <param name="username">username</param>
        /// <returns>AlumniProfileViewModel if found, null if not found</returns>
        public AlumniProfileViewModel GetAlumniProfileByUsername(string username)
        {
            var all = Data.AlumniData;
            AlumniProfileViewModel result = null;
            result = all.FirstOrDefault(x => x.AD_Username.ToLower() == username.ToLower());
            return result;
        }

        /// <summary>
        /// get photo path
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>PhotoPathViewModel if found, null if not found</returns>
        public PhotoPathViewModel GetPhotoPath(string id)
        {
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var idParam = new SqlParameter("@ID", id);
            var result = RawSqlQuery<PhotoPathViewModel>.query("PHOTO_INFO_PER_USER_NAME @ID", idParam).FirstOrDefault(); //run stored procedure

            if (result == null)
            {
                return null;
            }

            return result;
        }
        /// <summary>
        /// Fetches a single profile whose username matches the username provided as an argument
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>ProfileViewModel if found, null if not found</returns>
        public ProfileCustomViewModel GetCustomUserInfo(string username)
        {
            var query = _unitOfWork.ProfileCustomRepository.FirstOrDefault(x => x.username == username);
            if (query == null)
            {
                return new ProfileCustomViewModel();  //return a null object.
            }

            ProfileCustomViewModel result = query;
            return result;
        }
        /// <summary>
        /// Sets the path for the profile image.
        /// </summary>
        /// <param name="id">The student id</param>
        /// <param name="path"></param>
        /// <param name="name"></param>
        public void UpdateProfileImage(string id, string path, string name)
        {
            if (_unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id) == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }
            var authParam = new SqlParameter("@ID", id);
            var pathParam = new SqlParameter("@FILE_PATH", path);
            if (path == null)
                pathParam = new SqlParameter("@FILE_PATH", DBNull.Value);
            var nameParam = new SqlParameter("@FILE_NAME", name);
            if (name == null)
                nameParam = new SqlParameter("@FILE_NAME", DBNull.Value);
            var context = new CCTEntities1();
            context.Database.ExecuteSqlCommand("UPDATE_PHOTO_PATH @ID, @FILE_PATH, @FILE_NAME", authParam, pathParam, nameParam);   //run stored procedure.
        }


        /// <summary>
        /// Sets the path for the profile links.
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="type"></param>
        /// <param name="path"></param>
        public void UpdateProfileLink(string username, string type, CUSTOM_PROFILE path)
        {
            var original = _unitOfWork.ProfileCustomRepository.GetByUsername(username);

            if (original == null)
            {
                var nameParam = new SqlParameter("@USERNAME", username);
                var fParam = new SqlParameter("@FACEBOOK", DBNull.Value);
                var tParam = new SqlParameter("@TWITTER", DBNull.Value);
                var iParam = new SqlParameter("@INSTAGRAM", DBNull.Value);
                var lParam = new SqlParameter("@LINKEDIN", DBNull.Value);
                var context = new CCTEntities1();
                context.Database.ExecuteSqlCommand("CREATE_SOCIAL_LINKS @USERNAME, @FACEBOOK, @TWITTER, @INSTAGRAM, @LINKEDIN", nameParam, fParam, tParam, iParam, lParam); //run stored procedure to create a row in the database for this user.
                original = _unitOfWork.ProfileCustomRepository.GetByUsername(username);
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
        /// privacy setting of mobile phone.
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="value">Y or N</param>
        public void UpdateMobilePrivacy(string id, string value)
        {
            var original = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }
            var idParam = new SqlParameter("@ID", id);
            var valueParam = new SqlParameter("@VALUE", value);
            var context = new CCTEntities1();
            context.Database.ExecuteSqlCommand("UPDATE_PHONE_PRIVACY @ID, @VALUE", idParam, valueParam); // run stored procedure.

        }
        /// <summary>
        /// privacy setting user profile photo.
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="value">Y or N</param>
        public void UpdateImagePrivacy(string id, string value)
        {
            var original = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var accountID = original.gordon_id;
            var idParam = new SqlParameter("@ACCOUNT_ID", accountID);
            var valueParam = new SqlParameter("@VALUE", value);
            var context = new CCTEntities1();
            context.Database.ExecuteSqlCommand("UPDATE_SHOW_PIC @ACCOUNT_ID, @VALUE", idParam, valueParam); //run stored procedure.
        }

    }
}