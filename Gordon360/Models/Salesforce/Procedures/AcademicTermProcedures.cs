using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gordon360.Models.ViewModels;
using Microsoft.IdentityModel.Tokens;

namespace Gordon360.Models.Salesforce;

public class AcademicTermProcedures(SalesforceContext context)
{
    private readonly SalesforceContext context = context;

    private const string SoqlTemplate = @"
        SELECT
            gc_Jenz_Term_Code__c,
            gc_Jenz_Year_Code__c,
            StartDate,
            EndDate,
            Name
        FROM AcademicTerm
        {0}
        {1}
        {2}";

    /// <summary>
    /// </summary>
    /// <returns>Returns list of all available academic terms, ordered descending by start date</returns>
    public async Task<List<YearTermTableViewModel>> GetAllTerms()
    {
        var response = await Query(order: "StartDate DESC NULLS LAST");

        return [.. response.records.Select(t => new YearTermTableViewModel(t))];
    }

    /// <summary>
    /// </summary>
    /// <returns>Returns the current academic term, if it exists</returns>
    public async Task<YearTermTableViewModel?> GetCurrentTerm()
    {
        var response = await Query(where: "StartDate <= TODAY AND EndDate >= TODAY", limit_n: 1);
        AcademicTerm? term = response?.records?.FirstOrDefault();

        return term != null ? new YearTermTableViewModel(term) : null;

    }
    /// <summary>
    /// Constructs a SOQL query
    /// </summary>
    /// <param name="where">SOQL field selectors</param>
    /// <param name="order">SOQL ordering</param>
    /// <param name="limit_n">SOQL limit on number of records returned</param>
    /// <returns></returns>
    private async Task<SFQueryResult<AcademicTerm>> Query(string where = "", string order = "", int limit_n = 0)
    {
        where = where.IsNullOrEmpty() ? "" : "WHERE " + where;
        order = order.IsNullOrEmpty() ? "" : "ORDER BY " + order;
        string limit = limit_n == 0 ? "" : "LIMIT " + limit_n;

        string query = string.Format(SoqlTemplate, where, order, limit);
        var response = await context.Query<AcademicTerm>(query);
        return response;
    }
}