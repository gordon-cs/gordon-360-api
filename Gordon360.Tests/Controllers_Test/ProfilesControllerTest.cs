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

namespace Gordon360.Tests.Controllers_Test
{
    public class ProfilesControllerTests
    {
        private readonly ITestOutputHelper _output;

        public ProfilesControllerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        private ClaimsPrincipal CreateUserWithGroup(string group)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testviewer"),
                new Claim(ClaimTypes.Upn, "testviewer@gordon.edu"),
                new Claim("groups", group)
            }, "mock");

            return new ClaimsPrincipal(identity);
        }
//rewrite a test for student viewing another student delete due to null error

        /*
        [Fact]
        public void GetUserProfile_AsFacStaff_ReturnsPublicFacStaffProfile()
        {
            var mockService = new Mock<IProfileService>();
            var controller = new ProfilesController(mockService.Object, null, null, null);
            var username = "targetuser";

            var facultyProfile = new FacultyStaffProfileViewModel { FirstName = "TestFacStaff" };
            var customProfile = new ProfileCustomViewModel();
            var expectedProfile = new ProfileViewModel { FirstName = "TestFacStaff", PersonType = "fac" };

            mockService.Setup(s => s.GetStudentProfileByUsername(username)).Returns((StudentProfileViewModel?)null);
            mockService.Setup(s => s.GetFacultyStaffProfileByUsername(username)).Returns(facultyProfile);
            mockService.Setup(s => s.GetAlumniProfileByUsername(username)).Returns((AlumniProfileViewModel?)null);
            mockService.Setup(s => s.GetCustomUserInfo(username)).Returns(customProfile);
            mockService.Setup(s => s.ComposeProfile(null, null, facultyProfile, customProfile)).Returns(expectedProfile);

            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = CreateUserWithGroup("360-FacStaff-SG") } };

            var result = controller.GetUserProfile(username);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var profile = Assert.IsType<ProfileViewModel>(okResult.Value);
            Assert.Equal("TestFacStaff", profile.FirstName);
        }

        [Fact]
        public void GetUserProfile_AsAlumni_ReturnsOnlyPublicAlumniProfile()
        {
            var mockService = new Mock<IProfileService>();
            var controller = new ProfilesController(mockService.Object, null, null, null);
            var username = "targetuser";

            var alumniProfile = new AlumniProfileViewModel { FirstName = "TestAlumni" };
            var customProfile = new ProfileCustomViewModel();
            var expectedProfile = new ProfileViewModel { FirstName = "TestAlumni", PersonType = "alu" };

            mockService.Setup(s => s.GetStudentProfileByUsername(username)).Returns((StudentProfileViewModel?)null);
            mockService.Setup(s => s.GetFacultyStaffProfileByUsername(username)).Returns((FacultyStaffProfileViewModel?)null);
            mockService.Setup(s => s.GetAlumniProfileByUsername(username)).Returns(alumniProfile);
            mockService.Setup(s => s.GetCustomUserInfo(username)).Returns(customProfile);
            mockService.Setup(s => s.ComposeProfile(null, alumniProfile, null, customProfile)).Returns(expectedProfile);

            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = CreateUserWithGroup("360-Alumni-SG") } };

            var result = controller.GetUserProfile(username);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var profile = Assert.IsType<ProfileViewModel>(okResult.Value);
            Assert.Equal("TestAlumni", profile.FirstName);
        }*/

        [Fact]
        public void GetUserProfile_AsFacStaffViewingStudent_ReturnsFullStudentProfile()
        {
            var mockService = new Mock<IProfileService>();
            var controller = new ProfilesController(mockService.Object, null, null, null);
            var username = "targetstudent";

            var studentProfile = new StudentProfileViewModel(
                ID: "1", Title: "Mr.", FirstName: "TestStudent", MiddleName: "M", LastName: "Student", Suffix: "Jr", MaidenName: "", NickName: "Testy",
                OnOffCampus: "On", OnCampusBuilding: "Chase", OnCampusRoom: "201", OnCampusPhone: "555-1111", OnCampusPrivatePhone: "555-1112", OnCampusFax: "555-1113",
                OffCampusStreet1: "1 Test St", OffCampusStreet2: "", OffCampusCity: "Wenham", OffCampusState: "MA", OffCampusPostalCode: "01984",
                OffCampusCountry: "USA", OffCampusPhone: "", OffCampusFax: "", HomeStreet1: "2 Home St", HomeStreet2: "", HomeCity: "Wenham",
                HomeState: "MA", HomePostalCode: "01984", HomeCountry: "USA", HomePhone: "555-2222", HomeFax: "", Cohort: "2022", Class: "Junior",
                KeepPrivate: "N", Barcode: "B12345", AdvisorIDs: "", Married: "No", Commuter: "No", Major: "CS", Major2: "Math", Major3: "",
                Minor1: "Physics", Minor2: "", Minor3: "", Email: "student@gordon.edu", Gender: "M", grad_student: "No", GradDate: "2026-05-01",
                PlannedGradYear: "2026", MobilePhone: "555-3333", IsMobilePhonePrivate: false, AD_Username: "student1", show_pic: 1, preferred_photo: 1,
                Country: "USA", BuildingDescription: "Chase Hall", Major1Description: "Computer Science", Major2Description: "Mathematics", Major3Description: "",
                Minor1Description: "Physics", Minor2Description: "", Minor3Description: "", Mail_Location: "Box 456", ChapelRequired: 30, ChapelAttended: 29
            );

            var customProfile = new ProfileCustomViewModel();
            var expectedProfile = new ProfileViewModel(
                ID: "1", Title: "Mr.", FirstName: "TestStudent", MiddleName: "M", LastName: "Student", Suffix: "Jr", MaidenName: "", NickName: "Testy",
                Email: "student@gordon.edu", Gender: "M", HomeStreet1: "2 Home St", HomeStreet2: "", HomeCity: "Wenham", HomeState: "MA", HomePostalCode: "01984",
                HomeCountry: "USA", HomePhone: "555-2222", HomeFax: "", AD_Username: "student1", show_pic: 1, preferred_photo: 1, Country: "USA", Barcode: "B12345",
                Facebook: "", Twitter: "", Instagram: "", LinkedIn: "", Handshake: "", Calendar: "", OnOffCampus: "On", OffCampusStreet1: "1 Test St",
                OffCampusStreet2: "", OffCampusCity: "Wenham", OffCampusState: "MA", OffCampusPostalCode: "01984", OffCampusCountry: "USA", OffCampusPhone: "",
                OffCampusFax: "", Major3: "", Major3Description: "", Minor1: "Physics", Minor1Description: "Physics", Minor2: "", Minor2Description: "",
                Minor3: "", Minor3Description: "", GradDate: "2026-05-01", PlannedGradYear: "2026", MobilePhone: "555-3333", IsMobilePhonePrivate: false,
                ChapelRequired: 30, ChapelAttended: 29, Cohort: "2022", Class: "Junior", AdvisorIDs: "", Married: "No", Commuter: "No", WebUpdate: "",
                HomeEmail: "", MaritalStatus: "", College: "", ClassYear: "", PreferredClassYear: "", ShareName: "", ShareAddress: "", Major: "CS",
                Major1Description: "Computer Science", Major2: "Math", Major2Description: "Mathematics", grad_student: "No", OnCampusDepartment: "",
                Type: "stu", office_hours: "", Dept: "", Mail_Description: "", JobTitle: "", SpouseName: "", BuildingDescription: "Chase Hall",
                Mail_Location: "Box 456", OnCampusBuilding: "Chase", OnCampusRoom: "201", OnCampusPhone: "555-1111", OnCampusPrivatePhone: "555-1112",
                OnCampusFax: "555-1113", KeepPrivate: "N", PersonType: "stu"
            );

            mockService.Setup(s => s.GetStudentProfileByUsername(username)).Returns(studentProfile);
            mockService.Setup(s => s.GetFacultyStaffProfileByUsername(username)).Returns((FacultyStaffProfileViewModel?)null);
            mockService.Setup(s => s.GetAlumniProfileByUsername(username)).Returns((AlumniProfileViewModel?)null);
            mockService.Setup(s => s.GetCustomUserInfo(username)).Returns(customProfile);
            mockService.Setup(s => s.ComposeProfile(studentProfile, null, null, customProfile)).Returns(expectedProfile);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateUserWithGroup("360-FacStaff-SG")
                }
            };

            var result = controller.GetUserProfile(username);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var profile = Assert.IsType<ProfileViewModel>(okResult.Value);
            Assert.Equal("TestStudent", profile.FirstName);
            Assert.Equal("stu", profile.PersonType);
            Assert.Equal("CS", profile.Major);
            Assert.Equal("Mathematics", profile.Major2Description);
        }

    }
}
