﻿using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class ParticipantStatusCreatedViewModel
    {
        public int ID { get; set; }
        public int ParticipantID { get; set; }
        public int StatusID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public static implicit operator ParticipantStatusCreatedViewModel(ParticipantStatusHistory s)
        {
            return new ParticipantStatusCreatedViewModel
            {
                ID = s.ID,
                ParticipantID = s.ParticipantID,
                StatusID = s.StatusID,
                StartDate = s.StartDate,
                EndDate = s.EndDate
            };
        }
    }
}