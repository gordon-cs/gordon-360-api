using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCT_App.Models;
using CCT_App.Repositories;

namespace CCT_App.Services
{
    public class ParticipationService : IParticipationService
    {
        private IUnitOfWork _unitOfWork;

        public ParticipationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PART_DEF Get(string id)
        {
            var result = _unitOfWork.ParticipationRepository.GetById(id);
            return result;
        }

        public IEnumerable<PART_DEF> GetAll()
        {
            var result = _unitOfWork.ParticipationRepository.GetAll();
            return result;
        }
    }
}