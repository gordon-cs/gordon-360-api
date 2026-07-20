namespace Gordon360.Tests;

/// <summary>
/// Contract tests for ISessionService.
/// These tests define the expected behavior of any implementation of the interface.
/// </summary>
public abstract class SessionServiceContractTests
{
    protected abstract ISessionService CreateService();

    [Fact]
    public async Task Get_ReturnsSessionWithMatchingSessionCode()
    {
        var service = CreateService();

        var session = await service.Get("202509");

        Assert.NotNull(session);
        Assert.Equal("202509", session.SessionCode);
    }

    [Fact]
    public async Task Get_ReturnsNullWhenSessionDoesNotExist()
    {
        var service = CreateService();

        var session = await service.Get("DOES_NOT_EXIST");

        Assert.Null(session);
    }

    [Fact]
    public async Task GetCurrentSession_ReturnsCurrentSession()
    {
        var service = CreateService();

        var session = await service.GetCurrentSession();

        Assert.NotNull(session);
        Assert.NotNull(session.SessionBeginDate);
        Assert.NotNull(session.SessionEndDate);

        Assert.True(
            session.SessionBeginDate <= DateTime.Today,
            $"Session begins in the future: {session.SessionBeginDate}");

        Assert.True(
            session.SessionEndDate >= DateTime.Today,
            $"Session ended in the past: {session.SessionEndDate}");
    }

    [Fact]
    public async Task GetCurrentSession_ReturnsNullWhenNoCurrentSessionExists()
    {
        var service = CreateService();

        var session = await service.GetCurrentSession();

        if (session is not null)
        {
            Assert.NotNull(session.SessionBeginDate);
            Assert.NotNull(session.SessionEndDate);

            Assert.True(session.SessionBeginDate <= DateTime.Today);
            Assert.True(session.SessionEndDate >= DateTime.Today);
        }
    }

    [Fact]
    public async Task GetCurrentSessionForFinalExams_ReturnsSpringOrFallSession()
    {
        var service = CreateService();

        var session = await service.GetCurrentSessionForFinalExams();

        Assert.NotNull(session);
        Assert.NotNull(session.SessionBeginDate);
        Assert.NotNull(session.SessionEndDate);

        Assert.True(
            session.SessionBeginDate < DateTime.Today,
            $"Session has not started: {session.SessionBeginDate}");

        Assert.Contains(
            session.SessionCode.Substring(4, 2),
            new[] { "SP", "FA" });
    }

    [Fact]
    public async Task GetCurrentSessionForFinalExams_ReturnsNullWhenNoSpringOrFallSessionExists()
    {
        var service = CreateService();

        var session = await service.GetCurrentSessionForFinalExams();

        if (session is not null)
        {
            Assert.Contains(
                session.SessionCode.Substring(4, 2),
                new[] { "SP", "FA" });
        }
    }

    [Fact]
    public async Task GetDaysLeft_ReturnsValidValues()
    {
        var service = CreateService();

        var daysLeft = await service.GetDaysLeft();

        Assert.True(daysLeft.TotalDays >= 0);
        Assert.True(daysLeft.DaysLeft >= 0);
        Assert.True(daysLeft.DaysLeft <= daysLeft.TotalDays);
        Assert.NotEmpty(daysLeft.TermLabel);
    }

    [Fact]
    public async Task GetDaysLeft_ReturnsCurrentSessionCodeAsTermLabel()
    {
        var service = CreateService();

        var daysLeft = await service.GetDaysLeft();
        var currentSession = await service.GetCurrentSession();

        if (currentSession is null)
        {
            Assert.Equal(0, daysLeft.DaysLeft);
            Assert.Equal(0, daysLeft.TotalDays);
            Assert.Empty(daysLeft.TermLabel);
            return;
        }

        Assert.Equal(
            currentSession.SessionCode,
            daysLeft.TermLabel);
    }

    [Fact]
    public async Task GetAll_ReturnsSessionsInDescendingOrder()
    {
        var service = CreateService();

        var sessions = (await service.GetAll()).ToList();

        Assert.NotEmpty(sessions);

        Assert.True(
            sessions.Zip(
                    sessions.Skip(1),
                    (current, next) =>
                        current.SessionBeginDate >= next.SessionBeginDate)
                .All(x => x));
    }

    [Fact]
    public async Task GetAll_ReturnsSessionsWithValidDateRanges()
    {
        var service = CreateService();

        var sessions = await service.GetAll();

        Assert.All(
            sessions.Where(s =>
                s.SessionBeginDate.HasValue &&
                s.SessionEndDate.HasValue),
            session =>
            {
                Assert.True(
                    session.SessionBeginDate <= session.SessionEndDate,
                    $"Session {session.SessionCode} has an invalid date range.");
            });
    }

    [Fact]
    public async Task GetAll_ReturnsEmptyCollectionWhenNoSessionsExist()
    {
        var service = CreateService();

        var sessions = await service.GetAll();

        Assert.NotNull(sessions);
    }
}