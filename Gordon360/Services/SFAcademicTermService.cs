using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.Salesforce;
using Gordon360.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services;

public class SFAcademicTermService(SalesforceContext context) : IAcademicTermService
{
    public async Task<YearTermTableViewModel?> GetCurrentTermAsync()
    {
        var soql_query = $@"SELECT Id, Name
            FROM Academic_Term__c
            WHERE IsActive = true
            LIMIT 1";

        var response = await context.Query<AcademicTerm>(soql_query);

        AcademicTerm? term = response?.records?.FirstOrDefault();

        return term != null ? new YearTermTableViewModel(term) : null;
;
    }


    public async Task<IEnumerable<YearTermTableViewModel>> GetAllTermsAsync()
    {
        var soql_query = $@"SELECT Id, Name
            FROM Academic_Term__c
            WHERE AcademicYearId.IsActive = True";

        var terms = await context.Query<AcademicTerm>(soql_query);

        return terms.records.Select(t => new YearTermTableViewModel(t));
    }


    // The term list is in chronological order with the oldest term first.
    // In the main loop we search for the term we are currently in, or determine if we are
    // between terms.
    //
    // There are two cases:
    //   (1) we're in a regular academic term.
    //   (2) we're in between two existing academic terms.
    // Once we've found our place we construct the DaysLeftViewModel to display 
    // total days, days left, and term label of the period we are currently in.
    public async Task<DaysLeftViewModel> GetDaysLeftAsync()
    {
        var today = DateTime.Today;

        // Get all terms from the database
        var allTerms = await GetAllTermsAsync();

        // Filter terms to only include relevant ones (FA, SP, SU) and sort choronologically
        var relevantTerms = allTerms
            .Where(t => t.TermCode is "FA" or "SP" or "SU")
            .OrderBy(t => t.BeginDate)
            .ToList();

        // Iterate through the relevant terms to find where today's date fits
        for (int i = 0; i < relevantTerms.Count; i++)
        {
            var term = relevantTerms[i];

            if (term.BeginDate.HasValue && term.EndDate.HasValue)
            {
                var start = term.BeginDate.Value.Date;
                var end = term.EndDate.Value.Date;

                // Case 1: Today is inside an academic term
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
                // Case 2: Today is in between two terms
                if (i < relevantTerms.Count - 1)
                {
                    var nextTerm = relevantTerms[i + 1];
                    if (term.EndDate.HasValue && nextTerm.BeginDate.HasValue)
                    {
                        // Calculate the gap between the current term's end and the next term's start
                        var gapStart = term.EndDate.Value.Date.AddDays(1);
                        var gapEnd = nextTerm.BeginDate.Value.Date.AddDays(-1);

                        // If today is within the gap
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
        // If we reach here, it means today is not within any term, return default break
        return new DaysLeftViewModel
        {
            DaysLeft = 0,
            TotalDays = 0,
            TermLabel = "Break"
        };
    }

    public async Task<YearTermTableViewModel?> GetCurrentTermForFinalExamsAsync()
    {
        var currentDate = DateTime.Now;

        var soql_query = $@"SELECT Id, Name
            FROM Academic_Term__c
            WHERE AcademicYearId.IsActive = True AND (Season = Fall OR Season = Spring)
            ORDER BY DESC
            LIMIT 1";

        var terms = await context.Query<AcademicTerm>(soql_query);
        var finalExamTerm = terms.records.FirstOrDefault();

        return finalExamTerm != null ? new YearTermTableViewModel(finalExamTerm) : null;
    }
}
