using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCT_App.Models;
using CCT_App.Repositories;
using System.Data.SqlClient;
using System.Data;

namespace CCT_App.Services
{
    public class MembershipService : IMembershipService
    {
        private IUnitOfWork _unitOfWork;

        public MembershipService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Membership Add(Membership membership)
        {
            var personExists = _unitOfWork.AccountRepository.Where(x => x.gordon_id == membership.ID_NUM).Count() > 0;
            var roleExists = _unitOfWork.RoleRepository.Where(x => x.PART_CDE == membership.PART_LVL).Count() > 0;
            var sessionExists = _unitOfWork.SessionRepository.Where(x => x.SESS_CDE == membership.SESSION_CDE).Count() > 0;
            var activityExists = _unitOfWork.ActivityRepository.Where(x => x.ACT_CDE == membership.ACT_CDE).Count() > 0;

            if(!personExists || !roleExists || !sessionExists || !activityExists)
            {
                return null;
            }

            var activitiesThisSession = _unitOfWork.ActivityPerSessionRepository.ExecWithStoredProcedure("ACTIVE_CLUBS_PER_SESS_ID @SESS_CDE", new SqlParameter("SESS_CDE", SqlDbType.VarChar) { Value = membership.SESSION_CDE });

            bool offered = false;
            foreach (var activityResult in activitiesThisSession)
            {
                if (activityResult.ACT_CDE == membership.ACT_CDE )
                {
                    offered = true;
                }
            }

            if (!offered)
            {
                return null;
            }

            var addedMembership = _unitOfWork.MembershipRepository.Add(membership);
            _unitOfWork.Save();

            return addedMembership;

        }

        public Membership Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Membership Get(int id)
        {
            var result = _unitOfWork.MembershipRepository.GetById(id);
            return result;
        }

        public IEnumerable<Membership> GetAll()
        {
            var result = _unitOfWork.MembershipRepository.GetAll();
            return result;
        }

        public Membership Update(int id, Membership membership)
        {
            throw new NotImplementedException();
        }
    }
}