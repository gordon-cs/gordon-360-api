using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services.ComplexQueries;

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
        public Request Add(Request membershipRequest)
        {
            var isValidMembershipRequest = membershipRequestIsValid(membershipRequest);

            if(!isValidMembershipRequest)
            {
                return null;
            }

            var addedMembershipRequest = _unitOfWork.MembershipRequestRepository.Add(membershipRequest);
            _unitOfWork.Save();

            return addedMembershipRequest;

        }

        /// <summary>
        /// Delete the membershipRequest object whose id is given in the parameters 
        /// </summary>
        /// <param name="id">The membership request id</param>
        /// <returns>A copy of the deleted membership request</returns>
        public Request Delete(int id)
        {
            var result = _unitOfWork.MembershipRequestRepository.GetById(id);
            if (result == null)
            {
                return null;
            }
            result = _unitOfWork.MembershipRequestRepository.Delete(result);
            _unitOfWork.Save();
            return result;
        }
        /// <summary>
        /// Get the membership request object whose Id is specified in the parameters.
        /// </summary>
        /// <param name="id">The membership request id</param>
        /// <returns>If found, returns MembershipRequestViewModel. If not found, returns null.</returns>
        public MembershipRequestViewModel Get(int id)
        {
            var rawsqlquery = Constants.getMembershipRequestByIdQuery;
            var result = RawSqlQuery<MembershipRequestViewModel>.query(rawsqlquery, id).FirstOrDefault();

            if (result == null)
            {
                return null;
            }

            // Getting rid of database-inherited whitespace
            result.ActivityCode = result.ActivityCode.Trim();
            result.ActivityDescription = result.ActivityDescription.Trim();
            result.SessionCode = result.SessionCode.Trim();
            result.SessionDescription = result.SessionDescription.Trim();
            result.IDNumber = result.IDNumber.Trim();
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
            var rawsqlquery = Constants.getAllMembershipRequestsQuery;
            var result = RawSqlQuery<MembershipRequestViewModel>.query(rawsqlquery);
            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.ActivityCode = x.ActivityCode.Trim();
                trim.ActivityDescription = x.ActivityDescription.Trim();
                trim.SessionCode = x.SessionCode.Trim();
                trim.SessionDescription = x.SessionDescription.Trim();
                trim.IDNumber = x.IDNumber.Trim();
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
            var rawsqlQuery = Constants.getMembershipRequestsForStudentQuery;
            var result = RawSqlQuery<MembershipRequestViewModel>.query(rawsqlQuery, id);
            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.ActivityCode = x.ActivityCode.Trim();
                trim.ActivityDescription = x.ActivityDescription.Trim();
                trim.SessionCode = x.SessionCode.Trim();
                trim.SessionDescription = x.SessionDescription.Trim();
                trim.IDNumber = x.IDNumber.Trim();
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
        public Request Update(int id, Request membershipRequest)
        {
            var original = _unitOfWork.MembershipRequestRepository.GetById(id);
            if (original == null)
            {
                return null;
            }

            var isValidMembershipRequest = membershipRequestIsValid(membershipRequest);

            if (!isValidMembershipRequest)
            {
                return null;
            }

            original.ID_NUM = membershipRequest.ID_NUM;
            original.ACT_CDE = membershipRequest.ACT_CDE;
            original.SESS_CDE = membershipRequest.SESS_CDE;
            original.APPROVED = membershipRequest.APPROVED;
            original.COMMENT_TXT = membershipRequest.COMMENT_TXT;
            original.DATE_SENT = membershipRequest.DATE_SENT;
            original.PART_LVL = membershipRequest.PART_LVL;

            _unitOfWork.Save();

            return original;
        }

        // Helper method to help validate a membership request that comes in.
        private bool membershipRequestIsValid(Request membershipRequest)
        {
            var personExists = _unitOfWork.AccountRepository.Where(x => x.gordon_id.Trim() == membershipRequest.ID_NUM).Count() > 0;
            var activityExists = _unitOfWork.ActivityRepository.Where(x => x.ACT_CDE.Trim() == membershipRequest.ACT_CDE).Count() > 0;
            var participationExists = _unitOfWork.ParticipationRepository.Where(x => x.PART_CDE.Trim() == membershipRequest.PART_LVL).Count() > 0;
            var sessionExists = _unitOfWork.SessionRepository.Where(x => x.SESS_CDE.Trim() == membershipRequest.SESS_CDE).Count() > 0;

            if (!personExists || !activityExists || !participationExists || !sessionExists)
            {
                return false;
            }

            return true;
        }
    }
}