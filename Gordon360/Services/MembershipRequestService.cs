using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services.ComplexQueries;
using Gordon360.Static.Names;

namespace Gordon360.Services
{
    /// <summary>
    /// Service class to facilitate data transactions between the MembershipRequestController and the database
    /// </summary>
    public class MembershipRequestService : IMembershipRequestService
    {
        private IUnitOfWork _unitOfWork;

        public MembershipRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Generate a new request to join an activity at a participation level higher than 'Guest'
        /// </summary>
        /// <param name="membershipRequest">The membership request object</param>
        /// <returns>The membership request object once it is added</returns>
        public REQUEST Add(REQUEST membershipRequest)
        {
            // Validates the memberships request by throwing appropriate exceptions. The exceptions are caugth in the CustomExceptionFilter 
            validateMembershipRequest(membershipRequest);
            isPersonAlreadyInActivity(membershipRequest);

            membershipRequest.STATUS = Request_Status.PENDING;
            var addedMembershipRequest = _unitOfWork.MembershipRequestRepository.Add(membershipRequest);
            _unitOfWork.Save();

            return addedMembershipRequest;

        }

        /// <summary>
        /// Approves the request with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the request to be approved</param>
        /// <returns>The approved membership</returns>
        public MEMBERSHIP ApproveRequest(int id)
        {
            var query = _unitOfWork.MembershipRequestRepository.GetById(id);
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
                COMMENT_TXT = ""
            };

            MEMBERSHIP created;

            var personAlreadyInActivity = _unitOfWork.MembershipRepository.Where(x => x.SESS_CDE == query.SESS_CDE &&
                x.ACT_CDE == query.ACT_CDE && x.ID_NUM == query.ID_NUM);

            // If the person is already in the activity, we simply change his or her role
            if (personAlreadyInActivity.Count() > 0)
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
                created = _unitOfWork.MembershipRepository.Add(newMembership);

                if (created == null)
                {
                    // The main reason why a membership won't be added is if a similar one (similar id_num, part_lvl, sess_cde and act_cde ) exists.
                    throw new ResourceCreationException() { ExceptionMessage = "There was an error creating the membership. Verify that a similar membership doesn't already exist." };
                }
            }
            
            _unitOfWork.Save();

