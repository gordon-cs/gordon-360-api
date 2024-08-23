using Gordon360.Models.CCT;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Gordon360.Models.ViewModels;

public record ScheduleTermViewModel
{
    public required string Year;
    public required string TermCode;
    public required string Description;
    public DateOnly? Start;
    public DateOnly? End;
    public required IEnumerable<Subterm> Subterms;

    [SetsRequiredMembers]
    public ScheduleTermViewModel(IGrouping<string, ScheduleTerm> terms)
    {
        ScheduleTerm term = terms.Single(t => t.SubtermCode == null);

        Year = term.YearCode;
        TermCode = term.TermCode;
        Description = term.Description;
        Start = term.BeginDate;
        End = term.EndDate;
        Subterms = terms.Where(t => t.SubtermCode != null).Select(s => new Subterm(s.SubtermCode, s.Description, s.BeginDate, s.EndDate));
    }
}

public record Subterm(string SubtermCode,  string Description, DateOnly? Start, DateOnly? End);
