using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using RecIMController = Gordon360.Controllers.RecIM.MatchesController;
using RecIMService = Gordon360.Services.RecIM.IMatchService;
using RecIMTeamService = Gordon360.Services.RecIM.ITeamService;
using Gordon360.Models.ViewModels.RecIM;
using System.Collections.Generic;

namespace Gordon360.Tests.Controllers_Test
{
    /// <summary>
    /// Tests for the RecIM MatchesController
    /// </summary>
    public class RecIM_MatchesControllerTest
    {
        private readonly Mock<RecIMService> _mockService;
        private readonly Mock<RecIMTeamService> _mockTeamService;
        private readonly RecIMController _controller;

        public RecIM_MatchesControllerTest()
        {
            _mockService = new Mock<RecIMService>();
            _mockTeamService = new Mock<RecIMTeamService>();
            _controller = new RecIMController(_mockService.Object, _mockTeamService.Object);
        }

        private MatchExtendedViewModel GetSampleMatch() => new MatchExtendedViewModel
        {
            ID = 1
            // Add more valid properties as needed
        };

        [Fact]
        public void GetMatches_ReturnsOkWithMatches()
        {
            var matches = new List<MatchExtendedViewModel> { GetSampleMatch() };
            _mockService.Setup(s => s.GetAllMatches()).Returns(matches);
            var result = _controller.GetMatches();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(matches, okResult.Value);
        }

        // Add more tests for all endpoints and error cases
    }
} 