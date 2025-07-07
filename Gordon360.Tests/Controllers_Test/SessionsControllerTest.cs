using Gordon360.Controllers;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Gordon360.Tests.Controllers_Test;

public class SessionsControllerTest
{
    private readonly Mock<ISessionService> _mockService;
    private readonly SessionsController _controller;

    public SessionsControllerTest()
    {
        _mockService = new Mock<ISessionService>();
        _controller = new SessionsController(_mockService.Object);
    }

    private void SetUser(string username, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Upn, $"{username}@gordon.edu"),
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var user = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    private static SessionViewModel GetSampleSession()
    {
        return new SessionViewModel
        {
            SessionCode = "202401",
            SessionDescription = "Spring 2024",
            SessionBeginDate = new DateTime(2024, 1, 10),
            SessionEndDate = new DateTime(2024, 5, 10)
        };
    }

    [Fact]
    public void Get_ReturnsOk_WithList()
    {
        var sessions = new List<SessionViewModel> { GetSampleSession() };
        _mockService.Setup(s => s.GetAll()).Returns(sessions);

        var result = _controller.Get();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<SessionViewModel>>(okResult.Value);
        Assert.Single(returned);
        Assert.Equal("202401", returned.First().SessionCode);
    }

    [Fact]
    public void Get_ReturnsOk_WithEmptyList()
    {
        _mockService.Setup(s => s.GetAll()).Returns(new List<SessionViewModel>());

        var result = _controller.Get();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<SessionViewModel>>(okResult.Value);
        Assert.Empty(returned);
    }

    [Fact]
    public void Get_ById_ReturnsOk_WhenSessionExists()
    {
        var session = GetSampleSession();
        _mockService.Setup(s => s.Get("202401")).Returns(session);

        var result = _controller.Get("202401");

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<SessionViewModel>(okResult.Value);
        Assert.Equal("202401", returned.SessionCode);
    }

    [Fact]
    public void Get_ById_ReturnsNotFound_WhenSessionDoesNotExist()
    {
        _mockService.Setup(s => s.Get("notfound")).Returns((SessionViewModel)null);

        var result = _controller.Get("notfound");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public void GetCurrentSession_ReturnsOk_WhenSessionExists()
    {
        var session = GetSampleSession();
        _mockService.Setup(s => s.GetCurrentSession()).Returns(session);

        var result = _controller.GetCurrentSession();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<SessionViewModel>(okResult.Value);
        Assert.Equal("202401", returned.SessionCode);
    }

    [Fact]
    public void GetCurrentSession_ReturnsNotFound_WhenNoSession()
    {
        _mockService.Setup(s => s.GetCurrentSession()).Returns((SessionViewModel)null);

        var result = _controller.GetCurrentSession();

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public void GetDaysLeftInSemester_ReturnsOk_WithValidDays()
    {
        var days = new double[] { 30, 120 };
        _mockService.Setup(s => s.GetDaysLeft()).Returns(days);

        var result = _controller.GetDaysLeftInSemester();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<double[]>(okResult.Value);
        Assert.Equal(30, returned[0]);
        Assert.Equal(120, returned[1]);
    }

    [Fact]
    public void GetDaysLeftInSemester_ReturnsNotFound_WhenNoDays()
    {
        _mockService.Setup(s => s.GetDaysLeft()).Returns((double[])null);

        var result = _controller.GetDaysLeftInSemester();

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public void GetDaysLeftInSemester_ReturnsNotFound_WhenZeroDays()
    {
        _mockService.Setup(s => s.GetDaysLeft()).Returns(new double[] { 0, 0 });

        var result = _controller.GetDaysLeftInSemester();

        Assert.IsType<NotFoundResult>(result.Result);
    }
} 