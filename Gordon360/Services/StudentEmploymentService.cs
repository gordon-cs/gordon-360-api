using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
using Gordon360.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services;

public class StudentEmploymentService : IStudentEmploymentService
{
    private readonly CCTContext _context;
    private readonly IAccountService _accountService;

    public StudentEmploymentService(CCTContext context)
    {
        _context = context;
        _accountService = new AccountService(context);
    }

    /// <summary>
    /// get Student Employment records of given user
    /// </summary>
    /// <param name="username">AD Username of user to get employment</param>
    /// <returns>VictoryPromiseViewModel if found, null if not found</returns>
    public async Task<IEnumerable<StudentEmploymentViewModel>> GetEmploymentAsync(string username)
    {
        var account = _accountService.GetAccountByUsername(username);

        var result = await _context.Procedures.STUDENT_JOBS_PER_ID_NUMAsync(int.Parse(account.GordonID));
        if (result == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
        }
        // Transform the ActivityViewModel (ACT_CLUB_DEF) into ActivityInfoViewModel
        var studentEmploymentModel = result.Select(x => new StudentEmploymentViewModel
        {
            Job_Title = x.Job_Title ?? "",
            Job_Department = x.Job_Department ?? "",
            Job_Department_Name = x.Job_Department_Name ?? "",
            Job_Start_Date = x.Job_Start_Date ?? DateTime.MinValue,
            Job_End_Date = x.Job_End_Date ?? DateTime.Now,
            Job_Expected_End_Date = x.Job_Expected_End_Date ?? DateTime.Now,
        });
        return studentEmploymentModel;

    }
}