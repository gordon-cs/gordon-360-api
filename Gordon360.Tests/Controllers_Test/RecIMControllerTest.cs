using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using RecIMController = Gordon360.Controllers.RecIM.RecIMAdminController;
using RecIMService = Gordon360.Services.RecIM.IRecIMService;
using Gordon360.Models.ViewModels.RecIM;
using System;

namespace Gordon360.Tests.Controllers_Test
{
    /// <summary>
    /// Tests for the RecIM RecIMAdminController
    /// </summary>
    public class RecIM_RecIMAdminControllerTest
    {
        private readonly Mock<RecIMService> _mockService;
        private readonly RecIMController _controller;

        public RecIM_RecIMAdminControllerTest()
        {
            _mockService = new Mock<RecIMService>();
            _controller = new RecIMController(_mockService.Object);
        }

        [Fact]
        public void GetReport_ReturnsOkWithReport()
        {
            var start = DateTime.Now.AddMonths(-1);
            var end = DateTime.Now;
            var report = new RecIMGeneralReportViewModel();
            _mockService.Setup(s => s.GetReport(start, end)).Returns(report);
            var result = _controller.GetReport(start, end);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(report, okResult.Value);
        }

        // Add more tests for all endpoints and error cases
    }
} 