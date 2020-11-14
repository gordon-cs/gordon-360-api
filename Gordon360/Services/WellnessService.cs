using System;
using System.Collections.Generic;
using System.Linq;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Exceptions.CustomExceptions;
using System.Data.SqlClient;
using Gordon360.Services.ComplexQueries;

namespace Gordon360.Services
{
    public class WellnessService : IWellnessService
    {

        public WellnessService()
        {
        }

        /// <summary>
        /// gets status of wellness check
        /// </summary>
        /// <param name="id">ID of the user to get the status of</param>
        /// <returns> The status of the user, a WellnessStatusViewModel </returns>

        public WellnessStatusViewModel GetStatus(string id)
        {
            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var idParam = new SqlParameter("@ID_NUM", id);
            var result = RawSqlQuery<WellnessStatusViewModel>.query("GET_HEALTH_STATUS_BY_ID @ID_NUM", idParam);

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            var wellnessStatus = result.SingleOrDefault();

            wellnessStatus.IsValid = IsValidStatus(wellnessStatus);

            return wellnessStatus;
        }

        /// <summary>
        /// Stores wellness Status in database.
        /// </summary>
        /// <param name="id"> ID of the user to post the status for</param>
        /// <param name="status"> Status that is being posted, one of GREEN, YELLOW, or RED </param>
        /// <returns>Status that was successfully recorded</returns>

        public string PostStatus(string status, string id)
        {
            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var idParam = new SqlParameter("@ID_NUM", id);
            var statusParam = new SqlParameter("@Status", status);

            var result = RawSqlQuery<WellnessStatusViewModel>.query("INSERT_HEALTH_CHECK_UPDATED @ID_NUM, @Status", idParam, statusParam);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            return status;

        }

        /// <summary>
        /// gets the question for the wellness check from the back end
        /// </summary>
        /// <returns>list of strings with questions and prompts</returns>

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
        /// gets the question for the wellness check from the back end
        /// </summary>
        /// <returns>list of strings with questions and prompts</returns>

        public IEnumerable<WellnessQuestionViewModel> DEPRECATED_GetQuestion()
        {

            var result = RawSqlQuery<WellnessQuestionViewModel>.query("GET_HEALTH_CHECK_QUESTION"); //run stored procedure


            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            return result;

        }

        /// <summary>
        /// Checks whether a wellness status is still valid (i.e. new since 5AM)
        /// </summary>
        /// <param name='status'>Wellness status to check, an instance of a WellnessViewModel</param>
        /// <returns>boolean representing whether the status is still valid</returns>
        private bool IsValidStatus(WellnessStatusViewModel status)
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

        public IEnumerable<DEPRECATED_WellnessViewModel> DEPRECATED_GetStatus(string id)
        {
            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var idParam = new SqlParameter("@ID_NUM", id);
            var result = RawSqlQuery<DEPRECATED_WellnessViewModel>.query("GET_HEALTH_CHECK_BY_ID @ID_NUM", idParam); //run stored procedure

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }


            var wellnessModel = result.Select(x =>
            {
                DEPRECATED_WellnessViewModel y = new DEPRECATED_WellnessViewModel();

                DateTime currentTime = DateTime.Now;

                DateTime changeTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 5, 0, 0);

                // Answer is good until 5am the next day, so changeTime is most recent 5am
                // which might be yesterday.
                if (changeTime > currentTime)
                {
                    TimeSpan day = new TimeSpan(24, 0, 0);
                    changeTime = new DateTime(changeTime.Ticks - day.Ticks);
                }

                if (x.timestamp >= changeTime)
                {
                    y.answerValid = true;
                    y.userAnswer = x.userAnswer;
                    y.timestamp = x.timestamp;
                    return y;
                }

                y.answerValid = false;
                y.userAnswer = x.userAnswer;
                y.timestamp = x.timestamp;

                return y;
            });


            return wellnessModel;

        }

        /// <summary>
        ///  Gets answer to the wellness check answer and sends it to the back end.
        ///     If answer boolean is true: student is feeling symptomatic
        ///     If answer boolean is false: student is not feeling symptomatic
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="answer">answer</param>
        /// <returns>Ok if message was recorded</returns>
        public DEPRECATED_WellnessViewModel DEPRECATED_PostStatus(bool answer, string id)
        {
            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var idParam = new SqlParameter("@ID_NUM", id);
            var answerParam = new SqlParameter("@Answer", answer);

            var result = RawSqlQuery<DEPRECATED_WellnessViewModel>.query("INSERT_HEALTH_CHECK @ID_NUM, @Answer", idParam, answerParam); //run stored procedure
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            var UserAnswer = answer;

            DEPRECATED_WellnessViewModel y = new DEPRECATED_WellnessViewModel()
            {
                userAnswer = UserAnswer
            };



            return y;

        }

    }

}
