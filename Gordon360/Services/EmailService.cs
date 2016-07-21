using System;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using System.Collections.Generic;
using Gordon360.Models;
using System.Linq;
using System.Data.SqlClient;
using Gordon360.Services.ComplexQueries;
using Gordon360.Exceptions.CustomExceptions;

namespace Gordon360.Services
{
    /// <summary>
    /// Service class to facilitate getting emails for members of an activity.
    /// </summary>
    public class EmailService : IEmailService
    {
        private IUnitOfWork _unitOfWork;

        public EmailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get a list of the emails for all members in the activity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<EmailViewModel> GetEmailsForActivity(string id)
        {
            var idParam = new SqlParameter("@ACT_CDE", id);
            var result = RawSqlQuery<EmailViewModel>.query("EMAILS_PER_ACT_CDE @ACT_CDE", idParam);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
            }
            return result;           
        }

        /// <summary>
        /// Get a list of emails for leaders in the activity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<EmailViewModel> GetEmailsForActivityLeaders(string id)
        {
            var idParam = new SqlParameter("@ACT_CDE", id);
            var result = RawSqlQuery<EmailViewModel>.query("LEADER_EMAILS_PER_ACT_CDE @ACT_CDE", idParam);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
            }
            return result;
        }
    }
}