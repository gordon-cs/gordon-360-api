using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Services
{
    /// <summary>
    /// Service class to facilitate interacting with the Admin table.
    /// </summary>
    public class AdministratorService : IAdministratorService
    {
        private CCTContext _context;
        private IAccountService _accountService;

        public AdministratorService(CCTContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }
        /// <summary>
        /// Fetches all the administrators from the database
        /// </summary>
        /// <returns>Returns a list of administrators. If no administrators were found, an empty list is returned.</returns>
        public IEnumerable<AdminViewModel> GetAll()
        {
            return _context.ADMIN.Select(a => (AdminViewModel)a);
        }

        /// <summary>
        /// Fetches a specific admin from the database
        /// </summary>
        /// <returns>Returns a list of administrators. If no administrators were found, an empty list is returned.</returns>
        public AdminViewModel? GetByUsername(string username)
        {
            var admin = _context.ADMIN.FirstOrDefault(a => a.USER_NAME == username);
            if (admin != null)
                return (AdminViewModel)admin;
            return null;
        }

        /// <summary>
        /// Adds a new Administrator record to storage. Since we can't establish foreign key constraints and relationships on the database side,
        /// we do it here by using the validateAdmin() method.
        /// </summary>
        /// <param name="adminView">The admin to be added</param>
        /// <returns>The newly added Admin object</returns>
        public AdminViewModel Add(AdminViewModel adminView)
        {
            // validate returns a boolean value.
            validateAdmin(adminView);

            var gordonId = int.Parse(_accountService.GetAccountByUsername(adminView.Username).GordonID);

            // The Add() method returns the added membership.
            var payload = _context.ADMIN.Add(adminView.ToAdmin(gordonId));

            // There is a unique constraint in the Database on columns (ID_NUM, PART_LVL, SESS_CDE and ACT_CDE)
            if (payload == null)
            {
                throw new ResourceCreationException() { ExceptionMessage = "There was an error creating the admin." };
            }
            _context.SaveChanges();

            return adminView;

        }

        /// <summary>
        /// Delete the admin whose id is specified by the parameter.
        /// </summary>
        /// <param name="id">The admin id</param>
        /// <returns>The admin that was just deleted</returns>
        public AdminViewModel Delete(int id)
        {
            var result = _context.ADMIN.Find(id);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Admin was not found." };
            }
            _context.ADMIN.Remove(result);

            _context.SaveChanges();

            return (AdminViewModel)result;
        }

        /// <summary>
        /// Helper method to Validate an admin
        /// </summary>
        /// <param name="admin">The admin to validate</param>
        /// <returns>True if the admin is valid. Throws ResourceNotFoundException if not. Exception is cauth in an Exception Filter</returns>
        private bool validateAdmin(AdminViewModel adminView)
        {
            var personExists = _context.ACCOUNT.Where(a => a.AD_Username == adminView.Username).Count() > 0;
            if (!personExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Person was not found." };
            }

            var personIsAlreadyAdmin = _context.ADMIN.Any(a => a.USER_NAME == adminView.Username);
            if (personIsAlreadyAdmin)
            {
                throw new ResourceCreationException() { ExceptionMessage = "This person is already an admin." };
            }

            return true;
        }
    }
}