using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;

namespace Gordon360.Models.Salesforce;

public class SFStudentEmployment(ISalesforceContext context)
{
    private readonly ISalesforceContext _context = context;

    /* 
    public class PersonEmployment
{

    public string Position { get; set; } = "";
    public string DivisionDepartment__c { get; set; } = "";
    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime EndDate { get; set; } = DateTime.Now;
}
*/

    private const string SoqlTemplate = """
        SELECT
            Position,
            DivisionDepartment__c,
            StartDate,
            EndDate
        FROM PersonEmployment
        WHERE Account.Email LIKE '{0}%'
    """;

/// <summary>
/// Fetch IEnumerable of student employment records for the given user
/// </summary>
/// <param name="username">Active Directory username</param>
/// <returns>IEnumerable of student employment records for the user, or empty if unauthorized</returns>
    public async Task<IEnumerable<PersonEmploymentViewModel>> GetStudentEmployment(string username)
    {
        
        var response = await _context.Query<PersonEmployment>(string.Format(SoqlTemplate, username));

        return response?.records?
            .Select(c => MapToViewModel(c, username))
            .ToList() ?? new List<PersonEmploymentViewModel>();
    }

    private static PersonEmploymentViewModel MapToViewModel(PersonEmployment c, string username)
    {
        return new PersonEmploymentViewModel
        {
            Position = c.Position,
            DivisionDepartment__c = c.DivisionDepartment__c,
            StartDate = c.StartDate,
            EndDate = c.EndDate
        };

    }

    private static string DayCode(bool? flag, string code) => flag == true ? code : "";

    private static TimeSpan? ParseTime(string? time)
    {
        if (string.IsNullOrWhiteSpace(time))
        {
            return null;
        }
        else
        {
            var cleanedTime = time.Replace("Z", "");
            var isValid = TimeSpan.TryParse(cleanedTime, out var t);

            return isValid ? t : null;
        }
    }
}