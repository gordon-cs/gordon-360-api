using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCT_App.Models;
using CCT_App.Models.ViewModels;
using CCT_App.Repositories;

namespace CCT_App.Services
{
    /// <summary>
    /// Service Class that facilitates data transactions between the FacultyController and the Faculty database model.
    /// </summary>
    public class FacultyService : IFacultyService
    {
        private IUnitOfWork _unitOfWork;

        public FacultyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Fetches the faculty member whose gordon id is specified as the parameter.
        /// </summary>
        /// <param name="id">The gordon id</param>
        /// <returns></returns>
        public FacultyViewModel Get(string id)
        {
            var query = _unitOfWork.FacultyRepository.GetById(id);
            FacultyViewModel result = query;
            return result;
        }

        /// <summary>
        /// Fetches all the Faculty members from the database.
        /// </summary>
        /// <returns>FacultyViewModel IEnumerable. If no records are found an empty IEnumerable is returned.</returns>
        public IEnumerable<FacultyViewModel> GetAll()
        {
            var query = _unitOfWork.FacultyRepository.GetAll();
            var result = query.Select<Faculty, FacultyViewModel>(x => x); // Mapping between database model and view model.
            return result;
        }
    }
}