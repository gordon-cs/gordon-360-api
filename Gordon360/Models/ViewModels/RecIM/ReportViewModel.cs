using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class RecIMGeneralReportViewModel
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public IEnumerable<ActivityReportViewModel> Activities { get; set; }
        public int NumberOfNewParticipants { get; set; }
        public IEnumerable<ParticipantReportViewModel> NewParticipants { get; set; }
        public int NumberOfActiveParticipants { get; set; }
        public IEnumerable<ParticipantReducedReportViewModel> ActiveParticipants { get; set; }

    }

    public class ActivityReportViewModel
    {
        public int NumberOfParticipants { get; set; }
        public ActivityViewModel Activity { get; set; }
    }

    public class ParticipantReportViewModel
    {
        public AccountViewModel UserAccount { get; set; }
        public int NumberOfActivitiesParticipated { get; set; }
    }

    public class ParticipantReducedReportViewModel
    {
        public string Username { get; set; }
        public string SpecifiedGender { get; set; }
        public static implicit operator ParticipantReducedReportViewModel(Participant p)
        {
            return new ParticipantReducedReportViewModel
            {
                Username = p.Username,
                SpecifiedGender = p.SpecifiedGender ?? "U"
            };
        }
    }
}
