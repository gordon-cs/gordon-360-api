using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Gordon360.Controllers;
using Gordon360.Services;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Tests.Controllers_Test
{
    public class EventsControllerTest
    {
        private readonly Mock<IEventService> _mockService;
        private readonly EventsController _controller;

        public EventsControllerTest()
        {
            _mockService = new Mock<IEventService>();
            _controller = new EventsController(_mockService.Object);
        }

        private EventViewModel GetSampleEventViewModel() => new EventViewModel
        {
            Event_ID = "E1",
            Event_Name = "Sample Event",
            Event_Title = "Sample Title",
            Event_Type_Name = "Type1",
            HasCLAWCredit = true,
            IsPublic = true,
            Description = "Sample Description",
            StartDate = "2024-01-01T10:00:00",
            EndDate = "2024-01-01T12:00:00",
            Location = "Chapel",
            Organization = "Org1"
        };

        private AttendedEventViewModel GetSampleAttendedEventViewModel() => new AttendedEventViewModel
        {
            LiveID = "L1",
            CHDate = System.DateTime.Now,
            CHTermCD = "202401",
            Required = 1,
            Event_Name = "Attended Event",
            Event_Title = "Attended Title",
            Description = "Attended Description",
            Organization = "Org2",
            StartDate = "2024-02-01T10:00:00",
            EndDate = "2024-02-01T12:00:00",
            Location = "Auditorium"
        };

        [Fact]
        public void GetEventsByTerm_ReturnsOkWithEvents()
        {
            var attendedEvents = new List<AttendedEventViewModel> { GetSampleAttendedEventViewModel() };
            _mockService.Setup(s => s.GetEventsForStudentByTerm(It.IsAny<string>(), "202401")).Returns(attendedEvents);

            var result = _controller.GetEventsByTerm("202401");
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(attendedEvents, okResult.Value);
        }

        [Fact]
        public void GetEventsByTerm_ReturnsNotFoundWhenNull()
        {
            _mockService.Setup(s => s.GetEventsForStudentByTerm(It.IsAny<string>(), "202401")).Returns((List<AttendedEventViewModel>)null);

            var result = _controller.GetEventsByTerm("202401");
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetEventsByTerm_ReturnsOkWithEmptyList()
        {
            var attendedEvents = new List<AttendedEventViewModel>();
            _mockService.Setup(s => s.GetEventsForStudentByTerm(It.IsAny<string>(), "202401")).Returns(attendedEvents);

            var result = _controller.GetEventsByTerm("202401");
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Empty((IEnumerable<AttendedEventViewModel>)okResult.Value);
        }

        [Fact]
        public void GetEventsByTerm_ReturnsOkWithMultipleItems()
        {
            var attendedEvents = new List<AttendedEventViewModel> { GetSampleAttendedEventViewModel(), GetSampleAttendedEventViewModel() };
            _mockService.Setup(s => s.GetEventsForStudentByTerm(It.IsAny<string>(), "202401")).Returns(attendedEvents);

            var result = _controller.GetEventsByTerm("202401");
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(2, ((IEnumerable<AttendedEventViewModel>)okResult.Value).Count());
        }

        [Fact]
        public void GetAllEvents_ReturnsOkWithEvents()
        {
            var events = new List<EventViewModel> { GetSampleEventViewModel() };
            _mockService.Setup(s => s.GetAllEvents()).Returns(events);

            var result = _controller.GetAllEvents();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(events, okResult.Value);
        }

        [Fact]
        public void GetAllEvents_ReturnsNotFoundWhenNull()
        {
            _mockService.Setup(s => s.GetAllEvents()).Returns((List<EventViewModel>)null);

            var result = _controller.GetAllEvents();
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetAllEvents_ReturnsOkWithEmptyList()
        {
            var events = new List<EventViewModel>();
            _mockService.Setup(s => s.GetAllEvents()).Returns(events);

            var result = _controller.GetAllEvents();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Empty((IEnumerable<EventViewModel>)okResult.Value);
        }

        [Fact]
        public void GetAllEvents_ReturnsOkWithMultipleItems()
        {
            var events = new List<EventViewModel> { GetSampleEventViewModel(), GetSampleEventViewModel() };
            _mockService.Setup(s => s.GetAllEvents()).Returns(events);

            var result = _controller.GetAllEvents();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(2, ((IEnumerable<EventViewModel>)okResult.Value).Count());
        }

        [Fact]
        public void GetAllChapelEvents_ReturnsOkWithEvents()
        {
            var events = new List<EventViewModel> { GetSampleEventViewModel() };
            _mockService.Setup(s => s.GetCLAWEvents()).Returns(events);

            var result = _controller.GetAllChapelEvents();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(events, okResult.Value);
        }

        [Fact]
        public void GetAllChapelEvents_ReturnsNotFoundWhenNull()
        {
            _mockService.Setup(s => s.GetCLAWEvents()).Returns((List<EventViewModel>)null);

            var result = _controller.GetAllChapelEvents();
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetAllChapelEvents_ReturnsOkWithEmptyList()
        {
            var events = new List<EventViewModel>();
            _mockService.Setup(s => s.GetCLAWEvents()).Returns(events);

            var result = _controller.GetAllChapelEvents();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Empty((IEnumerable<EventViewModel>)okResult.Value);
        }

        [Fact]
        public void GetAllChapelEvents_ReturnsOkWithMultipleItems()
        {
            var events = new List<EventViewModel> { GetSampleEventViewModel(), GetSampleEventViewModel() };
            _mockService.Setup(s => s.GetCLAWEvents()).Returns(events);

            var result = _controller.GetAllChapelEvents();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(2, ((IEnumerable<EventViewModel>)okResult.Value).Count());
        }

        [Fact]
        public void GetAllPublicEvents_ReturnsOkWithEvents()
        {
            var events = new List<EventViewModel> { GetSampleEventViewModel() };
            _mockService.Setup(s => s.GetPublicEvents()).Returns(events);

            var result = _controller.GetAllPublicEvents();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(events, okResult.Value);
        }

        [Fact]
        public void GetAllPublicEvents_ReturnsNotFoundWhenNull()
        {
            _mockService.Setup(s => s.GetPublicEvents()).Returns((List<EventViewModel>)null);

            var result = _controller.GetAllPublicEvents();
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetAllPublicEvents_ReturnsOkWithEmptyList()
        {
            var events = new List<EventViewModel>();
            _mockService.Setup(s => s.GetPublicEvents()).Returns(events);

            var result = _controller.GetAllPublicEvents();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Empty((IEnumerable<EventViewModel>)okResult.Value);
        }

        [Fact]
        public void GetAllPublicEvents_ReturnsOkWithMultipleItems()
        {
            var events = new List<EventViewModel> { GetSampleEventViewModel(), GetSampleEventViewModel() };
            _mockService.Setup(s => s.GetPublicEvents()).Returns(events);

            var result = _controller.GetAllPublicEvents();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(2, ((IEnumerable<EventViewModel>)okResult.Value).Count());
        }

        [Fact]
        public void GetEventsByTerm_UsesAuthenticatedUsername()
        {
            // Arrange: Simulate a user
            var user = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "testuser")
            }, "mock"));
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext() { User = user }
            };

            var attendedEvents = new List<AttendedEventViewModel> { GetSampleAttendedEventViewModel() };
            _mockService.Setup(s => s.GetEventsForStudentByTerm("testuser", "202401")).Returns(attendedEvents);

            // Act
            var result = _controller.GetEventsByTerm("202401");

            // Assert
            _mockService.Verify(s => s.GetEventsForStudentByTerm("testuser", "202401"), Times.Once);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(attendedEvents, okResult.Value);
        }
    }
} 