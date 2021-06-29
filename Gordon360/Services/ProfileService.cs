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
using Gordon360.Static.Data;

namespace Gordon360.Services
{
    public class ProfileService : IProfileService
    {
        private IUnitOfWork _unitOfWork;
        private IAccountService _accountService;

        public ProfileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _accountService = new AccountService(_unitOfWork);
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
            var student = all.FirstOrDefault(x => x.AD_Username.ToLower() == username.ToLower());
            if (student != null)
                result = student;
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
            var facstaff = all.FirstOrDefault(x => x.AD_Username.ToLower() == username.ToLower());
            if (facstaff != null)
                result = facstaff;
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
            var alumni = all.FirstOrDefault(x => x.AD_Username.ToLower() == username.ToLower());
            if (alumni != null)
                result = alumni;
            return result;
        }
        /// <summary>
        /// get advisors for particular student
        /// </summary>
        /// <param name="id">student id</param>
        /// <returns></returns>
        public IEnumerable<AdvisorViewModel> GetAdvisors(string id)
        {
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var idParam = new SqlParameter("@ID", id);
            // Stored procedure returns row containing advisor1 ID, advisor2 ID, advisor3 ID 
            var idResult = RawSqlQuery<ADVISOR_SEPARATE_Result>.query("ADVISOR_SEPARATE @ID", idParam).FirstOrDefault();

            // Create empty advisor list to fill in and return.           
            List<AdvisorViewModel> resultList = new List<AdvisorViewModel>();
            
            // If idResult equal null, it means this user do not have advisor
            if (idResult == null)
            {
                //return empty list
                return resultList;
            }
            else
            {
                // Add advisors to resultList, then return the list
                if (!string.IsNullOrEmpty(idResult.Advisor1))
                {
                    resultList.Add(new AdvisorViewModel(
                        _accountService.Get(idResult.Advisor1).FirstName,
                        _accountService.Get(idResult.Advisor1).LastName,
                        _accountService.Get(idResult.Advisor1).ADUserName));
                }
                if (!string.IsNullOrEmpty(idResult.Advisor2))
                {
                    resultList.Add(new AdvisorViewModel(
                        _accountService.Get(idResult.Advisor2).FirstName, 
                        _accountService.Get(idResult.Advisor2).LastName, 
                        _accountService.Get(idResult.Advisor2).ADUserName));
                }
                if (!string.IsNullOrEmpty(idResult.Advisor3))
                {
                    resultList.Add(new AdvisorViewModel(
                        _accountService.Get(idResult.Advisor3).FirstName, 
                        _accountService.Get(idResult.Advisor3).LastName, 
                        _accountService.Get(idResult.Advisor3).ADUserName));
                }
            }
            //Set a list to return not null object in array
            return resultList;
        }

        /// <summary> Gets the clifton strengths of a particular user </summary>
        /// <param name="id"> The id of the user for which to retrieve info </param>
        /// <returns> Clifton strengths of the given user. </returns>
        public CliftonStrengthsViewModel GetCliftonStrengths(int id)
        {
            return _unitOfWork.CliftonStrengthsRepository.FirstOrDefault(x => x.ID_NUM == id);
        }

        /// <summary> Gets the emergency contact information of a particular user </summary>
        /// <param name="username"> The username of the user for which to retrieve info </param>
        /// <returns> Emergency contact information of the given user. </returns>
        public IEnumerable<EmergencyContactViewModel> GetEmergencyContact(string username)
        {
            var result = _unitOfWork.EmergencyContactRepository.GetAll((x) => x.AD_Username == username).Select((emrg) =>
            new EmergencyContactViewModel(emrg));

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "No emergency contacts found." };
            }
            return result;
        }

        /// <summary>
        /// Get photo path for profile
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
        /// Get ID photo path
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>PhotoPathViewModel if found, null if not found</returns>
        public PhotoPathViewModel GetIDPhotoPath(string id)
        {
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var idParam = new SqlParameter("@ID", id);
            var result = RawSqlQuery<PhotoPathViewModel>.query("ID_PHOTO_INFO_PER_USER_NAME @ID", idParam).FirstOrDefault(); //run stored procedure

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
            // Update value in cached data
            var student = Data.StudentData.FirstOrDefault(x => x.ID == id);
            var facStaff = Data.FacultyStaffData.FirstOrDefault(x => x.ID == id);
            var alum = Data.AlumniData.FirstOrDefault(x => x.ID == id);
            if (student != null)
            {
                student.preferred_photo = (path == null ? 0 : 1);
            }
            else if (facStaff != null)
            {
                facStaff.preferred_photo = (path == null ? 0 : 1);
            }
            else if (alum != null)
            {
                alum.preferred_photo = (path == null ? 0 : 1);
            }
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
                var hParam = new SqlParameter("@HANDSHAKE", DBNull.Value);
                var context = new CCTEntities1();
                context.Database.ExecuteSqlCommand("CREATE_SOCIAL_LINKS @USERNAME, @FACEBOOK, @TWITTER, @INSTAGRAM, @LINKEDIN, @HANDSHAKE", nameParam, fParam, tParam, iParam, lParam, hParam); //run stored procedure to create a row in the database for this user.
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

                case "handshake":
                    original.handshake = path.handshake;
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
            // Update value in cached data
            var student = Data.StudentData.FirstOrDefault(x => x.ID == id);
            if (student != null)
            {
                student.IsMobilePhonePrivate = (value == "Y" ? 1 : 0);
            }

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

            var accountID = original.account_id;
            var idParam = new SqlParameter("@ACCOUNT_ID", accountID);
            var valueParam = new SqlParameter("@VALUE", value);
            var context = new CCTEntities1();
            context.Database.ExecuteSqlCommand("UPDATE_SHOW_PIC @ACCOUNT_ID, @VALUE", idParam, valueParam); //run stored procedure.
            // Update value in cached data
            var student = Data.StudentData.FirstOrDefault(x => x.ID == id);
            var facStaff = Data.FacultyStaffData.FirstOrDefault(x => x.ID == id);
            var alum = Data.AlumniData.FirstOrDefault(x => x.ID == id);
            if (student != null)
            {
                student.show_pic = (value == "Y" ? 1 : 0);
            }
            else if (facStaff != null) {
                facStaff.show_pic = (value == "Y" ? 1 : 0);
            }
            else if (alum != null)
            {
                alum.show_pic = (value == "Y" ? 1 : 0);
            }
        }

    }
}
