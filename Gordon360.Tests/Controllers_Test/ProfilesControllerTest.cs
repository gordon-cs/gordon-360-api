/*using Xunit;
using Moq;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Gordon360.Controllers;
using Gordon360.Services;
using Gordon360.Models.ViewModels;
using Gordon360.Enums;
using Microsoft.Extensions.Configuration;

namespace Gordon360.Tests.Controllers_Test
{
    public class ProfilesControllerTests
    {
        private ProfilesController CreateControllerWithRoles(string[] roleStrings, out Mock<ProfileService> profileServiceMock)
        {
            profileServiceMock = new Mock<ProfileService> { CallBase = true }; // Use real ComposeProfile logic

            var accountServiceMock = new Mock<IAccountService>();
            var membershipServiceMock = new Mock<IMembershipService>();
            var configMock = new Mock<IConfiguration>();

            var claims = new List<Claim>();
            foreach (var role in roleStrings)
            {
                claims.Add(new Claim("groups", role));
            }

            var identity = new ClaimsIdentity(claims, "mock");
            var user = new ClaimsPrincipal(identity);

            var controller = new ProfilesController(
                profileServiceMock.Object,
                accountServiceMock.Object,
                membershipServiceMock.Object,
                configMock.Object
            );

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return controller;
        }

        [Fact]
        public void Student_ShouldNotSee_PrivateFacultyFields()
        {
            var controller = CreateControllerWithRoles(new[] { "Student" }, out var profileServiceMock);
            string username = "facultyuser";

            var faculty = new FacultyStaffProfileViewModel(
                "1", "Dr.", "Jane", "A", "Doe", "", "", "", "Math", "Jenks", "102", "1234", "5678", "9999",
                "123 Elm St", "", "Boston", "MA", "02115", "USA", "555-0000", "", "123 Elm St", "", "Boston", "MA", "02115", "USA", "555-0000", "",
                "2021", "Senior", "1", "B123", "", "", "No", "Math", "", "", "", "", "", "jane.doe@gordon.edu", "F", "No", "2025", "2025", "555-1234", true,
                "jdoe", 1, 1, "USA", "Jenks Hall", "Math", "", "", "", "", "", "Box 10", 30, 28
            );

            var student = new Mock<StudentProfileViewModel>().Object;
            var alumni = new Mock<AlumniProfileViewModel>().Object;
            var custom = new Mock<CustomProfileViewModel>().Object;

            profileServiceMock.Setup(p => p.GetFacultyStaffProfileByUsername(username)).Returns(faculty);
            profileServiceMock.Setup(p => p.GetStudentProfileByUsername(username)).Returns(student);
            profileServiceMock.Setup(p => p.GetAlumniProfileByUsername(username)).Returns(alumni);
            profileServiceMock.Setup(p => p.GetCustomUserInfo(username)).Returns(custom);

            var result = controller.GetUserProfile(username);
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var profile = Assert.IsType<ProfileViewModel>(ok.Value);

            // Assert redaction logic
            Assert.Equal("Private as requested.", profile.HomeCity);
            Assert.Equal("", profile.HomePhone);
            Assert.Equal("Private as requested.", profile.SpouseName);
        }
    }
}
*/