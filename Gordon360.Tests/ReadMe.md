# ✅ Writing Controller Unit Tests in xUnit with Moq (C# ASP.NET Core)

This guide walks through the key steps to write a proper unit test for ASP.NET Core controllers using **xUnit** and **Moq**, followed by a demo test case for `ScheduleController`.

---

## 🧩 Step 1: Setup Moq for Controller Dependencies

You need to mock all the services your controller depends on. For example:

```csharp
var mockService = new Mock<IProfileService>();
var controller = new ProfilesController(mockService.Object, null, null, null);
```

---

## 🔐 Step 2: Setup Authentication

Simulate an authenticated user by adding a mock identity to the controller’s `HttpContext`:

```csharp
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
```

---

## ⚙️ Step 3: Setup Mocked Methods

Define what the mocked services should return when their methods are called:

```csharp
mockService.Setup(s => s.GetAllInstructorCoursesAsync("testuser"))
           .ReturnsAsync(expectedCourses);
```

---

## 🧪 Example Test Case: ScheduleController

```csharp
using Xunit;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Gordon360.Controllers;
using Gordon360.Services;
using Gordon360.Models.ViewModels;
using Xunit.Abstractions;
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
            mockService.Setup(s => s.GetAllInstructorCoursesAsync("testuser"))
                       .ReturnsAsync(expectedCourses);

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
    }
}
```

---


