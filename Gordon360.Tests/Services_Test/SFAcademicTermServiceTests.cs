using Gordon360.Models.Salesforce;

namespace Gordon360.Tests.Services_Test;

public class SFAcademicTermServiceTests
    : AcademicTermServiceContractTests
{
    protected override IAcademicTermService CreateService()
    {
        var sfContext = new Mock<ISalesforceContext>();

        var procedures = new AcademicTermProcedures(sfContext.Object);

        return new SFAcademicTermService(procedures);
    }
}