            return created;

        }

        /// <summary>
        /// Delete the membershipRequest object whose id is given in the parameters 
        /// </summary>
        /// <param name="id">The membership request id</param>
        /// <returns>A copy of the deleted membership request</returns>
        public REQUEST Delete(int id)
        {
            var result = _unitOfWork.MembershipRequestRepository.GetById(id);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Request was not found." };
            }
            result = _unitOfWork.MembershipRequestRepository.Delete(result);
            _unitOfWork.Save();
            return result;
        }

        /// <summary>
        /// Denies the membership request object whose id is given in the parameters
        /// </summary>
        /// <param name="id">The membership request id</param>
        /// <returns></returns>
        public REQUEST DenyRequest(int id)
        {
            var query = _unitOfWork.MembershipRequestRepository.GetById(id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Request was not found." };
            }

            query.STATUS = Request_Status.DENIED;
            _unitOfWork.Save();
            return query;
        }

        /// <summary>
        /// Get the membership request object whose Id is specified in the parameters.
        /// </summary>
        /// <param name="id">The membership request id</param>
        /// <returns>If found, returns MembershipRequestViewModel. If not found, returns null.</returns>
        public MembershipRequestViewModel Get(int id)
        {
            
            var idParameter = new SqlParameter("@REQUEST_ID", id);
            var result = RawSqlQuery<MembershipRequestViewModel>.query("REQUEST_PER_REQUEST_ID @REQUEST_ID", idParameter).FirstOrDefault();
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Request was not found." };
            }

            // Getting rid of database-inherited whitespace
            result.ActivityCode = result.ActivityCode.Trim();
            result.ActivityDescription = result.ActivityDescription.Trim();
            result.SessionCode = result.SessionCode.Trim();
            result.SessionDescription = result.SessionDescription.Trim();
            result.IDNumber = result.IDNumber;
            result.FirstName = result.FirstName.Trim();
            result.LastName = result.LastName.Trim();
            result.Participation = result.Participation.Trim();
            result.ParticipationDescription = result.ParticipationDescription.Trim();

            return result;
        }

        /// <summary>
        /// Fetches all the membership request objects from the database.
        /// </summary>
        /// <returns>MembershipRequestViewModel IEnumerable. If no records are found, returns an empty IEnumerable.</returns>
        public IEnumerable<MembershipRequestViewModel> GetAll()
        {
            
            var result = RawSqlQuery<MembershipRequestViewModel>.query("ALL_REQUESTS");
            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.ActivityCode = x.ActivityCode.Trim();
                trim.ActivityDescription = x.ActivityDescription.Trim();
                trim.SessionCode = x.SessionCode.Trim();
                trim.SessionDescription = x.SessionDescription.Trim();
                trim.IDNumber = x.IDNumber;
                trim.FirstName = x.FirstName.Trim();
                trim.LastName = x.LastName.Trim();
                trim.Participation = x.Participation.Trim();
                trim.ParticipationDescription = x.ParticipationDescription.Trim();
                return trim;
            });
            return trimmedResult;
        }

        /// <summary>
        /// Fetches all the membership requests associated with this activity
        /// </summary>
        /// <param name="id">The activity id</param>
        /// <returns>MembershipRequestViewModel IEnumerable. If no records are found, returns an empty IEnumerable.</returns>
        public IEnumerable<MembershipRequestViewModel> GetMembershipRequestsForActivity(string id)
        {
            
            var idParameter = new SqlParameter("@ACT_CDE", id);
            var result = RawSqlQuery<MembershipRequestViewModel>.query("REQUESTS_PER_ACT_CDE @ACT_CDE", idParameter);

            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.ActivityCode = x.ActivityCode.Trim();
                trim.ActivityDescription = x.ActivityDescription.Trim();
                trim.SessionCode = x.SessionCode.Trim();
                trim.SessionDescription = x.SessionDescription.Trim();
                trim.IDNumber = x.IDNumber;
                trim.FirstName = x.FirstName.Trim();
                trim.LastName = x.LastName.Trim();
                trim.Participation = x.Participation.Trim();
                trim.ParticipationDescription = x.ParticipationDescription.Trim();
                return trim;
            });
            return trimmedResult; 
        }

        /// <summary>
        /// Fetches all the membership requests associated with this student
        /// </summary>
        /// <param name="id">The student id</param>
        /// <returns>MembershipRequestViewModel IEnumerable. If no records are found, returns an empty IEnumerable.</returns>
        public IEnumerable<MembershipRequestViewModel> GetMembershipRequestsForStudent(string id)
        {
            
            var idParameter = new SqlParameter("@STUDENT_ID", id);
            var result = RawSqlQuery<MembershipRequestViewModel>.query("REQUESTS_PER_STUDENT_ID @STUDENT_ID", idParameter);
            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.ActivityCode = x.ActivityCode.Trim();
                trim.ActivityDescription = x.ActivityDescription.Trim();
                trim.SessionCode = x.SessionCode.Trim();
                trim.SessionDescription = x.SessionDescription.Trim();
                trim.FirstName = x.FirstName.Trim();
                trim.LastName = x.LastName.Trim();
                trim.Participation = x.Participation.Trim();
                trim.ParticipationDescription = x.ParticipationDescription.Trim();
                return trim;
            });
            return trimmedResult;
        }

        /// <summary>
        /// Update an existing membership request object
        /// </summary>
        /// <param name="id">The membership request id</param>
        /// <param name="membershipRequest">The newly modified membership request</param>
        /// <returns></returns>
        public REQUEST Update(int id, REQUEST membershipRequest)
        {
            var original = _unitOfWork.MembershipRequestRepository.GetById(id);
            if (original == null)
            {
                return null;
            }

            // The validate function throws ResourceNotFoundExceptino where needed. The exceptions are caught in my CustomExceptionFilter
            validateMembershipRequest(membershipRequest);

            // Only a few fields should be able to be changed through an update.
            original.SESS_CDE = membershipRequest.SESS_CDE;
            original.COMMENT_TXT = membershipRequest.COMMENT_TXT;
            original.DATE_SENT = membershipRequest.DATE_SENT;
            original.PART_CDE = membershipRequest.PART_CDE;

            _unitOfWork.Save();

            return original;
        }

        // Helper method to help validate a membership request that comes in.
        // Return true if it is valid. Throws an exception if not. Exception is cauth in an Exception filter.
        private bool validateMembershipRequest(REQUEST membershipRequest)
        {
            var personExists = _unitOfWork.AccountRepository.Where(x => x.gordon_id.Trim() == membershipRequest.ID_NUM.ToString()).Count() > 0;
            if(!personExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Person was not found." };
            }
            var activityExists = _unitOfWork.ActivityRepository.Where(x => x.ACT_CDE.Trim() == membershipRequest.ACT_CDE).Count() > 0;
            if(!activityExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
            }
            var participationExists = _unitOfWork.ParticipationRepository.Where(x => x.PART_CDE.Trim() == membershipRequest.PART_CDE).Count() > 0;
            if(!participationExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Participation level was not found." };
            }
            var sessionExists = _unitOfWork.SessionRepository.Where(x => x.SESS_CDE.Trim() == membershipRequest.SESS_CDE).Count() > 0;
            if(!sessionExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Session was not found." };
            }

            return true;
        }

        private bool isPersonAlreadyInActivity(REQUEST membershipRequest)
        {
            var personAlreadyInActivity = _unitOfWork.MembershipRepository.Where(x => x.SESS_CDE == membershipRequest.SESS_CDE &&
                x.ACT_CDE == membershipRequest.ACT_CDE && x.ID_NUM == membershipRequest.ID_NUM).Count() > 0;
            if (personAlreadyInActivity)
            {
                throw new ResourceCreationException() { ExceptionMessage = "You are already part of the activity." };
            }

            return true;
        }
    }
}