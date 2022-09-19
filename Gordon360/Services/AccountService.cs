using Gordon360.Authorization;
using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Static.Names;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services
{

    /// <summary>
    /// Service Class that facilitates data transactions between the AccountsController and the Account database model.
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly CCTContext _context;

        public AccountService(CCTContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Fetches a single account record whose id matches the id provided as an argument
        /// </summary>
        /// <param name="id">The person's gordon id</param>
        /// <returns>AccountViewModel if found, null if not found</returns>
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.ACCOUNT)]
        public AccountViewModel GetAccountByID(string id)
        {
            var account = _context.ACCOUNT.FirstOrDefault(x => x.gordon_id == id);
            if (account == null)
            {
                // Custom Exception is thrown that will be cauth in the controller Exception filter.
                throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
            }

            return account;
        }

        /// <summary>
        /// Fetches all the account records from storage.
        /// </summary>
        /// <returns>AccountViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.ACCOUNT)]
        public IEnumerable<AccountViewModel> GetAll()
        {
            return (IEnumerable<AccountViewModel>)_context.ACCOUNT; //Map the database model to a more presentable version (a ViewModel)
        }

        /// <summary>
        /// Fetches the account record with the specified email.
        /// </summary>
        /// <param name="email">The email address associated with the account.</param>
        /// <returns>the student id number</returns>
        public int GetGordonIDFromEmail(string email)
        {
            var account = _context.ACCOUNT.FirstOrDefault(x => x.email == email);
            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
            }

            return account.gordon_id;
        }

        /// <summary>
        /// Fetches the account record with the specified username.
        /// </summary>
        /// <param name="username">The AD username associated with the account.</param>
        /// <returns>the student account information</returns>
        public AccountViewModel GetAccountByUsername(string username)
        {
            var account = _context.ACCOUNT.FirstOrDefault(x => x.AD_Username == username);
            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
            }

            return account;
        }

        public IEnumerable<AdvancedSearchViewModel> AdvancedSearch(
            List<string> accountTypes,
            string firstname,
            string lastname,
            string major,
            string minor,
            string hall,
            string classType,
            string homeCity,
            string state,
            string country,
            string department,
            string building)
        {
            // Accept common town abbreviations in advanced people search
            // East = E, West = W, South = S, North = N
            if (
                !string.IsNullOrEmpty(homeCity)
                && (
                  homeCity.StartsWith("e ") ||
                  homeCity.StartsWith("w ") ||
                  homeCity.StartsWith("s ") ||
                  homeCity.StartsWith("n ")
                )
              )
            {
                homeCity =
                    homeCity
                        .Replace("e ", "east ")
                        .Replace("w ", "west ")
                        .Replace("s ", "south ")
                        .Replace("n ", "north ");
            }

            // Create accounts viewmodel to search
            IEnumerable<AdvancedSearchViewModel> accounts = new List<AdvancedSearchViewModel>();
            if (accountTypes.Contains("student"))
            {
                accounts = accounts.Union(GetAllPublicStudentAccounts());
            }

            if (accountTypes.Contains("facstaff"))
            {
                if (string.IsNullOrEmpty(homeCity))
                {
                    accounts = accounts.Union(GetAllPublicFacultyStaffAccounts());
                }
                else
                {
                    accounts = accounts.Union(GetAllPublicFacultyStaffAccounts().Where(a => a.KeepPrivate == "0"));
                }
            }

            if (accountTypes.Contains("alumni"))
            {
                if (string.IsNullOrEmpty(homeCity))
                {
                    accounts = accounts.Union(GetAllPublicAlumniAccounts());
                }
                else
                {
                    accounts = accounts.Union(GetAllPublicAlumniAccounts().Where(a => a.ShareAddress.ToLower() != "n"));
                }
            }

            return accounts
                .Where(a =>
                       (
                               a.FirstName.ToLower().StartsWith(firstname)
                            || a.NickName.ToLower().StartsWith(firstname)
                       )
                    && (
                               a.LastName.ToLower().StartsWith(lastname)
                            || a.MaidenName.ToLower().StartsWith(lastname)
                       )
                    && (
                               a.Major1Description.StartsWith(major)
                            || a.Major2Description.StartsWith(major)
                            || a.Major3Description.StartsWith(major)
                       )
                    && (
                               a.Minor1Description.StartsWith(minor)
                            || a.Minor2Description.StartsWith(minor)
                            || a.Minor3Description.StartsWith(minor)
                       )
                    && a.Hall.StartsWith(hall)
                    && a.Class.StartsWith(classType)
                    && a.HomeCity.ToLower().StartsWith(homeCity)
                    && a.HomeState.StartsWith(state)
                    && a.Country.StartsWith(country)
                    && a.OnCampusDepartment.StartsWith(department)
                    && a.BuildingDescription.StartsWith(building)
                )
                .OrderBy(a => a.LastName)
                .ThenBy(a => a.FirstName);
        }

        /// <summary>
        /// Get basic info for all accounts
        /// </summary>
        /// <returns>BasicInfoViewModel of all accounts</returns>
        public async Task<IEnumerable<BasicInfoViewModel>> GetAllBasicInfoAsync()
        {

            var basicInfo = await _context.Procedures.ALL_BASIC_INFOAsync();
            return basicInfo.Select(
                b => new BasicInfoViewModel
                {
                    ConcatonatedInfo = b.ConcatonatedInfo,
                    FirstName = b.FirstName,
                    LastName = b.LastName,
                    Nickname = b.Nickname,
                    UserName = b.UserName
                });
        }

        /// <summary>
        /// Get basic info for all accounts except alumni
        /// </summary>
        /// <returns>BasicInfoViewModel of all accounts except alumni</returns>
        public async Task<IEnumerable<BasicInfoViewModel>> GetAllBasicInfoExceptAlumniAsync()
        {
            var basicInfo = await _context.Procedures.ALL_BASIC_INFO_NOT_ALUMNIAsync();
            return basicInfo.Select(
                b => new BasicInfoViewModel
                {
                    ConcatonatedInfo = b.ConcatonatedInfo,
                    FirstName = b.firstname,
                    LastName = b.lastname,
                    Nickname = b.Nickname,
                    UserName = b.Username
                });
        }

        private IEnumerable<AdvancedSearchViewModel> GetAllPublicStudentAccounts()
        {
            return _context.Student.Select<Student, AdvancedSearchViewModel>(s => s);
        }

        private IEnumerable<AdvancedSearchViewModel> GetAllPublicFacultyStaffAccounts()
        {
            return _context.FacStaff.Select<FacStaff, AdvancedSearchViewModel>(fs => fs);
        }

        private IEnumerable<AdvancedSearchViewModel> GetAllPublicAlumniAccounts()
        {
            return _context.Alumni.Select<Alumni, AdvancedSearchViewModel>(a => a);
        }
    }
}