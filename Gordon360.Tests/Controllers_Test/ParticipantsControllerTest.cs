using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using RecIMController = Gordon360.Controllers.RecIM.ParticipantsController;
using RecIMService = Gordon360.Services.RecIM.IParticipantService;
using RecIMAccountService = Gordon360.Services.IAccountService;
using Gordon360.Models.ViewModels.RecIM;
using System.Collections.Generic;

namespace Gordon360.Tests.Controllers_Test
{
    /// <summary>
    /// Tests for the RecIM ParticipantsController
    /// </summary>
    public class RecIM_ParticipantsControllerTest
    {
        private readonly Mock<RecIMService> _mockService;
        private readonly Mock<RecIMAccountService> _mockAccountService;
        private readonly RecIMController _controller;

        public RecIM_ParticipantsControllerTest()
        {
            _mockService = new Mock<RecIMService>();
            _mockAccountService = new Mock<RecIMAccountService>();
            _controller = new RecIMController(_mockService.Object, _mockAccountService.Object);
        }

        private ParticipantExtendedViewModel GetSampleParticipant() => new ParticipantExtendedViewModel
        {
            Username = "jdoe",
            Email = "jdoe@gordon.edu",
            Role = "Player",
            GamesAttended = 5,
            Status = "Active",
            IsAdmin = false,
            AllowEmails = true,
            IsCustom = false,
            FirstName = "John",
            LastName = "Doe",
            Hall = "Chase"
        };

        [Fact]
        public void GetParticipants_ReturnsOkWithParticipants()
        {
            var participants = new List<ParticipantExtendedViewModel> { GetSampleParticipant() };
            _mockService.Setup(s => s.GetParticipants()).Returns(participants);
            var result = _controller.GetParticipants();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(participants, okResult.Value);
        }

        // Add more tests for all endpoints and error cases
    }
} 