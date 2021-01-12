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
using Newtonsoft.Json.Linq;
using Gordon360.AuthorizationFilters;

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
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.ACCOUNT)]
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

            String key;
            int precedence = 0;

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
                    key = GenerateKey(match.FirstName, match.LastName, match.UserName, precedence);

                    while (allMatches.ContainsKey(key)) key = key + "1";
                    allMatches.Add(key, match);
                }
                precedence++;

                // Last name exact match
                foreach (var match in accounts.Where(s => !allMatches.ContainsValue(s)).Where(s => s.LastName.ToLower() == searchString))
                {
                    key = GenerateKey(match.LastName, match.FirstName, match.UserName, precedence);

                    while (allMatches.ContainsKey(key)) key = key + "1";
                    allMatches.Add(key, match);
                }
                precedence++;

                // First name starts with
                foreach (var match in accounts.Where(s => !allMatches.ContainsValue(s)).Where(s => s.FirstName.ToLower().StartsWith(searchString)))
                {
                    key = GenerateKey(match.FirstName, match.LastName, match.UserName, precedence);

                    while (allMatches.ContainsKey(key)) key = key + "1";
                    allMatches.Add(key, match);
                }
                precedence++;

                // Username (first name) starts with
                foreach (var match in accounts.Where(s => !allMatches.ContainsValue(s)).Where(s => s.UserName.Contains('.') && s.UserName.Split('.')[0].ToLower().StartsWith(searchString)))
                {
                    key = GenerateKey(match.UserName.Split('.')[1], match.UserName.Split('.')[0], match.UserName, precedence);

                    while (allMatches.ContainsKey(key)) key = key + "1";
                    allMatches.Add(key, match);
                }
                precedence++;

                // Last name starts with
                foreach (var match in accounts.Where(s => !allMatches.ContainsValue(s)).Where(s => s.LastName.ToLower().StartsWith(searchString)))
                {
                    key = GenerateKey(match.LastName, match.FirstName, match.UserName, precedence);

                    while (allMatches.ContainsKey(key)) key = key + "1";
                    allMatches.Add(key, match);
                }
                precedence++;

                // Username (last name) starts with
                foreach (var match in accounts.Where(s => !allMatches.ContainsValue(s)).Where(s => s.UserName.Contains('.') && s.UserName.Split('.')[1].ToLower().StartsWith(searchString)))
                {
                    key = GenerateKey(match.UserName.Split('.')[0], match.UserName.Split('.')[1], match.UserName, precedence); 

                    while (allMatches.ContainsKey(key)) key = key + "1";
                    allMatches.Add(key, match);
                }
                precedence++;

                // First name, last name, or username contains (Lowest priority)
                foreach (var match in accounts.Where(s => !allMatches.ContainsValue(s)).Where(s => s.FirstName.ToLower().Contains(searchString) || s.LastName.ToLower().Contains(searchString) || s.UserName.ToLower().Contains(searchString)))
                {
                    if (match.FirstName.ToLower().Contains(searchString)) key = GenerateKey(match.FirstName, match.LastName, match.UserName, precedence);
                    else if (match.LastName.ToLower().Contains(searchString)) key = GenerateKey(match.LastName, match.FirstName, match.UserName, precedence);
                    else key = GenerateKey(match.UserName, "", match.UserName, precedence);
                    
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
        /// <param name="secondaryString"> The second piece of the search terms </param>
        /// <returns> All accounts meeting some or all of the parameter</returns>
        [HttpGet]
        [Route("search/{searchString}/{secondaryString}")]
        public IHttpActionResult SearchWithSpace(string searchString, string secondaryString)
        {
            //get token data from context, username is the username of current logged in person
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var viewerName = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var viewerType = _roleCheckingService.getCollegeRole(viewerName);

            String key;
            int precedence = 0;

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
                    key = GenerateKey(match.FirstName, match.LastName, match.UserName, precedence);

                    while (allMatches.ContainsKey(key)) key = key + "1";
                    allMatches.Add(key, match);
                }
                precedence++;

                // First name and last name start with
                foreach (var match in accounts.Where(s => !allMatches.ContainsValue(s)).Where(s => s.FirstName.ToLower().StartsWith(searchString) && s.LastName.ToLower().StartsWith(secondaryString)))
                {
                    key = GenerateKey(match.FirstName, match.LastName, match.UserName, precedence);

                    while (allMatches.ContainsKey(key)) key = key + '1';

                    allMatches.Add(key, match);
                }
                precedence++;

                // Username (first and last) starts with
                foreach (var match in accounts.Where(s => !allMatches.ContainsValue(s)).Where(s => s.UserName.Contains('.') && (s.UserName.Split('.')[0].ToLower().StartsWith(searchString) && s.UserName.Split('.')[1].ToLower().StartsWith(secondaryString))))
                {
                    key = GenerateKey(match.FirstName, match.LastName, match.UserName, precedence);

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
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.ACCOUNT)]
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

        /// <summary>
        /// Return a list of accounts matching some or all of the search parameters
        /// We are searching through all the info of a user, then narrowing it down to get only what we want
        /// </summary>
        /// <param name="includeAlumniSearchParam"> For non-students: Include Alumni in search results or not </param>
        /// <param name="firstNameSearchParam"> The first name to search for </param>
        /// <param name="lastNameSearchParam"> The last name to search for </param>
        /// <param name="majorSearchParam"></param>
        /// <param name="minorSearchParam"></param>
        /// <param name="hallSearchParam"></param>
        /// <param name="classTypeSearchParam"></param>
        /// <param name="hometownSearchParam"></param>
        /// <param name="stateSearchParam"></param>
        /// <param name="countrySearchParam"></param>
        /// <param name="departmentSearchParam"></param>   
        /// <param name="buildingSearchParam"></param>     
        /// <returns> All accounts meeting some or all of the parameter</returns>
        [HttpGet]
        [Route("advanced-people-search/{includeAlumniSearchParam}/{firstNameSearchParam}/{lastNameSearchParam}/{majorSearchParam}/{minorSearchParam}/{hallSearchParam}/{classTypeSearchParam}/{hometownSearchParam}/{stateSearchParam}/{countrySearchParam}/{departmentSearchParam}/{buildingSearchParam}")]
        public IHttpActionResult AdvancedPeopleSearch(bool includeAlumniSearchParam, string firstNameSearchParam, string lastNameSearchParam, string majorSearchParam, string minorSearchParam, string hallSearchParam, string classTypeSearchParam,  string hometownSearchParam, string stateSearchParam, string countrySearchParam, string departmentSearchParam, string buildingSearchParam)
        {
            System.Diagnostics.Debug.WriteLine("A.P.S. been called");


            System.Diagnostics.Debug.WriteLine("Values of each search param. IncludeAlumni?: " + includeAlumniSearchParam + " FirstName: " + firstNameSearchParam + "  LastName: " + lastNameSearchParam + "  Major: " + majorSearchParam +
                "  Minor: " + minorSearchParam + "  Class: " + classTypeSearchParam + "  Hometown: " + hometownSearchParam + "  State: " + stateSearchParam + "  Country: " + countrySearchParam + "  Dept: " + departmentSearchParam + "  Building: " + buildingSearchParam);
            
            string sampleBuildingDescription = Gordon360.Services.ComplexQueries.RawSqlQuery<String>.query("SELECT BuildingDescription from STUDENT where AD_Username = 'nathaniel.rudenberg'").Cast<string>().ElementAt(0);
            System.Diagnostics.Debug.WriteLine("Sample Building description from Database: " + sampleBuildingDescription);

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
            if (majorSearchParam == "C\u266F")
            {
                majorSearchParam = "";
            } else if (majorSearchParam.Contains("_") || majorSearchParam.Contains("dash"))
            {
                majorSearchParam = majorSearchParam.Replace("_", "&");
                majorSearchParam = majorSearchParam.Replace("dash", "-");
            }
            if (minorSearchParam == "C\u266F")
            {
                minorSearchParam = "";
            }
            if (hallSearchParam == "C\u266F")
            {
                hallSearchParam = "";
            }
            if (classTypeSearchParam == "C\u266F")
            {
                classTypeSearchParam = "";
            }
            if (stateSearchParam == "C\u266F")
            {
                stateSearchParam = "";
            }
            if (countrySearchParam == "C\u266F")
            {
                countrySearchParam = "";
            }
            if (departmentSearchParam == "C\u266F")
            {
                departmentSearchParam = "";
            }
            else if (departmentSearchParam.Contains("_"))
            {
                departmentSearchParam = departmentSearchParam.Replace("_", "&");
            }
            if (buildingSearchParam == "C\u266F")
            {
                buildingSearchParam = "";
            }
            else if (buildingSearchParam.Contains("_"))
            {
                buildingSearchParam = buildingSearchParam.Replace("_", ".");
            }

            //get token data from context, username is the username of current logged in person
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var viewerName = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            var viewerType = _roleCheckingService.getCollegeRole(viewerName);

            // Create accounts viewmodel to search
            var studentAccounts = Data.PublicStudentData;
            var facStaffAccounts = Data.PublicFacultyStaffData;
            var alumniAccounts = Data.PublicAlumniData;
            var accountsWithoutCurrentStudents = Data.AllPublicAccountsWithoutCurrentStudents;
            var accountsWithoutAlumni = Data.AllPublicAccountsWithoutAlumni;
            var accounts = Data.AllPublicAccounts;

            IEnumerable<JObject> searchResults;
            if (viewerType == Position.POLICE || viewerType == Position.FACSTAFF || viewerType == Position.SUPERADMIN)
            {
                if (!includeAlumniSearchParam)
                {
                    searchResults = accountsWithoutAlumni.Where(a => (a["FirstName"].ToString().ToLower().StartsWith(firstNameSearchParam)) && (a["LastName"].ToString().ToLower().StartsWith(lastNameSearchParam)) && ((a["Major1Description"].ToString().StartsWith(majorSearchParam)) || (a["Major2Description"].ToString().StartsWith(majorSearchParam)) || (a["Major3Description"].ToString().StartsWith(majorSearchParam))) && ((a["Minor1Description"].ToString().StartsWith(minorSearchParam)) || (a["Minor2Description"].ToString().StartsWith(minorSearchParam)) || (a["Minor3Description"].ToString().StartsWith(minorSearchParam))) && (a["Hall"].ToString().StartsWith(hallSearchParam)) && (a["Class"].ToString().StartsWith(classTypeSearchParam)) && (a["HomeCity"].ToString().ToLower().StartsWith(hometownSearchParam)) && (a["HomeState"].ToString().StartsWith(stateSearchParam)) && (a["Country"].ToString().StartsWith(countrySearchParam)) && (a["OnCampusDepartment"].ToString().StartsWith(departmentSearchParam)) && (a["BuildingDescription"].ToString().StartsWith(buildingSearchParam))).OrderBy(a => a["LastName"]).ThenBy(a => a["FirstName"]);
                } else
                {
                    searchResults = accounts.Where(a => (a["FirstName"].ToString().ToLower().StartsWith(firstNameSearchParam)) && (a["LastName"].ToString().ToLower().StartsWith(lastNameSearchParam)) && ((a["Major1Description"].ToString().StartsWith(majorSearchParam)) || (a["Major2Description"].ToString().StartsWith(majorSearchParam)) || (a["Major3Description"].ToString().StartsWith(majorSearchParam))) && ((a["Minor1Description"].ToString().StartsWith(minorSearchParam)) || (a["Minor2Description"].ToString().StartsWith(minorSearchParam)) || (a["Minor3Description"].ToString().StartsWith(minorSearchParam))) && (a["Hall"].ToString().StartsWith(hallSearchParam)) && (a["Class"].ToString().StartsWith(classTypeSearchParam)) && (a["HomeCity"].ToString().ToLower().StartsWith(hometownSearchParam)) && (a["HomeState"].ToString().StartsWith(stateSearchParam)) && (a["Country"].ToString().StartsWith(countrySearchParam)) && (a["OnCampusDepartment"].ToString().StartsWith(departmentSearchParam)) && (a["BuildingDescription"].ToString().StartsWith(buildingSearchParam))).OrderBy(a => a["LastName"]).ThenBy(a => a["FirstName"]);
                }
            }
            else if (viewerType == Position.STUDENT)
            {
                searchResults = accountsWithoutAlumni.Where(a => (a["FirstName"].ToString().ToLower().StartsWith(firstNameSearchParam)) && (a["LastName"].ToString().ToLower().StartsWith(lastNameSearchParam)) && ((a["Major1Description"].ToString().StartsWith(majorSearchParam)) || (a["Major2Description"].ToString().StartsWith(majorSearchParam)) || (a["Major3Description"].ToString().StartsWith(majorSearchParam))) && ((a["Minor1Description"].ToString().StartsWith(minorSearchParam)) || (a["Minor2Description"].ToString().StartsWith(minorSearchParam)) || (a["Minor3Description"].ToString().StartsWith(minorSearchParam))) && (a["Hall"].ToString().StartsWith(hallSearchParam)) && (a["Class"].ToString().StartsWith(classTypeSearchParam)) && (a["HomeCity"].ToString().ToLower().StartsWith(hometownSearchParam)) && (a["HomeState"].ToString().StartsWith(stateSearchParam)) && (a["Country"].ToString().StartsWith(countrySearchParam)) && (a["OnCampusDepartment"].ToString().StartsWith(departmentSearchParam)) && (a["BuildingDescription"].ToString().StartsWith(buildingSearchParam))).OrderBy(a => a["LastName"]).ThenBy(a => a["FirstName"]);
            } else // Alumni should not be able to see current students
            {
                searchResults = accountsWithoutCurrentStudents.Where(a => (a["FirstName"].ToString().ToLower().StartsWith(firstNameSearchParam)) && (a["LastName"].ToString().ToLower().StartsWith(lastNameSearchParam)) && ((a["Major1Description"].ToString().StartsWith(majorSearchParam)) || (a["Major2Description"].ToString().StartsWith(majorSearchParam)) || (a["Major3Description"].ToString().StartsWith(majorSearchParam))) && ((a["Minor1Description"].ToString().StartsWith(minorSearchParam)) || (a["Minor2Description"].ToString().StartsWith(minorSearchParam)) || (a["Minor3Description"].ToString().StartsWith(minorSearchParam))) && (a["Class"].ToString().StartsWith(classTypeSearchParam)) && (a["HomeCity"].ToString().ToLower().StartsWith(hometownSearchParam)) && (a["HomeState"].ToString().StartsWith(stateSearchParam)) && (a["Country"].ToString().StartsWith(countrySearchParam)) && (a["OnCampusDepartment"].ToString().StartsWith(departmentSearchParam)) && (a["BuildingDescription"].ToString().StartsWith(buildingSearchParam))).OrderBy(a => a["LastName"]).ThenBy(a => a["FirstName"]);
            }

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
