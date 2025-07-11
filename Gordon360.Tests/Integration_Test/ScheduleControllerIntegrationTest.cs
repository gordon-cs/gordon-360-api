// Integration tests for ScheduleController using SQLite in-memory database.
// Covers Student, Alumni, Facstaff, and Advisor schedule access scenarios.

using Gordon360.Controllers;
using Gordon360.Models.CCT.Context;
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
using Microsoft.Extensions.Configuration;

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
        // Setup SQLite in-memory
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<CCTContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new CCTContext(options);
        _context.Database.EnsureCreated();

        // Minimal configuration and webSQLContext for ProfileService
        var configBuilder = new ConfigurationBuilder();
        _configuration = configBuilder.Build();
        var webSQLOptions = new DbContextOptionsBuilder<Gordon360.Models.webSQL.Context.webSQLContext>()
            .UseSqlite(_connection)
            .Options;
        _webSQLContext = new Gordon360.Models.webSQL.Context.webSQLContext(webSQLOptions);

        // Seed minimal data for integration
        SeedTestData();

        // Setup real services
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
        // Add a session
        _context.CM_SESSION_MSTR.Add(new Gordon360.Models.CCT.CM_SESSION_MSTR
        {
            SESS_CDE = "202401",
            SESS_DESC = "Spring 2024",
            SESS_BEGN_DTE = new DateTime(2024, 1, 10),
            SESS_END_DTE = new DateTime(2024, 5, 10)
        });
        // Add a term
        _context.YearTermTable.Add(new Gordon360.Models.CCT.YearTermTable
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
        // Add user courses
        _context.UserCourses.Add(new Gordon360.Models.CCT.UserCourses
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
        _context.UserCourses.Add(new Gordon360.Models.CCT.UserCourses
        {
            Username = "student2",
            Role = "Student",
            YR_CDE = "2024",
            TRM_CDE = "SP",
            CRS_CDE = "CS102",
            CRS_TITLE = "Data Structures",
            BLDG_CDE = "KEN",
            ROOM_CDE = "102",
            MONDAY_CDE = "Y",
            TUESDAY_CDE = "Y",
            WEDNESDAY_CDE = "N",
            THURSDAY_CDE = "N",
            FRIDAY_CDE = "Y",
            SATURDAY_CDE = "N",
            BEGIN_TIME = new TimeSpan(10, 30, 0),
            END_TIME = new TimeSpan(11, 45, 0),
            BEGIN_DATE = new DateTime(2024, 1, 10),
            END_DATE = new DateTime(2024, 5, 10),
            SUBTERM_DESC = "A"
        });
        _context.UserCourses.Add(new Gordon360.Models.CCT.UserCourses
        {
            Username = "alum1",
            Role = "Student",
            YR_CDE = "2024",
            TRM_CDE = "SP",
            CRS_CDE = "HIST101",
            CRS_TITLE = "World History",
            BLDG_CDE = "HIS",
            ROOM_CDE = "201",
            MONDAY_CDE = "N",
            TUESDAY_CDE = "Y",
            WEDNESDAY_CDE = "N",
            THURSDAY_CDE = "Y",
            FRIDAY_CDE = "N",
            SATURDAY_CDE = "N",
            BEGIN_TIME = new TimeSpan(13, 0, 0),
            END_TIME = new TimeSpan(14, 15, 0),
            BEGIN_DATE = new DateTime(2024, 1, 10),
            END_DATE = new DateTime(2024, 5, 10),
            SUBTERM_DESC = "A"
        });
        _context.UserCourses.Add(new Gordon360.Models.CCT.UserCourses
        {
            Username = "fac1",
            Role = "Student",
            YR_CDE = "2024",
            TRM_CDE = "SP",
            CRS_CDE = "ENG201",
            CRS_TITLE = "English Lit",
            BLDG_CDE = "ENG",
            ROOM_CDE = "301",
            MONDAY_CDE = "N",
            TUESDAY_CDE = "N",
            WEDNESDAY_CDE = "Y",
            THURSDAY_CDE = "Y",
            FRIDAY_CDE = "N",
            SATURDAY_CDE = "N",
            BEGIN_TIME = new TimeSpan(14, 30, 0),
            END_TIME = new TimeSpan(15, 45, 0),
            BEGIN_DATE = new DateTime(2024, 1, 10),
            END_DATE = new DateTime(2024, 5, 10),
            SUBTERM_DESC = "A"
        });
        _context.UserCourses.Add(new Gordon360.Models.CCT.UserCourses
        {
            Username = "advisor1",
            Role = "Student",
            YR_CDE = "2024",
            TRM_CDE = "SP",
            CRS_CDE = "ADV101",
            CRS_TITLE = "Advising 101",
            BLDG_CDE = "ADV",
            ROOM_CDE = "401",
            MONDAY_CDE = "Y",
            TUESDAY_CDE = "N",
            WEDNESDAY_CDE = "Y",
            THURSDAY_CDE = "N",
            FRIDAY_CDE = "Y",
            SATURDAY_CDE = "N",
            BEGIN_TIME = new TimeSpan(8, 0, 0),
            END_TIME = new TimeSpan(9, 15, 0),
            BEGIN_DATE = new DateTime(2024, 1, 10),
            END_DATE = new DateTime(2024, 5, 10),
            SUBTERM_DESC = "A"
        });
        _context.UserCourses.Add(new Gordon360.Models.CCT.UserCourses
        {
            Username = "hybrid1",
            Role = "Instructor",
            YR_CDE = "2024",
            TRM_CDE = "SP",
            CRS_CDE = "MATH301",
            CRS_TITLE = "Advanced Math",
            BLDG_CDE = "MAT",
            ROOM_CDE = "501",
            MONDAY_CDE = "Y",
            TUESDAY_CDE = "Y",
            WEDNESDAY_CDE = "Y",
            THURSDAY_CDE = "Y",
            FRIDAY_CDE = "Y",
            SATURDAY_CDE = "N",
            BEGIN_TIME = new TimeSpan(11, 0, 0),
            END_TIME = new TimeSpan(12, 15, 0),
            BEGIN_DATE = new DateTime(2024, 1, 10),
            END_DATE = new DateTime(2024, 5, 10),
            SUBTERM_DESC = "A"
        });
        // Add students
        _context.Student.Add(new Gordon360.Models.CCT.Student { ID = "S1234567", FirstName = "John", LastName = "Doe", AD_Username = "jdoe" });
        _context.Student.Add(new Gordon360.Models.CCT.Student { ID = "S7654321", FirstName = "Jane", LastName = "Smith", AD_Username = "student2" });
        // Add alumni
        _context.Alumni.Add(new Gordon360.Models.CCT.Alumni { ID = "A1111111", FirstName = "Alice", LastName = "Alumni", AD_Username = "alum1" });
        // Add facstaff
        _context.FacStaff.Add(new Gordon360.Models.CCT.FacStaff { ID = "F2222222", FirstName = "Frank", LastName = "Faculty", AD_Username = "fac1" });
        _context.FacStaff.Add(new Gordon360.Models.CCT.FacStaff { ID = "F3333333", FirstName = "Adrian", LastName = "Advisor", AD_Username = "advisor1" });
        _context.FacStaff.Add(new Gordon360.Models.CCT.FacStaff { ID = "F4444444", FirstName = "Hank", LastName = "Hybrid", AD_Username = "hybrid1" });
        _context.SaveChanges();
    }

    // ===================== Student Cases =====================

    /// <summary>Student for self: should return their own course</summary>
    [Fact]
    public async Task Student_ForSelf_ReturnsCourse()
    {
        SetUser("jdoe", "360-Student-SG");
        var result = await _controller.GetAllCoursesByTerm("jdoe");
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var data = Assert.IsAssignableFrom<IEnumerable<CoursesByTermViewModel>>(ok.Value);
        // Expect CS101 (jdoe's course)
        Assert.Contains(data.SelectMany(t => t.AllCourses), c => c.CRS_CDE == "CS101");
    }

    /// <summary>Student for other student: should return empty</summary>
    [Fact]
    public async Task Student_ForOtherStudent_ReturnsEmpty()
    {
        SetUser("jdoe", "360-Student-SG");
        var result = await _controller.GetAllCoursesByTerm("student2");
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var data = Assert.IsAssignableFrom<IEnumerable<CoursesByTermViewModel>>(ok.Value);
        Assert.Empty(data.SelectMany(t => t.AllCourses));
    }

    /// <summary>Student for alumni: should return empty</summary>
    [Fact]
    public async Task Student_ForAlumni_ReturnsEmpty()
    {
        SetUser("jdoe", "360-Student-SG");
        var result = await _controller.GetAllCoursesByTerm("alum1");
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var data = Assert.IsAssignableFrom<IEnumerable<CoursesByTermViewModel>>(ok.Value);
        Assert.Empty(data.SelectMany(t => t.AllCourses));
    }

    /// <summary>Student for facstaff: should return facstaff's course</summary>
    [Fact]
    public async Task Student_ForFacstaff_ReturnsCourse()
    {
        SetUser("jdoe", "360-Student-SG");
        var result = await _controller.GetAllCoursesByTerm("fac1");
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var data = Assert.IsAssignableFrom<IEnumerable<CoursesByTermViewModel>>(ok.Value);
        // Expect ENG201 (fac1's course)
        Assert.Contains(data.SelectMany(t => t.AllCourses), c => c.CRS_CDE == "ENG201");
    }

    /// <summary>Student for (Student,Faculty): should return instructor course</summary>
    [Fact]
    public async Task Student_ForHybrid_ReturnsInstructorCourse()
    {
        SetUser("jdoe", "360-Student-SG");
        var result = await _controller.GetAllCoursesByTerm("hybrid1");
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var data = Assert.IsAssignableFrom<IEnumerable<CoursesByTermViewModel>>(ok.Value);
        // Expect MATH301 (hybrid1's instructor course)
        Assert.Contains(data.SelectMany(t => t.AllCourses), c => c.CRS_CDE == "MATH301" && c.Role == "Instructor");
    }

    // ===================== Alumni Cases =====================

    /// <summary>Alumni for other alumni: should return empty</summary>
    [Fact]
    public async Task Alumni_ForOtherAlumni_ReturnsEmpty()
    {
        SetUser("alum1", "360-Alumni-SG");
        // Self: expect HIST101
        var result = await _controller.GetAllCoursesByTerm("alum1");
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var data = Assert.IsAssignableFrom<IEnumerable<CoursesByTermViewModel>>(ok.Value);
        Assert.Contains(data.SelectMany(t => t.AllCourses), c => c.CRS_CDE == "HIST101");
        // Other alumni (not seeded, so should be empty)
        result = await _controller.GetAllCoursesByTerm("alum2");
        ok = Assert.IsType<OkObjectResult>(result.Result);
        data = Assert.IsAssignableFrom<IEnumerable<CoursesByTermViewModel>>(ok.Value);
        Assert.Empty(data.SelectMany(t => t.AllCourses));
    }

    /// <summary>Alumni for facstaff: should return facstaff's course</summary>
    [Fact]
    public async Task Alumni_ForFacstaff_ReturnsCourse()
    {
        SetUser("alum1", "360-Alumni-SG");
        var result = await _controller.GetAllCoursesByTerm("fac1");
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var data = Assert.IsAssignableFrom<IEnumerable<CoursesByTermViewModel>>(ok.Value);
        Assert.Contains(data.SelectMany(t => t.AllCourses), c => c.CRS_CDE == "ENG201");
    }

    /// <summary>Alumni for self: should return their own course</summary>
    [Fact]
    public async Task Alumni_ForSelf_ReturnsCourse()
    {
        SetUser("alum1", "360-Alumni-SG");
        var result = await _controller.GetAllCoursesByTerm("alum1");
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var data = Assert.IsAssignableFrom<IEnumerable<CoursesByTermViewModel>>(ok.Value);
        Assert.Contains(data.SelectMany(t => t.AllCourses), c => c.CRS_CDE == "HIST101");
    }

    /// <summary>Alumni for (Student,Faculty): should return instructor course</summary>
    [Fact]
    public async Task Alumni_ForHybrid_ReturnsInstructorCourse()
    {
        SetUser("alum1", "360-Alumni-SG");
        var result = await _controller.GetAllCoursesByTerm("hybrid1");
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var data = Assert.IsAssignableFrom<IEnumerable<CoursesByTermViewModel>>(ok.Value);
        Assert.Contains(data.SelectMany(t => t.AllCourses), c => c.CRS_CDE == "MATH301" && c.Role == "Instructor");
    }

    // ===================== Facstaff/Advisor Cases =====================

    /// <summary>Facstaff for student: should return empty</summary>
    [Fact]
    public async Task Facstaff_ForStudent_ReturnsEmpty()
    {
        SetUser("fac1", "360-FacStaff-SG");
        var result = await _controller.GetAllCoursesByTerm("jdoe");
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var data = Assert.IsAssignableFrom<IEnumerable<CoursesByTermViewModel>>(ok.Value);
        Assert.Empty(data.SelectMany(t => t.AllCourses));
    }

    /// <summary>Advisor for student: should return student's course</summary>
    [Fact]
    public async Task Advisor_ForStudent_ReturnsCourse()
    {
        SetUser("advisor1", "360-FacStaff-SG,360-Advisor-SG");
        var result = await _controller.GetAllCoursesByTerm("jdoe");
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var data = Assert.IsAssignableFrom<IEnumerable<CoursesByTermViewModel>>(ok.Value);
        Assert.Contains(data.SelectMany(t => t.AllCourses), c => c.CRS_CDE == "CS101");
    }

    /// <summary>Facstaff for self: should return their own course</summary>
    [Fact]
    public async Task Facstaff_ForSelf_ReturnsCourse()
    {
        SetUser("fac1", "360-FacStaff-SG");
        var result = await _controller.GetAllCoursesByTerm("fac1");
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var data = Assert.IsAssignableFrom<IEnumerable<CoursesByTermViewModel>>(ok.Value);
        Assert.Contains(data.SelectMany(t => t.AllCourses), c => c.CRS_CDE == "ENG201");
    }

    /// <summary>Advisor for self: should return their own course</summary>
    [Fact]
    public async Task Advisor_ForSelf_ReturnsCourse()
    {
        SetUser("advisor1", "360-FacStaff-SG,360-Advisor-SG");
        var result = await _controller.GetAllCoursesByTerm("advisor1");
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var data = Assert.IsAssignableFrom<IEnumerable<CoursesByTermViewModel>>(ok.Value);
        Assert.Contains(data.SelectMany(t => t.AllCourses), c => c.CRS_CDE == "ADV101");
    }

    /// <summary>Facstaff for alumni: should return alumni's course</summary>
    [Fact]
    public async Task Facstaff_ForAlumni_ReturnsCourse()
    {
        SetUser("fac1", "360-FacStaff-SG");
        var result = await _controller.GetAllCoursesByTerm("alum1");
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var data = Assert.IsAssignableFrom<IEnumerable<CoursesByTermViewModel>>(ok.Value);
        Assert.Contains(data.SelectMany(t => t.AllCourses), c => c.CRS_CDE == "HIST101");
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Close();
    }
} 