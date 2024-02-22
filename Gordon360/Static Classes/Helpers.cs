using Gordon360.Models.Gordon360.Context;
using System;
using System.Linq;

namespace Gordon360.Static.Methods;

/// <summary>
/// Service class for methods that are shared between all services.
/// </summary>
public static class Helpers
{
    /// <summary>
    /// Helper method that gets the current session we are in.
    /// </summary>
    /// <returns>The session code of the current session</returns>
    public static string GetCurrentSession(Gordon360Context context)
    {
        return context.CM_SESSION_MSTR.Where(s => DateTime.Now > s.SESS_BEGN_DTE).OrderByDescending(s => s.SESS_BEGN_DTE).Select(s => s.SESS_CDE).First();
    }
}
