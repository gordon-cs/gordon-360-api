using Xunit;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Gordon360.Controllers;
using Gordon360.Services;
using Gordon360.Models.ViewModels;
using Xunit.Abstractions;
using Gordon360.Utilities;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace Gordon360.Tests.Controllers_Test
{
    public class ScheduleControllerTests
    {
        private readonly ITestOutputHelper _output;

        public ScheduleControllerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private List<CoursesBySessionViewModel> GetExpectedCourses()
        {
            return new List<CoursesBySessionViewModel>
            {
                new CoursesBySessionViewModel(
                    "303308",
                    "3033-3034 academic year",
                    new DateTime(3033, 8, 15),
                    new DateTime(3034, 7, 16),
                    new List<UserCoursesViewModel>
                    {
                        new UserCoursesViewModel
                        {
                            SessionCode = "303308",
                            CRS_CDE = "CS101",
                            CRS_TITLE = "Intro to Computer Science",
                            BLDG_CDE = "KEN",
                            ROOM_CDE = "204",
                            MONDAY_CDE = "Y",
                            TUESDAY_CDE = "N",
                            WEDNESDAY_CDE = "Y",
                            THURSDAY_CDE = "N",
                            FRIDAY_CDE = "Y",
                            SATURDAY_CDE = "N",
                            BEGIN_TIME = new TimeSpan(9, 0, 0),
                            END_TIME = new TimeSpan(10, 0, 0),
                            Role = "Student"
                        }
                    }
                )
            };
        }

        [Fact]
        public async Task GetAllCourses_AsStudent_SeesInstructorCourses()
        {
            var mockService = new Mock<IScheduleService>();
            var expectedCourses = GetExpectedCourses();
            mockService.Setup(s => s.GetAllInstructorCoursesAsync("testuser")).ReturnsAsync(expectedCourses);

            var controller = new ScheduleController(mockService.Object);
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "viewer"),
                new Claim(ClaimTypes.Upn, "viewer@gordon.edu"),
                new Claim("groups", "360-Student-SG")
            }, "mock");

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            var result = await controller.GetAllCourses("testuser");
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCourses = Assert.IsAssignableFrom<IEnumerable<CoursesBySessionViewModel>>(okResult.Value);
            Assert.Equal(expectedCourses, returnedCourses);
            mockService.Verify(s => s.GetAllInstructorCoursesAsync("testuser"), Times.Once);
            mockService.Verify(s => s.GetAllCoursesAsync("testuser"), Times.Never);
        }

        [Fact]
        public async Task GetAllCourses_AsFacStaff_SeesAllCourses()
        {
            var mockService = new Mock<IScheduleService>();
            var expectedCourses = GetExpectedCourses();
            mockService.Setup(s => s.GetAllCoursesAsync("testuser")).ReturnsAsync(expectedCourses);

            var controller = new ScheduleController(mockService.Object);
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "viewer"),
                new Claim(ClaimTypes.Upn, "viewer@gordon.edu"),
                new Claim("groups", "360-FacStaff-SG")
            }, "mock");

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            var result = await controller.GetAllCourses("testuser");
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCourses = Assert.IsAssignableFrom<IEnumerable<CoursesBySessionViewModel>>(okResult.Value);
            Assert.Equal(expectedCourses, returnedCourses);
            mockService.Verify(s => s.GetAllCoursesAsync("testuser"), Times.Once);
            mockService.Verify(s => s.GetAllInstructorCoursesAsync("testuser"), Times.Never);
        }

        [Fact]
        public async Task GetAllCourses_WhenAuthenticatedUserEqualsTargetUser_SeesAllCourses()
        {
            var mockService = new Mock<IScheduleService>();
            var expectedCourses = GetExpectedCourses();
            mockService.Setup(s => s.GetAllCoursesAsync("testuser")).ReturnsAsync(expectedCourses);

            var controller = new ScheduleController(mockService.Object);
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Upn, "testuser@gordon.edu")
            }, "mock");

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };

            var result = await controller.GetAllCourses("testuser");
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCourses = Assert.IsAssignableFrom<IEnumerable<CoursesBySessionViewModel>>(okResult.Value);
            Assert.Equal(expectedCourses, returnedCourses);
            mockService.Verify(s => s.GetAllCoursesAsync("testuser"), Times.Once);
            mockService.Verify(s => s.GetAllInstructorCoursesAsync("testuser"), Times.Never);
        }
    }
}