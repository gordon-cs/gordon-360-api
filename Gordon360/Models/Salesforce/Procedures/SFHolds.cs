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
        var result = await _context.Query<Hold>(string.Format(SoqlTemplate, username));
        var holds = result.records;

        

        var FinancialHold = false;
        var HighSchoolHold = false;
        var MedicalHold = false;
        var MajorHold = false;
        var RegistrarHold = false;
        var LaVidaHold = false;
        var MustRegisterForClasses = false;

        foreach (var hold in holds)
    {
        switch (hold.Hold_Type__r.Name)
        {
            case "Financial Hold":
                FinancialHold = true;
                break;

            case "High School Graduation":
                HighSchoolHold = true;
                break;

            case "Medical Hold":
                MedicalHold = true;
                break;

            case "Major Not Declared Hold":
                MajorHold = true;
                break;

            case "Registrar Hold":
                RegistrarHold = true;
                break;

            case "Discovery or La Vida Required":
                LaVidaHold = true;
                break;

            case "Must Register For Classes": // TODO: Is that correct? 
                MustRegisterForClasses = true;
                break;
        }
    }

        var enrollmentHolds = new EnrollmentCheckinHolds
        (
            FinancialHold, 
            HighSchoolHold, 
            MedicalHold, 
            MajorHold, 
            RegistrarHold, 
            LaVidaHold, 
            MustRegisterForClasses 
        );

        return enrollmentHolds;
}
    
}