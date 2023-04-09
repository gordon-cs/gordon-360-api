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
                .Select(a => GetActivityReport(a.ID));

            var newParticipants =  _context.ParticipantStatusHistory
                .Where(p => p.StatusID == 4 && p.StartDate > start)
                    .Select(p => p.ParticipantUsername)
                .Distinct()
                .Select(p => GetParticipantReport(p,start,end));

            var numberOfUniqueParticipants = _context.ParticipantTeam
                .Where(pt => pt.SignDate > start && pt.SignDate < end)
                .Select(pt => pt.ParticipantUsername)
                .Distinct()
                .Count();

            return new RecIMGeneralReportViewModel()
            {
                StartTime = start,
                EndTime = end,
                Activities = activities,
                NumberOfUniqueParticipants = numberOfUniqueParticipants,
                NumberOfNewParticipants = newParticipants.Count(),
                NewParticipants = newParticipants
            };
        }

        private ActivityReportViewModel GetActivityReport(int activityID)
        {
            var numberOfParticipants = _context.Team
                .Where(t => t.ActivityID == activityID)
                .Select(t => t.ParticipantTeam)
                .Count();
            var activityReport = new ActivityReportViewModel
                {
                    NumberOfParticipants = numberOfParticipants,
                    Activity = _context.Activity.Find(activityID)
                };

            return activityReport;
        }

        private ParticipantReportViewModel GetParticipantReport(string username, DateTime start, DateTime end)
        {
            var participantReport =  new ParticipantReportViewModel
                {
                    UserAccount = _accountService.GetAccountByUsername(username),
                    NumberOfActivitiesParticipated =
                        _context.ParticipantTeam
                        .Where(pt => pt.ParticipantUsername == username && pt.RoleTypeID != 0 && pt.RoleTypeID != 2 && pt.SignDate > start && pt.SignDate < end)
                        .Count()
                };
            return participantReport;
        }
    }
}
