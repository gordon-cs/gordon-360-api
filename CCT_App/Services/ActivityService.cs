using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCT_App.Models;
using CCT_App.Repositories;

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
            return result;
        }

        // TODO: Implement This method
        public SUPERVISOR GetSupervisorForActivity(string id)
        {
            throw new NotImplementedException();
        }
    }
}