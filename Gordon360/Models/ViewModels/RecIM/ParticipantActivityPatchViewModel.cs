using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class ParticipantActivityPatchViewModel
    {
        public int? ActivityID { get; set; }
        public string? ActivityPrivType { get; set; }
        public bool? isFreeAgent { get; set; }
    }
}