using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Gordon360.Controllers;
using Gordon360.Services;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;

namespace Gordon360.Tests.Controllers_Test
{
    public class EmailsControllerTest
    {
        private readonly Mock<IEmailService> _mockService;
        private readonly EmailsController _controller;

        public EmailsControllerTest()
        {
            _mockService = new Mock<IEmailService>();
            _controller = new EmailsController(_mockService.Object);
        }

        private EmailViewModel GetSampleEmailViewModel() => new EmailViewModel
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@gordon.edu",
            Description = "Member"
        };

        private EmailContentViewModel GetSampleEmailContent() => new EmailContentViewModel
        {
            ToAddress = "recipient@gordon.edu",
            FromAddress = "sender@gordon.edu",
            Subject = "Test Subject",
            Content = "Test Content",
            Password = "password123"
        };

        [Fact]
        public void GetEmailsForActivity_ReturnsOkWithEmails()
        {
            var emails = new List<EmailViewModel> { GetSampleEmailViewModel() };
            _mockService.Setup(s => s.GetEmailsForActivity("ACT123", null, null)).Returns(emails);

            var result = _controller.GetEmailsForActivity("ACT123");
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(emails, okResult.Value);
        }

        [Fact]
        public void DEPRECATED_GetEmailsForActivity_ReturnsOkWithEmails()
        {
            var emails = new List<EmailViewModel> { GetSampleEmailViewModel() };
            _mockService.Setup(s => s.GetEmailsForActivity("ACT123", null, It.IsAny<List<string>>())).Returns(emails);

            var result = _controller.DEPRECATED_GetEmailsForActivity("ACT123", null, "Member");
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(emails, okResult.Value);
        }

        [Fact]
        public void SendEmails_ReturnsOk()
        {
            var email = GetSampleEmailContent();
            _mockService.Setup(s => s.SendEmails(
                It.IsAny<string[]>(),
                email.FromAddress,
                email.Subject,
                email.Content,
                email.Password
            ));

            var result = _controller.SendEmails(email);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void SendEmailToActivity_ReturnsOk()
        {
            var email = GetSampleEmailContent();
            _mockService.Setup(s => s.SendEmailToActivity(
                "ACT123",
                "202401",
                email.FromAddress,
                email.Subject,
                email.Content,
                email.Password
            ));

            var result = _controller.SendEmailToActivity("ACT123", "202401", email);
            Assert.IsType<OkResult>(result);
        }
    }
} 