using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services.ComplexQueries;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Gordon360.Services
{
    public class NewsService : INewsService
    {
        private IUnitOfWork _unitOfWork;

        public NewsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Gets a news item entity by id
        /// </summary>
        /// <param name="newsID">The SNID (id of news item)</param>
        /// <returns>The news item</returns>
        public StudentNews Get(int newsID)
        {
            var newsItem = _unitOfWork.StudentNewsRepository.GetById(newsID);
            // Thrown exceptions will be converted to HTTP Responses by the CustomExceptionFilter
            if (newsItem == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The news item was not found." };
            }
            return newsItem;
        }

        public IEnumerable<StudentNewsViewModel> GetNewsNotExpired()
        {
            return RawSqlQuery<StudentNewsViewModel>.query("NEWS_NOT_EXPIRED");

            // TODO This trimming code was apparently used in services
            // to remove whitespace inherited from the database.
            // Postman showed no extra whitespace in the result, so
            // if there is also no extra whitespace in the result the frontend retrieves,
            // this commented code can be removed. Else, uncomment this code. It is 
            // likely the solution.
            // var trimmedResult = result.Select(x =>
            // {
            //     var trim = x;
            //     trim.SNID = x.SNID;
            //     trim.ADUN = x.ADUN.Trim();
            //     trim.categoryID = x.categoryID;
            //     trim.Subject = x.Subject.Trim();
            //     trim.Body = x.Body;
            //     trim.Sent = x.Sent;
            //     trim.thisPastMailing = x.thisPastMailing;
            //     trim.categoryName = x.categoryName.Trim();
            //     trim.SortOrder = x.SortOrder;
            //     trim.ManualExpirationDate = x.ManualExpirationDate;
            //     return trim;
            // });
            // return trimmedResult;
        }

        public IEnumerable<StudentNewsViewModel> GetNewsNew()
        {
            return RawSqlQuery<StudentNewsViewModel>.query("NEWS_NEW");

            // TODO This trimming code was apparently used in services
            // to remove whitespace inherited from the database.
            // Postman showed no extra whitespace in the result, so
            // if there is also no extra whitespace in the result the frontend retrieves,
            // this commented code can be removed. Else, uncomment this code. It is 
            // likely the solution.
            // var trimmedResult = result.Select(x =>
            // {
            //     var trim = x;
            //     trim.SNID = x.SNID;
            //     trim.ADUN = x.ADUN.Trim();
            //     trim.categoryID = x.categoryID;
            //     trim.Subject = x.Subject.Trim();
            //     trim.Body = x.Body;
            //     trim.Sent = x.Sent;
            //     trim.thisPastMailing = x.thisPastMailing;
            //     trim.Entered = x.Entered;
            //     trim.categoryName = x.categoryName.Trim();
            //     trim.SortOrder = x.SortOrder;
            //     trim.ManualExpirationDate = x.ManualExpirationDate;
            //     return trim;
            // });
            // return trimmedResult;
        }

        public IEnumerable<StudentNewsCategoryViewModel> GetNewsCategories()
        {
            return RawSqlQuery<StudentNewsCategoryViewModel>.query("NEWS_CATEGORIES");

            // TODO This trimming code was apparently used in services
            // to remove whitespace inherited from the database.
            // Postman showed no extra whitespace in the result, so
            // if there is also no extra whitespace in the result the frontend retrieves,
            // this commented code can be removed. Else, uncomment this code. It is 
            // likely the solution.
            // var trimmedResult = result.Select(x =>
            // {
            //     var trim = x;
            //     trim.categoryID = x.categoryID;
            //     trim.categoryName = x.categoryName.Trim();
            //     trim.SortOrder = x.SortOrder;
            //     return trim;
            // });
            // return trimmedResult;
        }

        /// <summary>
        /// Gets unapproved unexpired news submitted by user.
        /// </summary>
        /// <param name="id">user id</param>
        /// <param name="username">username</param>
        /// <returns>Result of query</returns>
        public IEnumerable<StudentNewsViewModel> GetNewsPersonalUnapproved(string id, string username)
        {
            // Verify account
            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            // Query the database
            var usernameParam = new SqlParameter("@Username", username);
            return RawSqlQuery<StudentNewsViewModel>.query("NEWS_PERSONAL_UNAPPROVED @Username", usernameParam);
        }


            /// <summary>
            /// Adds a news item record to storage.
            /// </summary>
            /// <param name="newsItem">The news item to be added</param>
            /// <param name="username">username</param>
            /// <param name="id">id</param>
            /// <returns>The newly added Membership object</returns>
            public StudentNews SubmitNews(StudentNews newsItem, string username, string id)
        {
            // Not currently used
            ValidateNewsItem(newsItem);

            VerifyAccount(id);

            // SQL Parameters
            var usernameParam = new SqlParameter("@Username", username);
            var categoryIDParam = new SqlParameter("@CategoryID", newsItem.categoryID);
            var subjectParam = new SqlParameter("@Subject", newsItem.Subject);
            var bodyParam = new SqlParameter("@Body", newsItem.Body);

            // Run stored procedure
            var result = RawSqlQuery<StudentNewsViewModel>.query("INSERT_NEWS_ITEM @Username, @CategoryID, @Subject, @Body", usernameParam, categoryIDParam, subjectParam, bodyParam);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            return newsItem;
        }

        /// <summary>
        /// (Service) Deletes a news item from the database
        /// </summary>
        /// <param name="newsID">The id of the news item to delete</param>
        /// <returns>The deleted news item</returns>
        /// <remarks>The news item must be authored by the user and must not be expired</remarks>
        public StudentNews DeleteNews(int newsID)
        {
            // Service method 'Get' throws its own exceptions
            var newsItem = Get(newsID);

            // DateTime of date entered is nullable, so we need to check that here before comparing
            // If the entered date is null we shouldn't allow deletion to be safe
            // Note: This check has been duplicated from StateYourBusiness because we do not SuperAdmins
            //    to be able to delete expired news, this should be fixed eventually by removing some of
            //    the SuperAdmin permissions that are not explicitly given
            if(newsItem.Entered == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The news item date could not be verified." };
            }
            var todaysDate = DateTime.Now;
            var newsDate = (DateTime)newsItem.Entered;
            var dateDiff = (todaysDate - newsDate).Days;
            if (dateDiff >= 14)
            {
                throw new Exceptions.CustomExceptions.UnauthorizedAccessException() { ExceptionMessage = "Unauthorized to delete expired news items." };
            }
            var result = _unitOfWork.StudentNewsRepository.Delete(newsItem);
            _unitOfWork.Save();
            return result;
        }

        /// <summary>
        /// Helper method to validate a news item
        /// </summary>
        /// <param name="newsItem">The news item to validate</param>
        /// <returns>True if valid. Throws ResourceNotFoundException if not. Exception is caught in an Exception Filter</returns>
        private bool ValidateNewsItem(StudentNews newsItem)
        {
            // any input sanitization should go here

            return true;
        }

        /// <summary>
        /// Verifies that a student account exists
        /// </summary>
        /// <param name="id">The id of the student</param>
        /// <returns>true if account exists, ResourceNotFoundException if null</returns>
        private bool VerifyAccount(string id)
        {
            // Verify account
            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }
            return true;
        }
    }
}
