using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services.ComplexQueries;
using System.Data.SqlClient;
using System.Data;
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

        /// <summary>
        /// Returns all attended events for a student
        /// </summary>
        /// <param name="ID"> The student's ID</param>
        /// <returns></returns>
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.ChapelEvent)]
        public IEnumerable<ChapelEventViewModel> GetAllForStudent(string ID)
        {
            var studentExists = _unitOfWork.AccountRepository.Where(x => x.gordon_id.Trim() == ID).Count() > 0;
            if (!studentExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
            }

            /// Declare the variables used
            var idParam = new SqlParameter("@STU_ID", ID);
            var result = RawSqlQuery<ChapelEventViewModel>.query("EVENTS_BY_STUDENT_ID @STU_ID", idParam);

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The student was not found" };
            }

            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.ROWID = x.ROWID;
                trim.CHBarEventID = x.CHBarEventID.Trim();
                trim.CHEventID = x.CHEventID.Trim();
                trim.CHCheckerID = x.CHCheckerID.Trim();
                trim.CHDate = x.CHDate;
                trim.CHTime = x.CHTime;
                trim.CHSource = x.CHSource.Trim();
                trim.CHTermCD = x.CHTermCD.Trim();
                return trim;
            });
            return result;
        }

        /// <summary>
        /// Returns all attended events for a student
        /// </summary>
        /// <param name="ID"> The student's ID</param>
        /// <param name="term"> The current term</param>
        /// <returns></returns>
        [StateYourBusiness(operation = Operation.READ_PARTIAL, resource = Resource.ChapelEvent)]
        public string GetCreditsForStudent(string ID, string term)
        {
            var studentExists = _unitOfWork.AccountRepository.Where(x => x.gordon_id.Trim() == ID).Count() > 0;
            if (!studentExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
            }

            /// Declare the variables used
            var idParam = new SqlParameter("@STU_ID", ID);
            var termParam = new SqlParameter("@TERM", term);
            string[] parameters = new string[2];
            parameters[0] = ID;
            parameters[1] = term;

            var result = RawSqlQuery<String>.query("TOTAL_CREDITS_PER_STUDENT @STU_ID @TERM", parameters );

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The student was not found" };
            }

            return result.ToString();
        }
       }
}