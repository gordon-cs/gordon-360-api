using Gordon360.Models.Salesforce;

namespace Gordon360.Tests.Services_Test;

public class SFAcademicTermServiceTests
    : AcademicTermServiceContractTests
{
    protected override IAcademicTermService CreateService()
    {
        var mockContext = new Mock<ISalesforceContext>();

        var procedures = new AcademicTermProcedures(mockContext.Object);

        return new SFAcademicTermService(procedures);
    }
}