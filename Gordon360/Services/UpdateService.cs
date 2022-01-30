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

        public IEnumerable<UpdateAlumniViewModel> updateInfo(int rowID, string email, string homePhone, string mobilePhone, string address1, string address2, string city, string state)
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
                Debug.WriteLine("ID Num: {}   Email: {}   Home Phone: {}   Mobile Phone: {}    Address 1: {}   Address 2: {}   City: {}   State: {}", rowID, email, homePhone, mobilePhone, address1, address2, city, state);
                // result = RawSqlQuery<
                // >.query("UPDATE student_info SET STATUS = 'Saved', EMAIL = @newEmail, HOME_PHONE = @newHomePhone , MOBILE_PHONE = @newMobilePhone, ADDRESS_1 = @newAddress1, ADDRESS_2 = @newAddress2, CITY = @newCity, STATE = @newState);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return result;
        }
    }
}