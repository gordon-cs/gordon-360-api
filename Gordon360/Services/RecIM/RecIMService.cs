using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Models.Gordon360.Context;
using System;
using Gordon360.Exceptions;
using System.Linq;
using System.Collections.Generic;

namespace Gordon360.Services.RecIM;

public class RecIMService(Gordon360Context context, IParticipantService participantSerivce) : IRecIMService
{
    public RecIMGeneralReportViewModel GetReport(DateTime start, DateTime end)
    {
        if (start > end) new UnprocessibleEntity { ExceptionMessage = $"End date cannot occur before the start date" };

        var activities = context.Activity
            .Where(a => a.RegistrationStart > start && a.RegistrationEnd < end)
            .Select(a =>
            new ActivityReportViewModel
                {
                    NumberOfParticipants = context.ParticipantTeam.
                        Where(pt => pt.Team.ActivityID == a.ID && pt.Team.StatusID != 0 && pt.RoleTypeID != 0)
                        .Count(),
                    Activity = a
                });

        var newParticipants =  context.ParticipantStatusHistory
            .Where(p => p.StatusID == 4 && p.StartDate > start)
                .Select(p => p.ParticipantUsername)
            .Distinct()
            .Select(username => new ParticipantReportViewModel
            {
                UserAccount = participantSerivce.GetUnaffiliatedAccountByUsername(username),
                NumberOfActivitiesParticipated =
                    context.ParticipantTeam
                    .Where(pt => 
                        pt.ParticipantUsername == username
                        && pt.RoleTypeID != 0
                        && pt.RoleTypeID != 2
                        && pt.SignDate > start
                        && pt.SignDate < end
                    )
                    .Count()
            });

        //active within timeframe
        var activeParticipants = context.ParticipantTeam
            .Where(pt => pt.SignDate > start && pt.SignDate < end && pt.RoleTypeID != 0)
            .Select(pt => pt.ParticipantUsername)
            .Distinct()
            .Select(username => (ParticipantReducedReportViewModel)context.Participant.FirstOrDefault(p => p.Username == username));

        return new RecIMGeneralReportViewModel()
        {
            StartTime = start,
            EndTime = end,
            Activities = activities,
            NumberOfNewParticipants = newParticipants.Count(),
            NewParticipants = newParticipants,
            NumberOfActiveParticipants = activeParticipants.Count(),
            ActiveParticipants = activeParticipants
        };
    }
}
