using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;

namespace Gordon360.Services
{
    /// <summary>
    /// Service Class that facilitates data transactions between the AccountsController and the Account database model.
    /// </summary>
    public class ChapelEventService : IChapelEventService
    {
        // See UnitOfWork class
        private IUnitOfWork _unitOfWork;

        public ChapelEventService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Fetches a single account record whose id matches the id provided as an argument
        /// </summary>
        /// <param name="id">The person's gordon id</param>
        /// <returns>AccountViewModel if found, null if not found</returns>
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.ChapelEvent)]
        public ChapelEventViewModel Get(string id)
        {
            var query = _unitOfWork.ChapelEventRepository.GetById(id);
            if (query == null)
            {
                // Custom Exception is thrown that will be cauth in the controller Exception filter.
                throw new ResourceNotFoundException() { ExceptionMessage = "The event was not found." };
            }
            ChapelEventViewModel result = query;
            return result;
        }

        /// <summary>
        /// Fetches all the account records from storage.
        /// </summary>
        /// <returns>AccountViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.ChapelEvent)]
        public IEnumerable<ChapelEventViewModel> GetAll()
        {
            var query = _unitOfWork.ChapelEventRepository.GetAll();
            var result = query.Select<ChapelEvent, ChapelEventViewModel>(x => x); //Map the database model to a more presentable version (a ViewModel)
            return result;
        }

        /// <summary>
        /// Fetches the account record with the specified email.
        /// </summary>
        /// <param name="CHEventID">The email address associated with the account.</param>
        /// <returns></returns>
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.ChapelEvent)]
        public ChapelEventViewModel GetChapelEventByChapelEventID(string CHEventID)
        {
            var query = _unitOfWork.ChapelEventRepository.FirstOrDefault(x => x.CHEventID == CHEventID);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The event was not found." };
            }
            ChapelEventViewModel result = query; // Implicit conversion happening here, see ViewModels.
            return result;
        }
    }
}