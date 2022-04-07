using Gordon360.Database.CCT;
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

        public MembershipRequestService(CCTContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Generate a new request to join an activity at a participation level higher than 'Guest'
        /// </summary>
        /// <param name="membershipRequest">The membership request object</param>
        /// <returns>The membership request object once it is added</returns>
        public REQUEST Add(REQUEST membershipRequest)
        {
            // Validates the memberships request by throwing appropriate exceptions. The exceptions are caugth in the CustomExceptionFilter 
            ValidateMembershipRequest(membershipRequest);
            IsPersonAlreadyInActivity(membershipRequest);

            membershipRequest.STATUS = Request_Status.PENDING;
            var addedMembershipRequest = _context.REQUEST.Add(membershipRequest);
            _context.SaveChanges();

            return membershipRequest;

        }

        /// <summary>
        /// Approves the request with the specified ID.
        /// </summary>
        /// <param name="requestID">The ID of the request to be approved</param>
        /// <returns>The approved membership</returns>
        public MEMBERSHIP ApproveRequest(int requestID)
        {
            var query = _context.REQUEST.Find(requestID);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Request was not found." };
            }
            query.STATUS = Request_Status.APPROVED;

            MEMBERSHIP newMembership = new MEMBERSHIP
            {
                ACT_CDE = query.ACT_CDE,
                ID_NUM = query.ID_NUM,
                SESS_CDE = query.SESS_CDE,
                PART_CDE = query.PART_CDE,
                BEGIN_DTE = DateTime.Now,
                COMMENT_TXT = "",
                GRP_ADMIN = false
            };

            MEMBERSHIP created;

            var personAlreadyInActivity = _context.MEMBERSHIP.Where(x => x.SESS_CDE == query.SESS_CDE &&
                x.ACT_CDE == query.ACT_CDE && x.ID_NUM == query.ID_NUM);

            // If the person is already in the activity, we simply change his or her role
            if (personAlreadyInActivity.Any())
            {
                created = personAlreadyInActivity.First();

                if (created == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "There was an error creating the membership." };
                }

                // Simply change role and comment
                created.COMMENT_TXT = newMembership.COMMENT_TXT;
                created.PART_CDE = newMembership.PART_CDE;
            }
            // Else, we add him or her to the activity
            else
            {
                // The add will fail if they are already a member.
                var added = _context.MEMBERSHIP.Add(newMembership);

                if (added == null)
                {
                    // The main reason why a membership won't be added is if a similar one (similar id_num, part_lvl, sess_cde and act_cde ) exists.
                    throw new ResourceCreationException() { ExceptionMessage = "There was an error creating the membership. Verify that a similar membership doesn't already exist." };
                }
                else
                {
                    created = newMembership;
                }
            }

            _context.SaveChanges();

            return created;

        }

        /// <summary>
        /// Delete the membershipRequest object whose id is given in the parameters 
        /// </summary>
        /// <param name="requestID">The membership request id</param>
        /// <returns>A copy of the deleted membership request</returns>
        public REQUEST Delete(int requestID)
        {
            var result = _context.REQUEST.Find(requestID);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Request was not found." };
            }
            _context.REQUEST.Remove(result);
            _context.SaveChanges();
            return result;
        }

        /// <summary>
        /// Denies the membership request object whose id is given in the parameters
        /// </summary>
        /// <param name="requestID">The membership request id</param>
        /// <returns></returns>
        public REQUEST DenyRequest(int requestID)
        {
            var query = _context.REQUEST.Find(requestID);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Request was not found." };
            }

            query.STATUS = Request_Status.DENIED;
            _context.SaveChanges();
            return query;
        }

        /// <summary>
        /// Get the membership request object whose Id is specified in the parameters.
        /// </summary>
        /// <param name="requestID">The membership request id</param>
        /// <returns>If found, returns MembershipRequestViewModel. If not found, returns null.</returns>
        public async Task<MembershipRequestViewModel> GetAsync(int requestID)
        {
            var requests = await _context.Procedures.REQUEST_PER_REQUEST_IDAsync(requestID);

            if (requests == null || requests.Count != 1)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Request was not found." };
            }

            var result = requests[0];

            return new MembershipRequestViewModel
            {
                ActivityCode = result.ActivityCode.Trim(),
                ActivityDescription = result.ActivityDescription.Trim(),
                IDNumber = result.IDNumber,
                FirstName = result.FirstName.Trim(),
                LastName = result.LastName.Trim(),
                Participation = result.Participation.Trim(),
                ParticipationDescription = result.ParticipationDescription.Trim(),
                DateSent = result.DateSent,
                RequestID = result.RequestID,
                CommentText = result.CommentText.Trim(),
                SessionCode = result.SessionCode.Trim(),
                SessionDescription = result.SessionDescription.Trim(),
                RequestApproved = result.RequestApproved.Trim(),
            };
        }

        /// <summary>
        /// Fetches all the membership request objects from the database.
        /// </summary>
        /// <returns>MembershipRequestViewModel IEnumerable. If no records are found, returns an empty IEnumerable.</returns>
        public async Task<IEnumerable<MembershipRequestViewModel>> GetAllAsync()
        {

            var allRequests = await _context.Procedures.ALL_REQUESTSAsync();

            return allRequests.Select(r => new MembershipRequestViewModel
            {
                ActivityCode = r.ActivityCode.Trim(),
                ActivityDescription = r.ActivityDescription.Trim(),
                IDNumber = r.IDNumber,
                FirstName = r.FirstName.Trim(),
                LastName = r.LastName.Trim(),
                Participation = r.Participation.Trim(),
                ParticipationDescription = r.ParticipationDescription.Trim(),
                DateSent = r.DateSent,
                RequestID = r.RequestID,
                CommentText = r.CommentText.Trim(),
                SessionCode = r.SessionCode.Trim(),
                SessionDescription = r.SessionDescription.Trim(),
                RequestApproved = r.RequestApproved.Trim(),
            });
        }

        /// <summary>
        /// Fetches all the membership requests associated with this activity
        /// </summary>
        /// <param name="activityCode">The activity id</param>
        /// <returns>MembershipRequestViewModel IEnumerable. If no records are found, returns an empty IEnumerable.</returns>
        public async Task<IEnumerable<MembershipRequestViewModel>> GetMembershipRequestsForActivityAsync(string activityCode)
        {
            var allRequests = await _context.Procedures.REQUESTS_PER_ACT_CDEAsync(activityCode);

            return allRequests.Select(r => new MembershipRequestViewModel
            {
                ActivityCode = r.ActivityCode.Trim(),
                ActivityDescription = r.ActivityDescription.Trim(),
                IDNumber = r.IDNumber,
                FirstName = r.FirstName.Trim(),
                LastName = r.LastName.Trim(),
                Participation = r.Participation.Trim(),
                ParticipationDescription = r.ParticipationDescription.Trim(),
                DateSent = r.DateSent,
                RequestID = r.RequestID,
                CommentText = r.CommentText.Trim(),
                SessionCode = r.SessionCode.Trim(),
                SessionDescription = r.SessionDescription.Trim(),
                RequestApproved = r.RequestApproved.Trim(),
            });
        }

        /// <summary>
        /// Fetches all the membership requests associated with this student
        /// </summary>
        /// <param name="username">The AD Username of the user</param>
        /// <returns>MembershipRequestViewModel IEnumerable. If no records are found, returns an empty IEnumerable.</returns>
        public async Task<IEnumerable<MembershipRequestViewModel>> GetMembershipRequestsForStudentAsync(string username)
        {
            var account = new AccountService(_context).GetAccountByUsername(username);
            var allRequests = await _context.Procedures.REQUESTS_PER_STUDENT_IDAsync(int.Parse(account.GordonID));

            return allRequests.Select(r => new MembershipRequestViewModel
            {
                ActivityCode = r.ActivityCode.Trim(),
                ActivityDescription = r.ActivityDescription.Trim(),
                IDNumber = r.IDNumber,
                FirstName = r.FirstName.Trim(),
                LastName = r.LastName.Trim(),
                Participation = r.Participation.Trim(),
                ParticipationDescription = r.ParticipationDescription.Trim(),
                DateSent = r.DateSent,
                RequestID = r.RequestID,
                CommentText = r.CommentText.Trim(),
                SessionCode = r.SessionCode.Trim(),
                SessionDescription = r.SessionDescription.Trim(),
                RequestApproved = r.RequestApproved.Trim(),
            });
        }

        /// <summary>
        /// Update an existing membership request object
        /// </summary>
        /// <param name="requestID">The membership request id</param>
        /// <param name="membershipRequest">The newly modified membership request</param>
        /// <returns></returns>
        public REQUEST Update(int requestID, REQUEST membershipRequest)
        {
            var original = _context.REQUEST.Find(requestID);
            if (original == null)
            {
                return null;
            }

            // The validate function throws ResourceNotFoundExceptino where needed. The exceptions are caught in my CustomExceptionFilter
            ValidateMembershipRequest(membershipRequest);

            // Only a few fields should be able to be changed through an update.
            original.SESS_CDE = membershipRequest.SESS_CDE;
            original.COMMENT_TXT = membershipRequest.COMMENT_TXT;
            original.DATE_SENT = membershipRequest.DATE_SENT;
            original.PART_CDE = membershipRequest.PART_CDE;

            _context.SaveChanges();

            return original;
        }

        // Helper method to help validate a membership request that comes in.
        // Return true if it is valid. Throws an exception if not. Exception is cauth in an Exception filter.
        private bool ValidateMembershipRequest(REQUEST membershipRequest)
        {
            var personExists = _context.ACCOUNT.Where(x => x.gordon_id.Trim() == membershipRequest.ID_NUM.ToString()).Any();
            if (!personExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Person was not found." };
            }
            var activityExists = _context.ACT_INFO.Where(x => x.ACT_CDE.Trim() == membershipRequest.ACT_CDE).Any();
            if (!activityExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
            }
            var participationExists = _context.PART_DEF.Where(x => x.PART_CDE.Trim() == membershipRequest.PART_CDE).Any();
            if (!participationExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Participation level was not found." };
            }
            var sessionExists = _context.CM_SESSION_MSTR.Where(x => x.SESS_CDE.Trim() == membershipRequest.SESS_CDE).Any();
            if (!sessionExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Session was not found." };
            }
            // Check for a pending request
            var pendingRequest = _context.REQUEST.Any(x => x.ID_NUM == membershipRequest.ID_NUM &&
                x.SESS_CDE.Equals(membershipRequest.SESS_CDE) && x.ACT_CDE.Equals(membershipRequest.ACT_CDE) &&
                x.STATUS.Equals("Pending"));
            if (pendingRequest)
            {
                throw new ResourceCreationException() { ExceptionMessage = "A request for this activity has already been made for you and is awaiting group leader approval." };
            }

            return true;
        }

        private bool IsPersonAlreadyInActivity(REQUEST membershipRequest)
        {
            var personAlreadyInActivity = _context.MEMBERSHIP.Where(x => x.SESS_CDE == membershipRequest.SESS_CDE &&
                x.ACT_CDE == membershipRequest.ACT_CDE && x.ID_NUM == membershipRequest.ID_NUM).Any();
            if (personAlreadyInActivity)
            {
                throw new ResourceCreationException() { ExceptionMessage = "You are already part of the activity." };
            }

            return true;
        }
    }
}