using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services.ComplexQueries;
using System;
using System.Collections.Generic;
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
    }
}