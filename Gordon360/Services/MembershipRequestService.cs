using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Static.Names;
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
        /// <returns>The membership request object once it is added</returns>
        public async Task<RequestView> AddAsync(RequestUploadViewModel membershipRequestUpload)
        {

            MembershipUploadViewModel m = membershipRequestUpload.ToMembershipUpload();
            // Validates the memberships request by throwing appropriate exceptions. The exceptions are caugth in the CustomExceptionFilter 
            await _membershipService.ValidateMembershipAsync(m);
            _membershipService.IsPersonAlreadyInActivity(m);

            var request = membershipRequestUpload.ToREQUEST();
            request.ID_NUM = int.Parse(_accountService.GetAccountByUsername(membershipRequestUpload.Username).GordonID);

            var addedMembershipRequest = _context.REQUEST.Add(request);
            _context.SaveChanges();

            return Get(addedMembershipRequest.Entity.REQUEST_ID);

        }

        /// <summary>
        /// Approves the request with the specified ID.
        /// </summary>
        /// <param name="requestID">The ID of the request to be approved</param>
        /// <returns>The approved membership</returns>
        public async Task<MembershipView> ApproveAsync(int requestID)
        {
            var request = await _context.REQUEST.FindAsync(requestID);
            if (request == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Request was not found." };
            }
            request.STATUS = Request_Status.APPROVED;

            MembershipUploadViewModel newMembership = MembershipUploadViewModel.FromREQUEST(request);
            newMembership.Username = _accountService.GetAccountByID(request.ID_NUM.ToString()).ADUserName;

            return await _membershipService.AddAsync(newMembership);

        }

        /// <summary>
        /// Delete the membershipRequest object whose id is given in the parameters 
        /// </summary>
        /// <param name="requestID">The membership request id</param>
        /// <returns>A copy of the deleted membership request</returns>
        public async Task<RequestView> DeleteAsync(int requestID)
        {
            var request = await _context.REQUEST.FindAsync(requestID);
            if (request == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Request was not found." };
            }

            RequestView rv = _context.RequestView.FirstOrDefault(r => r.RequestID == request.REQUEST_ID);

            _context.REQUEST.Remove(request);
            await _context.SaveChangesAsync();

            return rv;
        }

        /// <summary>
        /// Denies the membership request object whose id is given in the parameters
        /// </summary>
        /// <param name="requestID">The membership request id</param>
        /// <returns></returns>
        public async Task<RequestView> DenyAsync(int requestID)
        {
            var request = await _context.REQUEST.FindAsync(requestID);

            if (request == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Request was not found." };
            }

            request.STATUS = Request_Status.DENIED;
            await _context.SaveChangesAsync();

            return _context.RequestView.FirstOrDefault(r => r.RequestID == request.REQUEST_ID);
        }

        /// <summary>
        /// Get the membership request object whose Id is specified in the parameters.
        /// </summary>
        /// <param name="requestID">The membership request id</param>
        /// <returns>If found, returns MembershipRequestViewModel. If not found, returns null.</returns>
        public RequestView Get(int requestID)
        {
            var request = _context.RequestView.FirstOrDefault(r => r.RequestID == requestID);

            if (request == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Request was not found." };
            }

            return request;
        }

        /// <summary>
        /// Fetches all the membership request objects from the database.
        /// </summary>
        /// <returns>MembershipRequestViewModel IEnumerable. If no records are found, returns an empty IEnumerable.</returns>
        public IEnumerable<RequestView> GetAll()
        {
            return _context.RequestView;
        }

        /// <summary>
        /// Fetches all the membership requests associated with this activity
        /// </summary>
        /// <param name="activityCode">The activity id</param>
        /// <returns>MembershipRequestViewModel IEnumerable. If no records are found, returns an empty IEnumerable.</returns>
        public IEnumerable<RequestView> GetMembershipRequestsByActivity(string activityCode)
        {
            return _context.RequestView.Where(r => r.ActivityCode == activityCode);
        }

        /// <summary>
        /// Fetches all the membership requests associated with this student
        /// </summary>
        /// <param name="username">The AD Username of the user</param>
        /// <returns>MembershipRequestViewModel IEnumerable. If no records are found, returns an empty IEnumerable.</returns>
        public IEnumerable<RequestView> GetMembershipRequestsByUsername(string username)
        {
            return _context.RequestView.Where(r => r.Username == username);
        }

        /// <summary>
        /// Update an existing membership request object
        /// </summary>
        /// <param name="requestID">The membership request id</param>
        /// <param name="membershipRequest">The newly modified membership request</param>
        /// <returns></returns>
        public async Task<RequestView> UpdateAsync(int requestID, RequestUploadViewModel membershipRequest)
        {
            var original = await _context.REQUEST.FindAsync(requestID);
            if (original == null)
            {
                throw new ResourceNotFoundException { ExceptionMessage = "The Request was not found." };
            }

            // The validate function throws ResourceNotFoundException where needed. The exceptions are caught in my CustomExceptionFilter
            await _membershipService.ValidateMembershipAsync(membershipRequest.ToMembershipUpload());

            // Only a few fields should be able to be changed through an update.
            original.SESS_CDE = membershipRequest.SessCode;
            original.COMMENT_TXT = membershipRequest.CommentText;
            original.DATE_SENT = membershipRequest.DateSent;
            original.PART_CDE = membershipRequest.PartCode;

            await _context.SaveChangesAsync();

            return _context.RequestView.FirstOrDefault(r => r.RequestID == original.REQUEST_ID); ;
        }
    }
}