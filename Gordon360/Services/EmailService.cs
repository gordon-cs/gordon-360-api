using System;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using System.Collections.Generic;
using Gordon360.Models;
using System.Linq;
using System.Data.SqlClient;
using Gordon360.Services.ComplexQueries;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Static.Methods;
using System.Net.Mail;
using System.Net;

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
        /// Get a list of the emails for all members in the activity during the current session.
        /// </summary>
        /// <param name="activity_code"></param>
        /// <returns></returns>
        public IEnumerable<EmailViewModel> GetEmailsForActivity(string activity_code)
        {
            var currentSessionCode = Helpers.GetCurrentSession().SessionCode;
            var idParam = new SqlParameter("@ACT_CDE", activity_code);
            var sessParam = new SqlParameter("@SESS_CDE", currentSessionCode);
            var result = RawSqlQuery<EmailViewModel>.query("EMAILS_PER_ACT_CDE @ACT_CDE, @SESS_CDE", idParam, sessParam);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
            }

            return result;
        }


        /// <summary>
        /// Get a list of emails for group admin in the activity during the current session.
        /// </summary>
        /// <param name="activity_code"></param>
        /// <returns>A collection of group admin emails</returns>
        public IEnumerable<EmailViewModel> GetEmailsForGroupAdmin(string activity_code)
        {
            var currentSessionCode = Helpers.GetCurrentSession().SessionCode;
            var idParam = new SqlParameter("@ACT_CDE", activity_code);
            var sessParam = new SqlParameter("@SESS_CDE", currentSessionCode);
            var result = RawSqlQuery<EmailViewModel>.query("GRP_ADMIN_EMAILS_PER_ACT_CDE @ACT_CDE, @SESS_CDE", idParam, sessParam);

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
            }

            return result;
        }

        /// <summary>
        /// Get a list of emails for group admin in the activity during a specified session.
        /// </summary>
        /// <param name="activity_code"></param>
        /// <param name="session_code"></param>
        /// <returns>A collection of the group admin emails</returns>
        public IEnumerable<EmailViewModel> GetEmailsForGroupAdmin(string activity_code, string session_code)
        {
            var idParam = new SqlParameter("@ACT_CDE", activity_code);
            var sessParam = new SqlParameter("@SESS_CDE", session_code);
            var result = RawSqlQuery<EmailViewModel>.query("GRP_ADMIN_EMAILS_PER_ACT_CDE @ACT_CDE, @SESS_CDE", idParam, sessParam);

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
            }

            return result;
        }

        /// <summary>
        /// Get a list of emails for leaders in the activity during the current session.
        /// </summary>
        /// <param name="activity_code"></param>
        /// <returns></returns>
        public IEnumerable<EmailViewModel> GetEmailsForActivityLeaders(string activity_code)
        {
            var currentSessionCode = Helpers.GetCurrentSession().SessionCode;
            var idParam = new SqlParameter("@ACT_CDE", activity_code);
            var sessParam = new SqlParameter("@SESS_CDE", currentSessionCode);
            var result = RawSqlQuery<EmailViewModel>.query("LEADER_EMAILS_PER_ACT_CDE @ACT_CDE, @SESS_CDE", idParam, sessParam);

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
            }

            return result;
        }

        /// <summary>
        /// Get a list of emails for advisors in the activity during the current session.
        /// </summary>
        /// <param name="activity_code"></param>
        /// <returns></returns>
        public IEnumerable<EmailViewModel> GetEmailsForActivityAdvisors(string activity_code)
        {
            var currentSessionCode = Helpers.GetCurrentSession().SessionCode;
            var idParam = new SqlParameter("@ACT_CDE", activity_code);
            var sessParam = new SqlParameter("@SESS_CDE", currentSessionCode);
            var result = RawSqlQuery<EmailViewModel>.query("ADVISOR_EMAILS_PER_ACT_CDE @ACT_CDE, @SESS_CDE", idParam, sessParam);

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
            }

            return result;
        }

        /// <summary>
        /// Get a list of the emails for all members in the activity during a specific session
        /// </summary>
        /// <param name="activity_code">The activity code</param>
        /// <param name="session_code">The session code</param>
        /// <returns>List of the emails for the members of this activity</returns>
        public IEnumerable<EmailViewModel> GetEmailsForActivity(string activity_code, string session_code)
        {
            
            var idParam = new SqlParameter("@ACT_CDE", activity_code);
            var sessParam = new SqlParameter("@SESS_CDE", session_code);
            var result = RawSqlQuery<EmailViewModel>.query("EMAILS_PER_ACT_CDE @ACT_CDE, @SESS_CDE", idParam, sessParam);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
            }
            
            return result;           
        }

       

        /// <summary>
        /// Get a list of emails for leaders in the activity during a specified session
        /// </summary>
        /// <param name="activity_code">The activity code</param>
        /// <param name="session_code">The session code</param>
        /// <returns>List of emails for the leaders of this activity</returns>
        public IEnumerable<EmailViewModel> GetEmailsForActivityLeaders(string activity_code, string session_code)
        {
            var idParam = new SqlParameter("@ACT_CDE", activity_code);
            var sessParam = new SqlParameter("@SESS_CDE", session_code);
            var result = RawSqlQuery<EmailViewModel>.query("LEADER_EMAILS_PER_ACT_CDE @ACT_CDE, @SESS_CDE", idParam, sessParam);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
            }

            return result;
        }

        /// <summary>
        /// Get a list of emails for leaders in the activity during a specified session
        /// </summary>
        /// <param name="activity_code">The activity code</param>
        /// <param name="session_code">The session code</param>
        /// <returns>List of emails for the leaders of this activity</returns>
        public IEnumerable<EmailViewModel> GetEmailsForActivityAdvisors(string activity_code, string session_code)
        {
            var idParam = new SqlParameter("@ACT_CDE", activity_code);
            var sessParam = new SqlParameter("@SESS_CDE", session_code);
            var result = RawSqlQuery<EmailViewModel>.query("ADVISOR_EMAILS_PER_ACT_CDE @ACT_CDE, @SESS_CDE", idParam, sessParam);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
            }

            return result;
        }

        /// <summary>
        /// Send a email to a list of email addresses
        /// </summary>
        /// <param name="to_emails">All addresses to send this email to</param>
        /// <param name="from_email">The address this email is sent from</param>
        /// <param name="subject">Subject of the email to be sent</param>
        /// <param name="email_content">The content of the email to be sent</param>
        /// <param name="password">Password of the email sender</param>
        /// <returns></returns>
        public void SendEmails(string [] to_emails, string from_email, string subject, string email_content, string password)
        {
            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = from_email,
                    Password = password
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.gordon.edu";
                smtp.Port = 465;
                smtp.EnableSsl = true;
                var message = new MailMessage();
                message.From = new MailAddress(from_email);
                message.CC.Add(new MailAddress(from_email));
                foreach (string to_email in to_emails)
                {
                    message.To.Add(new MailAddress(to_email));
                }
                message.Subject = subject;
                message.Body = email_content;
                message.IsBodyHtml = true;

                smtp.Send(message);
            }
        }
    }
}