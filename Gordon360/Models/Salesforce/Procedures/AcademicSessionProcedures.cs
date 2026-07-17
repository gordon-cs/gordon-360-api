using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gordon360.Models.ViewModels;
using Microsoft.IdentityModel.Tokens;

namespace Gordon360.Models.Salesforce;

public class AcademicSessionProcedures(ISalesforceContext context)
{
    private const string SoqlTemplate = @"
        SELECT
            Name,
            gc_Jenz_Session_Code__c,
            gc_Jenz_Subterm_Code__c,
            gc_Jenz_Term_Code__c,
            gc_Jenz_Year_Code__c,
            ClassStartDate,
            ClassEndDate,
            ExamStartDate,
            ExamEndDate
        FROM AcademicSession
        {0}
        {1}
        {2}";

    /// <summary>
    /// </summary>
    /// <returns>Returns list of all available academic sessions, ordered descending by class start date</returns>
    public async Task<IEnumerable<SessionViewModel>> GetAllSessions()
    {
        var response = await Query(order: "ClassStartDate DESC NULLS LAST");

        return [.. response.records.Select(t => (SessionViewModel)t)];
    }

    /// <summary>
    /// </summary>
    /// <returns>Returns session matching a description, or none if no matching sessions are found</returns>
    public async Task<SessionViewModel?> GetSession(string desc)
    {
        var response = await Query(where: $"gc_Jenz_Session_Code__c = '{desc}'", limit_n: 1);

        var result = response.records.FirstOrDefault();

        return (result is null) ? null : (SessionViewModel)result;
    }

    /// <summary>
    /// </summary>
    /// <returns>Returns the current academic session, if it exists</returns>
    public async Task<SessionViewModel?> GetCurrentSession()
    {
        var response = await Query(where: "ClassStartDate <= TODAY AND ExamEndDate >= TODAY AND Type = 'Quarter' AND gc_Jenz_Subterm_Code__c LIKE '_Q'", limit_n: 1);
        AcademicSession? result = response?.records?.FirstOrDefault();

        return (result is null) ? null : (SessionViewModel)result;
    }

    /// <summary>
    /// </summary>
    /// <returns>Returns the most recent spring or fall session</returns>
    public async Task<SessionViewModel?> GetCurrentSessionForFinalExams()
    {
        var response = await Query(where: "ClassStartDate <= TODAY AND Type = 'Semester' AND gc_Jenz_Term_Code__c IN ('SP', 'FA')", order: "ClassStartDate DESC NULLS LAST", limit_n: 1);
        AcademicSession? result = response?.records?.FirstOrDefault();

        return (result is null) ? null : (SessionViewModel)result;
    }

    /// <summary>
    /// Constructs a SOQL query
    /// </summary>
    /// <param name="where">SOQL field selectors</param>
    /// <param name="order">SOQL ordering</param>
    /// <param name="limit_n">SOQL limit on number of records returned</param>
    /// <returns></returns>
    public async Task<SFQueryResult<AcademicSession>> Query(string where = "", string order = "", int limit_n = 0)
    {
        where = where.IsNullOrEmpty() ? "" : "WHERE " + where;
        order = order.IsNullOrEmpty() ? "" : "ORDER BY " + order;
        string limit = limit_n == 0 ? "" : "LIMIT " + limit_n;

        string query = string.Format(SoqlTemplate, where, order, limit);
        var response = await context.Query<AcademicSession>(query);
        return response;
    }
}