using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using RecIMController = Gordon360.Controllers.RecIM.SeriesController;
using RecIMService = Gordon360.Services.RecIM.ISeriesService;
using Gordon360.Models.ViewModels.RecIM;
using System.Collections.Generic;

namespace Gordon360.Tests.Controllers_Test
{
    /// <summary>
    /// Tests for the RecIM SeriesController
    /// </summary>
    public class RecIM_SeriesControllerTest
    {
        private readonly Mock<RecIMService> _mockService;
        private readonly RecIMController _controller;

        public RecIM_SeriesControllerTest()
        {
            _mockService = new Mock<RecIMService>();
            _controller = new RecIMController(_mockService.Object);
        }

        private SeriesExtendedViewModel GetSampleSeries() => new SeriesExtendedViewModel
        {
            ID = 1,
            Name = "Fall Series"
        };

        [Fact]
        public void GetSeries_ReturnsOkWithSeries()
        {
            var series = new List<SeriesExtendedViewModel> { GetSampleSeries() };
            _mockService.Setup(s => s.GetSeries(true)).Returns(series);
            var result = _controller.GetSeries(true);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(series, okResult.Value);
        }

        // Add more tests for all endpoints and error cases
    }
} 