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
}
