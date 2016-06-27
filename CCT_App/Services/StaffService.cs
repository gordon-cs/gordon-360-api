using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCT_App.Models;
using CCT_App.Repositories;

namespace CCT_App.Services
{
    public class StaffService : IStaffService
    {
        private IUnitOfWork _unitOfWork;

        public StaffService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Staff Get(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Staff> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}