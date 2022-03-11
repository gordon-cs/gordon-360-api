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

    /// TODO: Replace dummy variables with their actual variable names, change SQL feature names with the correct names
    /// NOTE: Not sure if "query" in RawSqlQuery.cs is the correct query to use. We may have to create our own method

    /// TODO: Change the following variable types to their appropriate types

    /// <summary>
    /// Service Class that facilitates data transactions between the UpdateController and the student_info database model.
    /// </summary>
    public class UpdateService : IUpdateService
    {
        private IUnitOfWork _unitOfWork;

        public UpdateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public void SendUpdateRequest(string to_email, string from_email, string subject, string email_content, string password)
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
                message.To.Add(new MailAddress(to_email));
                message.Subject = subject;
                message.Body = email_content;
                message.IsBodyHtml = true;

                smtp.Send(message);
            }
        }
        /*
        public IEnumerable<UpdateAlumniViewModel> updateInfo(
        int userID, 
        string userSalutation, 
        string userFirstName,
        string userLastName, 
        string userMiddleName, 
        string userPreferredName,
        string userPersonalEmail,
        string userWorkEmail,
        string userAlternateEmail,
        string userPreferredEmail,
        string userDoNotContact,
        string userDoNotMail,
        string userHomePhone,
        string userWorkPhone,
        string userMobilePhone,
        string userPreferredPhone,
        string userMailingStreet,
        string userMailingCity,
        string userMailingState,
        string userMailingZip,
        string userMailingCountry,
        string userMaritalStatus
        )
        {
            IEnumerable<UpdateAlumniViewModel> result = null;

            var id = new SqlParameter("@ID", rowID);
            var newEmail = new SqlParameter("@newEmail", email);
            var newHomePhone = new SqlParameter("@newHomePhone", homePhone);
            var newMobilePhone = new SqlParameter("@newMobilePhone", mobilePhone);
            var newAddress1 = new SqlParameter("@newAddress1", address1);
            var newAddress2 = new SqlParameter("@newAddress2", address2);
            var newCity = new SqlParameter("@newCity", city);
            var newState = new SqlParameter("@newState", state);

            try
            {
                result = <UpdateAlumniViewModel>.query("UPDATE student_info SET STATUS = 'Saved', EMAIL = @newEmail, HOME_PHONE = @newHomePhone , MOBILE_PHONE = @newMobilePhone, ADDRESS_1 = @newAddress1, ADDRESS_2 = @newAddress2, CITY = @newCity, STATE = @newState);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return result;
        }
        */
    }
}