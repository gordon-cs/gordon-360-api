using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using RecIMController = Gordon360.Controllers.RecIM.TeamsController;
using RecIMService = Gordon360.Services.RecIM.ITeamService;
using RecIMActivityService = Gordon360.Services.RecIM.IActivityService;
using RecIMParticipantService = Gordon360.Services.RecIM.IParticipantService;
using Gordon360.Models.ViewModels.RecIM;
using System.Collections.Generic;

namespace Gordon360.Tests.Controllers_Test
{
    /// <summary>
    /// Tests for the RecIM TeamsController
    /// </summary>
    public class RecIM_TeamsControllerTest
    {
        private readonly Mock<RecIMService> _mockService;
        private readonly Mock<RecIMActivityService> _mockActivityService;
        private readonly Mock<RecIMParticipantService> _mockParticipantService;
        private readonly RecIMController _controller;

        public RecIM_TeamsControllerTest()
        {
            _mockService = new Mock<RecIMService>();
            _mockActivityService = new Mock<RecIMActivityService>();
            _mockParticipantService = new Mock<RecIMParticipantService>();
            _controller = new RecIMController(_mockService.Object, _mockActivityService.Object, _mockParticipantService.Object);
        }

        private TeamExtendedViewModel GetSampleTeam() => new TeamExtendedViewModel
        {
            ID = 1,
            Name = "Team A"
        };

        [Fact]
        public void GetTeams_ReturnsOkWithTeams()
        {
            var teams = new List<TeamExtendedViewModel> { GetSampleTeam() };
            _mockService.Setup(s => s.GetTeams(true)).Returns(teams);
            var result = _controller.GetTeams(true);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(teams, okResult.Value);
        }

        // Add more tests for all endpoints and error cases
    }
} 