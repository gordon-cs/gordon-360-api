using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class ParticipantStatusViewModel
    {
        public int ID { get; set; }
        public string ParticipantUsername { get; set; }
        public int StatusID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public static implicit operator ParticipantStatusViewModel(ParticipantStatusHistory s)
        {
            return new ParticipantStatusViewModel
            {
                ID = s.ID,
                ParticipantUsername = s.ParticipantUsername,
                StatusID = s.StatusID,
                StartDate = s.StartDate,
                EndDate = s.EndDate
            };
        }
    }
}