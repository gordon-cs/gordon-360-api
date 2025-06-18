using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services;

public class AcademicTermService : IAcademicTermService
{
    private readonly CCTContext _context;

    public AcademicTermService(CCTContext context)
    {
        _context = context;
    }

    public async Task<YearTermTable?> GetCurrentTermAsync()
    {
        var today = DateTime.Today;

        return await _context.YearTermTable
            .Where(t => t.TRM_BEGIN_DTE <= today &&
                        (t.TRM_CDE == "FA" || t.TRM_CDE == "SP" || t.TRM_CDE == "SU"))
            .OrderByDescending(t => t.TRM_BEGIN_DTE)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<YearTermTableViewModel>> GetAllTermsAsync()
    {
        var terms = await _context.YearTermTable
            .OrderByDescending(t => t.TRM_BEGIN_DTE)
            .ToListAsync();

        return terms.Select(t => new YearTermTableViewModel(t));
    }
}
