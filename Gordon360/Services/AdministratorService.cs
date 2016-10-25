using System;
using System.Collections.Generic;
using System.Linq;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Models;
using Gordon360.Repositories;

namespace Gordon360.Services
{
    /// <summary>
    /// Service class to facilitate interacting with the Admin table.
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
                throw new ResourceNotFoundException() { ExceptionMessage = "The Administrator was not found." };
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
                throw new ResourceNotFoundException() { ExceptionMessage = "The Administrator was not found." };
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

        /// <summary>
        /// Adds a new Administrator record to storage. Since we can't establish foreign key constraints and relationships on the database side,
        /// we do it here by using the validateAdmin() method.
        /// </summary>
        /// <param name="admin">The admin to be added</param>
        /// <returns>The newly added Admin object</returns>
        public ADMIN Add(ADMIN admin)
        {
            // validate returns a boolean value.
            validateAdmin(admin);

            // The Add() method returns the added membership.
            var payload = _unitOfWork.AdministratorRepository.Add(admin);

            // There is a unique constraint in the Database on columns (ID_NUM, PART_LVL, SESS_CDE and ACT_CDE)
            if (payload == null)
            {
                throw new ResourceCreationException() { ExceptionMessage = "There was an error creating the admin. Verify that this admin doesn't already exist." };
            }
            _unitOfWork.Save();

            return payload;

        }

        /// <summary>
        /// Delete the admin whose id is specified by the parameter.
        /// </summary>
        /// <param name="id">The admin id</param>
        /// <returns>The admin that was just deleted</returns>
        public ADMIN Delete(int id)
        {
            var result = _unitOfWork.AdministratorRepository.GetById(id);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Admin was not found." };
            }
            result = _unitOfWork.AdministratorRepository.Delete(result);

            _unitOfWork.Save();

            return result;
        }

        /// <summary>
        /// Helper method to Validate an admin
        /// </summary>
        /// <param name="admin">The admin to validate</param>
        /// <returns>True if the admin is valid. Throws ResourceNotFoundException if not. Exception is cauth in an Exception Filter</returns>
        private bool validateAdmin(ADMIN admin)
        {
            var personExists = _unitOfWork.AccountRepository.Where(x => x.gordon_id.Trim() == admin.ID_NUM.ToString()).Count() > 0;
            if (!personExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Person was not found." };
            }

            return true;
        }
    }
}