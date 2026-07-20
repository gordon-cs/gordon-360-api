using Gordon360.Models.Salesforce;

namespace Gordon360.Tests.Services_Test;

public class SFSessionServiceTests
    : SessionServiceContractTests
{
    protected override ISessionService CreateService()
    {
        var today = DateTime.Today;

        var currentSession = new AcademicSession
        {
            Name = "Current Fall Session",
            gc_Jenz_Session_Code__c = $"{today.Year}09",
            gc_Jenz_Subterm_Code__c = "1Q",
            gc_Jenz_Term_Code__c = "FA",
            gc_Jenz_Year_Code__c = today.Year.ToString(),

            ClassStartDate = today.AddDays(-30).ToString("o"),
            ClassEndDate = today.AddDays(20).ToString("o"),
            ExamStartDate = today.AddDays(21).ToString("o"),
            ExamEndDate = today.AddDays(30).ToString("o")
        };

        var previousSession = new AcademicSession
        {
            Name = "Previous Spring Session",
            gc_Jenz_Session_Code__c = $"{today.Year - 1}01",
            gc_Jenz_Subterm_Code__c = "1Q",
            gc_Jenz_Term_Code__c = "SP",
            gc_Jenz_Year_Code__c = (today.Year - 1).ToString(),

            ClassStartDate = today.AddDays(-180).ToString("o"),
            ClassEndDate = today.AddDays(-130).ToString("o"),
            ExamStartDate = today.AddDays(-129).ToString("o"),
            ExamEndDate = today.AddDays(-120).ToString("o")
        };

        var mockContext = new Mock<ISalesforceContext>();

        mockContext
            .Setup(context =>
                context.Query<AcademicSession>(
                    It.IsAny<string>()))
            .ReturnsAsync(new SFQueryResult<AcademicSession>
            {
                records =
                [
                    currentSession,
                    previousSession
                ]
            });

        var procedures =
            new AcademicSessionProcedures(mockContext.Object);

        return new SFSessionService(procedures);
    }
}