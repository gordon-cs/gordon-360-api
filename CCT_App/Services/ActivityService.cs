using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCT_App.Models;
using CCT_App.Models.ViewModels;
using CCT_App.Repositories;
using CCT_App.Services.ComplexQueries;

namespace CCT_App.Services
{
    public class ActivityService : IActivityService
    {
        private IUnitOfWork _unitOfWork;

        public ActivityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ACT_CLUB_DEF Get(string id)
        {
            var result = _unitOfWork.ActivityRepository.GetById(id);
            return result;
        }

        public IEnumerable<ACT_CLUB_DEF> GetAll()
        {
            var result = _unitOfWork.ActivityRepository.GetAll();
            return result.ToList();
        }

        public IEnumerable<Membership> GetLeadersForActivity(string id)
        {
            var query = _unitOfWork.MembershipRepository.Where(x => x.ACT_CDE.Trim() == id);
            var filterQuery = query.Where(x => Constants.LeaderParticipationCodes.Contains(x.PART_LVL.Trim()));
            return filterQuery.ToList();
        }

        public IEnumerable<MembershipViewModel> GetMembershipsForActivity(string id)
        {
            //var result = _unitOfWork.MembershipRepository.Where(x => x.ACT_CDE.Trim() == id);
            var query = Constants.getMembershipForActivityQuery;
            var result = RawSqlQuery<MembershipViewModel>.query(query, id);
            return result;
        }

        public IEnumerable<SUPERVISOR> GetSupervisorsForActivity(string id)
        {
            var result = _unitOfWork.SupervisorRepository.Where(x => x.ACT_CDE.Trim() == id);
            return result.ToList();
        }
    }
}