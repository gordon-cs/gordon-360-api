using Gordon360.Models.Salesforce;

namespace Gordon360.Tests.Services_Test;

public class SFAcademicTermServiceTests
    : AcademicTermServiceContractTests
{
    protected override IAcademicTermService CreateService()
    {
        var today = DateTime.Today;
        var currentTerm = new AcademicTerm
        {
            gc_Jenz_Term_Code__c = "FA",
            gc_Jenz_Year_Code__c = today.Year.ToString(),
            Name = "Current Term",
            StartDate = today.AddDays(-1).ToString("o"),
            EndDate = today.AddDays(7).ToString("o")
        };
        var previousTerm = new AcademicTerm
        {
            gc_Jenz_Term_Code__c = "SP",
            gc_Jenz_Year_Code__c = (today.Year - 1).ToString(),
            Name = "Previous Term",
            StartDate = today.AddDays(-45).ToString("o"),
            EndDate = today.AddDays(-20).ToString("o")
        };

        var mockContext = new Mock<ISalesforceContext>();
        mockContext
            .Setup(context => context.RawQuery<AcademicTerm>(It.IsAny<string>()))
            .ReturnsAsync(new SFQueryResult<AcademicTerm>
            {
                records = [currentTerm, previousTerm]
            });

        var procedures = new AcademicTermProcedures(mockContext.Object);

        return new SFAcademicTermService(procedures);
    }
}