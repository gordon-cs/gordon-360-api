using System;
using System.Collections.Generic;
using System.Linq;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Exceptions.CustomExceptions;
using System.Data.SqlClient;
using Gordon360.Services.ComplexQueries;
using static Gordon360.Controllers.Api.WellnessController;
using Gordon360.Models;

namespace Gordon360.Services
{
    public class WellnessService : IWellnessService
    {

        private IUnitOfWork _unitOfWork;

        public WellnessService()
        {
            _unitOfWork = new UnitOfWork();
        }

        /// <summary>
        /// Get the status of the user by id
        /// </summary>
        /// <param name="id">ID of the user to get the status of</param>
        /// <returns> The status of the user, a WellnessViewModel </returns>

        public WellnessViewModel GetStatus(string id)
        {
            var account = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            int IDNum = int.Parse(id);
            var result = _unitOfWork.WellnessRepository.Where(x => x.ID_Num == IDNum).OrderByDescending(x => x.Created).FirstOrDefault();

            if (result == null)
            {
                // No status was found for the given user, so return an invalid fake status
                return new WellnessViewModel
                {
                    Status = "GREEN",
                    Created = new DateTime(1900, 01, 01),
                    IsValid = false
                };
            }
            else
            {
                return new WellnessViewModel
                {
                    Status = ((WellnessStatusColor)result.HealthStatusID).ToString(),
                    Created = result.Created,
                    IsValid = result.Expires == null || DateTime.Now < result.Expires
                };
            }
        }

        /// <summary>
        /// Stores wellness Status in database.
        /// </summary>
        /// <param name="id"> ID of the user to post the status for</param>
        /// <param name="status"> Status that is being posted, one of the WellnessStatusColors </param>
        /// <returns>Status that was successfully recorded</returns>
        
        public Health_Status PostStatus(WellnessStatusColor status, string id)
        {
            var account = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var now = DateTime.Now;

            var statusObject = new Health_Status
            {
                ID_Num = int.Parse(id),
                HealthStatusID = (byte) status,
                Created = now,
                Expires = ExpirationDate(now),
                CreatedBy = "360.WellnessCheck",
                Emailed = null
            };

            var result = _unitOfWork.WellnessRepository.Add(statusObject);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }
            _unitOfWork.Save();

            return result;
        }

        /// <summary>
        /// gets the question for the wellness check from the back end
        /// </summary>
        /// <returns>A WellnessQuestionViewModel including the text of the question and the disclaimers for positive and negative answers.</returns>

        public WellnessQuestionViewModel GetQuestion()
        {

            var result = RawSqlQuery<WellnessQuestionViewModel>.query("GET_HEALTH_CHECK_QUESTION"); //run stored procedure
            
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            var wellnessQuestionModel = result.SingleOrDefault();

            return wellnessQuestionModel;
        }


        /// <summary>
        /// Creates an expiration date for a check in at the current time
        /// </summary>
        /// <param name="currentTime">The time of the check in</param>
        /// <returns>When the check in should expire (the next 5AM).</returns>
        private DateTime ExpirationDate(DateTime currentTime)
        {
            // Answer expires at 5AM
            DateTime expirationTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 5, 0, 0);

            // If it's past 5AM, expirationTime is 5AM tomorrow.
            if (currentTime > expirationTime)
            {
                TimeSpan day = new TimeSpan(24, 0, 0);
                expirationTime = new DateTime(expirationTime.Ticks + day.Ticks);
            }

            return expirationTime;
        }

        /// <summary>
        /// Checks whether a wellness status is still valid (i.e. new since 5AM)
        /// </summary>
        /// <param name='status'>Wellness status to check, an instance of a WellnessViewModel</param>
        /// <returns>boolean representing whether the status is still valid</returns>
        private bool DEPRECATED_IsValidStatus(DEPRECATED_WellnessStatusViewModel status)
        {
            if (status.IsOverride)
            {
                return true;
            }

            DateTime currentTime = DateTime.Now;

            DateTime changeTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 5, 0, 0);

            // Answer is good until 5am the next day, so changeTime is most recent 5am
            // which might be yesterday.
            if (changeTime > currentTime)
            {
                TimeSpan day = new TimeSpan(24, 0, 0);
                changeTime = new DateTime(changeTime.Ticks - day.Ticks);
            }

            return status.Created >= changeTime;
        }

        /// <summary>
        /// DEPRECATED
        /// gets status of wellness check
        /// </summary>
        /// <param name="id">ID of the user to get the status of</param>
        /// <returns> The status of the user, a WellnessStatusViewModel </returns>

        public DEPRECATED_WellnessStatusViewModel DEPRECATED_GetStatus(string id)
        {
            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var idParam = new SqlParameter("@ID_NUM", id);
            var result = RawSqlQuery<DEPRECATED_WellnessStatusViewModel>.query("GET_HEALTH_STATUS_BY_ID @ID_NUM", idParam);

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            var wellnessStatus = result.SingleOrDefault();

            if (wellnessStatus == null)
            {
                // No status was found for the given user, so return an invalid fake status
                return new DEPRECATED_WellnessStatusViewModel
                {
                    Status = "GREEN",
                    Created = new DateTime(1900, 01, 01),
                    IsValid = false,
                    IsOverride = false
                };
            }

            wellnessStatus.IsValid = DEPRECATED_IsValidStatus(wellnessStatus);

            return wellnessStatus;
        }

        /// <summary>
        /// DEPRECATED
        /// Stores wellness Status in database.
        /// </summary>
        /// <param name="id"> ID of the user to post the status for</param>
        /// <param name="status"> Status that is being posted, one of GREEN, YELLOW, or RED </param>
        /// <returns>Status that was successfully recorded</returns>

        public string DEPRECATED_PostStatus(string status, string id)
        {
            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var idParam = new SqlParameter("@ID_NUM", id);
            var statusParam = new SqlParameter("@Status", status);

            var result = RawSqlQuery<DEPRECATED_WellnessStatusViewModel>.query("INSERT_HEALTH_CHECK_UPDATED @ID_NUM, @Status", idParam, statusParam);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            return status;

        }

    }

}
