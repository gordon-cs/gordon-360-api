using Gordon360.AuthorizationFilters;
using Gordon360.Database.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Data;
using Gordon360.Static.Names;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class AccountsController : GordonControllerBase
    {
        private readonly IRoleCheckingService _roleCheckingService;
        private readonly IAccountService _accountService;

        public AccountsController(CCTContext context)
        {
            _accountService = new AccountService(context);
            _roleCheckingService = new RoleCheckingService(context);
        }

        [HttpGet]
        [Route("email/{email}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.ACCOUNT)]
        public ActionResult<AccountViewModel> GetByAccountEmail(string email)
        {
            var result = _accountService.GetAccountByEmail(email);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("username/{username}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.ACCOUNT)]
        public ActionResult<AccountViewModel> GetByAccountUsername(string username)
        {
            var result = _accountService.GetAccountByUsername(username);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Return a list of accounts matching some or all of the search parameter
        /// </summary>
        /// 
        /// 
        /// Full Explanation:
        /// 
        /// Returns a list of accounts ordered by key of a combination of users first/last/user name in the following order
        ///     1.first or last name begins with search query,
        ///     2.first or last name in Username that begins with search query
        ///     3.first or last name that contains the search query
        ///     
        /// If Full Names of any two accounts are the same the follow happens to the dictionary key to solve this problem
        ///     1. If there is a number attached to their account this is appened to the end of their key
        ///     2. Otherwise an '1' is appended to the end
        ///     
        /// Note:
        /// A '1' is added inbetween a key's first and last name or first and last username in order to preserve the presedence set by shorter names
        /// as both first and last are used as a part of the key in order to order matching first/last names with the remaining part of their name
        /// but this resulted in the presedence set by shorter names to be lost
        /// 
        /// Note:
        /// "z" s are added in order to keep each case split into each own group in the dictionary
        /// 
        /// <param name="searchString"> The input to search for </param>
        /// <returns> All accounts meeting some or all of the parameter</returns>
        [HttpGet]
        [Route("search/{searchString}")]
        public async Task<ActionResult<IEnumerable<BasicInfoViewModel>>> SearchAsync(string searchString)
        {
            var authenticatedUserUsername = AuthUtils.GetAuthenticatedUserUsername(User);
            var viewerType = _roleCheckingService.GetCollegeRole(authenticatedUserUsername);

            IEnumerable<BasicInfoViewModel> accounts = await _accountService.GetAllBasicInfoExceptAlumniAsync();

            int precedence = 0;

            var allMatches = new SortedDictionary<string, BasicInfoViewModel>();

            void appendMatch(string key, BasicInfoViewModel match)
            {
                while (allMatches.ContainsKey(key)) key += "1";
                allMatches.Add(key, match);

                accounts = accounts.Where(a => !a.Equals(match));
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                // First name exact match (Highest priority)
                foreach (var match in accounts.Where(s => s.FirstNameMatches(searchString)))
                {
                    string key = GenerateKey(match.FirstName, match.LastName, match.UserName, precedence);

                    appendMatch(key, match);

                    accounts = accounts.Where(a => !a.Equals(match));
                }
                precedence++;

                // Nickname exact match
                foreach (var match in accounts
                                        .Where(s => s.NicknameMatches(searchString)))
                {
                    string key = GenerateKey(match.FirstName, match.LastName, match.UserName, precedence);

                    appendMatch(key, match);
                }
                precedence++;

                // Last name exact match
                foreach (var match in accounts
                                        .Where(s => s.LastNameMatches(searchString)))
                {
                    string key = GenerateKey(match.LastName, match.FirstName, match.UserName, precedence);

                    appendMatch(key, match);
                }
                precedence++;

                // Maiden name exact match
                foreach (var match in accounts
                                        .Where(s => !allMatches.ContainsValue(s))
                                        .Where(s => s.MaidenNameMatches(searchString)))
                {
                    string key = GenerateKey(match.LastName, match.FirstName, match.UserName, precedence);

                    appendMatch(key, match);
                }
                precedence++;

                // First name starts with
                foreach (var match in accounts
                                        .Where(s => s.FirstNameStartsWith(searchString)))
                {
                    string key = GenerateKey(match.FirstName, match.LastName, match.UserName, precedence);

                    appendMatch(key, match);
                }
                precedence++;

                // Username (first name) starts with
                foreach (var match in accounts
                                        .Where(s => s.UsernameFirstNameStartsWith(searchString)))
                {
                    string key = GenerateKey(match.GetFirstNameFromUsername(), match.GetLastNameFromUsername(), match.UserName, precedence);

                    appendMatch(key, match);
                }
                precedence++;

                // Nickname starts with
                foreach (var match in accounts
                                        .Where(s => s.NicknameStartsWith(searchString)))
                {
                    string key = GenerateKey(match.FirstName, match.LastName, match.UserName, precedence);

                    appendMatch(key, match);
                }
                precedence++;

                // Last name starts with
                foreach (var match in accounts
                                        .Where(s => s.LastNameStartsWith(searchString)))
                {
                    string key = GenerateKey(match.LastName, match.FirstName, match.UserName, precedence);

                    appendMatch(key, match);
                }
                precedence++;

                // Maiden name starts with
                foreach (var match in accounts
                                        .Where(s => !allMatches.ContainsValue(s))
                                        .Where(s => s.MaidenNameStartsWith(searchString)))
                {
                    string key = GenerateKey(match.LastName, match.FirstName, match.UserName, precedence);

                    appendMatch(key, match);
                }
                precedence++;

                // Username (last name) starts with
                foreach (var match in accounts
                                        .Where(s => s.UsernameLastNameStartsWith(searchString)))
                {
                    string key = GenerateKey(match.GetLastNameFromUsername(), match.GetFirstNameFromUsername(), match.UserName, precedence);

                    appendMatch(key, match);
                }
                precedence++;

                // First name, last name, nickname, maidenname or username contains (Lowest priority)
                foreach (var match in accounts
                                        .Where(s => !allMatches.ContainsValue(s))
                                        .Where(s => s.FirstNameContains(searchString) || s.NicknameContains(searchString) || s.LastNameContains(searchString) || s.MaidenNameContains(searchString) || s.UsernameContains(searchString)))
                {
                    string key;
                    if (match.FirstNameContains(searchString))
                    {
                        key = GenerateKey(match.FirstName, match.LastName, match.UserName, precedence);
                    }
                    else if (match.NicknameContains(searchString))
                    {
                        key = GenerateKey(match.FirstName, match.LastName, match.UserName, precedence);
                    }
                    else if (match.LastNameContains(searchString))
                    {
                        key = GenerateKey(match.LastName, match.FirstName, match.UserName, precedence);
                    }
                    else if (match.MaidenNameContains(searchString))
                    {
                        key = GenerateKey(match.LastName, match.FirstName, match.UserName, precedence);
                    }
                    else {
                        key = GenerateKey(match.UserName, "", match.UserName, precedence);
                    }

                    appendMatch(key, match);
                }

                allMatches.OrderBy(m => m.Key);
                accounts = allMatches.Values;
            }

            // Return all of the 
            return Ok(accounts);
        }

        /// <summary>
        /// Return a list of accounts matching some or all of the search parameter
        /// We are searching through a concatonated string, containing several pieces of info about each user.
        /// </summary>
        /// <param name="searchString"> The input to search for </param>
        /// <param name="secondaryString"> The second piece of the search terms </param>
        /// <returns> All accounts meeting some or all of the parameter</returns>
        [HttpGet]
        [Route("search/{searchString}/{secondaryString}")]
        public async Task<ActionResult<IEnumerable<BasicInfoViewModel>>> SearchWithSpaceAsync(string searchString, string secondaryString)
        {
            var authenticatedUserUsername = AuthUtils.GetAuthenticatedUserUsername(User);
            var viewerType = _roleCheckingService.GetCollegeRole(authenticatedUserUsername);

            int precedence = 0;

            var allMatches = new SortedDictionary<string, BasicInfoViewModel>();

            // Create accounts viewmodel to search
            IEnumerable<BasicInfoViewModel> accounts = await _accountService.GetAllBasicInfoExceptAlumniAsync();

            void appendMatch(string key, BasicInfoViewModel match)
            {
                while (allMatches.ContainsKey(key)) key += "1";
                allMatches.Add(key, match);

            // Create accounts viewmodel to search
            }

            if (!string.IsNullOrEmpty(searchString) && !string.IsNullOrEmpty(secondaryString))
            {
                // Exact match in both first and last name (Highest priority)
                foreach (var match in accounts.Where(s => s.FirstNameMatches(searchString) && s.LastNameMatches(secondaryString)))
                {
                    string key = GenerateKey(match.FirstName, match.LastName, match.UserName, precedence);

                    appendMatch(key, match);
                }
                precedence++;

                // First name and last name start with
                foreach (var match in accounts
                                        .Where(s => s.FirstNameStartsWith(searchString) && s.LastNameStartsWith(secondaryString)))
                {
                    string key = GenerateKey(match.FirstName, match.LastName, match.UserName, precedence);

                    appendMatch(key, match);
                }
                precedence++;

                // Username (first and last) starts with
                foreach (var match in accounts
                                        .Where(s => s.UsernameFirstNameStartsWith(searchString) && s.UsernameLastNameStartsWith(secondaryString)))
                {
                    string key = GenerateKey(match.GetFirstNameFromUsername(), match.GetLastNameFromUsername(), match.UserName, precedence);

                    appendMatch(key, match);
                }
                precedence++;

                // Exact match in both nickname and last name
                foreach (var match in accounts
                                        .Where(s => !allMatches.ContainsValue(s))
                                        .Where(s => s.NicknameMatches(searchString) && s.LastNameMatches(secondaryString)))
                {
                    string key = GenerateKey(match.FirstName, match.LastName, match.UserName, precedence);

                    appendMatch(key, match);
                }
                precedence++;

                // Nickname and last name start with
                foreach (var match in accounts
                                        .Where(s => s.NicknameStartsWith(searchString) && s.LastNameStartsWith(secondaryString)))
                {
                    string key = GenerateKey(match.FirstName, match.LastName, match.UserName, precedence);

                    appendMatch(key, match);
                }

                // Exact match in both first name and maiden name
                foreach (var match in accounts
                                        .Where(s => !allMatches.ContainsValue(s))
                                        .Where(s => s.FirstNameMatches(searchString) && s.MaidenNameMatches(secondaryString)))
                {
                    string key = GenerateKey(match.FirstName, match.LastName, match.UserName, precedence);

                    appendMatch(key, match);
                }
                precedence++;

                // First name and maiden name start with (Lowest Priority)
                foreach (var match in accounts
                                        .Where(s => s.FirstNameStartsWith(searchString) && s.MaidenNameStartsWith(secondaryString)))
                {
                    string key = GenerateKey(match.FirstName, match.LastName, match.UserName, precedence);

                    appendMatch(key, match);
                }

                allMatches.OrderBy(s => s.Key);
                accounts = allMatches.Values;
            }

            // Return all of the 
            return Ok(accounts);
        }

        /// <summary>
        /// Return a list of accounts matching some or all of the search parameters
        /// We are searching through all the info of a user, then narrowing it down to get only what we want
        /// </summary>
        /// <param name="accountTypes"> Which account types to search. Accepted values: "student", "alumni", "facstaff"  </param>
        /// <param name="firstname"> The first name to search for </param>
        /// <param name="lastname"> The last name to search for </param>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="hall"></param>
        /// <param name="classType"></param>
        /// <param name="homeCity"></param>
        /// <param name="state"></param>
        /// <param name="country"></param>
        /// <param name="department"></param>   
        /// <param name="building"></param>     
        /// <returns> All accounts meeting some or all of the parameter</returns>
        [HttpGet]
        [Route("advanced-people-search")]
        public async Task<ActionResult<IEnumerable<AdvancedSearchViewModel>>> AdvancedPeopleSearchAsync(
            [FromQuery] string[] accountTypes,
            string? firstname = "",
            string? lastname = "",
            string? major = "",
            string? minor = "",
            string? hall = "",
            string? classType = "",
            string? homeCity = "",
            string? state = "",
            string? country = "",
            string? department = "",
            string? building = "")
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

            var authenticatedUserUsername = AuthUtils.GetAuthenticatedUserUsername(User);
            var viewerType = _roleCheckingService.GetCollegeRole(authenticatedUserUsername);

            // Create accounts viewmodel to search
            IEnumerable<AdvancedSearchViewModel> accounts = new List<AdvancedSearchViewModel>();
            if (accountTypes.Contains("student") && viewerType != Position.ALUMNI)
            {
                accounts = accounts.Union(_accountService.GetAllPublicStudentAccounts());
            }

            if (accountTypes.Contains("facstaff"))
            {
                if (string.IsNullOrEmpty(homeCity))
                {
                    accounts = accounts.Union(_accountService.GetAllPublicFacultyStaffAccounts());
                }
                else
                {
                    accounts = accounts.Union(_accountService.GetAllPublicFacultyStaffAccounts().Where(a => a.KeepPrivate == "0"));
                }
            }

            if (accountTypes.Contains("alumni") && viewerType != Position.STUDENT)
            {
                if (string.IsNullOrEmpty(homeCity))
                {
                    accounts = accounts.Union( _accountService.GetAllPublicAlumniAccounts());
                }
                else
                {
                    accounts = accounts.Union(_accountService.GetAllPublicAlumniAccounts().Where(a => a.ShareAddress.ToLower() != "n"));
                }
            }

            var searchResults =
                accounts
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

            // Return all of the profile views
            return Ok(searchResults);
        }

        /// <Summary>
        ///   This function generates a key for each account
        /// </Summary>
        ///
        /// <param name="keyPart1">This is what you would want to sort by first, used for first part of key</param>
        /// <param name="keyPart2">This is what you want to sort by second, used for second part of key</param>
        /// <param name="precedence">Set where in the dictionary this key group will be ordered</param>
        /// <param name="userName">The User's Username</param>
        public string GenerateKey(string keyPart1, string keyPart2, string userName, int precedence)
        {
            string key = keyPart1 + "1" + keyPart2;

            if (Regex.Match(userName, "[0-9]+").Success)
                key += Regex.Match(userName, "[0-9]+").Value;

            key = string.Concat(Enumerable.Repeat("z", precedence)) + key;

            return key;
        }
    }
}
