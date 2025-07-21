﻿using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gordon360.Services;

/// <summary>
/// Service class to facilitate data transactions between the Controller and the database model.
/// </summary>
public class SessionService(CCTContext context) : ISessionService
{

    /// <summary>
    /// Get the session record whose sesssion code matches the parameter.
    /// </summary>
    /// <param name="sessionCode">The session code.</param>
    /// <returns>A SessionViewModel if found, null if not found.</returns>
    public SessionViewModel Get(string sessionCode)
    {
        var query = context.CM_SESSION_MSTR.Where(s => s.SESS_CDE == sessionCode).FirstOrDefault();
        if (query == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The Session was not found." };
        }
        return query;
    }

    public SessionViewModel GetCurrentSession()
    {
        return context.CM_SESSION_MSTR.Where(s => DateTime.Now > s.SESS_BEGN_DTE).OrderByDescending(s => s.SESS_BEGN_DTE).First();
    }

    public SessionViewModel GetCurrentSessionForFinalExams()
    {
        return context.CM_SESSION_MSTR.Where(s => DateTime.Now > s.SESS_BEGN_DTE &&
                                          (s.YRTRM_CDE_2.EndsWith("SP") || s.YRTRM_CDE_2.EndsWith("FA")))
                                      .OrderByDescending(s => s.SESS_BEGN_DTE).First();
    }

    // Return the days left in the semester, and the total days in the current session
    public double[] GetDaysLeft()
    {
        var currentSession = GetCurrentSession();
        DateTime sessionEnd = currentSession.SessionEndDate.Value;
        DateTime sessionBegin = currentSession.SessionBeginDate.Value;
        DateTime startTime = DateTime.Today;

        double daysLeft = (sessionEnd - startTime).TotalDays;
        // Account for possible negative value in between sessions
        daysLeft = daysLeft < 0 ? 0 : daysLeft;

        double daysInSemester = (sessionEnd - sessionBegin).TotalDays;

        return new double[2] {
        // Days left in semester
        daysLeft,
        // Total days in the semester
        daysInSemester
        };
    }

    /// <summary>
    /// Fetches all the session records from the database.
    /// </summary>
    /// <returns>A SessionViewModel IEnumerable. If nothing is found, an empty IEnumerable is returned.</returns>
    public IEnumerable<SessionViewModel> GetAll()
    {
        return context.CM_SESSION_MSTR.Select<CM_SESSION_MSTR, SessionViewModel>(s => s);
    }


}