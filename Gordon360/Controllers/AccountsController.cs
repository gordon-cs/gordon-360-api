using Gordon360.Authorization;
using Gordon360.Enums;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Gordon360.Static.Names;
using Microsoft.AspNetCore.Mvc;
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
        /// Return a list of accounts matching some or all of <c>searchString</c>.
        /// </summary>
        /// <param name="searchString">The input to search for</param>
        /// <returns>All accounts meeting some or all of the parameter, sorted according to how well the account matched the search.</returns>
        [HttpGet]
        [Route("search/{searchString}")]
        public async Task<ActionResult<IEnumerable<BasicInfoViewModel>>> SearchAsync(string searchString)
        {
            var accounts = await _accountService.GetAllBasicInfoExceptAlumniAsync();

            var searchResults = accounts.AsParallel()
                .Select(account => (matchKey: account.MatchSearch(searchString), account))
                .Where(pair => pair.matchKey is not null)
                .OrderBy(pair => pair.matchKey)
                .Select(pair => pair.account);

            return Ok(searchResults);
        }

        /// <summary>
        /// Return a list of accounts matching some or all of the search parameter.
        /// </summary>
        /// <param name="firstnameSearch">The firstname portion of the search</param>
        /// <param name="lastnameSearch">The lastname portion of the search</param>
        /// <returns> All accounts matching some or all of both the firstname and lastname parameters, sorted by how well the account matched the search.</returns>
        [HttpGet]
        [Route("search/{firstnameSearch}/{lastnameSearch}")]
        public async Task<ActionResult<IEnumerable<BasicInfoViewModel>>> SearchWithSpaceAsync(string firstnameSearch, string lastnameSearch)
        {
            var accounts = await _accountService.GetAllBasicInfoExceptAlumniAsync();

            var searchResults = accounts.AsParallel()
                .Select(account => (matchKey: account.MatchSearch(firstnameSearch, lastnameSearch), account))
                .Where(pair => pair.matchKey is not null)
                .OrderBy(pair => pair.matchKey)
                .Select(pair => pair.account);

            return Ok(searchResults);
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
        /// <param name="preferredClassYear"></param>
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
            string? preferredClassYear,
            string? homeCity,
            string? state,
            string? country,
            string? department,
            string? building)
        {
            IEnumerable<AuthGroup> viewerGroups = AuthUtils.GetGroups(User);

            var accounts = _accountService.GetAccountsToSearch(accountTypes, viewerGroups, homeCity);

            var searchResults = _accountService.AdvancedSearch(
                accounts,
                firstname,
                lastname,
                major,
                minor,
                hall,
                classType,
                preferredClassYear,
                homeCity,
                state,
                country,
                department,
                building);

            // Return all of the profile views
            return Ok(searchResults);
        }
    }
}
