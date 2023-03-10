using Gordon360.Models.CCT;
using Gordon360.Static.Methods;
using Microsoft.Graph;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class ParticipantNotificationViewModel
    {
        public int ID { get; set; }
        public string ParticipantUsername { get; set; }
        public string Message { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DispatchDate { get; set; }
        public static implicit operator ParticipantNotificationViewModel(ParticipantNotification p)
        {
            return new ParticipantNotificationViewModel
            {
                ID = p.ID,
                ParticipantUsername = p.ParticipantUsername,
                Message = p.Message,
                EndDate = Helpers.FormatDateTimeToUtc(p.EndDate),
                DispatchDate = Helpers.FormatDateTimeToUtc(p.DispatchDate)
            };
        }
    }
}