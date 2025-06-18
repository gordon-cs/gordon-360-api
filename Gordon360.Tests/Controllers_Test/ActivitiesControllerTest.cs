using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Gordon360.Controllers.RecIM;
using Gordon360.Services.RecIM;
using Gordon360.Models.ViewModels.RecIM;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Gordon360.Tests.Controllers_Test
{
    public class ActivitiesControllerTest
    {
        private readonly Mock<IActivityService> _mockService;
        private readonly ActivitiesController _controller;

        public ActivitiesControllerTest()
        {
            _mockService = new Mock<IActivityService>();
            _controller = new ActivitiesController(_mockService.Object);
        }

        private ActivityExtendedViewModel GetSampleActivityExtended()
        {
            return new ActivityExtendedViewModel
            {
                ID = 1,
                Name = "Soccer League",
                RegistrationStart = new DateTime(2024, 1, 1),
                RegistrationEnd = new DateTime(2024, 2, 1),
                RegistrationOpen = true,
                Sport = new SportViewModel
                {
                    ID = 10,
                    Name = "Soccer",
                    Description = "Soccer sport",
                    Rules = "Standard rules",
                    Logo = "soccer.png"
                },
                Status = "Active",
                MinCapacity = 5,
                MaxCapacity = 11,
                SoloRegistration = false,
                Logo = "league.png",
                Completed = false,
                Type = "League",
                StartDate = new DateTime(2024, 3, 1),
                EndDate = new DateTime(2024, 5, 1),
                SeriesScheduleID = 100,
                Series = new List<SeriesExtendedViewModel>(),
                Team = new List<TeamExtendedViewModel>()
            };
        }

        private LookupViewModel GetSampleLookup() => new LookupViewModel { ID = 1, Description = "Sample Type" };

        private ActivityViewModel GetSampleActivityViewModel() => new ActivityViewModel
        {
            ID = 2,
            Name = "Basketball League",
            RegistrationStart = new DateTime(2024, 1, 10),
            RegistrationEnd = new DateTime(2024, 2, 10),
            SportID = 20,
            StatusID = 1,
            MinCapacity = 3,
            MaxCapacity = 5,
            SoloRegistration = true,
            Logo = "basketball.png",
            Completed = false,
            TypeID = 2,
            StartDate = new DateTime(2024, 3, 10),
            EndDate = new DateTime(2024, 4, 10),
            SeriesScheduleID = 200
        };

        private ActivityUploadViewModel GetSampleActivityUpload() => new ActivityUploadViewModel
        {
            Name = "Basketball League",
            RegistrationStart = new DateTime(2024, 1, 10),
            RegistrationEnd = new DateTime(2024, 2, 10),
            SportID = 20,
            MinCapacity = 3,
            MaxCapacity = 5,
            SoloRegistration = true,
            Logo = "basketball.png",
            TypeID = 2,
            StartDate = new DateTime(2024, 3, 10),
            EndDate = new DateTime(2024, 4, 10),
            SeriesScheduleID = 200
        };

        private ActivityPatchViewModel GetSampleActivityPatch() => new ActivityPatchViewModel
        {
            Name = "Updated League",
            RegistrationStart = new DateTime(2024, 1, 15),
            RegistrationEnd = new DateTime(2024, 2, 15),
            SportID = 21,
            StatusID = 2,
            MinCapacity = 4,
            MaxCapacity = 6,
            SoloRegistration = false,
            Completed = true,
            TypeID = 3,
            StartDate = new DateTime(2024, 3, 15),
            EndDate = new DateTime(2024, 4, 15),
            SeriesScheduleID = 201,
            WinnerID = 5,
            Points = 10
        };

        [Fact]
        public void GetActivities_ReturnsAll_WhenNoActiveParam()
        {
            var activities = new List<ActivityExtendedViewModel> { GetSampleActivityExtended() };
            _mockService.Setup(s => s.GetActivities()).Returns(activities);

            var result = _controller.GetActivities(null);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(activities, okResult.Value);
        }

        [Fact]
        public void GetActivities_ReturnsFiltered_WhenActiveParamProvided()
        {
            var activities = new List<ActivityExtendedViewModel> { GetSampleActivityExtended() };
            _mockService.Setup(s => s.GetActivitiesByCompletion(false)).Returns(activities);

            var result = _controller.GetActivities(true);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(activities, okResult.Value);
        }

        [Fact]
        public void GetActivityByID_ReturnsActivity()
        {
            var activity = GetSampleActivityExtended();
            _mockService.Setup(s => s.GetActivityByID(1)).Returns(activity);

            var result = _controller.GetActivityByID(1);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(activity, okResult.Value);
        }

        [Fact]
        public void GetActivityRegistrationStatus_ReturnsStatus()
        {
            _mockService.Setup(s => s.ActivityRegistrationClosed(1)).Returns(false);
            var result = _controller.GetActivityRegistrationStatus(1);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public void GetActivityTypes_ReturnsOk_WhenFound()
        {
            var lookups = new List<LookupViewModel> { GetSampleLookup() };
            _mockService.Setup(s => s.GetActivityLookup("type")).Returns(lookups);
            var result = _controller.GetActivityTypes("type");
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(lookups, okResult.Value);
        }

        [Fact]
        public void GetActivityTypes_ReturnsNotFound_WhenNull()
        {
            _mockService.Setup(s => s.GetActivityLookup("type")).Returns((IEnumerable<LookupViewModel>)null);
            var result = _controller.GetActivityTypes("type");
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateActivityAsync_ReturnsCreated()
        {
            var upload = GetSampleActivityUpload();
            var created = GetSampleActivityViewModel();
            _mockService.Setup(s => s.PostActivityAsync(upload)).ReturnsAsync(created);
            var result = await _controller.CreateActivityAsync(upload);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(created, createdResult.Value);
        }

        [Fact]
        public async Task UpdateActivityAsync_ReturnsCreated()
        {
            var patch = GetSampleActivityPatch();
            var updated = GetSampleActivityViewModel();
            updated.ID = 3;
            _mockService.Setup(s => s.UpdateActivityAsync(3, patch)).ReturnsAsync(updated);
            var result = await _controller.UpdateActivityAsync(3, patch);
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(updated, createdResult.Value);
        }

        [Fact]
        public async Task DeleteActivityCascadeAsync_ReturnsOk()
        {
            _mockService.Setup(s => s.DeleteActivityCascade(4)).ReturnsAsync(true);
            var result = await _controller.DeleteActivityCascadeAsync(4);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value);
        }
    }
} 