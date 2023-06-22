using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Static.Names;
using Gordon360.Static.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services
{
    /// <summary>
    /// Service class to facilitate data transactions between the MembershipRequestController and the database
    /// </summary>
    public class MembershipRequestService : IMembershipRequestService
    {
        private CCTContext _context;
        private IMembershipService _membershipService;
        private IAccountService _accountService;

        public MembershipRequestService(CCTContext context, IMembershipService membershipService, IAccountService accountService)
        {
            _context = context;
            _membershipService = membershipService;
            _accountService = accountService;
        }

        /// <summary>
        /// Generate a new request to join an activity at a participation level higher than 'Guest'
        /// </summary>
        /// <param name="membershipRequestUpload">The membership request object</param>
        /// <returns>The new request object as a RequestView</returns>
        public async Task<RequestView> AddAsync(RequestUploadViewModel membershipRequestUpload)
        {

            MembershipUploadViewModel m = (MembershipUploadViewModel) membershipRequestUpload;
            // Validates the memberships request by throwing appropriate exceptions. The exceptions are caugth in the CustomExceptionFilter 
            _membershipService.ValidateMembership(m);
            _membershipService.IsPersonAlreadyInActivity(m);
            if (RequestAlreadyExists(membershipRequestUpload))
            {
                throw new ResourceCreationException() { ExceptionMessage = "A request already exists with this activity, session, and user." };
            }

            var request = (REQUEST) membershipRequestUpload;
            request.ID_NUM = int.Parse(_accountService.GetAccountByUsername(membershipRequestUpload.Username).GordonID);

            var addedMembershipRequest = _context.REQUEST.Add(request);
            await _context.SaveChangesAsync();

            return Get(addedMembershipRequest.Entity.REQUEST_ID);

        }

        private bool RequestAlreadyExists(RequestUploadViewModel requestUpload)
        {
            var g_id = Int32.Parse(_accountService.GetAccountByUsername(requestUpload.Username).GordonID);
            return _context.REQUEST.Any(r => 
                r.STATUS == Request_Status.PENDING
                && r.ACT_CDE == requestUpload.Activity
                && r.SESS_CDE == requestUpload.Session
                && r.ID_NUM == g_id
            );
        }

        /// <summary>
        /// Approves the request with the specified ID.
        /// </summary>
        /// <param name="requestID">The ID of the request to be approved</param>
        /// <returns>The approved request as a RequestView</returns>
        public async Task<RequestView> ApproveAsync(int requestID)
        {
            var request = await _context.REQUEST.FindAsync(requestID);
            if (request == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Request was not found." };
            }

            if (request.STATUS == Request_Status.APPROVED)
            {
                throw new BadInputException() { ExceptionMessage = "The request has already been approved."};
            }

            var username = _accountService.GetAccountByID(request.ID_NUM.ToString()).ADUserName;
            MembershipUploadViewModel newMembership = MembershipUploadViewModel.FromRequest(request, username);

            var createdMembership = await _membershipService.AddAsync(newMembership);

            if (createdMembership == null)
                throw new ResourceCreationException();

            request.STATUS = Request_Status.APPROVED;
            await _context.SaveChangesAsync();

            return Get(request.REQUEST_ID);

        }

        /// <summary>
        /// Denies the membership request object whose id is given in the parameters
        /// </summary>
        /// <param name="requestID">The membership request id</param>
        /// <returns>A RequestView object of the denied request</returns>
        public async Task<RequestView> DenyAsync(int requestID)
        {
            var request = await _context.REQUEST.FindAsync(requestID);

            if (request == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Request was not found." };
            }

            if (request.STATUS == Request_Status.DENIED)
            {
                throw new BadInputException() { ExceptionMessage = "The request has already been denied." };
            }

            if (request.STATUS == Request_Status.APPROVED)
            {
                throw new BadInputException() { ExceptionMessage = "The request has already been approved" };
            }

            request.STATUS = Request_Status.DENIED;
            await _context.SaveChangesAsync();

            return Get(request.REQUEST_ID);
        }

        /// <summary>
        /// Denies the membership request object whose id is given in the parameters
        /// </summary>
        /// <param name="requestID">The membership request id</param>
        /// <returns>A RequestView object of the now pending request</returns>
        public async Task<RequestView> SetPendingAsync(int requestID)
        {
            var request = await _context.REQUEST.FindAsync(requestID);

            if (request == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Request was not found." };
            }

            if (request.STATUS == Request_Status.PENDING)
            {
                throw new BadInputException() { ExceptionMessage = "The request is already pending." };
            }

            if (request.STATUS == Request_Status.APPROVED)
            {
                throw new BadInputException() { ExceptionMessage = "The request has already been approved" };
            }

            request.STATUS = Request_Status.PENDING;
            await _context.SaveChangesAsync();

            return Get(request.REQUEST_ID);
        }

        /// <summary>
        /// Delete the membershipRequest object whose id is given in the parameters 
        /// </summary>
        /// <param name="requestID">The membership request id</param>
        /// <returns>A copy of the deleted request as a RequestView</returns>
        public async Task<RequestView> DeleteAsync(int requestID)
        {
            var request = await _context.REQUEST.FindAsync(requestID);
            if (request == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Request was not found." };
            }

            RequestView rv = Get(request.REQUEST_ID);

            _context.REQUEST.Remove(request);
            await _context.SaveChangesAsync();

            return rv;
        }

        /// <summary>
        /// Get the membership request object whose Id is specified in the parameters.
        /// </summary>
        /// <param name="requestID">The membership request id</param>
        /// <returns>The matching RequestView</returns>
        public RequestView Get(int requestID)
        {
            RequestView? query = _context.RequestView.FirstOrDefault(rv => rv.RequestID == requestID);

            if (query is not RequestView request) throw new ResourceNotFoundException() { ExceptionMessage = "The request was not found"};

            return request;
        }

        /// <summary>
        /// Fetches all the membership request objects from the database.
        /// </summary>
        /// <returns>RequestView IEnumerable. If no records are found, returns an empty IEnumerable.</returns>
        public IEnumerable<RequestView> GetAll()
        {
            return _context.RequestView;
        }

        /// <summary>
        /// Fetches all the membership requests associated with this activity
        /// </summary>
        /// <param name="activityCode">The activity id</param>
        /// <param name="sessionCode">The session code to filter by</param>
        /// <param name="requestStatus">The request status to filter by</param>
        /// <returns>A RequestView IEnumerable. If no records are found, returns an empty IEnumerable.</returns>
        public IEnumerable<RequestView> GetMembershipRequests(string activityCode, string? sessionCode = null, string? requestStatus = null)
        {
            if (!_context.ACT_INFO.Any(a => a.ACT_CDE == activityCode))
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The activity could not be found." };
            }

            sessionCode ??= Helpers.GetCurrentSession(_context);
            var requests = _context.RequestView.Where(r => r.ActivityCode == activityCode && r.SessionCode == sessionCode);
            if (requestStatus != null)
            {
                requests = requests.Where(r => r.Status == requestStatus);
            }
            return requests;
        }

        /// <summary>
        /// Fetches all the membership requests associated with this student
        /// </summary>
        /// <param name="username">The AD Username of the user</param>
        /// <returns>A RequestView IEnumerable. If no records are found, returns an empty IEnumerable.</returns>
        public IEnumerable<RequestView> GetMembershipRequestsByUsername(string username)
        {
            return _context.RequestView.Where(r => r.Username == username);
        }

        /// <summary>
        /// Update an existing membership request object
        /// </summary>
        /// <param name="requestID">The membership request id</param>
        /// <param name="membershipRequest">The newly modified membership request</param>
        /// <returns>A RequestView object of the updated request</returns>
        public async Task<RequestView?> UpdateAsync(int requestID, RequestUploadViewModel membershipRequest)
        {
            var original = await _context.REQUEST.FindAsync(requestID);
            if (original == null)
            {
                throw new ResourceNotFoundException { ExceptionMessage = "The Request was not found." };
            }

            // The validate function throws ResourceNotFoundException where needed. The exceptions are caught in my CustomExceptionFilter
            _membershipService.ValidateMembership((MembershipUploadViewModel) membershipRequest);

            // Only a few fields should be able to be changed through an update.
            original.SESS_CDE = membershipRequest.Session;
            original.COMMENT_TXT = membershipRequest.CommentText;
            original.DATE_SENT = membershipRequest.DateSent;
            original.PART_CDE = membershipRequest.Participation;

            await _context.SaveChangesAsync();

            return Get(original.REQUEST_ID);
        }
    }
}
