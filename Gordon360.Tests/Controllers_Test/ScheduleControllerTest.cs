using Gordon360.Controllers;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Gordon360.Tests.Controllers_Test;

public class ScheduleControllerTest
{
    private readonly Mock<IProfileService> _mockProfileService;
    private readonly Mock<IScheduleService> _mockScheduleService;
    private readonly Mock<IAccountService> _mockAccountService;
    private readonly ScheduleController _controller;

    public ScheduleControllerTest()
    {
        _mockProfileService = new Mock<IProfileService>();
        _mockScheduleService = new Mock<IScheduleService>();
        _mockAccountService = new Mock<IAccountService>();
        _controller = new ScheduleController(
            _mockProfileService.Object,
            _mockScheduleService.Object,
            _mockAccountService.Object
        );
    }

    private void SetUser(string username, string group)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Upn, $"{username}@gordon.edu"),
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

    private static UserCoursesViewModel GetSampleCourse() => new UserCoursesViewModel
    {
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
    };

    [Fact]
    public async Task GetAllCoursesByTerm_ReturnsOk_ForSelf()
    {
        var username = "jdoe";
        var courses = new List<UserCoursesViewModel> { GetSampleCourse() };
        var term = new CoursesByTermViewModel("2024", "SP", "Spring 2024", new DateTime(2024, 1, 10), new DateTime(2024, 5, 10), courses);
        var terms = new List<CoursesByTermViewModel> { term };
        _mockScheduleService.Setup(s => s.GetAllCoursesByTermAsync(username)).ReturnsAsync(terms);

        SetUser(username, "360-Student-SG");

        var result = await _controller.GetAllCoursesByTerm(username);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<CoursesByTermViewModel>>(okResult.Value);
        Assert.Single(returned);
        Assert.Equal("2024", returned.First().YearCode);
    }

    [Fact]
    public async Task GetAllCoursesByTerm_ReturnsOk_ForFacStaff()
    {
        var username = "facstaff";
        var courses = new List<UserCoursesViewModel> { GetSampleCourse() };
        var term = new CoursesByTermViewModel("2024", "SP", "Spring 2024", new DateTime(2024, 1, 10), new DateTime(2024, 5, 10), courses);
        var terms = new List<CoursesByTermViewModel> { term };
        _mockScheduleService.Setup(s => s.GetAllCoursesByTermAsync(username)).ReturnsAsync(terms);

        SetUser(username, "360-FacStaff-SG");

        var result = await _controller.GetAllCoursesByTerm(username);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<CoursesByTermViewModel>>(okResult.Value);
        Assert.Single(returned);
        Assert.Equal("2024", returned.First().YearCode);
    }

    [Fact]
    public async Task GetAllCoursesByTerm_ReturnsEmpty_ForOtherStudent()
    {
        var requestingUsername = "studentA";
        var targetUsername = "studentB";

        SetUser(requestingUsername, "360-Student-SG");

        _mockScheduleService.Setup(s => s.GetAllCoursesByTermAsync(It.IsAny<string>()))
                    .ReturnsAsync(new List<CoursesByTermViewModel>());

        var result = await _controller.GetAllCoursesByTerm(targetUsername);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<CoursesByTermViewModel>>(okResult.Value);
        Assert.Empty(returned);
    }



}