using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Models;

namespace Gordon360.Services
{
    /// <summary>
    /// Service class for facilitation data transfers between the front end views and the backend database models
    /// </summary>
    public class ActivityInfoService : IActivityInfoService
    {
        IUnitOfWork _unitOfWork;

        public ActivityInfoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// Fetches the activity info whose Activity Code is specified as a parameter
        /// </summary>
        /// <param name="id">The activity code</param>
        /// <returns>An ActivityViewModel if one was found, null if none were found.</returns>
        public ActivityInfoViewModel Get(string id)
        {
            var query = _unitOfWork.ActivityInfoRepository.GetById(id);
            ActivityInfoViewModel result = query;
            return result; 
        }

        /// <summary>
        /// Fetches all available activity info from the database
        /// </summary>
        /// <returns>ActivityInfoViewModel IEnumerable. If nothing was found, an empty IEnumerable is returned</returns>
        public IEnumerable<ActivityInfoViewModel> GetAll()
        {
            var query = _unitOfWork.ActivityInfoRepository.GetAll();
            var result = query.Select<Activity_Info, ActivityInfoViewModel>(x => x);
            return result;
        }
    }
}