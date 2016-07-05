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
    /// <summary>
    /// Service Class that facilitates data transactions between the ActivitiesController and the ACT_CLUB_DEF database model.
    /// </summary>
    public class ActivityService : IActivityService
    {
        private IUnitOfWork _unitOfWork;

        public ActivityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// Fetches a single activity record whose id matches the id provided as an argument
        /// </summary>
        /// <param name="id">The activity code</param>
        /// <returns>ActivityViewModel if found, null if not found</returns>
        public ActivityViewModel Get(string id)
        {
            var query = _unitOfWork.ActivityRepository.GetById(id);
            ActivityViewModel result = query;
            return result;
        }

        /// <summary>
        /// Fetches all activity records from storage.
        /// </summary>
        /// <returns>ActivityViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public IEnumerable<ActivityViewModel> GetAll()
        {
            var query = _unitOfWork.ActivityRepository.GetAll();
            var result = query.Select<ACT_CLUB_DEF, ActivityViewModel>(x => x);
            return result;
        }

        /// <summary>
        /// Fetches the leaders of the activity whose activity code is specified by the parameter.
        /// </summary>
        /// <param name="id">The activity code.</param>
        /// <returns>MembershipViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public IEnumerable<MembershipViewModel> GetLeadersForActivity(string id)
        {
            var rawsqlquery = Constants.getLeadersForActivityQuery;
            var currentSession = Helpers.GetCurrentSession().SessionCode;
            var result = RawSqlQuery<MembershipViewModel>.query(rawsqlquery, id, currentSession);
            
            // Getting rid of whitespace inherited from the database .__.
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
            //var query = _unitOfWork.MembershipRepository.Where(x => x.ACT_CDE.Trim() == id);
            //var filterQuery = query.Where(x => Constants.LeaderParticipationCodes.Contains(x.PART_LVL.Trim())).ToList();
            //var result = filterQuery.Select<Membership, MembershipViewModel>(x => x);
            return trimmedResult;
        }

        /// <summary>
        /// Fetches the memberships associated with the activity whose code is specified by the parameter.
        /// </summary>
        /// <param name="id">The activity code.</param>
        /// <returns>MembershipViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public IEnumerable<MembershipViewModel> GetMembershipsForActivity(string id)
        {
            var rawsqlquery = Constants.getMembershipForActivityQuery;
            var result = RawSqlQuery<MembershipViewModel>.query(rawsqlquery, id);
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
        /// Fetches the supervisors of the activity whose activity code is specified by the parameter.
        /// </summary>
        /// <param name="id">The activity code.</param>
        /// <returns>SupervisorViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public IEnumerable<SupervisorViewModel> GetSupervisorsForActivity(string id)
        {
            var rawsqlquery = Constants.getSupervisorsForActivityQuery;
            var result = RawSqlQuery<SupervisorViewModel>.query(rawsqlquery, id);
            // No trimming here because we made the Supervisor Table, and we made sure to use varchar(n).
            return result;
        }
    }
}