using Gordon360.Models.CCT;
using Microsoft.Graph;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class ParticipantExtendedViewModel
    {
        public string Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public int? GamesAttended { get; set; }
        public string Status { get; set; }
        public IEnumerable<ParticipantNotificationViewModel> Notification { get; set; } 
        public bool IsAdmin { get; set; }
        public bool AllowEmails { get; set; }
        public string SpecifiedGender { get; set; }

        public static implicit operator ParticipantExtendedViewModel(ACCOUNT a)
        {
            return new ParticipantExtendedViewModel
            {
                Username = a.AD_Username.Trim() ?? "",
                Email = a.email ?? "", 
            };

        }

    }
}