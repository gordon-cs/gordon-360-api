using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels.RecIM;

public class ActivityViewModel
{
    public int ID { get; set; }
    public string Name { get; set; }
    public DateTime RegistrationStart { get; set; }
    public DateTime RegistrationEnd { get; set; }
    public int SportID { get; set; }
    public int StatusID { get; set; }
    public int MinCapacity { get; set; }
    public int? MaxCapacity { get; set; }
    public bool SoloRegistration { get; set; }
    public string? Logo { get; set; }
    public bool Completed { get; set; }
    public int TypeID { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? SeriesScheduleID { get; set; }

    public static implicit operator ActivityViewModel(Activity a)
    {
        return new ActivityViewModel
        {
            ID = a.ID,
            Name = a.Name,
            RegistrationStart = a.RegistrationStart.SpecifyUtc(),
            RegistrationEnd = a.RegistrationEnd.SpecifyUtc(),
            SportID = a.SportID,
            StatusID = a.StatusID,
            MinCapacity = a.MinCapacity,
            MaxCapacity = a.MaxCapacity,
            SoloRegistration = a.SoloRegistration,
            Logo = a.Logo,
            Completed = a.Completed,
            TypeID = a.TypeID,
            StartDate = a.StartDate.SpecifyUtc(),
            EndDate = a.EndDate.SpecifyUtc(),
            SeriesScheduleID = a.SeriesScheduleID,
        };
    }
}