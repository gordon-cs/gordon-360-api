using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCT_App.Models;
using CCT_App.Repositories;

namespace CCT_App.Services
{
    public class JenzibarActivityService : IJenzibarActivityService
    {
        private IUnitOfWork _unitOfWork;

        public JenzibarActivityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public JNZB_ACTIVITIES Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<JNZB_ACTIVITIES> GetAll()
        {
            throw new NotImplementedException();
        }
    
    }
}