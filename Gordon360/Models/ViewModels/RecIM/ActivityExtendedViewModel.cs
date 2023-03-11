using Gordon360.Extensions.System;
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
        public string Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? SeriesScheduleID { get; set; }

        public IEnumerable<SeriesExtendedViewModel> Series { get; set; }
        public IEnumerable<TeamExtendedViewModel> Team { get; set; }


        public static implicit operator ActivityExtendedViewModel(Activity? a)
        {
            if (a is null) return null;
            //redundant check in case admins forget to mark activity completed
            //or if we are looking for an end date automatically via Series.EndDate
            bool completed = !a.Completed
                ? DateTime.UtcNow > (a.EndDate ?? DateTime.MaxValue) 
                : a.Completed;
            return new ActivityExtendedViewModel
            {
                ID = a.ID,
                Name = a.Name,
                RegistrationStart = a.RegistrationStart.SpecifyUtc(),
                RegistrationEnd = a.RegistrationEnd.SpecifyUtc(),
                RegistrationOpen = DateTime.UtcNow > a.RegistrationStart.SpecifyUtc()
                        && DateTime.UtcNow < a.RegistrationEnd.SpecifyUtc(),
                MinCapacity = a.MinCapacity,
                MaxCapacity = a.MaxCapacity,
                SoloRegistration = a.SoloRegistration,
                Logo = a.Logo,
                Type = a.Type?.Description,
                Completed = completed,
                StartDate = a.StartDate.SpecifyUtc(),
                EndDate = a.EndDate.SpecifyUtc(),
                SeriesScheduleID = a.SeriesScheduleID,
            };
        }
    }
}