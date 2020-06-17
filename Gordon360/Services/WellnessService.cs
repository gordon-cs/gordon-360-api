using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Exceptions.CustomExceptions;
using System.Data.SqlClient;
using Gordon360.Services.ComplexQueries;
using System.Diagnostics;

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
        /// <param name="id">id</param>
        /// <returns>boolean if found, null if not found</returns>

        public IEnumerable<WellnessViewModel> GetStatus(string id)
        {
            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var idParam = new SqlParameter("@ID_NUM", id);
            var result = RawSqlQuery<WellnessViewModel>.query("GET_HEALTH_CHECK_BY_ID @ID_NUM", idParam); //run stored procedure
         
            if (result == null)
            {
                 throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }


            var wellnessModel = result.Select(x =>
            {
                WellnessViewModel y = new WellnessViewModel();

                DateTime currentTime = DateTime.Now;
                
                DateTime changeTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 5, 0, 0);

                // Answer is good until 5am the next day, so changeTime is most recent 5am
                // which might be yesterday.
                if(changeTime > currentTime)
                {
                    TimeSpan day = new TimeSpan(24, 0, 0);
                    changeTime = new DateTime(changeTime.Ticks - day.Ticks);
                }

                if (x.timestamp >= changeTime)
                {
                    y.currentStatus = true;
                    y.userAnswer = x.userAnswer;
                    y.timestamp = x.timestamp;
                    return y;
                }

                y.currentStatus = false;
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

        public WellnessViewModel PostStatus(bool answer, string id)
        {
            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                 throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var idParam = new SqlParameter("@ID_NUM", id);
            var answerParam = new SqlParameter("@Answer", answer);

            var result = RawSqlQuery<WellnessViewModel>.query("INSERT_HEALTH_CHECK @ID_NUM, @Answer", idParam, answerParam); //run stored procedure
            if (result == null)
            {
                 throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            var UserAnswer = answer;

            WellnessViewModel y = new WellnessViewModel()
            {
                userAnswer = UserAnswer
            };



            return y;

        }



    }
}