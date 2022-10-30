using Gordon360.Models.CCT;
using Microsoft.Graph;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class ParticipantViewModel
    {
        public string ADUserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public static implicit operator ParticipantViewModel(ACCOUNT a)
        {
            ParticipantViewModel vm = new ParticipantViewModel
            {
                ADUserName = a.AD_Username.Trim() ?? "",
                Email = a.email ?? "", 
            };

            return vm;
        }

    }
}