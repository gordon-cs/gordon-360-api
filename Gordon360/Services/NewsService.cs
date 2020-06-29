using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services.ComplexQueries;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Gordon360.Services
{
    public class NewsService : INewsService
    {
        private IUnitOfWork _unitOfWork;

        public NewsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

        public IEnumerable<StudentNewsViewModel> GetNewsPersonalUnapproved(string id, string username)
        {
            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

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
            ValidateNewsItem(newsItem);

            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var usernameParam = new SqlParameter("@Username", username);
            var categoryIDParam = new SqlParameter("@CategoryID", newsItem.categoryID);
            var subjectParam = new SqlParameter("@Subject", newsItem.Subject);
            var bodyParam = new SqlParameter("@Body", newsItem.Body);

            // run stored procedure
            var result = RawSqlQuery<StudentNewsViewModel>.query("INSERT_NEWS_ITEM @Username, @CategoryID, @Subject, @Body", usernameParam, categoryIDParam, subjectParam, bodyParam);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            return newsItem;

        }

        /// <summary>
        /// Helper method to validate a news item
        /// </summary>
        /// <param name="newsItem">The news item to validate</param>
        /// <returns>True if valid. Throws ResourceNotFoundException if not. Exception is caught in an Exception Filter</returns>
        private bool ValidateNewsItem(StudentNews newsItem)
        {
            // need to sanitize inputs? Or stored procedures safe enough as is

            return true;
        }
    }
}
