using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Gordon360.Controllers;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using System.Collections.Generic;

namespace Gordon360.Tests.Controllers_Test
{
    public class SessionsControllerTest
    {
        private readonly Mock<ISessionService> _mockService;
        private readonly SessionsController _controller;

        public SessionsControllerTest()
        {
            _mockService = new Mock<ISessionService>();
            _controller = new SessionsController(_mockService.Object);
        }

        private SessionViewModel GetSampleSession() => new SessionViewModel
        {
            SessionCode = "202401",
            SessionDescription = "Spring 2024",
            SessionBeginDate = new System.DateTime(2024, 1, 10),
            SessionEndDate = new System.DateTime(2024, 5, 10)
        };

        [Fact]
        public void GetAll_ReturnsOkWithSessions()
        {
            var sessions = new List<SessionViewModel> { GetSampleSession() };
            _mockService.Setup(s => s.GetAll()).Returns(sessions);

            var result = _controller.Get();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(sessions, okResult.Value);
        }

        [Fact]
        public void GetById_ReturnsOkWithSession()
        {
            var session = GetSampleSession();
            _mockService.Setup(s => s.Get("202401")).Returns(session);

            var result = _controller.Get("202401");
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(session, okResult.Value);
        }

        [Fact]
        public void GetById_ReturnsNotFoundIfNull()
        {
            _mockService.Setup(s => s.Get("202402")).Returns((SessionViewModel)null);

            var result = _controller.Get("202402");
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetCurrentSession_ReturnsOkWithSession()
        {
            var session = GetSampleSession();
            _mockService.Setup(s => s.GetCurrentSession()).Returns(session);

            var result = _controller.GetCurrentSession();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(session, okResult.Value);
        }

        [Fact]
        public void GetCurrentSession_ReturnsNotFoundIfNull()
        {
            _mockService.Setup(s => s.GetCurrentSession()).Returns((SessionViewModel)null);

            var result = _controller.GetCurrentSession();
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetDaysLeftInSemester_ReturnsOkWithDays()
        {
            var days = new double[] { 10, 5, 0 };
            _mockService.Setup(s => s.GetDaysLeft()).Returns(days);

            var result = _controller.GetDaysLeftInSemester();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(days, okResult.Value);
        }

        [Fact]
        public void GetDaysLeftInSemester_ReturnsNotFoundIfNullOrZero()
        {
            _mockService.Setup(s => s.GetDaysLeft()).Returns(new double[] { 0, 0, 0 });

            var result = _controller.GetDaysLeftInSemester();
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
} 