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
    public class SaveService : ISaveService
    {
        private IUnitOfWork _unitOfWork;

        public SaveService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Fetch the ride item whose id is specified by the parameter
        /// </summary>
        /// <param name="rideID">The ride id</param>
        /// <returns> ride item if found, null if not found</returns>
        public IEnumerable<RIDE> GetUpcoming()
        {
            var result = RawSqlQuery<RideViewModel>.query("UPCOMING_RIDES"); //run stored procedure

            if (result == null)
            {
                return null;
            }

            return result;
            //var result = _unitOfWork.SaveRepository.FirstOrDefault(x => x.Ride_ID == rideID);
            //if (result == null)
            //{
            //    return null;
            //}

            //return result;
        }

    }
}