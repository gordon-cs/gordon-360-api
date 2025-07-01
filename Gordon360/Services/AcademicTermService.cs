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

    // Return the days left in the semester, and the total days in the current term
    public async Task<double[]> GetDaysLeftAsync()
    {
        var currentTerm =  await GetCurrentTermAsync();

        if (currentTerm == null || currentTerm.EndDate == null || currentTerm.BeginDate == null)
        {
            // If no current term or dates are missing, return 0's
            return new double[] { 0, 0 };
        }

        DateTime termEnd = currentTerm.EndDate.Value;
        DateTime termBegin = currentTerm.BeginDate.Value;
        DateTime startTime = DateTime.Today;

        double daysLeft = (termEnd - startTime).TotalDays;
        // Account for possible negative value in between sessions
        daysLeft = daysLeft < 0 ? 0 : daysLeft;

        double daysInTerm = (termEnd - termBegin).TotalDays;

        return new double[2] {
        // Days left in semester
        daysLeft,
        // Total days in the semester
        daysInTerm
        };
    }
    public async Task<IEnumerable<YearTermTableViewModel>> GetUndergradTermsAsync()
    {
        var terms = await context.YearTermTable
            .Where(t => t.TRM_CDE == "FA" || t.TRM_CDE == "SP" || t.TRM_CDE == "SU")
            .OrderByDescending(t => t.TRM_BEGIN_DTE)
            .ToListAsync();

        return terms.Select(t => new YearTermTableViewModel(t));
    }

}
