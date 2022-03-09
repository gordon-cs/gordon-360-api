using Gordon360.Database.CCT;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Models.ViewModels;
using Gordon360.Static.Methods;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Gordon360.Services
{
    /// <summary>
    /// Service class to facilitate getting emails for members of an activity.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly CCTContext _context;

        public EmailService(CCTContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a list of the emails for all members in the activity during the current session.
        /// </summary>
        /// <param name="activity_code"></param>
        /// <returns></returns>
        public async Task<IEnumerable<EmailViewModel>> GetEmailsForActivity(string activity_code)
        {
            var currentSession = await Helpers.GetCurrentSession();
            return await GetEmailsForActivity(activity_code, currentSession.SessionCode);
        }


        /// <summary>
        /// Get a list of emails for group admin in the activity during the current session.
        /// </summary>
        /// <param name="activity_code"></param>
        /// <returns>A collection of group admin emails</returns>
        public async Task<IEnumerable<EmailViewModel>> GetEmailsForGroupAdmin(string activity_code)
        {
            var currentSession = await Helpers.GetCurrentSession();
            return await GetEmailsForGroupAdmin(activity_code, currentSession.SessionCode);
        }

        /// <summary>
        /// Get a list of emails for group admin in the activity during a specified session.
        /// </summary>
        /// <param name="activity_code"></param>
        /// <param name="session_code"></param>
        /// <returns>A collection of the group admin emails</returns>
        public async Task<IEnumerable<EmailViewModel>> GetEmailsForGroupAdmin(string activity_code, string session_code)
        {
            var result = await _context.Procedures.GRP_ADMIN_EMAILS_PER_ACT_CDEAsync(activity_code, session_code);

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
            }

            return (IEnumerable<EmailViewModel>)result;
        }

        /// <summary>
        /// Get a list of emails for leaders in the activity during the current session.
        /// </summary>
        /// <param name="activity_code"></param>
        /// <returns></returns>
        public async Task<IEnumerable<EmailViewModel>> GetEmailsForActivityLeaders(string activity_code)
        {
            var currentSession = await Helpers.GetCurrentSession();
            return await GetEmailsForActivityLeaders(activity_code, currentSession.SessionCode);
        }

        /// <summary>
        /// Get a list of emails for advisors in the activity during the current session.
        /// </summary>
        /// <param name="activity_code"></param>
        /// <returns></returns>
        public async Task<IEnumerable<EmailViewModel>> GetEmailsForActivityAdvisors(string activity_code)
        {
            var currentSession = await Helpers.GetCurrentSession();
            return await GetEmailsForActivityAdvisors(activity_code, currentSession.SessionCode);
        }

        /// <summary>
        /// Get a list of the emails for all members in the activity during a specific session
        /// </summary>
        /// <param name="activity_code">The activity code</param>
        /// <param name="session_code">The session code</param>
        /// <returns>List of the emails for the members of this activity</returns>
        public async Task<IEnumerable<EmailViewModel>> GetEmailsForActivity(string activity_code, string session_code)
        {
            var result = await _context.Procedures.EMAILS_PER_ACT_CDEAsync(activity_code, session_code);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
            }

            return (IEnumerable<EmailViewModel>)result;
        }


        /// <summary>
        /// Get a list of emails for leaders in the activity during a specified session
        /// </summary>
        /// <param name="activity_code">The activity code</param>
        /// <param name="session_code">The session code</param>
        /// <returns>List of emails for the leaders of this activity</returns>
        public async Task<IEnumerable<EmailViewModel>> GetEmailsForActivityLeaders(string activity_code, string session_code)
        {
            var result = await _context.Procedures.LEADER_EMAILS_PER_ACT_CDEAsync(activity_code, session_code);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
            }

            return (IEnumerable<EmailViewModel>)result;
        }

        /// <summary>
        /// Get a list of emails for leaders in the activity during a specified session
        /// </summary>
        /// <param name="activity_code">The activity code</param>
        /// <param name="session_code">The session code</param>
        /// <returns>List of emails for the leaders of this activity</returns>
        public async Task<IEnumerable<EmailViewModel>> GetEmailsForActivityAdvisors(string activity_code, string session_code)
        {
            var result = await _context.Procedures.ADVISOR_EMAILS_PER_ACT_CDEAsync(activity_code, session_code);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
            }

            return (IEnumerable<EmailViewModel>)result;
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
        public void SendEmails(string[] to_emails, string from_email, string subject, string email_content, string password)
        {
            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = from_email,
                    Password = password
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.office365.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                var message = new MailMessage();
                message.From = new MailAddress(from_email);
                message.Bcc.Add(new MailAddress(from_email));
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

        /// <summary>
        /// Send a email to members of an activity
        /// </summary>
        /// <param name="activityCode">The activity code to send this email to</param>
        /// <param name="sessionCode">The session of activity to select members from</param>
        /// <param name="from_email">The address this email is sent from</param>
        /// <param name="subject">Subject of the email to be sent</param>
        /// <param name="email_content">The content of the email to be sent</param>
        /// <param name="password">Password of the email sender</param>
        /// <returns></returns>
        public async Task SendEmailToActivity(string activityCode, string sessionCode, string from_email, string subject, string email_content, string password)
        {
            var credential = new NetworkCredential
            {
                UserName = from_email,
                Password = password
            };

            using var smtp = new SmtpClient
            {
                Credentials = credential,
                Host = "smtp.office365.com",
                Port = 587,
                EnableSsl = true
            };

            var message = new MailMessage
            {
                From = new MailAddress(from_email),
                Subject = subject,
                Body = email_content,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(from_email));

            var to_emails = (await GetEmailsForActivity(activityCode, sessionCode)).Select(x => x.Email);
            foreach (string to_email in to_emails)
            {
                message.Bcc.Add(new MailAddress(to_email));
            }

            smtp.Send(message);
        }
    }
}