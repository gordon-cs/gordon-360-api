using System;
using System.Collections.Generic;
using Gordon360.Models;
using Gordon360.Repositories;

namespace Gordon360.Services
{
    /// <summary>
    /// Service class to facilitate interacting with teh Admin table.
    /// </summary>
    public class AdministratorService : IAdministratorService
    {
        private IUnitOfWork _unitOfWork;

        public AdministratorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        /// <summary>
        /// Fetches the admin resource whose id is specified as an argument.
        /// </summary>
        /// <param name="id">The admin ID.l</param>
        /// <returns>The Specified administrator. If none was found, a null value is returned.</returns>
        public ADMIN Get(int id)
        {
            var query = _unitOfWork.AdministratorRepository.GetById(id);
            if (query == null)
            {
                return null;
            }
            return query;
        }

        /// <summary>
        /// Fetches the admin resource whose username matches the specified argument
        /// </summary>
        /// <param name="gordon_id">The administrator's gordon id</param>
        /// <returns>The Specified administrator. If none was found, a null value is returned.</returns>
        public ADMIN Get(string gordon_id)
        {
            var query = _unitOfWork.AdministratorRepository.FirstOrDefault(x => x.ID_NUM.ToString() == gordon_id);
            if(query == null)
            {
                return null;
            }
            return query;
        }
        /// <summary>
        /// Fetches all the administrators from the database
        /// </summary>
        /// <returns>Returns a list of administrators. If no administrators were found, an empty list is returned.</returns>
        public IEnumerable<ADMIN> GetAll()
        {
            var query = _unitOfWork.AdministratorRepository.GetAll();
            return query;
        }
    }
}