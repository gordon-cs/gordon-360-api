using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;

namespace Gordon360.Models.Salesforce;

public class SFHolds(ISalesforceContext context)
{
    private readonly ISalesforceContext _context = context;

    private const string SoqlTemplate = """
        SELECT 
            Hold_Type__r.Name,
            Status__c
        FROM Hold__c
        WHERE Contact__r.AD_Username__c LIKE '{0}%'  AND Status__c = 'Active'
    """; 


    public async Task<EnrollmentCheckinHolds> GetHolds(string username)
{
    var holds = await _context.Query<Hold>(string.Format(SoqlTemplate, username));

    var enrollmentHolds = new EnrollmentCheckinHolds
    {
        FinancialHold = false,
        HighSchoolHold = false,
        MedicalHold = false,
        MajorHold = false,
        RegistrarHold = false,
        LaVidaHold = false,
        MustRegisterForClasses = false
    };

    foreach (var hold in holds)
    {
        switch (hold.Hold_Type__r.Name)
        {
            case "Financial Hold":
                enrollmentHolds.FinancialHold = true;
                break;

            case "High School Graduation":
                enrollmentHolds.HighSchoolHold = true;
                break;

            case "Medical Hold":
                enrollmentHolds.MedicalHold = true;
                break;

            case "Major Not Declared Hold":
                enrollmentHolds.MajorHold = true;
                break;

            case "Registrar Hold":
                enrollmentHolds.RegistrarHold = true;
                break;

            case "Discovery or La Vida Required":
                enrollmentHolds.LaVidaHold = true;
                break;

            case "Must Register For Classes": // TODO: Is that correct? 
                enrollmentHolds.MustRegisterForClasses = true;
                break;
        }
    }

    return enrollmentHolds;
}
    
}