using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class ActivityUploadViewModel
    {
        public string Name { get; set; }
        public DateTime RegistrationStart { get; set; }
        public DateTime RegistrationEnd { get; set; }
        public int SportID { get; set; }
        public int? MinCapacity { get; set; }
        public int MaxCapacity { get; set; }
        public bool SoloRegistration { get; set; }
        public string? Logo { get; set; }
        public int TypeID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}