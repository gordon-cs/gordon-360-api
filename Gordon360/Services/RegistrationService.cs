using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services
{
    public class RegistrationService
    {
        private readonly CCTContext _context;
        private readonly ILogger<RegistrationService> _logger;

        public RegistrationService(CCTContext context, ILogger<RegistrationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets the registration window for a given user and evaluates their eligibility to register.
        /// </summary>
        /// <param name="username">The AD username of the student.</param>
        /// <returns>
        /// A RegistrationPeriodViewModel containing registration start/end dates and eligibility status.
        /// Returns null if either the account or registration window is missing.
        /// </returns>
        public async Task<RegistrationPeriodViewModel> GetRegistrationWindowAsync(string username)
        {
            var now = DateTime.Now;
            _logger.LogInformation($"Fetching registration window for user: {username}");

            // Step 1: Look up the student's registration period (start/end dates)
            var record = _context.CourseRegistrationDates
                .Where(r => r.Username == username && r.StartDate.HasValue && r.EndDate.HasValue)
                .AsEnumerable()
                .FirstOrDefault();

            // Step 2: Get account info to retrieve student ID
            var account = _context.ACCOUNT.FirstOrDefault(a => a.AD_Username == username);
            if (account == null || record == null)
            {
                // If the user has no account or no registration window, return null
                _logger.LogWarning($"Missing account or registration data for user: {username}");
                return null;
            }
            var studentID = account.account_id;

            // Step 3: Call stored procedure to get any registration-related holds for the student
            var holdResults = await _context.Procedures.GetEnrollmentCheckinHoldsAsync(studentID);
            var hold = holdResults?.FirstOrDefault();

            // Step 4: Determine if the student has any kind of hold (including financial, medical, academic, etc.)
            bool hasHolds = hold != null &&
                (hold.FinancialHold == true ||
                 hold.MedicalHold == true ||
                 hold.MajorHold == true ||
                 hold.HighSchoolHold == true ||
                 hold.RegistrarHold == true ||
                 hold.LaVidaHold == true ||
                 hold.MustRegisterForClasses == true);

            // Step 5: Check if the current time is within the allowed registration window and the student has no holds
            bool isEligible = record.StartDate <= now &&
                              now <= record.EndDate &&
                              !hasHolds;

            // Step 6: Return the registration period view model
            return new RegistrationPeriodViewModel
            {
                Term = record.TermDescription ?? "Unknown",
                StartTime = record.StartDate ?? DateTime.MinValue,
                EndTime = record.EndDate ?? DateTime.MinValue,
                IsEligible = isEligible,
                IsClearedToRegister = !hasHolds,
                HasHolds = hasHolds
            };
        }
    }
}
