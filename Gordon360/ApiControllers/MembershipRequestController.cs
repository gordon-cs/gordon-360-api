using System.Collections.Generic;
using System.Security.Claims;
using Gordon360.Models;
using Gordon360.Repositories;
using Gordon360.Services;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using Gordon360.Exceptions.ExceptionFilters;
using Gordon360.Exceptions.CustomExceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Gordon360.Models.ViewModels;
using Gordon360.Database.CCT;
using Gordon360.Models.CCT;

namespace Gordon360.Controllers.Api
{
    [Route("api/requests")]
    [Authorize]
    [CustomExceptionFilter]
    public class MembershipRequestController : ControllerBase
    {

        public IMembershipRequestService _membershipRequestService;

        public MembershipRequestController(CCTContext context)
        {
            _membershipRequestService = new MembershipRequestService(context);
        }

        public MembershipRequestController(IMembershipRequestService membershipRequestService)
        {
            _membershipRequestService = membershipRequestService;
        }

        /// <summary>
        /// Gets all Membership Request Objects
        /// </summary>
        /// <returns>List of all requests for membership</returns>
        [HttpGet]
        [Route("")]
        [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.MEMBERSHIP_REQUEST)]
        public ActionResult<IEnumerable<MembershipViewModel>> Get()
        {
            var all = _membershipRequestService.GetAll();
            return Ok(all);
        }

        /// <summary>
        ///  Gets a specific Membership Request Object
        /// </summary>
        /// <param name="id">The ID of the membership request</param>
        /// <returns>A memberships request with the specified id</returns>
        [HttpGet]
        [Route("{id}")]
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.MEMBERSHIP_REQUEST)]
        public ActionResult<MembershipViewModel> Get(int id)
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

            var result = _membershipRequestService.Get(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }


        /// <summary>
        /// Gets the memberships requests for the specified activity
        /// </summary>
        /// <param name="id">The activity code</param>
        /// <returns>All membership requests associated with the activity</returns>
        [HttpGet]
        [Route("activity/{id}")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.MEMBERSHIP_REQUEST_BY_ACTIVITY)]
        public ActionResult<IEnumerable<MembershipViewModel>> GetMembershipsRequestsForActivity(string id)
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

            var result = _membershipRequestService.GetMembershipRequestsForActivity(id);

            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets the memberships requests for the person making the request
        /// </summary>
        /// <returns>All membership requests associated with the student</returns>
        [HttpGet]
        [Route("student")]
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.MEMBERSHIP_REQUEST_BY_STUDENT)]
        public ActionResult<IEnumerable<MembershipViewModel>> GetMembershipsRequestsForStudent()
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
            var authenticatedUserIdString = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var result = _membershipRequestService.GetMembershipRequestsForStudent(authenticatedUserIdString);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Creates a new membership request
        /// </summary>
        /// <param name="membershipRequest">The request to be added</param>
        /// <returns>The added request if successful. HTTP error message if not.</returns>
        [HttpPost]
        [Route("",Name = "membershipRequest")]
        [StateYourBusiness(operation = Operation.ADD, resource = Resource.MEMBERSHIP_REQUEST)]
        public ActionResult<REQUEST> Post([FromBody] REQUEST membershipRequest)
        {
            if( !ModelState.IsValid || membershipRequest == null)
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

            var result = _membershipRequestService.Add(membershipRequest);

            if (result == null)
            {
                return NotFound();
            }

            return Created("membershipRequest", result);
        }

        /// <summary>
        /// Updates a membership request
        /// </summary>
        /// <param name="id">The membership request id</param>
        /// <param name="membershipRequest">The updated membership request object</param>
        /// <returns>The updated request if successful. HTTP error message if not.</returns>
        [HttpPut]
        [Route("{id}")]
        [StateYourBusiness(operation = Operation.UPDATE, resource = Resource.MEMBERSHIP_REQUEST)]
        public ActionResult<REQUEST> Put(int id, REQUEST membershipRequest)
        {
            if (!ModelState.IsValid || membershipRequest == null || id != membershipRequest.REQUEST_ID)
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

            var result = _membershipRequestService.Update(id, membershipRequest);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(membershipRequest);
        }

        /// <summary>
        /// Sets a membership request to Approved
        /// </summary>
        /// <param name="id">The id of the membership request in question.</param>
        /// <returns>If successful: THe updated membership request wrapped in an OK Http status code.</returns>
        [HttpPost]
        [Route("{id}/approve")]
        [StateYourBusiness(operation = Operation.DENY_ALLOW, resource = Resource.MEMBERSHIP_REQUEST)]
        public ActionResult<MEMBERSHIP> ApproveRequest(int id)
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

            var result = _membershipRequestService.ApproveRequest(id);

            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Sets the membership request to Denied
        /// </summary>
        /// <param name="id">The id of the membership request in question.</param>
        /// <returns>If successful: The updated membership request wrapped in an OK Http status code.</returns>
        [HttpPost]
        [Route("{id}/deny")]
        [StateYourBusiness(operation = Operation.DENY_ALLOW, resource = Resource.MEMBERSHIP_REQUEST)]
        public ActionResult<REQUEST> DenyRequest(int id)
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

            var result = _membershipRequestService.DenyRequest(id);

            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
        /// <summary>
        /// Deletes a membership request
        /// </summary>
        /// <param name="id">The id of the membership request to delete</param>
        /// <returns>The deleted object</returns>
        [HttpDelete]
        [Route("{id}")]
        [StateYourBusiness(operation = Operation.DELETE, resource = Resource.MEMBERSHIP_REQUEST)]
        public ActionResult<REQUEST> Delete(int id)
        {
            var result = _membershipRequestService.Delete(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
