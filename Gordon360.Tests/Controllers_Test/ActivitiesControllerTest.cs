using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using RecIMController = Gordon360.Controllers.RecIM.ActivitiesController;
using RecIMService = Gordon360.Services.RecIM.IActivityService;
using Gordon360.Models.ViewModels.RecIM;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Gordon360.Tests.Controllers_Test
{
    /// <summary>
    /// Tests for the RecIM ActivitiesController
    /// </summary>
    public class RecIM_ActivitiesControllerTest
    {
        private readonly Mock<RecIMService> _mockService;
        private readonly RecIMController _controller;

        public RecIM_ActivitiesControllerTest()
        {
            _mockService = new Mock<RecIMService>();
            _controller = new RecIMController(_mockService.Object);
        }

        private ActivityExtendedViewModel GetSampleActivity() => new ActivityExtendedViewModel
        {
            ID = 1,
            Name = "Sample Activity",
            Sport = new SportViewModel { ID = 1, Name = "Soccer" },
            Status = "Active",
            Logo = "logo.png",
            Type = "League",
            Series = new List<SeriesExtendedViewModel>(),
            Team = new List<TeamExtendedViewModel>(),
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(10)
        };

        [Fact]
        public void GetActivities_ReturnsOkWithActivities()
        {
            var activities = new List<ActivityExtendedViewModel> { GetSampleActivity() };
            _mockService.Setup(s => s.GetActivities()).Returns(activities);
            var result = _controller.GetActivities(null);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(activities, okResult.Value);
        }

        // Add more tests for all endpoints, including POST, PATCH, DELETE, and error cases
    }
} 