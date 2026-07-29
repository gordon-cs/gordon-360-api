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

    [Fact]
    public async Task GetCurrentTermAsync_ReturnsTheCurrentTerm()
    {
        var service = CreateService();

        var currentTerm = await service.GetCurrentTermAsync();

        Assert.NotNull(currentTerm);
        Assert.NotNull(currentTerm.BeginDate);
        Assert.NotNull(currentTerm.EndDate);
        Assert.True(currentTerm.BeginDate <= DateTime.Today);
        Assert.True(currentTerm.EndDate >= DateTime.Today);
    }

    [Fact]
    public async Task GetCurrentTermForFinalExamsAsync_ReturnsTheMostRecentSpringOrFallTerm()
    {
        var service = CreateService();

        var term = await service.GetCurrentTermForFinalExamsAsync();

        Assert.NotNull(term);
        Assert.Contains(term!.TermCode, new[] { "SP", "FA" });
        Assert.True(term.BeginDate < DateTime.Today);
    }

    [Fact]
    public async Task GetDaysLeftAsync_ReturnsPositiveRemainingDaysForTheCurrentTerm()
    {
        var service = CreateService();

        var daysLeft = await service.GetDaysLeftAsync();

        Assert.True(daysLeft.TotalDays > 0);
        Assert.True(daysLeft.DaysLeft > 0);
        Assert.False(string.IsNullOrWhiteSpace(daysLeft.TermLabel));
        Assert.True(daysLeft.DaysLeft <= daysLeft.TotalDays);
    }
}