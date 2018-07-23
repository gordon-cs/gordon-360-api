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
        /// We are searching through a concatonated string, containing several pieces of info about each user.
        /// </summary>
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

            var BeginningMatches = new SortedDictionary<String, BasicInfoViewModel>();
            var ContainingMatches = new SortedDictionary<String, BasicInfoViewModel>();

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
                // for every stored account, convert it to lowercase and compare it to the search paramter 
                //accounts = accounts.Where(s => s.ConcatonatedInfo.ToLower().Contains(searchString));
                foreach(var match in accounts.Where(s => s.FirstName.ToLower().StartsWith(searchString) || s.LastName.ToLower().StartsWith(searchString)))
                {
                    String key;

                    if (match.FirstName.ToLower().StartsWith(searchString)) key = match.FirstName + "1" + match.LastName;
                    else key = match.LastName + "1" + match.FirstName;

                    if (Regex.Match(match.UserName, "[0-9]+").Success)
                        key += Regex.Match(match.UserName, "[0-9]+").Value;

                    while (BeginningMatches.ContainsKey(key)) key = key + "1";
                    System.Diagnostics.Debug.WriteLine(key);
                    BeginningMatches.Add(key, match);
                }
                
                //var FirstLast = new int[2];
                foreach(var match in accounts.Where(s => !BeginningMatches.ContainsValue(s)).Where(s=> (s.UserName.Split('.'))[0].ToLower().StartsWith(searchString) || (s.UserName.Split('.'))[1].ToLower().StartsWith(searchString)))
                {
                    String key;
                    if (match.UserName.Split('.')[0].ToLower().StartsWith(searchString)) key = match.UserName.Split('.')[0];
                    else
                        key = match.UserName.Split('.')[1];

                    if (Regex.Match(match.UserName, "[0-9]+").Success)
                        key += Regex.Match(match.UserName, "[0-9]+").Value;

                    key = 'z' + key;

                }
                

                
                
                foreach(var match in accounts.Where(s => !BeginningMatches.ContainsValue(s)).Where(s => s.FirstName.ToLower().Contains(searchString) || s.LastName.ToLower().Contains(searchString) || s.UserName.ToLower().Contains(searchString)))
                {
                    System.Diagnostics.Debug.WriteLine(match.UserName);
                    String key;

                    if (match.FirstName.ToLower().Contains(searchString)) key = match.FirstName + "1" + match.LastName;
                    else if (match.LastName.ToLower().Contains(searchString)) key = match.LastName + "1" + match.FirstName;
                    else key = match.UserName;


                    if (Regex.Match(match.UserName, "[0-9]+").Success)
                        key += Regex.Match(match.UserName, "[0-9]+").Value;

                    key = "zz" + key;
                    
                    while (BeginningMatches.ContainsKey(key)) key = key + 'a';
                    System.Diagnostics.Debug.WriteLine(key);
                    BeginningMatches.Add(key, match);

                }
                //accounts = accounts.OrderBy(s => s.FirstName.CompareTo(searchString)).ThenBy(s => s.LastName.CompareTo(searchString));

                BeginningMatches.OrderBy(s => s.Key);
                //ContainingMatches.OrderBy(s => s.Key);
                System.Diagnostics.Debug.WriteLine("------------");
                accounts = BeginningMatches.Values;
                foreach (var x in accounts)
                    System.Diagnostics.Debug.WriteLine(x.FirstName, x.LastName);
                
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
