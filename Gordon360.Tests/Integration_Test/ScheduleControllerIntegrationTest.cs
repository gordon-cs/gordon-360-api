using Gordon360.Controllers;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.CCT;
using Gordon360.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Gordon360.Tests.Integration_Test;

public class ScheduleControllerIntegrationTest : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly CCTContext _context;
    private readonly ScheduleController _controller;
    private readonly ScheduleService _scheduleService;
    private readonly ProfileService _profileService;
    private readonly AccountService _accountService;
    private readonly SessionService _sessionService;
    private readonly AcademicTermService _academicTermService;
    private readonly IConfiguration _configuration;
    private readonly Gordon360.Models.webSQL.Context.webSQLContext _webSQLContext;

    public ScheduleControllerIntegrationTest()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<CCTContext>()
            .UseSqlite(_connection)
            .Options;
        _context = new CCTContext(options);
        _context.Database.EnsureCreated();
        var configBuilder = new ConfigurationBuilder();
        _configuration = configBuilder.Build();
        var webSQLOptions = new DbContextOptionsBuilder<Gordon360.Models.webSQL.Context.webSQLContext>()
            .UseSqlite(_connection)
            .Options;
        _webSQLContext = new Gordon360.Models.webSQL.Context.webSQLContext(webSQLOptions);
        SeedTestData();
        _sessionService = new SessionService(_context);
        _academicTermService = new AcademicTermService(_context);
        _scheduleService = new ScheduleService(_context, _sessionService, _academicTermService);
        _accountService = new AccountService(_context);
        _profileService = new ProfileService(_context, _configuration, _accountService, _webSQLContext);
        _controller = new ScheduleController(_profileService, _scheduleService, _accountService);
    }

    private void SetUser(string username, string group)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim("groups", group)
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var user = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    private void SeedTestData()
    {
        _context.CM_SESSION_MSTR.Add(new CM_SESSION_MSTR
        {
            SESS_CDE = "202401",
            SESS_DESC = "Spring 2024",
            SESS_BEGN_DTE = new DateTime(2024, 1, 10),
            SESS_END_DTE = new DateTime(2024, 5, 10)
        });
        _context.YearTermTable.Add(new YearTermTable
        {
            YR_CDE = "2024",
            TRM_CDE = "SP",
            TRM_BEGIN_DTE = new DateTime(2024, 1, 10),
            TRM_END_DTE = new DateTime(2024, 5, 10),
            YR_TRM_DESC = "Spring 2024",
            PRT_INPROG_ON_TRAN = "Y",
            ONLINE_ADM_APP_OPEN = "Y",
            PREREG_STS = "Y",
            APPROWVERSION = new byte[] { 1 },
            SHOW_ON_WEB = "Y"
        });
        _context.UserCourses.Add(new UserCourses
        {
            Username = "jdoe",
            Role = "Student",
            YR_CDE = "2024",
            TRM_CDE = "SP",
            CRS_CDE = "CS101",
            CRS_TITLE = "Intro to CS",
            BLDG_CDE = "KEN",
            ROOM_CDE = "101",
            MONDAY_CDE = "Y",
            TUESDAY_CDE = "N",
            WEDNESDAY_CDE = "Y",
            THURSDAY_CDE = "N",
            FRIDAY_CDE = "Y",
            SATURDAY_CDE = "N",
            BEGIN_TIME = new TimeSpan(9, 0, 0),
            END_TIME = new TimeSpan(10, 15, 0),
            BEGIN_DATE = new DateTime(2024, 1, 10),
            END_DATE = new DateTime(2024, 5, 10),
            SUBTERM_DESC = "A"
        });
        _context.Student.Add(new Student { ID = "S1234567", FirstName = "John", LastName = "Doe", AD_Username = "jdoe" });
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllCoursesByTerm_ReturnsCourses_ForSelf()
    {
        SetUser("jdoe", "360-Student-SG");
        var result = await _controller.GetAllCoursesByTerm("jdoe");
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var data = Assert.IsAssignableFrom<IEnumerable<Gordon360.Models.ViewModels.CoursesByTermViewModel>>(ok.Value);
        Assert.Contains(data.SelectMany(t => t.AllCourses), c => c.CRS_CDE == "CS101");
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
    }
} 