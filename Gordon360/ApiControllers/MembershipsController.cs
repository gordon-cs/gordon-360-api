using System.Web.Http;
using Gordon360.Models;
using Gordon360.Services;
using Gordon360.Repositories;
using Gordon360.Models.ViewModels;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using System;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Exceptions.CustomExceptions;

namespace Gordon360.Controllers.Api
{
    [RoutePrefix("api/memberships")]
    [Authorize]
    [CustomExceptionFilter]
    public class MembershipsController : ApiController
    {

        private IMembershipService _membershipService;


        public MembershipsController()
        {
            IUnitOfWork _unitOfWork = new UnitOfWork();
            _membershipService = new MembershipService(_unitOfWork);
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
        /// Get a single membership based on the id given
        /// </summary>
        /// <param name="id">The id of a membership within the database</param>
        /// <remarks>Queries the database about the specified membership</remarks>
        /// <returns>The information about one specific membership</returns>
        // GET api/<controller>/5
        [HttpGet]
        [Route("{id}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.MEMBERSHIP)]
        public IHttpActionResult Get(int id)
        {
            if(!ModelState.IsValid)
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

            var result =  _membershipService.Get(id);

            if( result == null)
            {
                return NotFound();
            }

            MembershipViewModel membership = result;
            return Ok(membership);
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
        /// Gets the number of members and followers of an activity
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
        /// Gets the number of members and followers of an activity
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
        ///  /// <param name="id">The id of the membership</param>
        /// <remarks>Calls the server to make a call and update the database with the changed information</remarks>
        [HttpPut]
        [Route("{id}/privacy/{p}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.MEMBERSHIP)]
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
    }
}
