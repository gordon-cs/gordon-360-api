using Gordon360.Models.CCT;
using System;
using static Gordon360.Controllers.WellnessController;

namespace Gordon360.Models.ViewModels;

public class WellnessViewModel
{
    public string Status { get; set; }

    public DateTime Created { get; set; }

    public bool IsValid { get; set; }

    public string StatusDescription {  get; set; }

    public static implicit operator WellnessViewModel(Health_Status hs)
    {
        return new WellnessViewModel
        {
            Status = ((WellnessStatusColor)hs.HealthStatusID).ToString(),
            Created = hs.Created,
            IsValid = hs.Expires == null || DateTime.Now < hs.Expires,
            StatusDescription = hs.Notes?.StartsWith("STATUS: ") ?? false ? hs.Notes.Substring(8, hs.Notes.IndexOf(";") - 8) : ""
        };
    }
}