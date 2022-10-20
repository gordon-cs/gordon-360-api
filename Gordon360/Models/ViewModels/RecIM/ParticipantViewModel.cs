using Gordon360.Models.CCT;
using Microsoft.Graph;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class ParticipantViewModel
    {
        public int ID { get; set; }
        public string ADUserName { get; set; }
        public string Email { get; set; }

        public static implicit operator ParticipantViewModel(ACCOUNT a)
        {
            ParticipantViewModel vm = new ParticipantViewModel
            {
                ID = Convert.ToInt32(a.gordon_id.Trim()),
                ADUserName = a.AD_Username.Trim() ?? "",
                Email = a.email ?? "", 
            };

            return vm;
        }

    }
}