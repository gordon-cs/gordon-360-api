using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using RecIMController = Gordon360.Controllers.RecIM.AffiliationsController;
using RecIMService = Gordon360.Services.RecIM.IAffiliationService;
using Gordon360.Models.ViewModels.RecIM;
using System.Collections.Generic;

namespace Gordon360.Tests.Controllers_Test
{
    /// <summary>
    /// Tests for the RecIM AffiliationsController
    /// </summary>
    public class RecIM_AffiliationsControllerTest
    {
        private readonly Mock<RecIMService> _mockService;
        private readonly RecIMController _controller;

        public RecIM_AffiliationsControllerTest()
        {
            _mockService = new Mock<RecIMService>();
            _controller = new RecIMController(_mockService.Object);
        }

        private AffiliationExtendedViewModel GetSampleAffiliation() => new AffiliationExtendedViewModel
        {
            Name = "Student",
            Points = 10,
            Series = new List<SeriesViewModel>()
        };

        [Fact]
        public void GetAllAffiliationDetails_ReturnsOkWithAffiliations()
        {
            var affiliations = new List<AffiliationExtendedViewModel> { GetSampleAffiliation() };
            _mockService.Setup(s => s.GetAllAffiliationDetails()).Returns(affiliations);
            var result = _controller.GetAllAffiliationDetails();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(affiliations, okResult.Value);
        }

        // Add more tests for all endpoints and error cases
    }
} 