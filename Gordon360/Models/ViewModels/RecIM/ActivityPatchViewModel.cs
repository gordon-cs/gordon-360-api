using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class ActivityPatchViewModel
    {
        public string? Name { get; set; }
        public DateTime? RegistrationStart { get; set; }
        public DateTime? RegistrationEnd { get; set; }
        public int? SportID { get; set; }
        public int? StatusID { get; set; }
        public int? MinCapacity { get; set; }
        public int? MaxCapacity { get; set; }
        public bool? SoloRegistration { get; set; }
        public LogoPatchViewModel? Logo { get; set; }
        public bool? Completed { get; set; }
        public int? TypeID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? SeriesScheduleID { get; set; }
    }
}