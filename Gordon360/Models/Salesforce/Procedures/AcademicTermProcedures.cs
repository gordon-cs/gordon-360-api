using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gordon360.Models.ViewModels;

namespace Gordon360.Models.Salesforce;

public class AcademicTermProcedures(ISalesforceContext context)
{
    private const string SoqlTemplate = @"
        SELECT
            gc_Jenz_Term_Code__c,
            gc_Jenz_Year_Code__c,
            StartDate,
            EndDate,
            Name
        FROM AcademicTerm";

    /// <summary>
    /// </summary>
    /// <returns>Returns list of all available academic terms, ordered descending by start date</returns>
    public async Task<List<YearTermTableViewModel>> GetAllTerms()
    {
        var response = await context.SoqlQuery<AcademicTerm>(SoqlTemplate, order: "StartDate DESC NULLS LAST");

        return [.. response.records.Select(t => new YearTermTableViewModel(t))];
    }

    /// <summary>
    /// </summary>
    /// <returns>Returns the current academic term, if it exists</returns>
    public async Task<YearTermTableViewModel?> GetCurrentTerm()
    {
        var response = await context.SoqlQuery<AcademicTerm>(SoqlTemplate, where: "StartDate <= TODAY AND EndDate >= TODAY", limit_n: 1);
        AcademicTerm? term = response?.records?.FirstOrDefault();

        return term != null ? new YearTermTableViewModel(term) : null;

    }
}