using Gordon360.Models.CCT;
using Microsoft.Graph;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class ParticipantExtendedViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string? Role { get; set; }
        public string Status { get; set; }
        public IEnumerable<ParticipantNotificationExtendedViewModel> Notification { get; set; } 
            = new List<ParticipantNotificationExtendedViewModel>();

        public bool IsAdmin { get; set; }

        public static implicit operator ParticipantExtendedViewModel(ACCOUNT a)
        {
            ParticipantExtendedViewModel vm = new ParticipantExtendedViewModel
            {
                Username = a.AD_Username.Trim() ?? "",
                Email = a.email ?? "", 
            };

            return vm;
        }

    }
}