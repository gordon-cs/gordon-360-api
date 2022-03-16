using Gordon360.AuthorizationFilters;
using Gordon360.Database.CCT;
using Gordon360.Exceptions;
using Gordon360.Models.ViewModels;
using Gordon360.Static.Names;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Services
{

    /// <summary>
    /// Service Class that facilitates data transactions between the AccountsController and the Account database model.
    /// </summary>
    public class AccountService : IAccountService
    {
        // See UnitOfWork class
        private CCTContext _context;

        public AccountService(CCTContext context)
        {
            _context = context;
        }



        /// <summary>
        /// Fetches a single account record whose id matches the id provided as an argument
        /// </summary>
        /// <param name="id">The person's gordon id</param>
        /// <returns>AccountViewModel if found, null if not found</returns>
        [StateYourBusiness(operation = Operation.READ_ONE, resource = Resource.ACCOUNT)]
        public AccountViewModel? GetAccountByID(string id)
        {
            var account = _context.ACCOUNT.FirstOrDefault(x => x.gordon_id == id);
            if (account == null)
            {
                // Custom Exception is thrown that will be cauth in the controller Exception filter.
                throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
            }
            return account;
        }

        /// <summary>
        /// Fetches all the account records from storage.
        /// </summary>
        /// <returns>AccountViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        [StateYourBusiness(operation = Operation.READ_ALL, resource = Resource.ACCOUNT)]
        public IEnumerable<AccountViewModel> GetAll()
        {
            return (IEnumerable<AccountViewModel>)_context.ACCOUNT; //Map the database model to a more presentable version (a ViewModel)
        }

        /// <summary>
        /// Fetches the account record with the specified email.
        /// </summary>
        /// <param name="email">The email address associated with the account.</param>
        /// <returns>the student account information</returns>
        public AccountViewModel? GetAccountByEmail(string email)
        {
            var account = _context.ACCOUNT.FirstOrDefault(x => x.email == email);
            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
            }
            return account;
        }

        /// <summary>
        /// Fetches the account record with the specified username.
        /// </summary>
        /// <param name="username">The AD username associated with the account.</param>
        /// <returns>the student account information</returns>
        public AccountViewModel? GetAccountByUsername(string username)
        {
            var account = _context.ACCOUNT.FirstOrDefault(x => x.AD_Username == username);
            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
            }
            return account;
        }
    }
}