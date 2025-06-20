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

    public async Task<YearTermTable?> GetCurrentTermAsync()
    {
        var terms = await context.YearTermTable
            .FromSqlRaw("EXEC dbo.GetCurrentTerm")
            .AsNoTracking()
            .ToListAsync();

        return terms.FirstOrDefault();
    }

    public async Task<IEnumerable<YearTermTableViewModel>> GetAllTermsAsync()
    {
        var terms = await context.YearTermTable
            .OrderByDescending(t => t.TRM_BEGIN_DTE)
            .ToListAsync();

        return terms.Select(t => new YearTermTableViewModel(t));
    }
}
