using Gordon360.Models.CCT;
using Microsoft.Graph;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class ParticipantNotificationExtendedViewModel
    {
        public int ID { get; set; }
        public string Message { get; set; }
        public DateTime DispatchDate { get; set; }
        public static implicit operator ParticipantNotificationExtendedViewModel(ParticipantNotification p)
        {
            return new ParticipantNotificationExtendedViewModel
            {
                ID = p.ID,
                Message = p.Message,
                DispatchDate = p.DispatchDate
            };
        }
    }
}