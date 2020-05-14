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
            //TODO change to correct stored procedure
            var result = RawSqlQuery<StudentNewsViewModel>.query("NEWS_NOT_EXPIRED");

            // Getting rid of whitespace inherited from the database o_o 
            // TODO (I want to see what would happen if I didn't do this)
            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.SNID = x.SNID;
                trim.ADUN = x.ADUN.Trim();
                trim.categoryID = x.categoryID;
                trim.Subject = x.Subject.Trim();
                trim.Body = x.Body;
                trim.Sent = x.Sent;
                trim.thisPastMailing = x.thisPastMailing;
                trim.Entered = x.Entered;
                trim.fname = x.fname.Trim();
                trim.lname = x.lname.Trim();
                trim.categoryName = x.categoryName.Trim();
                trim.SortOrder = x.SortOrder;
                trim.ManualExpirationDate = x.ManualExpirationDate;
                return trim;
            });

            return trimmedResult;
        }

        public IEnumerable<StudentNewsViewModel> GetNewsNew()
        {
            var result = RawSqlQuery<StudentNewsViewModel>.query("NEWS_NEW");

            // Getting rid of whitespace inherited from the database o_o 
            // TODO (I want to see what would happen if I didn't do this)
            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.SNID = x.SNID;
                trim.ADUN = x.ADUN.Trim();
                trim.categoryID = x.categoryID;
                trim.Subject = x.Subject.Trim();
                trim.Body = x.Body;
                trim.Sent = x.Sent;
                trim.thisPastMailing = x.thisPastMailing;
                trim.Entered = x.Entered;
                trim.fname = x.fname.Trim();
                trim.lname = x.lname.Trim();
                trim.categoryName = x.categoryName.Trim();
                trim.SortOrder = x.SortOrder;
                trim.ManualExpirationDate = x.ManualExpirationDate;
                return trim;
            });

            return trimmedResult;
        }

        public IEnumerable<StudentNewsCategory> GetNewsCategories()
        {
            var result = RawSqlQuery<StudentNewsCategory>.query("NEWS_CATEGORIES");

            // Getting rid of whitespace inherited from the database o_o 
            // TODO (I want to see what would happen if I didn't do this)
            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.categoryID = x.categoryID;
                trim.categoryName = x.categoryName.Trim();
                trim.SortOrder = x.SortOrder;
                return trim;
            });

            return trimmedResult;
        }
    }
}