using Gordon360.Controllers;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.CCT.dbo;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Gordon360.Tests.Controllers_Test;

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

    public ScheduleControllerIntegrationTest()
    {
        // Setup SQLite in-memory
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<CCTContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new CCTContext(options);
        _context.Database.EnsureCreated();

        // Seed minimal data for integration
        SeedTestData();

        // Setup real services
        _sessionService = new SessionService(_context);
        _academicTermService = new AcademicTermService(_context);
        _scheduleService = new ScheduleService(_context, _sessionService, _academicTermService);
        _profileService = new ProfileService(_context);
        _accountService = new AccountService(_context);

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
        // Add a session
        _context.CM_SESSION_MSTR.Add(new CM_SESSION_MSTR
        {
            SessionCode = "202401",
            Description = "Spring 2024",
            BeginDate = new DateTime(2024, 1, 10),
            EndDate = new DateTime(2024, 5, 10)
        });

        // Add a term
        _context.YearTermTable.Add(new YearTermTableViewModel
        {
            YearCode = "2024",
            TermCode = "SP",
            TermDescription = "Spring 2024",
            TermBeginDate = new DateTime(2024, 1, 10),
            TermEndDate = new DateTime(2024, 5, 10)
        });

        // Add a user course
        _context.UserCourses.Add(new UserCoursesViewModel
        {
            Username = "jdoe",
            SessionCode = "202401",
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
            SUB_TERM_CDE = "A",
            Role = "Student"
        });

        // Add a student profile
        _context.Student.Add(new Student
        {
            Username = "jdoe",
            FirstName = "John",
            LastName = "Doe",
            ID_NUM = "1234567"
        });

        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllCoursesByTerm_ReturnsCourses_ForSelf()
    {
        // Arrange
        SetUser("jdoe", "360-Student-SG");

        // Act
        var result = await _controller.GetAllCoursesByTerm("jdoe");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<CoursesByTermViewModel>>(okResult.Value);
        Assert.Single(returned);
        Assert.Equal("2024", returned.First().YearCode);
        Assert.Equal("SP", returned.First().TermCode);
        Assert.Single(returned.First().AllCourses);
        Assert.Equal("CS101", returned.First().AllCourses.First().CRS_CDE);
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
    }
} 