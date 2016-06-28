using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCT_App.Models;
using CCT_App.Repositories;

namespace CCT_App.Services
{
    public class FacultyService : IFacultyService
    {
        private IUnitOfWork _unitOfWork;

        public FacultyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Faculty Get(string id)
        {
            var result = _unitOfWork.FacultyRepository.GetById(id);
            return result;
        }

        public IEnumerable<Faculty> GetAll()
        {
            var result = _unitOfWork.FacultyRepository.GetAll();
            return result;
        }
    }
}