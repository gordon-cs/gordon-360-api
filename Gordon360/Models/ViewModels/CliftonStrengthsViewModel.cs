using Gordon360.Models.Gordon360;
using System;

namespace Gordon360.Models.ViewModels;

public record CliftonStrengthsViewModel(string AccessCode, string Email, DateTime? DateCompleted, string[] Themes, bool Private)
{
    public static implicit operator CliftonStrengthsViewModel?(Clifton_Strengths? cs) => cs is null ? null : new(
        AccessCode: cs.ACCESS_CODE,
        Email: cs.EMAIL,
        DateCompleted: cs.DTE_COMPLETED,
        Themes: new string[5] { cs.THEME_1, cs.THEME_2, cs.THEME_3, cs.THEME_4, cs.THEME_5 },
        Private: cs.Private);
};
