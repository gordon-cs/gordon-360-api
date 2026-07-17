using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.Salesforce;
using Gordon360.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services;

/// <summary>
/// Service class to facilitate data transactions between the Controller and the database model.
/// </summary>
public class SFSessionService(AcademicSessionProcedures academicSessionProcedures) : ISessionService
{

    public async Task<SessionViewModel?> Get(string sessionCode)
    {
        var result = await academicSessionProcedures.GetSession(sessionCode);
        return result;
    }

    public async Task<SessionViewModel?> GetCurrentSession()
    {
        var result = await academicSessionProcedures.GetCurrentSession();
        return result;
    }

    public async Task<SessionViewModel?> GetCurrentSessionForFinalExams()
    {
        var result = await academicSessionProcedures.GetCurrentSessionForFinalExams();
        return result;
    }

    public async Task<double[]> GetDaysLeft()
    {
        var currentSession = await GetCurrentSession();
        if (currentSession is null)
        {
            return [0, 0];
        }
        DateTime sessionEnd = currentSession.SessionEndDate ?? DateTime.Today;
        DateTime sessionBegin = currentSession.SessionBeginDate ?? DateTime.Today;
        DateTime startTime = DateTime.Today;

        double daysLeft = (sessionEnd - startTime).TotalDays;
        // Account for possible negative value in between sessions
        daysLeft = daysLeft < 0 ? 0 : daysLeft;

        double daysInSemester = (sessionEnd - sessionBegin).TotalDays;

        return [
        // Days left in semester
        daysLeft,
        // Total days in the semester
        daysInSemester
        ];
    }

    public async Task<IEnumerable<SessionViewModel>> GetAll()
    {
        return await academicSessionProcedures.GetAllSessions();
    }


}