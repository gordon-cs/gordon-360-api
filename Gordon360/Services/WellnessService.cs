using Gordon360.Database.CCT;
using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using System;
using System.Linq;
using static Gordon360.Controllers.WellnessController;

namespace Gordon360.Services
{
    public class WellnessService : IWellnessService
    {

        private readonly CCTContext _context;

        public WellnessService(CCTContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get the status of the user by id
        /// </summary>
        /// <param name="id">ID of the user to get the status of</param>
        /// <returns> The status of the user, a WellnessViewModel </returns>

        public WellnessViewModel GetStatus(string id)
        {
            var account = _context.ACCOUNT.FirstOrDefault(x => x.gordon_id == id);
            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            int IDNum = int.Parse(id);
            var result = _context.Health_Status.Where(x => x.ID_Num == IDNum).OrderByDescending(x => x.Created).FirstOrDefault();

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
                    IsValid = result.Expires == null || DateTime.Now < result.Expires,
                    StatusDescription = result.Notes?.StartsWith("STATUS: ") ?? false ? result.Notes.Substring(8, result.Notes.IndexOf(";") - 8) : null
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
            var account = _context.ACCOUNT.FirstOrDefault(x => x.gordon_id == id);
            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var now = DateTime.Now;

            var statusObject = new Health_Status
            {
                ID_Num = int.Parse(id),
                HealthStatusID = (byte)status,
                Created = now,
                Expires = ExpirationDate(now),
                CreatedBy = "360.WellnessCheck",
                Notes = status == WellnessStatusColor.YELLOW ? $"STATUS: Symptomatic;" : null,
                Emailed = null
            };

            var result = _context.Health_Status.Add(statusObject);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }
            _context.SaveChanges();

            return statusObject;
        }

        /// <summary>
        /// gets the question for the wellness check from the back end
        /// </summary>
        /// <returns>A WellnessQuestionViewModel including the text of the question and the disclaimers for positive and negative answers.</returns>

        public WellnessQuestionViewModel GetQuestion()
        {

            var result = _context.Health_Question.Select(q => new WellnessQuestionViewModel
            {
                question = q.Question,
                noPrompt = q.NoPrompt,
                yesPrompt = q.YesPrompt
            }).FirstOrDefault();

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            return result;
        }


        /// <summary>
        /// Creates an expiration date for a check in at the current time
        /// </summary>
        /// <param name="currentTime">The time of the check in</param>
        /// <returns>When the check in should expire (the next 5AM).</returns>
        private DateTime ExpirationDate(DateTime currentTime)
        {
            // Answer expires at 5AM
            DateTime expirationTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 4, 45, 0);

            // If it's past 5AM, expirationTime is 5AM tomorrow.
            if (currentTime > expirationTime)
            {
                TimeSpan day = new TimeSpan(24, 0, 0);
                expirationTime = new DateTime(expirationTime.Ticks + day.Ticks);
            }

            return expirationTime;
        }

    }

}
