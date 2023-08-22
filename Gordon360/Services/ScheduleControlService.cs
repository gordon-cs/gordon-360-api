
using Gordon360.Exceptions;
using Gordon360.Models.CCT.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services;

/// <summary>
/// Service Class that facilitates data transactions between the ScheduleControlController and the ScheduleControl part of the database model.
/// </summary>
public class ScheduleControlService : IScheduleControlService
{
    private CCTContext _context;

    public ScheduleControlService(CCTContext context)
    {
        _context = context;
    }

    /// <summary>
    /// privacy setting of schedule.
    /// </summary>
    /// <param name="username">AD Username</param>
    /// <param name="value">Y or N</param>
    public async Task UpdateSchedulePrivacyAsync(string username, string value)
    {
        var account = _context.ACCOUNT.FirstOrDefault(x => x.AD_Username == username);

        if (account == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
        }

        await _context.Procedures.UPDATE_SCHEDULE_PRIVACYAsync(int.Parse(account.gordon_id), value);

    }

    /// <summary>
    /// description of schedule.
    /// </summary>
    /// <param name="username">AD Username</param>
    /// <param name="value">New description</param>
    public async Task UpdateDescriptionAsync(string username, string value)
    {
        var account = _context.ACCOUNT.FirstOrDefault(x => x.AD_Username == username);

        if (account == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
        }

        await _context.Procedures.UPDATE_DESCRIPTIONAsync(int.Parse(account.gordon_id), value);
    }


    /// <summary>
    /// Update timestamp of modification in schedule.
    /// </summary>
    /// <param name="username">AD Username</param>
    /// <param name="value">Modified Time</param>
    public async Task UpdateModifiedTimeStampAsync(string username, DateTime value)
    {
        var account = _context.ACCOUNT.FirstOrDefault(x => x.AD_Username == username);

        if (account == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
        }

        await _context.Procedures.UPDATE_TIMESTAMPAsync(int.Parse(account.gordon_id), value);
    }
}