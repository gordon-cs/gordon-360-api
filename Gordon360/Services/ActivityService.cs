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
using Gordon360.Static.Names;

namespace Gordon360.Services
{
    /// <summary>
    /// Service Class that facilitates data transactions between the ActivitiesController and the ACT_CLUB_DEF database model.
    /// ACT_INFO (ActivityInfo) and ACT_CLUB_DEF(Activity) are very similar. 
    /// ACT_INFO is basically a copy of the ACT_CLUB_DEF domain model but with extra fields that we want to store (activity image, blurb etc...)
    /// Activity Info and ACtivity may be talked about interchangeably.
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
                throw new ResourceNotFoundException() { ExceptionMessage = "No Activities for this session was not found." };
            }
            
            // Transform the ACT_CLUB_DEF into ActivityInfoViewModel
            var activityInfo = query.Select(x =>
            {
                ActivityInfoViewModel y = new ActivityInfoViewModel();
                var record = _unitOfWork.ActivityInfoRepository.GetById(x.ACT_CDE);
                if (record == null )
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "The Activity Info was not found." };
                }
                y.ActivityCode = x.ACT_CDE.Trim();
                y.ActivityDescription = x.ACT_DESC ?? "";
                y.ActivityBlurb = record.ACT_BLURB ?? "";
                y.ActivityURL = record.ACT_URL ?? "";
                y.ActivityImagePath = record.ACT_IMG_PATH.Trim() ?? "";
                y.ActivityType = record.ACT_TYPE.Trim() ?? "";
                y.ActivityTypeDescription = record.ACT_TYPE_DESC.Trim() ?? "";
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

        /// <summary>
        /// Updates the Activity Info 
        /// </summary>
        /// <param name="activity">The activity info resource with the updated information</param>
        /// <param name="id">The id of the activity info to be updated</param>
        /// <returns>The updated activity info resource</returns>
        public ACT_INFO Update(string id,ACT_INFO activity)
        {
            var original = _unitOfWork.ActivityInfoRepository.GetById(id);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity Info was not found." };
            }

            validateActivityInfo(activity);

            // One can only update certain fields within a membrship
            original.ACT_BLURB = activity.ACT_BLURB;
            original.ACT_URL = activity.ACT_URL;

            _unitOfWork.Save();

            return original;

        }

        /// <summary>
        /// Sets the path for the activity image.
        /// </summary>
        /// <param name="id">The activity code</param>
        /// <param name="path"></param>
        public void UpdateActivityImage(string id, string path)
        {
            var original = _unitOfWork.ActivityInfoRepository.GetById(id);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity Info was not found." };
            }

            original.ACT_IMG_PATH = path;

            _unitOfWork.Save();
        }
        /// <summary>
        /// Reset the path for the activity image
        /// </summary>
        /// <param name="id">The activity code</param>
        public void ResetActivityImage(string id)
        {
            var original = _unitOfWork.ActivityInfoRepository.GetById(id);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity Info was not found." };
            }

            original.ACT_IMG_PATH = Defaults.DEFAULT_ACTIVITY_IMAGE_PATH;

            _unitOfWork.Save();
        }

        // Helper method to validate an activity info post. Throws an exception that gets caught later if something is not valid.
        // Returns true if all is well. The return value is not really used though. This could be of type void.
        private bool validateActivityInfo(ACT_INFO activity)
        {
            var activityExists = _unitOfWork.ActivityInfoRepository.Where(x => x.ACT_CDE == activity.ACT_CDE).Count() > 0;
            if (!activityExists)
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };

            return true;

        }


    }
}