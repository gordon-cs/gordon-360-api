using System.Collections.Generic;
using System;
using System.Linq;
using System.Web.Mvc;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.AuthorizationFilters;
using Gordon360.Static.Names;
using Gordon360.Static.Data;

namespace Gordon360.Services
{
    /// <summary>
    /// Service Class that facilitates data transactions between the AccountsController and the Account database model.
    /// </summary>
    public class AccountService : IAccountService
    {
        // See UnitOfWork class
        private IUnitOfWork _unitOfWork;

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
 

        /// <summary>
        /// Fetches a single account record whose id matches the id provided as an argument
        /// </summary>
        /// <param name="id">The person's gordon id</param>
        /// <returns>AccountViewModel if found, null if not found</returns>
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.ACCOUNT)]
        public AccountViewModel Get(string id)
        {
            var query = _unitOfWork.AccountRepository.GetById(id);
            if (query == null)
            {
                // Custom Exception is thrown that will be cauth in the controller Exception filter.
                throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
            }
            AccountViewModel result = query;
            return result;
        }

        /// <summary>
        /// Fetches all the account records from storage.
        /// </summary>
        /// <returns>AccountViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.ACCOUNT)]
        public IEnumerable<AccountViewModel> GetAll()
        {
            var query = _unitOfWork.AccountRepository.GetAll();
            var result = query.Select<ACCOUNT, AccountViewModel>(x => x); //Map the database model to a more presentable version (a ViewModel)
            return result;
        }

        /// <summary>
        /// Fetches the account record with the specified email.
        /// </summary>
        /// <param name="email">The email address associated with the account.</param>
        /// <returns></returns>
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.ACCOUNT)]
        public AccountViewModel GetAccountByEmail(string email)
        {
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.email == email);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
            }
            AccountViewModel result = query; // Implicit conversion happening here, see ViewModels.
            return result;
        }

        /// <summary>
        /// Fetches the account record with the specified username.
        /// </summary>
        /// <param name="username">The username associated with the account.</param>
        /// <returns></returns>
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.ACCOUNT)]
        public AccountViewModel GetAccountByUsername(string username)
        {
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.AD_Username == username);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
            }
            AccountViewModel result = query; // Implicit conversion happening here, see ViewModels.
            return result;
        }
    }
}