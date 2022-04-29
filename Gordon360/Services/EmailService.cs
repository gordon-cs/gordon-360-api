using Gordon360.Database.CCT;
using Gordon360.Exceptions;
using Gordon360.Models.ViewModels;
using Gordon360.Static.Methods;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using static Gordon360.Services.MembershipService;

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
        /// <param name="activityCode">The code of the activity to get emails for.</param>
        /// <param name="sessionCode">Optionally, the session to get emails for. Defaults to the current session</param>
        /// <param name="participationType">The participation type to get emails of. If unspecified, gets emails of all participation types.</param>
        /// <returns>A list of emails (along with first and last name) associated with that activity</returns>
        public async Task<IEnumerable<EmailViewModel>> GetEmailsForActivityAsync(string activityCode, string? sessionCode = null, ParticipationType? participationType = null)
        {
            if (sessionCode == null)
            {
                var currentSession = await Helpers.GetCurrentSessionAsync();
                sessionCode = currentSession.SessionCode;
            }

            var membershipService = new MembershipService(_context);

            var result = membershipService.MembershipEmails(activityCode, sessionCode, participationType);

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
        public async Task SendEmailToActivityAsync(string activityCode, string sessionCode, string from_email, string subject, string email_content, string password)
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

            var to_emails = (await GetEmailsForActivityAsync(activityCode, sessionCode)).Select(x => x.Email);
            foreach (string to_email in to_emails)
            {
                message.Bcc.Add(new MailAddress(to_email));
            }

            smtp.Send(message);
        }
    }
}