using Gordon360.Database.CCT;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Models;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Static.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services
{
    public class ProfileService : IProfileService
    {
        private CCTContext _context;
        private IAccountService _accountService;

        public ProfileService(CCTContext context)
        {
            _context = context;
            _accountService = new AccountService(context);
        }

        /// <summary>
        /// get student profile info
        /// </summary>
        /// <param name="username">username</param>
        /// <returns>StudentProfileViewModel if found, null if not found</returns>
        public StudentProfileViewModel GetStudentProfileByUsername(string username)
        {
            return Data.StudentData.FirstOrDefault(x => x.AD_Username.ToLower() == username.ToLower());
        }

        /// <summary>
        /// get faculty staff profile info
        /// </summary>
        /// <param name="username">username</param>
        /// <returns>FacultyStaffProfileViewModel if found, null if not found</returns>
        public FacultyStaffProfileViewModel GetFacultyStaffProfileByUsername(string username)
        {
            return Data.FacultyStaffData.FirstOrDefault(x => x.AD_Username.ToLower() == username.ToLower());
        }

        /// <summary>
        /// get alumni profile info
        /// </summary>
        /// <param name="username">username</param>
        /// <returns>AlumniProfileViewModel if found, null if not found</returns>
        public AlumniProfileViewModel GetAlumniProfileByUsername(string username)
        {
            return Data.AlumniData.FirstOrDefault(x => x.AD_Username.ToLower() == username.ToLower());
        }

        /// <summary>
        /// get mailbox combination
        /// </summary>
        /// <param name="username">The current user's username</param>
        /// <returns>MailboxViewModel with the combination</returns>
        public MailboxViewModel GetMailboxCombination(string username)
        {
            var mailboxNumber = 
                Data.StudentData
                .FirstOrDefault(x => x.AD_Username.ToLower() == username.ToLower())
                .Mail_Location;

            MailboxViewModel combo = _unitOfWork.MailboxRepository.FirstOrDefault(m => m.BoxNo == mailboxNumber);

            if (combo == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "A combination was not found for the specified mailbox number." };
            }

            return combo;
        }

        /// <summary>
        /// get a user's birthday
        /// </summary>
        /// <param name="username">The username of the person to get the birthdate of</param>
        /// <returns>Date the user's date of birth</returns>
        public DateTime GetBirthdate(string username)
        {
            var birthdate = _unitOfWork.AccountRepository.FirstOrDefault(a => a.AD_Username == username)?.Birth_Date;

            if (birthdate == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "A birthday was not found for this user." };
            }

            try
            {
                return (DateTime)(birthdate);
            } catch
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The user's birthdate was invalid." };
            }
        }

        /// <summary>
        /// get advisors for particular student
        /// </summary>
        /// <param name="id">student id</param>
        /// <returns></returns>
        public async Task<IEnumerable<AdvisorViewModel>> GetAdvisors(string id)
        {
            var query = _context.ACCOUNT.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                //Return an empty list if the id account does not have advisor
                return resultList;
            }

            // Stored procedure returns row containing advisor1 ID, advisor2 ID, advisor3 ID 
            var advisorIDsEnumerable = await _context.Procedures.ADVISOR_SEPARATEAsync(int.Parse(id));
            var advisorIDs = advisorIDsEnumerable.FirstOrDefault();

            List<AdvisorViewModel> resultList = new();

            foreach (var advisorID in new[] { advisorIDs.Advisor1, advisorIDs.Advisor2, advisorIDs.Advisor3 })
            {
                if (!string.IsNullOrEmpty(advisorID))
                {
                    var advisor = _accountService.Get(advisorID);
                    resultList.Add(new AdvisorViewModel(advisor.FirstName, advisor.LastName, advisor.ADUserName));
                }
            }

            return resultList;
        }

        /// <summary> Gets the clifton strengths of a particular user </summary>
        /// <param name="id"> The id of the user for which to retrieve info </param>
        /// <returns> Clifton strengths of the given user. </returns>
        public CliftonStrengthsViewModel GetCliftonStrengths(int id)
        {
            var strengths = _context.Clifton_Strengths.Find(id);
            if (strengths == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "No emergency contacts found." };
            }

            return result;
        }

        /// <summary> Gets the emergency contact information of a particular user </summary>
        /// <param name="username"> The username of the user for which to retrieve info </param>
        /// <returns> Emergency contact information of the given user. </returns>
        public IEnumerable<EmergencyContactViewModel> GetEmergencyContact(string username)
        {
            var result = _context.EmergencyContact.Where(x => x.AD_Username == username).Select(x => (EmergencyContactViewModel)x);

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
        public async Task<PhotoPathViewModel> GetPhotoPath(string id)
        {
            var query = _context.ACCOUNT.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var photoInfoList = await _context.Procedures.PHOTO_INFO_PER_USER_NAMEAsync(int.Parse(id));
            var photoInfo = photoInfoList.FirstOrDefault();

            return new PhotoPathViewModel { Img_Name = photoInfo.Img_Name, Img_Path = photoInfo.Img_Path, Pref_Img_Name = photoInfo.Pref_Img_Name, Pref_Img_Path = photoInfo.Pref_Img_Path };
        }

        /// <summary>
        /// Fetches a single profile whose username matches the username provided as an argument
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>ProfileViewModel if found, null if not found</returns>
        public ProfileCustomViewModel GetCustomUserInfo(string username)
        {
            return _context.CUSTOM_PROFILE.FirstOrDefault(x => x.username == username);
        }

        /// <summary>
        /// Sets the path for the profile image.
        /// </summary>
        /// <param name="id">The student id</param>
        /// <param name="path"></param>
        /// <param name="name"></param>
        public async void UpdateProfileImage(string id, string path, string name)
        {
            if (_context.ACCOUNT.FirstOrDefault(x => x.gordon_id == id) == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            await _context.Procedures.UPDATE_PHOTO_PATHAsync(int.Parse(id), path, name);
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
        public async void UpdateProfileLink(string username, string type, CUSTOM_PROFILE path)
        {
            var original = _context.CUSTOM_PROFILE.FirstOrDefault(p => p.username == username);

            if (original == null)
            {
                await _context.Procedures.CREATE_SOCIAL_LINKSAsync(username, path.facebook, path.twitter, path.instagram, path.linkedin, path.handshake);
            }
            else
            {

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
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// privacy setting of mobile phone.
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="value">Y or N</param>
        public async void UpdateMobilePrivacy(string id, string value)
        {
            var original = _context.ACCOUNT.FirstOrDefault(x => x.gordon_id == id);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }
            await _context.Procedures.UPDATE_PHONE_PRIVACYAsync(int.Parse(id), value);
            // Update value in cached data
            var student = Data.StudentData.FirstOrDefault(x => x.ID == id);
            if (student != null)
            {
                student.IsMobilePhonePrivate = (value == "Y" ? 1 : 0);
            }

            _context.SaveChanges();
        }

        /// <summary>
        /// mobile phone number setting
        /// </summary>
        /// <param name="profile"> The profile for the user whose phone is to be updated </param>
        public StudentProfileViewModel UpdateMobilePhoneNumber(StudentProfileViewModel profile)
        {
            var idParam = new SqlParameter("@UserID", profile.ID);
            var newPhoneNumberParam = new SqlParameter("@PhoneUnformatted", profile.MobilePhone);
            var result = RawSqlQuery<StudentProfileViewModel>.query("UPDATE_CELL_PHONE @UserID, @PhoneUnformatted", idParam, newPhoneNumberParam);

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found" };
            }

            // Update value in cached data
            var student = Data.StudentData.FirstOrDefault(x => x.ID == profile.ID);
            if (student != null)
            {
                student.MobilePhone = profile.MobilePhone;
            }

            return profile;
        }

        /// <summary>
        /// privacy setting user profile photo.
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="value">Y or N</param>
        public async void UpdateImagePrivacy(string id, string value)
        {
            var original = _context.ACCOUNT.FirstOrDefault(x => x.gordon_id == id);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            await _context.Procedures.UPDATE_SHOW_PICAsync(original.account_id, value);
            // Update value in cached data
            var student = Data.StudentData.FirstOrDefault(x => x.ID == id);
            var facStaff = Data.FacultyStaffData.FirstOrDefault(x => x.ID == id);
            var alum = Data.AlumniData.FirstOrDefault(x => x.ID == id);
            if (student != null)
            {
                student.show_pic = (value == "Y" ? 1 : 0);
            }
            else if (facStaff != null)
            {
                facStaff.show_pic = (value == "Y" ? 1 : 0);
            }
            else if (alum != null)
            {
                alum.show_pic = (value == "Y" ? 1 : 0);
            }

            _context.SaveChanges();
        }

    }
}
