using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels;

public sealed record EnrollmentCheckinHolds(
    bool FinancialHold,
    string FinancialHoldText,
    bool HighSchoolHold,
    bool MedicalHold,
    bool MajorHold,
    bool RegistrarHold,
    bool LaVidaHold,
    bool MustRegisterForClasses,
    int NewStudent,
    string MeetingDate,
    string MeetingLocations)
{
    public EnrollmentCheckinHolds(FINALIZATION_GETHOLDSBYIDResult holds)
        : this(
             FinancialHold: holds.FinancialHold ?? false,
             FinancialHoldText: holds.FinancialHoldText,
             HighSchoolHold: holds.HighSchoolHold ?? false,
             MedicalHold: holds.MedicalHold ?? false,
             MajorHold: holds.MajorHold ?? false,
             RegistrarHold: holds.RegistrarHold ?? false,
             LaVidaHold: holds.LaVidaHold ?? false,
             MustRegisterForClasses: holds.MustRegisterForClasses ?? false,
             NewStudent: holds.NewStudent,
             MeetingDate: holds.MeetingDate,
             MeetingLocations: holds.MeetingLocations
              ) { }
}
