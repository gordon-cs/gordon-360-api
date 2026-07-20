namespace Gordon360.Tests;

/// <summary>
/// Contract tests for IScheduleService.
/// These tests define the expected behavior of any implementation of the interface.
/// </summary>
public abstract class ScheduleServiceContractTests
{
    protected abstract IScheduleService CreateService();

    [Fact]
    public async Task GetAllCoursesAsync_ReturnsSessionsWithMatchingCoursesInDescendingOrder()
    {
        var service = CreateService();

        var sessions = (await service.GetAllCoursesAsync("testuser")).ToList();

        Assert.NotEmpty(sessions);
        Assert.Equal("202509", sessions[0].SessionCode);
        Assert.All(sessions, s => Assert.NotEmpty(s.AllCourses));
        Assert.True(sessions.Zip(sessions.Skip(1), (current, next) => string.Compare(current.SessionCode, next.SessionCode, StringComparison.Ordinal) >= 0).All(x => x));
    }

    [Fact]
    public async Task GetAllInstructorCoursesAsync_ReturnsSessionsWithInstructorCoursesInDescendingOrder()
    {
        var service = CreateService();

        var sessions = (await service.GetAllInstructorCoursesAsync("testuser")).ToList();

        Assert.NotEmpty(sessions);
        Assert.Equal("202509", sessions[0].SessionCode);
        Assert.All(sessions, s => Assert.NotEmpty(s.AllCourses));
        Assert.All(sessions.SelectMany(s => s.AllCourses), course => Assert.Equal("Teacher", course.Role));
    }

    [Fact]
    public async Task GetAllCoursesByTermAsync_ReturnsTermsWithMatchingCoursesInDescendingOrder()
    {
        var service = CreateService();

        var terms = (await service.GetAllCoursesByTermAsync("testuser")).ToList();

        Assert.NotEmpty(terms);
        Assert.Equal("FA", terms[0].TermCode);
        Assert.All(terms, t => Assert.NotEmpty(t.AllCourses));
        Assert.True(terms.Zip(terms.Skip(1), (current, next) => current.TermBeginDate >= next.TermBeginDate).All(x => x));
    }

    [Fact]
    public async Task GetAllInstructorCoursesByTermAsync_ReturnsTermsWithInstructorCoursesInDescendingOrder()
    {
        var service = CreateService();

        var terms = (await service.GetAllInstructorCoursesByTermAsync("testuser")).ToList();

        Assert.NotEmpty(terms);
        Assert.Equal("FA", terms[0].TermCode);
        Assert.All(terms, t => Assert.NotEmpty(t.AllCourses));
        Assert.All(terms.SelectMany(t => t.AllCourses), course => Assert.Equal("Teacher", course.Role));
    }
}
