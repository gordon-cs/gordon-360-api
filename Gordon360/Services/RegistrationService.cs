using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

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
        /// Gets the registration period for the specified user and determines eligibility.
        /// </summary>
        public RegistrationPeriodViewModel GetRegistrationWindow(string username)
        {
            var now = DateTime.Now;
            _logger.LogInformation($"Attempting to fetch registration window for user: {username}");

            // Pull data into memory and filter out rows with nulls
            var record = _context.CourseRegistrationDates
                .Where(r => r.Username == username &&
                            r.StartDate.HasValue &&
                            r.EndDate.HasValue)
                .AsEnumerable() // switch to in-memory to prevent SQL null access errors
                .FirstOrDefault();

            if (record == null)
            {
                _logger.LogWarning($"No valid registration period found for user: {username}");
                return new RegistrationPeriodViewModel
                {
                    Term = "Not Available",
                    StartTime = DateTime.MinValue,
                    EndTime = DateTime.MinValue,
                    IsEligible = false
                };
            }

            return new RegistrationPeriodViewModel
            {
                Term = record.TermDescription ?? "Unknown",
                StartTime = record.StartDate.Value,
                EndTime = record.EndDate.Value,
                IsEligible = record.StartDate <= now && now <= record.EndDate
            };
        }
    }
}
