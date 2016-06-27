using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCT_App.Models;
using CCT_App.Repositories;


namespace CCT_App.Services
{
    public class SessionService : ISessionService
    {
        private IUnitOfWork _unitOfWork;

        public SessionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public CM_SESSION_MSTR Get(string id)
        {
            var result = _unitOfWork.SessionRepository.GetById(id);
            return result;
        }

        public IEnumerable<ACTIVE_CLUBS_PER_SESS_ID_Result> GetActivitiesForSession(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CM_SESSION_MSTR> GetAll()
        {
            var result = _unitOfWork.SessionRepository.GetAll();
            return result;
        }
    }
}