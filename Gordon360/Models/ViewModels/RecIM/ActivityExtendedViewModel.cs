using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM
{
    public class ActivityExtendedViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime RegistrationStart { get; set; }
        public DateTime RegistrationEnd { get; set; }
        public bool RegistrationOpen { get; set; }
        public SportViewModel Sport { get; set; }
        public string Status { get; set; }
        public int? MinCapacity { get; set; }
        public int? MaxCapacity { get; set; }
        public bool SoloRegistration { get; set; }
        public string Logo { get; set; }
        public bool Completed { get; set; }

        public IEnumerable<SeriesExtendedViewModel> Series { get; set; }
        public IEnumerable<TeamExtendedViewModel> Team { get; set; }

    }
}