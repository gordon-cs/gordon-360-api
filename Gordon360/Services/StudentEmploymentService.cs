using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
using Gordon360.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services;

public class StudentEmploymentService(CCTContext context, SFStudentEmployment sfStudentEmployment, IAccountService accountService) : IStudentEmploymentService
{

    /// <summary>
    /// get Student Employment records of given user
    /// </summary>
    /// <param name="username">AD Username of user to get employment</param>
    /// <returns>VictoryPromiseViewModel if found, null if not found</returns>
    public async Task<IEnumerable<StudentEmploymentViewModel>> GetEmploymentAsync(string username)
    {
        /*var account = accountService.GetAccountByUsername(username);

        var result = await context.Procedures.STUDENT_JOBS_PER_ID_NUMAsync(int.Parse(account.GordonID));
        if (result == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
        }
        */

        var result = await sfStudentEmployment.GetStudentEmployment(username);

        return result;

    }
}