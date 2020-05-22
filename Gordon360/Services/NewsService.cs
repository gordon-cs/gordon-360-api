using Gordon360.Repositories;
using Gordon360.Models;
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
            //TODO change to correct stored procedure and parameter
            var idParam = new SqlParameter("@ACT_CDE", id);
            var result = RawSqlQuery<StudentNewsViewModel>.query("MEMBERSHIPS_PER_ACT_CDE @ACT_CDE", idParam);

            // Getting rid of whitespace inherited from the database o_o 
            // TODO (I want to see what would happen if I didn't do this)
            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.SNID = x.SNID.Trim();
                trim.ADUN = x.ADUN.Trim();
                trim.categoryID = x.categoryID.Trim();
                trim.Subject = x.Subject.Trim();
                trim.Body = x.Body;
                trim.Sent = x.Sent.Trim();
                trim.thisPastMailing = x.thisPastMailing.Trim();
                trim.Entered = x.Entered.Trim();
                trim.fname = x.fname.Trim();
                trim.lname = x.lname.Trim();
                trim.categoryName = x.categoryName.Trim();
                trim.SortOrder = x.SortOrder.Trim();
                trim.ManualExpirationDate = x.ManualExpirationDate.Trim();
                return trim;
            });

            return trimmedResult;
        }

        public IEnumerable<StudentNewsViewModel> GetNewsNew()
        {
            //TODO change to correct stored procedure and check whether parameter needs to be string or DateTime
            DateTime yesterday = DateTime.Today.AddDays(-1);
            DateTime yestAt10 = yesterday.AddHours(10);

            var timeParam = new SqlParameter("@CUTOFF_TIME", yestAt10.ToString());
            var result = RawSqlQuery<StudentNewsViewModel>.query("MEMBERSHIPS_PER_ACT_CDE @CUTOFF_TIME", timeParam);

            // Getting rid of whitespace inherited from the database o_o 
            // TODO (I want to see what would happen if I didn't do this)
            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.SNID = x.SNID.Trim();
                trim.ADUN = x.ADUN.Trim();
                trim.categoryID = x.categoryID.Trim();
                trim.Subject = x.Subject.Trim();
                trim.Body = x.Body;
                trim.Sent = x.Sent.Trim();
                trim.thisPastMailing = x.thisPastMailing.Trim();
                trim.Entered = x.Entered.Trim();
                trim.fname = x.fname.Trim();
                trim.lname = x.lname.Trim();
                trim.categoryName = x.categoryName.Trim();
                trim.SortOrder = x.SortOrder.Trim();
                trim.ManualExpirationDate = x.ManualExpirationDate.Trim();
                return trim;
            });

            return trimmedResult;
        }

        public IEnumerable<StudentNewsCategory> GetNewsCategories()
        {
            //TODO change to correct stored procedure
            var result = RawSqlQuery<StudentNewsCategory>.query("MEMBERSHIPS_PER_ACT_CDE @ACT_CDE");

            // Getting rid of whitespace inherited from the database o_o 
            // TODO (I want to see what would happen if I didn't do this)
            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.categoryID = x.categoryID.Trim();
                trim.categoryName = x.categoryName.Trim();
                trim.SortOrder = x.SortOrder.Trim();
                return trim;
            });

            return trimmedResult;
        }

        public StudentNews Add(StudentNews newsItem)
        {

            // TODO validate the news item?

            // TODO add stored procedure
            return null;
        }
    }
}