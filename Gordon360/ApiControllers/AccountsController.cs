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
        /// Return a list of accounts matching some or all of the search parameter: MAJOR
        /// We are searching through a concatonated string, containing several pieces of info about each user.
        /// </summary>
        /// <param name="majorSearchString"> The input to search for </param>
        /// <returns> All accounts meeting some or all of the parameter</returns>
        [HttpGet]
        [Route("search-major/{majorSearchString}")]
        public IHttpActionResult searchMajors(string majorSearchString)
        {
            //get token data from context, username is the username of current logged in person
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var viewerName = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var viewerType = _roleCheckingService.getCollegeRole(viewerName);

            // Create accounts viewmodel to search
            var studentAccounts = Data.StudentData;
            var facStaffAccounts = Data.FacultyStaffData;
            var alumniAccounts = Data.AlumniData;
            IList<BasicInfoViewModel> basicAccountInfo = new List<BasicInfoViewModel>();

            // Create accounts viewmodel to search
            switch (viewerType)
            {
                case Position.SUPERADMIN:
                    studentAccounts = Data.StudentData;
                    facStaffAccounts = Data.FacultyStaffData;
                    alumniAccounts = Data.AlumniData;
                    break;

                case Position.POLICE:
                    studentAccounts = Data.StudentData;
                    facStaffAccounts = Data.FacultyStaffData;
                    alumniAccounts = Data.AlumniData;
                    break;

                case Position.STUDENT:
                    studentAccounts = Data.StudentData;
                    facStaffAccounts = Data.FacultyStaffData;
                    break;

                case Position.FACSTAFF:
                    studentAccounts = Data.StudentData;
                    facStaffAccounts = Data.FacultyStaffData;
                    alumniAccounts = Data.AlumniData;
                    break;
            }

            if (!String.IsNullOrEmpty(majorSearchString))
            {
                // for every stored account, convert it to lowercase and compare it to the search parameter 
                studentAccounts = studentAccounts.Where(s => s.Major1Description.ToLower().Contains(majorSearchString) || s.Major2Description.ToLower().Contains(majorSearchString) || s.Major3Description.ToLower().Contains(majorSearchString)) ;
                studentAccounts = studentAccounts.OrderBy(s => s.LastName).ThenBy(s => s.FirstName);

                alumniAccounts = alumniAccounts.Where(s => s.Major1Description.ToLower().Contains(majorSearchString) || s.Major2Description.ToLower().Contains(majorSearchString));
                alumniAccounts = alumniAccounts.OrderBy(s => s.LastName).ThenBy(s => s.FirstName);

                //foreach (Models.Student student in studentAccounts)
               // {

                 //   BasicInfoViewModel accountViewModel = new BasicInfoViewModel
                   // {
                       // FirstName = student.FirstName,
                     //   LastName = student.LastName,
                        //UserName = student.AD_Username ?? "",
                        //ConcatonatedInfo = ""
                    //};

                    //basicAccountInfo.Add(accountViewModel);
                //}
            }

            // Return all of the 
            return Ok(studentAccounts);
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
