
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Gordon360.Models;
    using Gordon360.Models.ViewModels;
    using Gordon360.Repositories;
    using Gordon360.Utils.ComplexQueries;
    using System.Data.SqlClient;
    using System.Data;
    using Gordon360.Exceptions.CustomExceptions;
    using Gordon360.Static.Methods;
    using System.Diagnostics;

namespace Gordon360.Utils
{
    /// <summary>
    /// Service Class that facilitates data transactions between the ScheduleControlController and the ScheduleControl part of the database model.
    /// </summary>
    public class ScheduleControlService : IScheduleControlService
    {
        private IUnitOfWork _unitOfWork;

        public ScheduleControlService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// privacy setting of schedule.
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="value">Y or N</param>
        public void UpdateSchedulePrivacy(string id, string value)
        {
            var original = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }
            
            var idParam = new SqlParameter("@ID", id);
            var valueParam = new SqlParameter("@VALUE", value);
            var context = new CCTEntities1();
            context.Database.ExecuteSqlCommand("UPDATE_SCHEDULE_PRIVACY @ID, @VALUE", idParam, valueParam); // run stored procedure.

        }


        /// <summary>
        /// description of schedule.
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="value">New description</param>
        public void UpdateDescription(string id, string value)
        {
            var original = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var idParam = new SqlParameter("@ID", id);
            var valueParam = new SqlParameter("@VALUE", value);
            var context = new CCTEntities1();
            context.Database.ExecuteSqlCommand("UPDATE_DESCRIPTION @ID, @VALUE", idParam, valueParam); // run stored procedure.

        }


        /// <summary>
        /// Update timestamp of modification in schedule.
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="value">Modified Time</param>
        public void UpdateModifiedTimeStamp(string id, DateTime value)
        {
            var original = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var idParam = new SqlParameter("@ID", id);
            var valueParam = new SqlParameter("@VALUE", value);
            var context = new CCTEntities1();
            context.Database.ExecuteSqlCommand("UPDATE_TIMESTAMP @ID, @VALUE", idParam, valueParam); // run stored procedure.

        }
    }
}