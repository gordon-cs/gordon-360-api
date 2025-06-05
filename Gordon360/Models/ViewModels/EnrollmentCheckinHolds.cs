using Gordon360.Models.CCT;

namespace Gordon360.Models.ViewModels;

public sealed record EnrollmentCheckinHolds(
    bool FinancialHold,
    bool HighSchoolHold,
    bool MedicalHold,
    bool MajorHold,
    bool RegistrarHold,
    bool LaVidaHold,
    bool MustRegisterForClasses)
{
    public EnrollmentCheckinHolds(GetEnrollmentCheckinHoldsResult holds)
        : this(
             FinancialHold: holds.FinancialHold ?? false,
             HighSchoolHold: holds.HighSchoolHold ?? false,
             MedicalHold: holds.MedicalHold ?? false,
             MajorHold: holds.MajorHold ?? false,
             RegistrarHold: holds.RegistrarHold ?? false,
             LaVidaHold: holds.LaVidaHold ?? false,
             MustRegisterForClasses: holds.MustRegisterForClasses ?? false
              ) { }
}
