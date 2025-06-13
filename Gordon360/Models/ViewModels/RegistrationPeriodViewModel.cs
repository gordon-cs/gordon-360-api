using System;

namespace Gordon360.Models.ViewModels
{
    public class RegistrationPeriodViewModel
    {
        public string Term { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool IsEligible { get; set; }
    }
}
