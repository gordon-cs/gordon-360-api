using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Gordon360.Controllers;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Gordon360.Tests.Controllers_Test
{
    public class RequestsControllerTest
    {
        private readonly Mock<IMembershipRequestService> _mockService;
        private readonly RequestsController _controller;

        public RequestsControllerTest()
        {
            _mockService = new Mock<IMembershipRequestService>();
            _controller = new RequestsController(_mockService.Object);
        }

        private RequestView GetSampleRequestView() => new RequestView
        {
            RequestID = 1,
            DateSent = DateTime.Now,
            ActivityCode = "ACT123",
            ActivityDescription = "Sample Activity",
            ActivityImagePath = "/images/sample.png",
            SessionCode = "202401",
            SessionDescription = "Spring 2024",
            Username = "jdoe",
            FirstName = "John",
            LastName = "Doe",
            Participation = "MEM",
            ParticipationDescription = "Member",
            Description = "Request to join",
            Status = "Pending"
        };

        private RequestUploadViewModel GetSampleRequestUpload() => new RequestUploadViewModel
        {
            Activity = "ACT123",
            Session = "202401",
            Username = "jdoe",
            Participation = "MEM",
            DateSent = DateTime.Now,
            CommentText = "Request to join",
            Status = "Pending"
        };

        private void SetMockUser(string username = "jdoe")
        {
            var identity = new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, username),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Upn, username + "@gordon.edu")
            }, "mock");
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext { User = new System.Security.Claims.ClaimsPrincipal(identity) }
            };
        }

        [Fact]
        public void GetAll_ReturnsOkWithRequests()
        {
            var requests = new List<RequestView> { GetSampleRequestView() };
            _mockService.Setup(s => s.GetAll()).Returns(requests);

            var result = _controller.Get();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(requests, okResult.Value);
        }

        [Fact]
        public void GetById_ReturnsOkWithRequest()
        {
            var request = GetSampleRequestView();
            _mockService.Setup(s => s.Get(1)).Returns(request);

            var result = _controller.Get(1);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(request, okResult.Value);
        }

        [Fact]
        public void GetById_ReturnsNotFoundIfNull()
        {
            _mockService.Setup(s => s.Get(2)).Returns((RequestView)null);

            var result = _controller.Get(2);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetMembershipRequestsByActivity_ReturnsOkWithRequests()
        {
            var requests = new List<RequestView> { GetSampleRequestView() };
            _mockService.Setup(s => s.GetMembershipRequests("ACT123", "202401", null)).Returns(requests);

            var result = _controller.GetMembershipRequestsByActivity("ACT123", "202401");
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(requests, okResult.Value);
        }

        [Fact]
        public void GetMembershipsRequestsForCurrentUser_ReturnsOkWithRequests()
        {
            var requests = new List<RequestView> { GetSampleRequestView() };
            _mockService.Setup(s => s.GetMembershipRequestsByUsername(It.IsAny<string>())).Returns(requests);
            SetMockUser();
            var result = _controller.GetMembershipsRequestsForCurrentUser();
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(requests, okResult.Value);
        }

        [Fact]
        public void GetMembershipsRequestsForCurrentUser_ReturnsNotFoundIfNull()
        {
            _mockService.Setup(s => s.GetMembershipRequestsByUsername(It.IsAny<string>())).Returns((IEnumerable<RequestView>)null);
            SetMockUser();
            var result = _controller.GetMembershipsRequestsForCurrentUser();
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostAsync_ReturnsCreatedWithRequest()
        {
            var upload = GetSampleRequestUpload();
            var request = GetSampleRequestView();
            _mockService.Setup(s => s.AddAsync(upload)).ReturnsAsync(request);

            var result = await _controller.PostAsync(upload);
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            Assert.Equal(request, createdResult.Value);
        }

        [Fact]
        public async Task PostAsync_ReturnsNotFoundIfNull()
        {
            var upload = GetSampleRequestUpload();
            _mockService.Setup(s => s.AddAsync(upload)).ReturnsAsync((RequestView)null);

            var result = await _controller.PostAsync(upload);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PutAsync_ReturnsOkWithRequest()
        {
            var upload = GetSampleRequestUpload();
            var request = GetSampleRequestView();
            _mockService.Setup(s => s.UpdateAsync(1, upload)).ReturnsAsync(request);

            var result = await _controller.PutAsync(1, upload);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(upload, okResult.Value);
        }

        [Fact]
        public async Task PutAsync_ReturnsNotFoundIfNull()
        {
            var upload = GetSampleRequestUpload();
            _mockService.Setup(s => s.UpdateAsync(2, upload)).ReturnsAsync((RequestView)null);

            var result = await _controller.PutAsync(2, upload);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateStatusAsync_Approve_ReturnsOkWithRequest()
        {
            var request = GetSampleRequestView();
            _mockService.Setup(s => s.ApproveAsync(1)).ReturnsAsync(request);

            var result = await _controller.UpdateStatusAsync(1, "Approved");
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(request, okResult.Value);
        }

        [Fact]
        public async Task UpdateStatusAsync_Deny_ReturnsOkWithRequest()
        {
            var request = GetSampleRequestView();
            _mockService.Setup(s => s.DenyAsync(1)).ReturnsAsync(request);

            var result = await _controller.UpdateStatusAsync(1, "Denied");
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(request, okResult.Value);
        }

        [Fact]
        public async Task UpdateStatusAsync_Pending_ReturnsOkWithRequest()
        {
            var request = GetSampleRequestView();
            _mockService.Setup(s => s.SetPendingAsync(1)).ReturnsAsync(request);

            var result = await _controller.UpdateStatusAsync(1, "Pending");
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(request, okResult.Value);
        }

        [Fact]
        public async Task UpdateStatusAsync_ReturnsNotFoundIfNull()
        {
            _mockService.Setup(s => s.ApproveAsync(2)).ReturnsAsync((RequestView)null);

            var result = await _controller.UpdateStatusAsync(2, "Approved");
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Delete_ReturnsOkWithRequest()
        {
            var request = GetSampleRequestView();
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(request);

            var result = await _controller.Delete(1);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(request, okResult.Value);
        }

        [Fact]
        public async Task Delete_ReturnsNotFoundIfNull()
        {
            _mockService.Setup(s => s.DeleteAsync(2)).ReturnsAsync((RequestView)null);

            var result = await _controller.Delete(2);
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
} 