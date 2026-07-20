namespace Gordon360.Tests.Controllers_Test;

public class SessionsControllerTest
{
    private readonly Mock<ISessionService> _mockService;
    private readonly SessionsController _controller;

    public SessionsControllerTest()
    {
        _mockService = new Mock<ISessionService>();
        _controller = new SessionsController(_mockService.Object);
    }

    private void SetUser(string username, string group)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Upn, $"{username}@gordon.edu"),
            new(ClaimTypes.Name, username),
            new("groups", group)
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var user = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    private static SessionViewModel GetSampleSession()
    {
        return new SessionViewModel
        {
            SessionCode = "202401",
            SessionDescription = "Spring 2024",
            SessionBeginDate = new DateTime(2024, 1, 10),
            SessionEndDate = new DateTime(2024, 5, 10)
        };
    }

    [Fact]
    public async Task Get_ReturnsOk_WithList()
    {
        var sessions = (IEnumerable<SessionViewModel>)[GetSampleSession()];
        _mockService.Setup(s => s.GetAll()).Returns(Task.FromResult(sessions));

        var result = await _controller.Get();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<SessionViewModel>>(okResult.Value);
        Assert.Single(returned);
        Assert.Equal("202401", returned.First().SessionCode);
    }

    [Fact]
    public async Task Get_ReturnsOk_WithEmptyList()
    {
        _mockService.Setup(s => s.GetAll()).Returns(Task.FromResult((IEnumerable<SessionViewModel>) []));

        var result = await _controller.Get();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<SessionViewModel>>(okResult.Value);
        Assert.Empty(returned);
    }

    [Fact]
    public async Task Get_ById_ReturnsOk_WhenSessionExists()
    {
        var username = "jdoe";
        var session = GetSampleSession();
        _mockService.Setup(s => s.Get(username)).ReturnsAsync(session);

        SetUser(username, "360-Student-SG");

        var result = await _controller.Get(username);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<SessionViewModel>(okResult.Value);
        Assert.Equal("202401", returned.SessionCode);
    }

    [Fact]
    public async Task Get_ById_ReturnsNotFound_WhenSessionDoesNotExist()
    {
        var username = "notfound";
        _mockService.Setup(s => s.Get(username)).ReturnsAsync((SessionViewModel)null);

        SetUser(username, "360-Student-SG");

        var result = await _controller.Get(username);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetCurrentSession_ReturnsOk_WhenSessionExists()
    {
        var session = GetSampleSession();
        _mockService.Setup(s => s.GetCurrentSession()).ReturnsAsync(session);

        var result = await _controller.GetCurrentSession();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<SessionViewModel>(okResult.Value);
        Assert.Equal("202401", returned.SessionCode);
    }

    [Fact]
    public async Task GetCurrentSession_ReturnsNotFound_WhenNoSession()
    {
        _mockService.Setup(s => s.GetCurrentSession()).ReturnsAsync((SessionViewModel)null);

        var result = await _controller.GetCurrentSession();

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetDaysLeftInSemester_ReturnsOk_WithValidDays()
    {
        var days = new DaysLeftViewModel { DaysLeft = 30, TotalDays = 120, TermLabel = "2026FA" };
        _mockService.Setup(s => s.GetDaysLeft()).Returns(Task.FromResult(days));

        var result = await _controller.GetDaysLeftInSemester();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<double[]>(okResult.Value);
        Assert.Equal(30, returned[0]);
        Assert.Equal(120, returned[1]);
    }

    [Fact]
    public async Task GetDaysLeftInSemester_ReturnsNotFound_WhenZeroDays()
    {
        var days = new DaysLeftViewModel { DaysLeft = 0, TotalDays = 0, TermLabel = "" };
        _mockService.Setup(s => s.GetDaysLeft()).Returns(Task.FromResult(days));

        var result = await _controller.GetDaysLeftInSemester();

        Assert.IsType<NotFoundResult>(result.Result);
    }
} 