using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Models.CCT.Context;
using Microsoft.Extensions.Configuration;
using System;
using Gordon360.Exceptions;
using Microsoft.Graph;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Gordon360.Models.ViewModels;

namespace Gordon360.Services.RecIM
{
    public class RecIMService : IRecIMService
    {
        private readonly CCTContext _context;
        private readonly IMatchService _matchService;
        private readonly IParticipantService _participantService;
        private readonly IAccountService _accountService;
        private readonly IConfiguration _config;

        public RecIMService(CCTContext context, IConfiguration config, IParticipantService participantSerivce, IMatchService matchService, IAccountService accountServices)
        {
            _context = context;
            _config = config;
            _matchService = matchService;
            _accountService = accountServices;
            _participantService = participantSerivce;
        }

        public RecIMGeneralReportViewModel GetReport(DateTime start, DateTime end)
        {
            if (start > end) new UnprocessibleEntity { ExceptionMessage = $"End date cannot occur before the start date" };

            var activities = _context.Activity
                .Where(a => a.RegistrationStart > start && a.RegistrationEnd < end)
                .Select(a =>
                new ActivityReportViewModel
                    {
                        NumberOfParticipants = _context.ParticipantTeam.
                            Where(pt => pt.Team.ActivityID == a.ID && pt.Team.StatusID != 0 && pt.RoleTypeID != 0)
                            .Count(),
                        Activity = a
                    });

            var newParticipants =  _context.ParticipantStatusHistory
                .Where(p => p.StatusID == 4 && p.StartDate > start)
                    .Select(p => p.ParticipantUsername)
                .Distinct()
                .Select(username => new ParticipantReportViewModel
                {
                    UserAccount = _accountService.GetUnaffilicatedAccountByUsername(username),
                    NumberOfActivitiesParticipated =
                        _context.ParticipantTeam
                        .Where(pt => pt.ParticipantUsername == username && pt.RoleTypeID != 0 && pt.RoleTypeID != 2 && pt.SignDate > start && pt.SignDate < end)
                        .Count()
                });

            //active within timeframe
            var activeParticipants = _context.ParticipantTeam
                .Where(pt => pt.SignDate > start && pt.SignDate < end && pt.RoleTypeID != 0)
                .Select(pt => pt.ParticipantUsername)
                .Distinct()
                .Select(username => (ParticipantReducedReportViewModel)_context.Participant.FirstOrDefault(p => p.Username == username));

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
}
