using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services.ComplexQueries;

namespace Gordon360.Services
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
        public ActivityInfoViewModel Get(string id)
        {
            var query = _unitOfWork.ActivityInfoRepository.GetById(id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
            }
            ActivityInfoViewModel result = query;
            return result;
        }

        /// <summary>
        /// Fetches the Activities that are active during the session whose code is specified as parameter.
        /// </summary>
        /// <param name="id">The session code</param>
        /// <returns>ActivityViewModel IEnumerable. If nothing is found, an empty IEnumerable is returned.</returns>
        public IEnumerable<ActivityInfoViewModel> GetActivitiesForSession(string id)
        {
            // Stored procedure returns columns ACT_CDE and ACT_DESC
            var query = RawSqlQuery<ACT_CLUB_DEF>.query("ACTIVE_CLUBS_PER_SESS_ID @SESS_CDE", new SqlParameter("SESS_CDE", SqlDbType.VarChar) { Value = id });
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Session was not found." };
            }
            // We Transform the result into an activityViewModel
            var result = query.Select<ACT_CLUB_DEF, ActivityViewModel>(x => x);
            // Then transform the ActivityViewModel into ActivityInfoViewModel
            var activityInfo = result.Select(x =>
            {
                ActivityInfoViewModel y = new ActivityInfoViewModel();
                var record = _unitOfWork.ActivityInfoRepository.GetById(x.ActivityCode);
                y.ActivityCode = x.ActivityCode;
                y.ActivityDescription = x.ActivityDescription;
                y.ActivityImage = record.ACT_IMAGE ?? "";
                y.MeetingDay = record.MTG_DAY ?? "";
                y.MeetingTime = record.MTG_TIME ?? "";
                return y;
            });
            return activityInfo;
        }

        /// <summary>
        /// Fetches all activity records from storage.
        /// </summary>
        /// <returns>ActivityViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public IEnumerable<ActivityInfoViewModel> GetAll()
        {
            var query = _unitOfWork.ActivityInfoRepository.GetAll();
            var result = query.Select<ACT_INFO, ActivityInfoViewModel>(x => x);
            return result;
        }

    }
}