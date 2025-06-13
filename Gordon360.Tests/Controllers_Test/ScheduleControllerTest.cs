namespace Gordon360.Tests.Controllers_Test
{
    public class ScheduleControllerTests
    {
        [Fact]
        public async Task GetAllCourses_WhenStudentTriesToAccessAnotherStudentSchedule_CallsGetAllInstructorCoursesAsync()
        {
            // Arrange
            var mockService = new Mock<IScheduleService>();
            var expectedCourses = new List<CoursesBySessionViewModel>
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

            mockService.Setup(s => s.GetAllInstructorCoursesAsync("student2"))
                       .ReturnsAsync(expectedCourses);

            mockService.Setup(s => s.GetAllCoursesAsync("student2"))
                       .ReturnsAsync(expectedCourses);

            var controller = new ScheduleController(mockService.Object);

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "student1"),
                new Claim(ClaimTypes.Upn, "student1@gordon.edu"),
                new Claim("groups", "Student")
            }, "mock");

            var fakeUser = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = fakeUser
                }
            };

            // Act
            var result = await controller.GetAllCourses("student2");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCourses = Assert.IsAssignableFrom<IEnumerable<CoursesBySessionViewModel>>(okResult.Value);
            Assert.Equal(expectedCourses, returnedCourses);

            mockService.Verify(s => s.GetAllCoursesAsync(It.IsAny<string>()), Times.Never);
            mockService.Verify(s => s.GetAllInstructorCoursesAsync("student2"), Times.Once);
        }
    }
}