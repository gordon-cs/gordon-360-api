namespace Gordon360.Tests;

/// <summary>
/// Contract tests for IAcademicTermService.
/// These tests define the expected behavior of any implementation of the interface.
/// </summary>
public abstract class AcademicTermServiceContractTests
{
    protected abstract IAcademicTermService CreateService();

    [Fact]
    public async Task GetAllTermsAsync_ReturnsTermsInDescendingStartDateOrder()
    {
        var service = CreateService();

        var terms = (await service.GetAllTermsAsync()).ToList();

        Assert.True(terms.Count > 1);

        for (int i = 0; i < terms.Count - 1; i++)
        {
            Assert.True(
                terms[i].BeginDate >= terms[i + 1].BeginDate,
                $"Term {i} starts before term {i + 1}.");
        }
    }
}