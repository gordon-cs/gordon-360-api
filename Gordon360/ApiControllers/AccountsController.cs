using System;
using System.Security.Claims;
using System.Linq;
using Gordon360.Static.Data;
using Gordon360.Static.Names;
using System.Web.Http;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.Exceptions.CustomExceptions;
using System.Collections.Generic;
using Gordon360.Models.ViewModels;
using System.Collections;
using System.Text.RegularExpressions;

namespace Gordon360.ApiControllers
{
    [Authorize]
    [CustomExceptionFilter]
    [RoutePrefix("api/accounts")]
    public class AccountsController : ApiController
    {
        private IRoleCheckingService _roleCheckingService;

        IAccountService _accountService;

        public AccountsController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
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
        public IHttpActionResult Search(string searchString)
        {
            //get token data from context, username is the username of current logged in person
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var viewerName = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var viewerType = _roleCheckingService.getCollegeRole(viewerName);

            var accounts = Data.AllBasicInfo;

            var allMatches = new SortedDictionary<String, BasicInfoViewModel>();

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

            if (!String.IsNullOrEmpty(searchString)) {

                // First name exact match (Highest priority)
                foreach (var match in accounts.Where(s => s.FirstName.ToLower() == searchString))
                {
                    String key = GenerateKey(match.FirstName, match.LastName, match.UserName, 0);

                    while (allMatches.ContainsKey(key)) key = key + "1";
                    allMatches.Add(key, match);
                }

                // Last name exact match
                foreach (var match in accounts.Where(s => !allMatches.ContainsValue(s)).Where(s => s.LastName.ToLower() == searchString))
                {
                    String key = GenerateKey(match.LastName, match.FirstName, match.UserName, 1);

                    while (allMatches.ContainsKey(key)) key = key + "1";
                    allMatches.Add(key, match);
                }

                // First name starts with
                foreach (var match in accounts.Where(s => !allMatches.ContainsValue(s)).Where(s => s.FirstName.ToLower().StartsWith(searchString)))
                {
                    String key = GenerateKey(match.FirstName, match.LastName, match.UserName, 2);

                    while (allMatches.ContainsKey(key)) key = key + "1";
                    allMatches.Add(key, match);
                }

                // Last name starts with
                foreach (var match in accounts.Where(s => !allMatches.ContainsValue(s)).Where(s => s.LastName.ToLower().StartsWith(searchString)))
                {
                    String key = GenerateKey(match.LastName, match.FirstName, match.UserName, 3);

                    while (allMatches.ContainsKey(key)) key = key + "1";
                    allMatches.Add(key, match);
                }
                
                // Username (first or last name) starts with
                foreach(var match in accounts.Where(s => !allMatches.ContainsValue(s)).Where(s => s.UserName.Contains('.') && (s.UserName.Split('.')[0].ToLower().StartsWith(searchString) || s.UserName.Split('.')[1].ToLower().StartsWith(searchString))))
                {
                    String key;
                    if (match.UserName.Split('.')[0].ToLower().StartsWith(searchString))
                        key = GenerateKey(match.UserName.Split('.')[0], match.UserName.Split('.')[1], match.UserName, 4); 
                    else
                        key = GenerateKey(match.UserName.Split('.')[1], match.UserName.Split('.')[0], match.UserName, 4);


                    while (allMatches.ContainsKey(key)) key = key + "1";
                    allMatches.Add(key, match);
                }

                // First name, last name, or username contains (Lowest priority)
                foreach(var match in accounts.Where(s => !allMatches.ContainsValue(s)).Where(s => s.FirstName.ToLower().Contains(searchString) || s.LastName.ToLower().Contains(searchString) || s.UserName.ToLower().Contains(searchString)))
                {
                   
                    String key;

                    if (match.FirstName.ToLower().Contains(searchString)) key = GenerateKey(match.FirstName, match.LastName, match.UserName, 5);
                    else if (match.LastName.ToLower().Contains(searchString)) key = GenerateKey(match.LastName, match.FirstName, match.UserName, 5);
                    else key = GenerateKey(match.UserName, "", match.UserName, 5);
                    
                    while (allMatches.ContainsKey(key)) key = key + '1';
                    allMatches.Add(key, match);
                }

                allMatches.OrderBy(s => s.Key);
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
        /// <returns> All accounts meeting some or all of the parameter</returns>
        [HttpGet]
        [Route("search/{searchString}/{secondaryString}")]
        public IHttpActionResult SearchWithSpace(string searchString, string secondaryString)
        {
            //get token data from context, username is the username of current logged in person
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var viewerName = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var viewerType = _roleCheckingService.getCollegeRole(viewerName);
            var allMatches = new SortedDictionary<String, BasicInfoViewModel>();
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
                // Exact match in both first and last name (Highest priority)
                foreach (var match in accounts.Where(s => s.FirstName.ToLower() == searchString && s.LastName.ToLower() == secondaryString))
                {
                    String key = GenerateKey(match.FirstName, match.LastName, match.UserName, 0);

                    while (allMatches.ContainsKey(key)) key = key + "1";
                    allMatches.Add(key, match);
                }

                // Exact match in first name
                foreach (var match in accounts.Where(s => !allMatches.ContainsValue(s)).Where(s => s.FirstName.ToLower() == searchString))
                {
                    String key = GenerateKey(match.FirstName, match.LastName, match.UserName, 1);

                    while (allMatches.ContainsKey(key)) key = key + "1";
                    allMatches.Add(key, match);
                }

                // Exact match in last name
                foreach (var match in accounts.Where(s => !allMatches.ContainsValue(s)).Where(s => s.LastName.ToLower() == secondaryString))
                {
                    String key = GenerateKey(match.LastName, match.FirstName, match.UserName, 2);

                    while (allMatches.ContainsKey(key)) key = key + "1";
                    allMatches.Add(key, match);
                }

                // First name and last name start with
                foreach (var match in accounts.Where(s => !allMatches.ContainsValue(s)).Where(s => s.FirstName.ToLower().StartsWith(searchString) && s.LastName.ToLower().StartsWith(secondaryString)))
                {
                    String key = GenerateKey(match.FirstName, match.LastName, match.UserName, 3);

                    while (allMatches.ContainsKey(key)) key = key + '1';

                    allMatches.Add(key, match);
                }

                // Username (first and last) starts with
                foreach (var match in accounts.Where(s => !allMatches.ContainsValue(s)).Where(s => s.UserName.Contains('.') && (s.UserName.Split('.')[0].ToLower().StartsWith(searchString) && s.UserName.Split('.')[1].ToLower().StartsWith(secondaryString))))
                {
                    String key = GenerateKey(match.FirstName, match.LastName, match.UserName, 4);

                    while (allMatches.ContainsKey(key)) key = key + '1';

                    allMatches.Add(key, match);
                }


                allMatches.OrderBy(s => s.Key);
                accounts = allMatches.Values;
            }

            // Return all of the 
            return Ok(accounts);
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
        /*
         * This function generates a key for each account
         * 
         * 
         * 
         */
        public String GenerateKey (String keyPart1, String keyPart2, String userName, int precedence)
        {
            String key =  keyPart1 + "1" + keyPart2 ;

            if (Regex.Match(userName, "[0-9]+").Success)
                key += Regex.Match(userName, "[0-9]+").Value;

            key = String.Concat(Enumerable.Repeat("z", precedence)) + key;

            return key;
        }

    }

  
    
}
