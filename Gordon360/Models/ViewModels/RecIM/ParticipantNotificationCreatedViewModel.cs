using Gordon360.Models.CCT;
using Microsoft.Graph;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class ParticipantNotificationCreatedViewModel
    {
        public int ID { get; set; }
        public int ParticipantID { get; set; }
        public string Message { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DispatchDate { get; set; }
        public static implicit operator ParticipantNotificationCreatedViewModel(ParticipantNotification p)
        {
            return new ParticipantNotificationCreatedViewModel
            {
                ID = p.ID,
                ParticipantID = p.ParticipantID,
                Message = p.Message,
                EndDate = p.EndDate,
                DispatchDate = p.DispatchDate
            };
        }
    }
}