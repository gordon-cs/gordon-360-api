using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCT_App.Models;
using CCT_App.Repositories;

namespace CCT_App.Services
{
    public class MembershipRequestService : IMembershipRequestService
    {
        private IUnitOfWork _unitOfWork;

        public MembershipRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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

        public Request Delete(int id)
        {
            var result = _unitOfWork.MembershipRequestRepository.GetById(id);
            if (result == null)
            {
                return null;
            }
            result = _unitOfWork.MembershipRequestRepository.Delete(result);
            return result;
        }

        public Request Get(int id)
        {
            var result = _unitOfWork.MembershipRequestRepository.GetById(id);
            return result;
        }

        public IEnumerable<Request> GetAll()
        {
            var results = _unitOfWork.MembershipRequestRepository.GetAll();
            return results;
        }

        public Request Update(int id, Request membershipRequest)
        {
            return null;
        }

        private bool membershipRequestIsValid(Request membershipRequest)
        {
            var personExists = _unitOfWork.AccountRepository.Where(x => x.gordon_id == membershipRequest.ID_NUM).Count() > 0;
            var activityExists = _unitOfWork.ActivityRepository.Where(x => x.ACT_CDE == membershipRequest.ACT_CDE).Count() > 0;
            var participationExists = _unitOfWork.ParticipationRepository.Where(x => x.PART_CDE == membershipRequest.PART_LVL).Count() > 0;
            var sessionExists = _unitOfWork.SessionRepository.Where(x => x.SESS_CDE == membershipRequest.SESS_CDE).Count() > 0;

            if (!personExists || !activityExists || !participationExists || !sessionExists)
            {
                return false;
            }

            return true;
        }
    }
}