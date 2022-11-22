using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class ParticipantActivityCreatedViewModel
    {
        public int ID { get; set; }
        public int ActivityID { get; set; }
        public int ParticipantID { get; set; }
        public int PrivTypeID { get; set; }
        public bool isFreeAgent { get; set; }

        public static implicit operator ParticipantActivityCreatedViewModel(ParticipantActivity p)
        {
            return new ParticipantActivityCreatedViewModel
            {
                ID = p.ID,
                ActivityID = p.ActivityID,
                ParticipantID = p.ParticipantID,
                PrivTypeID = p.PrivTypeID,
                isFreeAgent = p.isFreeAgent
            };
        }
    }
}