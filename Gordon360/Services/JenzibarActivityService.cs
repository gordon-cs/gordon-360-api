using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;
using Gordon360.Repositories;

namespace Gordon360.Utils
{
    /// <summary>
    /// Service class to faclitate data transfers between in and out of the JNZB_ACTIVITIES table.
    /// This is a special class whose contents might not end up being exposed. The original idea 
    /// was for this class to represent all the memberships that would be moved to Jenzibar.
    /// We have not reached that point yet though.
    /// </summary>
    public class JenzibarActivityService : IJenzibarActivityService
    {
        private IUnitOfWork _unitOfWork;

        public JenzibarActivityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public JNZB_ACTIVITIES Get(int id)
        {
            var result = _unitOfWork.JenzibarActvityRepository.GetById(id);
            return result;
        }

        public IEnumerable<JNZB_ACTIVITIES> GetAll()
        {
            var result = _unitOfWork.JenzibarActvityRepository.GetAll();
            return result;
        }
    
    }
}