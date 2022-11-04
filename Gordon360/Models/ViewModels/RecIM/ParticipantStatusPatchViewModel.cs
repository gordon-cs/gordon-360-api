using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class ParticipantStatusPatchViewModel
    {
        public string Username { get; set; }
        public string StatusDescription { get; set; }
        public DateTime? EndDate { get; set; }

    }
}