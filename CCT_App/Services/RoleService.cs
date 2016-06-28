using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCT_App.Models;
using CCT_App.Repositories;

namespace CCT_App.Services
{
    public class RoleService : IRoleService
    {
        private IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PART_DEF Get(string id)
        {
            var result = _unitOfWork.RoleRepository.GetById(id);
            return result;
        }

        public IEnumerable<PART_DEF> GetAll()
        {
            var result = _unitOfWork.RoleRepository.GetAll();
            return result;
        }
    }
}