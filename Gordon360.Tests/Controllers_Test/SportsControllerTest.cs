using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using RecIMController = Gordon360.Controllers.RecIM.SportsController;
using RecIMService = Gordon360.Services.RecIM.ISportService;
using Gordon360.Models.ViewModels.RecIM;
using System.Collections.Generic;

namespace Gordon360.Tests.Controllers_Test
{
    /// <summary>
    /// Tests for the RecIM SportsController
    /// </summary>
    public class RecIM_SportsControllerTest
    {
        private readonly Mock<RecIMService> _mockService;
        private readonly RecIMController _controller;

        public RecIM_SportsControllerTest()
        {
            _mockService = new Mock<RecIMService>();
            _controller = new RecIMController(_mockService.Object);
        }

        private SportViewModel GetSampleSport() => new SportViewModel
        {
            ID = 1,
            Name = "Soccer"
        };

        [Fact]
        public void GetSports_ReturnsOkWithSports()
        {
            var sports = new List<SportViewModel> { GetSampleSport() };
            _mockService.Setup(s => s.GetSports()).Returns(sports);
            var result = _controller.GetSports();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(sports, okResult.Value);
        }

        // Add more tests for all endpoints and error cases
    }
} 