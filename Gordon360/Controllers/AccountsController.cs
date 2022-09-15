using Gordon360.Authorization;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Controllers
{
    [Route("api/[controller]")]
    public class AccountsController : GordonControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
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
            var accounts = await _accountService.GetAllBasicInfoExceptAlumniAsync();

            var matches = new ConcurrentDictionary<string, BasicInfoViewModel>();

            Parallel.ForEach(accounts, account =>
            {
                if (account.MatchSearch(searchString) is (string match, int precedence))
                {
                    var key = GenerateKey(match, precedence);
                    while (!matches.TryAdd(key, account)) key += "1";
                }
            });

            return Ok(matches.OrderBy(pair => pair.Key).Select(pair => pair.Value));
        }

        /// <summary>
        /// Return a list of accounts matching some or all of the search parameter
        /// We are searching through a concatonated string, containing several pieces of info about each user.
        /// </summary>
        /// <param name="firstnameSearch"> The firstname portion of the search</param>
        /// <param name="lastnameSearch"> The lastname portion of the search</param>
        /// <returns> All accounts matching some or all of both the firstname and lastname parameters</returns>
        [HttpGet]
        [Route("search/{firstnameSearch}/{lastnameSearch}")]
        public async Task<ActionResult<IEnumerable<BasicInfoViewModel>>> SearchWithSpaceAsync(string firstnameSearch, string lastnameSearch)
        {
            var accounts = await _accountService.GetAllBasicInfoExceptAlumniAsync();

            var matches = new ConcurrentDictionary<string, BasicInfoViewModel>();
            Parallel.ForEach(accounts, account =>
            {
                if (account.MatchSearch(firstnameSearch, lastnameSearch) is
                    (string firstnameMatch,
                     int    firstnamePrecedence,
                     string lastnameMatch,
                     int    lastnamePrecedence))
                {
                    string key = GenerateKey(firstnameMatch, lastnameMatch, firstnamePrecedence + lastnamePrecedence);
                    while (!matches.TryAdd(key, account)) key += "1";
                }
            });

            return Ok(matches.OrderBy(pair => pair.Key).Select(pair => pair.Value));
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
            [FromQuery] List<string> accountTypes,
            string? firstname,
            string? lastname,
            string? major,
            string? minor,
            string? hall,
            string? classType,
            string? homeCity,
            string? state,
            string? country,
            string? department,
            string? building)
        {
            var viewerGroups = AuthUtils.GetGroups(User);

            // Only students and FacStaff can search students
            if (accountTypes.Contains("student") && !(viewerGroups.Contains(AuthGroup.Student) || viewerGroups.Contains(AuthGroup.FacStaff)))
            {
                accountTypes.Remove("student");
            }

            var searchResults = _accountService.AdvancedSearch(
                accountTypes,
                firstname?.ToLower() ?? "",
                lastname?.ToLower() ?? "",
                major ?? "",
                minor ?? "",
                hall ?? "",
                classType ?? "",
                homeCity?.ToLower() ?? "",
                state ?? "",
                country ?? "",
                department ?? "",
                building ?? "");


            // Return all of the profile views
            return Ok(searchResults);
        }

        /// <Summary>
        ///   This function generates a key for each account
        ///   The key is of the form "z...keyBase" where z is repeated precedence times.
        /// </Summary>
        /// <remarks>
        ///   The leading precedence number of z's are used to put keep the highest precedence matches first.
        ///   The keyBase is used to sort within the precedence level.
        /// </remarks>
        ///
        /// <param name="keyBase">The base value to use for the key - i.e. the user's highest precedence info that matches the search string</param>
        /// <param name="precedence">Set where in the dictionary this key group will be ordered</param>
        private static string GenerateKey(string keyBase, int precedence)
        {
            return string.Concat(Enumerable.Repeat("z", precedence)) + keyBase;
        }


        /// <Summary>
        ///   This function generates a key for each account
        ///   The key is of the form "z...firstname1lastname" where z is repeated precedence times.
        /// </Summary>
        /// <remarks>
        ///   The leading precedence number of z's are used to put keep the highest precedence matches first.
        ///   The keyBase is used to sort within the precedence level.
        /// </remarks>
        ///
        /// <param name="firstnameKey">The firstname value to use for the key - i.e. the user's highest precedence firstname info that matches the search string</param>
        /// <param name="lastnameKey">The lastname value to use for the key - i.e. the user's highest precedence lastname info that matches the search string</param>
        /// <param name="precedence">Set where in the dictionary this key group will be ordered</param>
        private static string GenerateKey(string firstnameKey, string lastnameKey, int precedence)
        {
            return string.Concat(Enumerable.Repeat("z", precedence)) + $"{firstnameKey}1${lastnameKey}";
        }
    }
}
