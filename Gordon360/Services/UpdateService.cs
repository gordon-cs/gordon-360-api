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
using Gordon360.Static.Methods;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;

namespace Gordon360.Services
{
    /// <summary>
    /// Service Class that facilitates data transactions between the UpdateController and the email sender.
    /// </summary>
    public class UpdateService : IUpdateService
    {
        private IUnitOfWork _unitOfWork;

        public UpdateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public void SendUpdateRequest(int userID, string email_content)
        {
            using (var smtp = new SmtpClient())
            {
                string to_email = "TO EMAIL";
                string from_email = "FROM EMAIL";
                string subject = String.Format("UPDATE REQUEST: Alumni {0}", userID);
                var credential = new NetworkCredential
                {
                    UserName = from_email,
                    Password = "PASSWORD"
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp.office365.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                var message = new MailMessage();
                message.From = new MailAddress(from_email);
                message.Bcc.Add(new MailAddress(from_email));
                message.To.Add(new MailAddress(to_email));
                message.Subject = subject;
                message.Body = email_content;
                message.IsBodyHtml = true;

                smtp.Send(message);
            }
        }
    }
}