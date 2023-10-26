using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;

namespace Gordon360.Models.ViewModels;

public record ScheduleViewModel
{
    public ScheduleViewModel(ScheduleTerms session, IEnumerable<ScheduleCourseViewModel> courses)
    {
        Session = new(
            Year: session.YearCode,
            TermCode: session.TermCode,
            Description: session.Description,
            Start: session.TermBeginDate is DateTime begin ? DateOnly.FromDateTime(begin) : null,
            End: session.TermEndDate is DateTime end ? DateOnly.FromDateTime(end) : null,
            Subterm: session.SubTermCode is not null ? new Subterm(Code: session.SubTermCode,
                                                                   SortOrder: session.SubTermSortOrder,
                                                                   Description: session.SubTermDescription) : null);

        Courses = courses;
    }

    public ScheduleSession Session { get; init; }
    public IEnumerable<ScheduleCourseViewModel> Courses { get; init; }
}

public record ScheduleSession(string Year, string TermCode, string Description, DateOnly? Start, DateOnly? End, Subterm? Subterm);

public record Subterm(string Code, int? SortOrder, string Description);
