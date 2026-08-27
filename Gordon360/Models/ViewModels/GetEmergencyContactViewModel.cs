using Gordon360.Models.CCT;
using System;

namespace Gordon360.Models.ViewModels;

public class GetEmergencyContactViewModel
{

    public static GetEmergencyContactViewModel From(EmergencyContact emrg) => new()
    {
        FirstName = emrg.firstname ?? "",
        LastName = emrg.lastname ?? "",
        HomePhone = emrg.HomePhone ?? "",
        WorkPhone = emrg.WorkPhone ?? "",
        MobilePhone = emrg.MobilePhone ?? "",
        Relationship = emrg.relationship ?? "",
    };

    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string HomePhone { get; set; }
    public required string WorkPhone { get; set; }
    public required string MobilePhone { get; set; }
    public required string Relationship { get; set; }
}
