using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;

namespace Gordon360.Services
{
    /// <summary>
    /// Service class to facilitate the data transactions between the controllers and the database models.
    /// </summary>
    public class StaffService : IStaffService
    {
        private IUnitOfWork _unitOfWork;

        public StaffService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Fetches the staff record whose id matches the provided parameter.
        /// </summary>
        /// <param name="id">The staff's gordon id.</param>
        /// <returns>A StaffViewModel if the record exists, null if it doesn't.</returns>
        public StaffViewModel Get(string id)
        {
            var query = _unitOfWork.StaffRepository.GetById(id);
            StaffViewModel result = query;
            return result;
        }

        /// <summary>
        /// Fetches all the staff records from the database.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<StaffViewModel> GetAll()
        {
            var query = _unitOfWork.StaffRepository.GetAll();
            var result = query.Select<Staff, StaffViewModel>(x => x);
            return result;
        }
    }
}