using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels.RecIM;

public class ParticipantActivityPatchViewModel
{
    public int? ActivityID { get; set; }
    public int? ActivityPrivID { get; set; }
    public bool? IsFreeAgent { get; set; }
    public int? TeamID { get; set; }
    public string? TeamRole { get; set; }

}