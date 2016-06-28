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
            var result = _unitOfWork.StaffRepository.GetById(id);
            return result;
        }

        public IEnumerable<Staff> GetAll()
        {
            var all = _unitOfWork.StaffRepository.GetAll();
            return all;
        }
    }
}