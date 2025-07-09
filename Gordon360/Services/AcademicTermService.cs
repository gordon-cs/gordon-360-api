using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services;

public class AcademicTermService(CCTContext context) : IAcademicTermService
{

    public async Task<YearTermTableViewModel?> GetCurrentTermAsync()
    {
        var terms = await context.YearTermTable
            .FromSqlRaw("EXEC dbo.GetCurrentTerm")
            .AsNoTracking()
            .ToListAsync();

        var currentTerm = terms.FirstOrDefault();

        return currentTerm != null ? new YearTermTableViewModel(currentTerm) : null;
    }


    public async Task<IEnumerable<YearTermTableViewModel>> GetAllTermsAsync()
    {
        var terms = await context.YearTermTable
            .OrderByDescending(t => t.TRM_BEGIN_DTE)
            .ToListAsync();

        return terms.Select(t => new YearTermTableViewModel(t));
    }

    public async Task<DaysLeftViewModel> GetDaysLeftAsync()
    {
        var today = DateTime.Today;

        var allTerms = await GetAllTermsAsync();

        var relevantTerms = allTerms
            .Where(t => t.TermCode is "FA" or "SP" or "SU")
            .OrderBy(t => t.BeginDate)
            .ToList();

        for (int i = 0; i < relevantTerms.Count; i++)
        {
            var term = relevantTerms[i];

            if (term.BeginDate.HasValue && term.EndDate.HasValue)
            {
                var start = term.BeginDate.Value.Date;
                var end = term.EndDate.Value.Date;

                if (today >= start && today <= end)
                {
                    int totalDays = (end - start).Days + 1;
                    int daysLeft = (end - today).Days + 1;
                    string label = $"{term.Description}";

                    return new DaysLeftViewModel
                    {
                        DaysLeft = daysLeft,
                        TotalDays = totalDays,
                        TermLabel = label
                    };
                }
                if (i < relevantTerms.Count - 1)
                {
                    var nextTerm = relevantTerms[i + 1];
                    if (term.EndDate.HasValue && nextTerm.BeginDate.HasValue)
                    {
                        var gapStart = term.EndDate.Value.Date.AddDays(1);
                        var gapEnd = nextTerm.BeginDate.Value.Date.AddDays(-1);

                        if (today >= gapStart && today <= gapEnd)
                        {
                            int totalDays = (gapEnd - gapStart).Days + 1;
                            int daysLeft = (gapEnd - today).Days + 1;

                            return new DaysLeftViewModel
                            {
                                DaysLeft = daysLeft,
                                TotalDays = totalDays,
                                TermLabel = $"Break before {nextTerm.Description}"
                            };
                        }
                    }
                }
            }
        }
        return new DaysLeftViewModel
        {
            DaysLeft = 0,
            TotalDays = 0,
            TermLabel = "Break"
        };
    }
}
