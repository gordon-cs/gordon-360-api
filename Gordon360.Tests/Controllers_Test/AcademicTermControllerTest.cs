using Gordon360.Controllers;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Gordon360.Tests.Controllers_Test;

public class AcademicTermControllerTest
{
    private readonly Mock<IAcademicTermService> _mockService;
    private readonly AcademicTermController _controller;

    public AcademicTermControllerTest()
    {
        _mockService = new Mock<IAcademicTermService>();
        _controller = new AcademicTermController(_mockService.Object);
    }

    private static YearTermTable GetSampleYearTermTable()
    {
        return new YearTermTable
        {
            YR_CDE = "2024",
            TRM_CDE = "SP",
            TRM_BEGIN_DTE = new DateTime(2024, 1, 10),
            TRM_END_DTE = new DateTime(2024, 5, 10),
            YR_TRM_DESC = "Spring 2024",
            PRT_INPROG_ON_TRAN = "Y",
            CENSUS_DTE = new DateTime(2024, 2, 1),
            CENSUS_PERCENTAGE = 100,
            SHOW_ON_WEB = "Y",
            ONLINE_ADM_APP_OPEN = "Y",
            PREREG_STS = "Y",
            FIRST_DROP_ADD_DTE = new DateTime(2024, 1, 15),
            LAST_DROP_ADD_DTE = new DateTime(2024, 1, 20),
            FIRST_FINAL_GRD_DTE = new DateTime(2024, 5, 1),
            LAST_FINAL_GRD_DTE = new DateTime(2024, 5, 10),
            FIRST_MIDTRM_GRD_DTE = new DateTime(2024, 3, 1),
            LAST_MIDTRM_GRD_DTE = new DateTime(2024, 3, 10),
            LAST_WITHDRAW_PASS_DTE = new DateTime(2024, 4, 1),
            PESC_SESSION_TYPE = "REG",
            BILLING_PERIOD_ID = 1,
            PAYMENT_DUE_DATE = new DateTime(2024, 1, 5),
            ONLINE_BALANCE_DISPLAY_DEFAULT = true,
            STUDENT_WITHDRAWAL_END_DATE = new DateTime(2024, 4, 20),
            APPROWVERSION = new byte[] { 1 },
            USER_NAME = "testuser",
            JOB_NAME = "job",
            JOB_TIME = new DateTime(2024, 1, 1)
        };
    }

    [Fact]
    public async Task GetCurrentTerm_ReturnsOk_WhenTermExists()
    {
        var entity = GetSampleYearTermTable();
        var viewModel = new YearTermTableViewModel(entity);
        _mockService.Setup(s => s.GetCurrentTermAsync()).ReturnsAsync(viewModel);

        var result = await _controller.GetCurrentTerm();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<YearTermTableViewModel>(okResult.Value);
        Assert.Equal("2024", returned.YearCode);
        Assert.Equal("SP", returned.TermCode);
    }

    [Fact]
    public async Task GetCurrentTerm_ReturnsNotFound_WhenNoTerm()
    {
        _mockService.Setup(s => s.GetCurrentTermAsync()).ReturnsAsync((YearTermTableViewModel)null);

        var result = await _controller.GetCurrentTerm();

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetAllTerms_ReturnsOk_WithList()
    {
        var entity = GetSampleYearTermTable();
        var viewModel = new YearTermTableViewModel(entity);
        var list = new List<YearTermTableViewModel> { viewModel };
        _mockService.Setup(s => s.GetAllTermsAsync()).ReturnsAsync(list);

        var result = await _controller.GetAllTerms();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsAssignableFrom<IEnumerable<YearTermTableViewModel>>(okResult.Value);
        Assert.Single(returned);
        Assert.Equal("2024", returned.First().YearCode);
    }

    [Fact]
    public async Task GetAllTerms_ReturnsOk_WithEmptyList()
    {
        _mockService.Setup(s => s.GetAllTermsAsync()).ReturnsAsync(new List<YearTermTableViewModel>());

        var result = await _controller.GetAllTerms();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsAssignableFrom<IEnumerable<YearTermTableViewModel>>(okResult.Value);
        Assert.Empty(returned);
    }
    [Fact]
    public async Task GetDaysLeft_ReturnsOk_WithZeroDays()
    {
        var viewModel = new DaysLeftViewModel
        {
            DaysLeft = 0,
            TotalDays = 0,
            TermLabel = "Break"
        };

        _mockService.Setup(s => s.GetDaysLeftAsync()).ReturnsAsync(viewModel);

        var result = await _controller.GetDaysLeft();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<DaysLeftViewModel>(okResult.Value);
        Assert.Equal(0, returned.DaysLeft);
        Assert.Equal(0, returned.TotalDays);
        Assert.Equal("Break", returned.TermLabel);
    }

    [Fact]
    public async Task GetDaysLeft_ReturnsOk_WithValidDays()
    {
        var viewModel = new DaysLeftViewModel
        {
            DaysLeft = 30,
            TotalDays = 120,
            TermLabel = "Spring 2024"
        };

        _mockService.Setup(s => s.GetDaysLeftAsync()).ReturnsAsync(viewModel);

        var result = await _controller.GetDaysLeft();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<DaysLeftViewModel>(okResult.Value);
        Assert.Equal(30, returned.DaysLeft);
        Assert.Equal(120, returned.TotalDays);
        Assert.Equal("Spring 2024", returned.TermLabel);
    }

}