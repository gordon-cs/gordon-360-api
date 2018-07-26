using System;
using System.Security.Claims;
using System.Linq;
using Gordon360.Static.Data;
using Gordon360.Static.Names;
using System.Web.Http;
using System.Collections.Generic;
using Gordon360.Models.ViewModels;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Static.Methods;
using Newtonsoft.Json.Linq;

namespace Gordon360.ApiControllers
{
    [Authorize]
    [CustomExceptionFilter]
    [RoutePrefix("api/accounts")]
    public class AccountsController : ApiController
    {
        private IRoleCheckingService _roleCheckingService;
        private IProfileService _profileService;
        IAccountService _accountService;

        public AccountsController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _profileService = new ProfileService(_unitOfWork);
            _accountService = new AccountService(_unitOfWork);
            _roleCheckingService = new RoleCheckingService(_unitOfWork);
        }

        // GET: api/Accounts
        [HttpGet]
        [Route("email/{email}")]
        public IHttpActionResult GetByAccountEmail(string email)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(email))
            {
                string errors = "";
                foreach (var modelstate in ModelState.Values)
                {
                    foreach (var error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                throw new BadInputException() { ExceptionMessage = errors };
            }

            var result = _accountService.GetAccountByEmail(email);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


        /// <summary>
        /// Return a list of accounts matching some or all of the search parameter
        /// We are searching through a concatonated string, containing several pieces of info about each user.
        /// </summary>
        /// <param name="searchString"> The input to search for </param>
        /// <returns> All accounts meeting some or all of the parameter</returns>
        [HttpGet]
        [Route("search/{searchString}")]
        public IHttpActionResult Search(string searchString)
        {

            Console.WriteLine("in regular search");
            //get token data from context, username is the username of current logged in person
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var viewerName = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var viewerType = _roleCheckingService.getCollegeRole(viewerName);

            var accounts = Data.AllBasicInfo;

            // Create accounts viewmodel to search
            switch (viewerType)
            {
                case Position.SUPERADMIN:
                    accounts = Data.AllBasicInfo;
                    break;

                case Position.POLICE:
                    accounts = Data.AllBasicInfo;
                    break;

                case Position.STUDENT:
                    accounts = Data.AllBasicInfoWithoutAlumni;
                    break;

                case Position.FACSTAFF:

                    accounts = Data.AllBasicInfo;
                    break;
            }


            if (!String.IsNullOrEmpty(searchString))
            {
                // for every stored account, convert it to lowercase and compare it to the search paramter 
                accounts = accounts.Where(s => s.ConcatonatedInfo.ToLower().Contains(searchString));
                accounts = accounts.OrderBy(s => s.FirstName.CompareTo(searchString)).ThenBy(s => s.LastName.CompareTo(searchString));

            }

            // Return all of the 
            return Ok(accounts);
        }

        /// <summary>
        /// Return a list of accounts matching some or all of the search parameter
        /// We are searching through a concatonated string, containing several pieces of info about each user.
        /// </summary>
        /// <param name="searchString"> The input to search for </param>
        /// <returns> All accounts meeting some or all of the parameter</returns>
        [HttpGet]
        [Route("search/{searchString}/{secondaryString}")]
        public IHttpActionResult SearchWithSpace(string searchString, string secondaryString)
        {
            Console.WriteLine("in search with space");
            //get token data from context, username is the username of current logged in person
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var viewerName = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var viewerType = _roleCheckingService.getCollegeRole(viewerName);

            // Create accounts viewmodel to search
            var accounts = Data.AllBasicInfo;

            // Create accounts viewmodel to search
            switch (viewerType)
            {
                case Position.SUPERADMIN:
                    accounts = Data.AllBasicInfo;
                    break;

                case Position.POLICE:
                    accounts = Data.AllBasicInfo;
                    break;

                case Position.STUDENT:
                    accounts = Data.AllBasicInfoWithoutAlumni;
                    break;

                case Position.FACSTAFF:

                    accounts = Data.AllBasicInfo;
                    break;
            }

            if (!String.IsNullOrEmpty(searchString) && !String.IsNullOrEmpty(secondaryString))
            {
                // for every stored account, convert it to lowercase and compare it to the search paramter 
                accounts = accounts.Where(s => s.ConcatonatedInfo.ToLower().Contains(searchString) && s.ConcatonatedInfo.ToLower().Contains(secondaryString));
                accounts = accounts.OrderBy(s => s.LastName.CompareTo(secondaryString)).ThenBy(s => s.FirstName.CompareTo(searchString));
            }

            // Return all of the 
            return Ok(accounts);
        }

        /// <summary>
        /// Return a list of accounts matching some or all of the search parameters
        /// We are searching through all the info of a user, then narrowing it down to get only what we want
        /// </summary>
        /// <param name="firstNameSearchParam"> The first name to search for </param>
        /// <param name="lastNameSearchParam"> The last name to search for </param>
        /// <param name="hometownSearchParam"></param>        
        /// <returns> All accounts meeting some or all of the parameter</returns>
        [HttpGet]
        [Route("advanced-people-search/{firstNameSearchParam}/{lastNameSearchParam}/{hometownSearchParam}")]
        public IHttpActionResult advancedPeopleSearch(string firstNameSearchParam, string lastNameSearchParam, string hometownSearchParam)
        {
            System.Diagnostics.Debug.WriteLine("A.P.S. been called");
            // If any search params were not entered, set them to empty strings
            if (firstNameSearchParam == "C\u266F")
            {
                firstNameSearchParam = "";
            }
            if (lastNameSearchParam == "C\u266F")
            {
                lastNameSearchParam = "";
            }
            if (hometownSearchParam == "C\u266F")
            {
                hometownSearchParam = "";
            }

            //get token data from context, username is the username of current logged in person
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var viewerName = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var viewerType = _roleCheckingService.getCollegeRole(viewerName);

            // Create accounts viewmodel to search
            var studentAccounts = Data.PublicStudentData;
            var facStaffAccounts = Data.PublicFacultyStaffData;
            var alumniAccounts = Data.PublicAlumniData;
            var accountsWithoutAlumni = Data.AllPublicAccountsWithoutAlumni;
            var accounts = Data.AllPublicAccounts;

            // Create accounts viewmodel to search
            switch (viewerType)
            {
                case Position.SUPERADMIN:
                    studentAccounts = Data.PublicStudentData;
                    facStaffAccounts = Data.PublicFacultyStaffData;
                    alumniAccounts = Data.PublicAlumniData;
                    break;

                case Position.POLICE:
                    studentAccounts = Data.PublicStudentData;
                    facStaffAccounts = Data.PublicFacultyStaffData;
                    alumniAccounts = Data.PublicAlumniData;
                    break;


                case Position.STUDENT:
                    studentAccounts = Data.PublicStudentData;
                    facStaffAccounts = Data.PublicFacultyStaffData;
                    break;

                case Position.FACSTAFF:
                    studentAccounts = Data.PublicStudentData;
                    facStaffAccounts = Data.PublicFacultyStaffData;
                    alumniAccounts = Data.PublicAlumniData;
                    break;

            }
            IEnumerable<JObject> searchResults;
            if (viewerType != Position.STUDENT)
            {
                searchResults = accounts.Where(a => (a["FirstName"].ToString().ToLower().StartsWith(firstNameSearchParam)) && (a["LastName"].ToString().ToLower().StartsWith(lastNameSearchParam)) && (a["HomeCity"].ToString().ToLower().StartsWith(hometownSearchParam))).OrderBy(a => a["LastName"]).ThenBy(a => a["FirstName"]);
            }
            else
            {
                searchResults = accountsWithoutAlumni.Where(a => (a["FirstName"].ToString().ToLower().StartsWith(firstNameSearchParam)) && (a["LastName"].ToString().ToLower().StartsWith(lastNameSearchParam)) && (a["HomeCity"].ToString().ToLower().StartsWith(hometownSearchParam))).OrderBy(a => a["LastName"]).ThenBy(a => a["FirstName"]);
            }

            // Return all of the profile views
            return Ok(searchResults);
        }

        // GET: api/Accounts
        [HttpGet]
        [Route("username/{username}")]
        public IHttpActionResult GetByAccountUsername(string username)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(username))
            {
                string errors = "";
                foreach (var modelstate in ModelState.Values)
                {
                    foreach (var error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                throw new BadInputException() { ExceptionMessage = errors };
            }

            var result = _accountService.GetAccountByUsername(username);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

    }



}
