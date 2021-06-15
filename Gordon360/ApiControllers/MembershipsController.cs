using System.Web.Http;
using Gordon360.Models;
using Gordon360.Utils;
using Gordon360.Repositories;
using Gordon360.Models.ViewModels;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using System;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Exceptions.CustomExceptions;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/memberships")]
    [Authorize]
    [CustomExceptionFilter]
    public class MembershipsController : ApiController
    {

        private IMembershipService _membershipService;
        private IAccountService _accountService;
        private IActivityService _activityService;

        public MembershipsController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _membershipService = new MembershipService(_unitOfWork);
            _accountService = new AccountService(_unitOfWork);
            _activityService = new ActivityService(_unitOfWork);
        }
        public MembershipsController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        /// <summary>
        /// Get all memberships
        /// </summary>
        /// <returns>
        /// A list of all memberships
        /// </returns>
        /// <remarks>
        /// Server makes call to the database and returns all current memberships
        /// </remarks>
        // GET api/<controller>
        [HttpGet]
        [Route("")]
        [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.MEMBERSHIP)]
        public IHttpActionResult Get()
        {
            
            var result = _membershipService.GetAll();
            return Ok(result);
        }

        /// <summary>
        /// Get all the memberships associated with a given activity
        /// </summary>
        /// <param name="id">The activity ID</param>
        /// <returns>IHttpActionResult</returns>
        [HttpGet]
        [Route("activity/{id}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.MEMBERSHIP_BY_ACTIVITY)]
        public IHttpActionResult GetMembershipsForActivity(string id)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
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

            var result = _membershipService.GetMembershipsForActivity(id);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets the group admin memberships associated with a given activity.
        /// </summary>
        /// <param name="id">The activity ID.</param>
        /// <returns>A list of all leader-type memberships for the specified activity.</returns>
        [HttpGet]
        [Route("activity/{id}/group-admin")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.GROUP_ADMIN_BY_ACTIVITY)]
        public IHttpActionResult GetGroupAdminForActivity(string id)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
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
            var result = _membershipService.GetGroupAdminMembershipsForActivity(id);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets the leader-type memberships associated with a given activity.
        /// </summary>
        /// <param name="id">The activity ID.</param>
        /// <returns>A list of all leader-type memberships for the specified activity.</returns>
        [HttpGet]
        [Route("activity/{id}/leaders")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.LEADER_BY_ACTIVITY)]
        public IHttpActionResult GetLeadersForActivity(string id)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
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
            var result = _membershipService.GetLeaderMembershipsForActivity(id);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets the advisor-type memberships associated with a given activity.
        /// </summary>
        /// <param name="id">The activity ID.</param>
        /// <returns>A list of all advisor-type memberships for the specified activity.</returns>
        [HttpGet]
        [Route("activity/{id}/advisors")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.ADVISOR_BY_ACTIVITY)]
        public IHttpActionResult GetAdvisorsForActivity(string id)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
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
            var result = _membershipService.GetAdvisorMembershipsForActivity(id);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets the number of followers of an activity
        /// </summary>
        /// <param name="id">The activity ID.</param>
        /// <returns>The number of followers of the activity</returns>
        [HttpGet]
        [Route("activity/{id}/followers")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.MEMBERSHIP)]
        public IHttpActionResult GetActivityFollowersCount(string id)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
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
            var result = _membershipService.GetActivityFollowersCount(id);

            return Ok(result);
        }

        /// <summary>
        /// Gets the number of members (besides followers) of an activity
        /// </summary>
        /// <param name="id">The activity ID.</param>
        /// <returns>The number of members of the activity</returns>
        [HttpGet]
        [Route("activity/{id}/members")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.MEMBERSHIP)]
        public IHttpActionResult GetActivityMembersCount(string id)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
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
            var result = _membershipService.GetActivityMembersCount(id);

            return Ok(result);
        }

        /// <summary>
        /// Gets the number of followers of an activity
        /// </summary>
        /// <param name="id">The activity ID.</param>
        /// <param name="sess_cde">The session code</param>
        /// <returns>The number of followers of the activity</returns>
        [HttpGet]
        [Route("activity/{id}/followers/{sess_cde}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.MEMBERSHIP)]
        public IHttpActionResult GetActivityFollowersCountForSession(string id, string sess_cde)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
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
            var result = _membershipService.GetActivityFollowersCountForSession(id, sess_cde);

            return Ok(result);
        }

        /// <summary>
        /// Gets the number of members (excluding followers) of an activity
        /// </summary>
        /// <param name="id">The activity ID.</param>
        /// <param name="sess_cde">The session code</param>
        /// <returns>The number of members of the activity</returns>
        [HttpGet]
        [Route("activity/{id}/members/{sess_cde}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.MEMBERSHIP)]
        public IHttpActionResult GetActivityMembersCountForSession(string id, string sess_cde)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(id))
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
            var result = _membershipService.GetActivityMembersCountForSession(id, sess_cde);

            return Ok(result);
        }

        /// <summary>Create a new membership item to be added to database</summary>
        /// <param name="membership">The membership item containing all required and relevant information</param>
        /// <returns></returns>
        /// <remarks>Posts a new membership to the server to be added into the database</remarks>
        // POST api/<controller>
        [HttpPost]
        [Route("", Name="Memberships")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.MEMBERSHIP)]
        public IHttpActionResult Post([FromBody] MEMBERSHIP membership)
        {
            if(!ModelState.IsValid || membership == null)
            {
                string errors = "";
                foreach (var modelstate in ModelState.Values)
                {
                    foreach(var error in modelstate.Errors)
                    {
                        errors += "|" + error.ErrorMessage + "|" + error.Exception;
                    }

                }
                throw new BadInputException() { ExceptionMessage = errors };
            }

            var result = _membershipService.Add(membership);

            if ( result == null)
            {
                return NotFound();
            }

            return Created("memberships", membership);

        }

        /// <summary>Fetch memberships that a specific student has been a part of</summary>
        /// <param name="id">The Student id</param>
        /// <returns>The membership information that the student is a part of</returns>
        [Route("student/{id}")]
        [HttpGet]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.MEMBERSHIP_BY_STUDENT)]
        public IHttpActionResult GetMembershipsForStudent(string id)
        {

            if (!ModelState.IsValid || String.IsNullOrWhiteSpace(id))
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
            var result = _membershipService.GetMembershipsForStudent(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        /// <summary>Fetch memberships that a specific student has been a part of</summary>
        /// <param name="username">The Student Username</param>
        /// <returns>The membership information that the student is a part of</returns>
        [Route("student/username/{username}")]
        [HttpGet]
        public IHttpActionResult GetMembershipsForStudentByUsename(string username)
        {

            if (!ModelState.IsValid || String.IsNullOrWhiteSpace(username))
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

            var id = _accountService.GetAccountByUsername(username).GordonID;
            var result = _membershipService.GetMembershipsForStudent(id);

            if (result == null)
            {
                return NotFound();
            }
            // privacy control of membership view model
            var authenticatedUser = this.ActionContext.RequestContext.Principal as ClaimsPrincipal;
            var viewerID = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "id").Value;
            var viewerName = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "user_name").Value;
            bool issuperAdmin = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "college_role").Value.Equals(Position.SUPERADMIN);
            bool isPolice = authenticatedUser.Claims.FirstOrDefault(x => x.Type == "college_role").Value.Equals(Position.POLICE);
            if (issuperAdmin||isPolice)                    //super admin and gordon police reads all
                return Ok(result);
            else
            {
                List<MembershipViewModel> list = new List<MembershipViewModel>();
                foreach (var item in result)
                {
                    var act = _activityService.Get(item.ActivityCode);
                    var admins = _membershipService.GetGroupAdminMembershipsForActivity(item.ActivityCode);
                    bool groupAdmin = false;
                    foreach (var admin in admins)               // group admin of a group can read membership of this group
                    {
                        if (admin.IDNumber.ToString().Equals(viewerID))
                            groupAdmin = true;
                    }
                    if (groupAdmin)
                    {
                        list.Add(item);
                    }
                    else if (act.Privacy != true)               // check group privacy
                    {
                        if (item.Privacy != true)               // check personal membership privacy
                        {
                            list.Add(item);
                        }
                    }
                }
                return Ok(list);
            }
        }

        /// <summary>Update an existing membership item</summary>
        /// <param name="id">The membership id of whichever one is to be changed</param>
        /// <param name="membership">The content within the membership that is to be changed and what it will change to</param>
        /// <remarks>Calls the server to make a call and update the database with the changed information</remarks>
        // PUT api/<controller>/5
        [HttpPut]
        [Route("{id}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.MEMBERSHIP)]
        public IHttpActionResult Put(int id, [FromBody]MEMBERSHIP membership)
        {
            if (!ModelState.IsValid || membership == null || id != membership.MEMBERSHIP_ID)
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

            var result = _membershipService.Update(id, membership);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(membership);
        }

        /// <summary>Update an existing membership item to be a group admin or not</summary>
        ///  /// <param name="membership">The content within the membership that is to be changed</param>
        /// <remarks>Calls the server to make a call and update the database with the changed information</remarks>
        [HttpPut]
        [Route("{id}/group-admin")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.MEMBERSHIP)]
        public IHttpActionResult ToggleGroupAdmin([FromBody]MEMBERSHIP membership)
        {
            if (!ModelState.IsValid || membership == null)
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
            var id = membership.MEMBERSHIP_ID;

            var result = _membershipService.ToggleGroupAdmin(id, membership);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>Update an existing membership item to be private or not</summary>
        /// <param name="id">The id of the membership</param>
        /// <param name = "p">the boolean value</param>
        /// <remarks>Calls the server to make a call and update the database with the changed information</remarks>
        [HttpPut]
        [Route("{id}/privacy/{p}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.MEMBERSHIP_PRIVACY)]
        public IHttpActionResult TogglePrivacy(int id, bool p)
        {
            if (!ModelState.IsValid)
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

            _membershipService.TogglePrivacy(id, p);
            return Ok();
        }

        /// <summary>Delete an existing membership</summary>
        /// <param name="id">The identifier for the membership to be deleted</param>
        /// <remarks>Calls the server to make a call and remove the given membership from the database</remarks>
        // DELETE api/<controller>/5
        [HttpDelete]
        [Route("{id}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.MEMBERSHIP)]
        public IHttpActionResult Delete(int id)
        {
            var result = _membershipService.Delete(id);

            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>	
        /// Determines whether or not the given student is a Group Admin of some activity	
        /// </summary>
        /// <param name="id">The student id</param>
        [HttpGet]
        [Route("isGroupAdmin/{id}")]
        public IHttpActionResult IsGroupAdmin(int id)
        {
            var result = _membershipService.IsGroupAdmin(id);

            return Ok(result);
        }
    }
}
