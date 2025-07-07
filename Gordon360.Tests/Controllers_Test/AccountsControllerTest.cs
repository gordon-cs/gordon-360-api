using Gordon360.Controllers;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Gordon360.Enums;
using Gordon360.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Gordon360.Tests.Controllers_Test;

public class AccountsControllerTest
{
    private readonly Mock<IAccountService> _mockAccountService;
    private readonly AccountsController _controller;

    public AccountsControllerTest()
    {
        _mockAccountService = new Mock<IAccountService>();
        _controller = new AccountsController(_mockAccountService.Object);
    }

    private void SetUser(string username, string group)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Upn, $"{username}@gordon.edu"),
            new Claim(ClaimTypes.Name, username),
            new Claim("groups", group) // Use full group name, e.g., "360-Student-SG"
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var user = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public void GetByAccountEmail_ReturnsOk_WhenAccountExists()
    {
        var account = new AccountViewModel { Email = "test@gordon.edu", GordonID = "1" };
        _mockAccountService.Setup(s => s.GetAccountByEmail("test@gordon.edu")).Returns(account);

        var result = _controller.GetByAccountEmail("test@gordon.edu");

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(account, okResult.Value);
    }

    [Fact]
    public void GetByAccountEmail_ReturnsNotFound_WhenAccountDoesNotExist()
    {
        _mockAccountService.Setup(s => s.GetAccountByEmail("notfound@gordon.edu")).Returns((AccountViewModel)null);

        var result = _controller.GetByAccountEmail("notfound@gordon.edu");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public void GetByAccountUsername_ReturnsOk_WhenAccountExists()
    {
        var account = new AccountViewModel { ADUserName = "jdoe", GordonID = "2" };
        _mockAccountService.Setup(s => s.GetAccountByUsername("jdoe")).Returns(account);

        var result = _controller.GetByAccountUsername("jdoe");

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(account, okResult.Value);
    }

    [Fact]
    public void GetByAccountUsername_ReturnsNotFound_WhenAccountDoesNotExist()
    {
        _mockAccountService.Setup(s => s.GetAccountByUsername("nouser")).Returns((AccountViewModel)null);

        var result = _controller.GetByAccountUsername("nouser");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task SearchAsync_ReturnsOk_WithResults()
    {
        var accounts = new List<BasicInfoViewModel> { new BasicInfoViewModel { FirstName = "John", LastName = "Doe", UserName = "jdoe" } };
        _mockAccountService.Setup(s => s.GetAllBasicInfoExceptAlumniAsync()).ReturnsAsync(accounts);
        _mockAccountService.Setup(s => s.Search("john", accounts)).Returns(accounts.AsParallel());

        var result = await _controller.SearchAsync("john");

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<BasicInfoViewModel>>(okResult.Value);
        Assert.Single(returned);
        Assert.Equal("John", returned.First().FirstName);
    }

    [Fact]
    public async Task SearchWithSpaceAsync_ReturnsOk_WithResults()
    {
        var accounts = new List<BasicInfoViewModel> { new BasicInfoViewModel { FirstName = "Jane", LastName = "Smith", UserName = "jsmith" } };
        _mockAccountService.Setup(s => s.GetAllBasicInfoExceptAlumniAsync()).ReturnsAsync(accounts);
        _mockAccountService.Setup(s => s.Search("Jane", "Smith", accounts)).Returns(accounts.AsParallel());

        var result = await _controller.SearchWithSpaceAsync("Jane", "Smith");

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<BasicInfoViewModel>>(okResult.Value);
        Assert.Single(returned);
        Assert.Equal("Jane", returned.First().FirstName);
    }

    [Fact]
    public async Task AdvancedPeopleSearchAsync_ReturnsOk_WithResults()
    {
        var accountTypes = new List<string> { "student" };
        var accounts = new List<AdvancedSearchViewModel> { new AdvancedSearchViewModel { FirstName = "Alex", LastName = "Johnson", AD_Username = "ajohnson" } };
        var searchResults = new List<AdvancedSearchViewModel> { new AdvancedSearchViewModel { FirstName = "Alex", LastName = "Johnson", AD_Username = "ajohnson" } };

        _mockAccountService.Setup(s => s.GetAccountsToSearch(accountTypes, It.IsAny<IEnumerable<AuthGroup>>(), null)).Returns(accounts);
        _mockAccountService.Setup(s => s.AdvancedSearch(accounts, "Alex", "Johnson", null, null, null, null, null, null, null, null, null, null, null, null, null, null)).Returns(searchResults);

        SetUser("ajohnson", "360-Student-SG");

        var result = await _controller.AdvancedPeopleSearchAsync(accountTypes, "Alex", "Johnson", null, null, null, null, null, null, null, null, null, null, null, null, null, null);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<AdvancedSearchViewModel>>(okResult.Value);
        Assert.Single(returned);
        Assert.Equal("Alex", returned.First().FirstName);
    }

    [Fact]
    public async Task AdvancedPeopleSearchAsync_ReturnsOk_WithEmptyResults_WhenNoMatch()
    {
        var accountTypes = new List<string> { "student" };
        var accounts = new List<AdvancedSearchViewModel>();
        var searchResults = new List<AdvancedSearchViewModel>();

        _mockAccountService.Setup(s => s.GetAccountsToSearch(accountTypes, It.IsAny<IEnumerable<AuthGroup>>(), null)).Returns(accounts);
        _mockAccountService.Setup(s => s.AdvancedSearch(accounts, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null)).Returns(searchResults);

        SetUser("ajohnson", "360-Student-SG");

        var result = await _controller.AdvancedPeopleSearchAsync(accountTypes, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<AdvancedSearchViewModel>>(okResult.Value);
        Assert.Empty(returned);
    }

    [Fact]
    public async Task AdvancedPeopleSearchAsync_ReturnsOk_WithFacStaffResults()
    {
        var accountTypes = new List<string> { "facstaff" };
        var accounts = new List<AdvancedSearchViewModel> {
            new AdvancedSearchViewModel { FirstName = "Sam", LastName = "Faculty", AD_Username = "sfaculty", Type = "FacStaff" }
        };
        var searchResults = new List<AdvancedSearchViewModel> {
            new AdvancedSearchViewModel { FirstName = "Sam", LastName = "Faculty", AD_Username = "sfaculty", Type = "FacStaff" }
        };

        _mockAccountService.Setup(s => s.GetAccountsToSearch(accountTypes, It.IsAny<IEnumerable<AuthGroup>>(), null)).Returns(accounts);
        _mockAccountService.Setup(s => s.AdvancedSearch(accounts, "Sam", "Faculty", null, null, null, null, null, null, null, null, null, null, null, null, null, null)).Returns(searchResults);

        SetUser("sfaculty", "360-FacStaff-SG");

        var result = await _controller.AdvancedPeopleSearchAsync(accountTypes, "Sam", "Faculty", null, null, null, null, null, null, null, null, null, null, null, null, null, null);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<AdvancedSearchViewModel>>(okResult.Value);
        Assert.Single(returned);
        Assert.Equal("Sam", returned.First().FirstName);
        Assert.Equal("FacStaff", returned.First().Type);
    }

    [Fact]
    public async Task AdvancedPeopleSearchAsync_ReturnsOk_WithAlumniResults()
    {
        var accountTypes = new List<string> { "alumni" };
        var accounts = new List<AdvancedSearchViewModel> {
            new AdvancedSearchViewModel { FirstName = "Alex", LastName = "Alum", AD_Username = "aalum", Type = "Alumni" }
        };
        var searchResults = new List<AdvancedSearchViewModel> {
            new AdvancedSearchViewModel { FirstName = "Alex", LastName = "Alum", AD_Username = "aalum", Type = "Alumni" }
        };

        _mockAccountService.Setup(s => s.GetAccountsToSearch(accountTypes, It.IsAny<IEnumerable<AuthGroup>>(), null)).Returns(accounts);
        _mockAccountService.Setup(s => s.AdvancedSearch(accounts, "Alex", "Alum", null, null, null, null, null, null, null, null, null, null, null, null, null, null)).Returns(searchResults);

        SetUser("aalum", "360-FacStaff-SG"); // Use FacStaff for alumni search permissions

        var result = await _controller.AdvancedPeopleSearchAsync(accountTypes, "Alex", "Alum", null, null, null, null, null, null, null, null, null, null, null, null, null, null);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsAssignableFrom<IEnumerable<AdvancedSearchViewModel>>(okResult.Value);
        Assert.Single(returned);
        Assert.Equal("Alex", returned.First().FirstName);
        Assert.Equal("Alumni", returned.First().Type);
    }
} 