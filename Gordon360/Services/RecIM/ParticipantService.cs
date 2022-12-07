using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Models.CCT.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Gordon360.Services.RecIM
{
    public class ParticipantService : IParticipantService
    {
        private readonly CCTContext _context;
        private readonly IAccountService _accountService;

        public ParticipantService(CCTContext context, IAccountService accountService)
        {
            _accountService = accountService;
            _context = context;
        }

        public ACCOUNT GetAccountByParticipantID(int ID)
        {
            return _context.ACCOUNT
                        .FirstOrDefault(a => a.gordon_id == $"{ID}");
        }
        public ParticipantViewModel GetParticipantByUsername(string username)
        {
            var account = _accountService.GetAccountByUsername(username);
            var participant = _context.Participant
                                .Where(p => p.Username == username)
                                .Select(p => new ParticipantViewModel
                                {
                                    Username = username,
                                    Email = account.Email,
                                    Status = _context.ParticipantStatusHistory
                                                .Where(psh => psh.ParticipantUsername == username)
                                                .OrderByDescending(psh => psh.ID)
                                                .Take(1)
                                                    .Join(_context.ParticipantStatus,
                                                        psh => psh.StatusID,
                                                        ps => ps.ID,
                                                        (psh, ps) => ps.Description)
                                                .FirstOrDefault(),
                                    Notification = _context.ParticipantNotification
                                                    .Where(pn => pn.ParticipantUsername == username && pn.EndDate > DateTime.Now)
                                                    .OrderByDescending(pn => pn.DispatchDate)
                                                    .Select(pn => (ParticipantNotificationViewModel)pn)
                                                    .AsEnumerable(),
                                    IsAdmin = p.IsAdmin
                                })
                                .FirstOrDefault();
            return participant;
        }

        public async Task<ParticipantNotificationCreatedViewModel> SendParticipantNotification(string username, ParticipantNotificationUploadViewModel notificationVM)
        {
            var newNotification = new ParticipantNotification
            {
                ParticipantUsername = username,
                Message = notificationVM.Message,
                EndDate = notificationVM.EndDate,
                DispatchDate = DateTime.Now
            };
            await _context.ParticipantNotification.AddAsync(newNotification);
            await _context.SaveChangesAsync();
            return newNotification;
        }
        public IEnumerable<ParticipantStatusViewModel> GetParticipantStatusHistory(string username)
        {
            var status = _context.ParticipantStatusHistory
                            .Where(psh => psh.ParticipantUsername == username)
                            .OrderByDescending(psh => psh.ID)
                                .Join(_context.ParticipantStatus,
                                    psh => psh.StatusID,
                                    ps => ps.ID,
                                    (psh, ps) => new ParticipantStatusViewModel
                                    {
                                        Username = username,
                                        Status = ps.Description,
                                        StartDate = psh.StartDate,
                                        EndDate = psh.EndDate
                                    }).AsEnumerable();
            return status;
        }
        public IEnumerable<TeamViewModel> GetParticipantTeams(string username)
        {
            //to be handled by teamservice
            var teams = _context.ParticipantTeam
                            .Where(pt => pt.ParticipantUsername == username)
                                .Join(_context.Team,
                                    pt => pt.TeamID,
                                    t => t.ID,
                                    (pt, t) => new TeamViewModel
                                    {
                                        ID = t.ID,
                                        ActivityID = t.ActivityID,
                                        Name = t.Name,
                                        Status = _context.TeamStatus
                                                    .FirstOrDefault(ts => ts.ID == t.StatusID)
                                                    .Description,
                                        Logo = t.Logo
                                    });
            return teams;
        }

        public IEnumerable<ParticipantViewModel> GetParticipants()
        {
            //temporary, slow and will be adjusted after we implement views in the DB
            var participants = _context.Participant.Select(p => new ParticipantViewModel
            {
                Username = p.Username,
                Email = _accountService.GetAccountByUsername(p.Username).Email,
                Status = _context.ParticipantStatusHistory
                                                .Where(psh => psh.ParticipantUsername == p.Username)
                                                .OrderByDescending(psh => psh.ID)
                                                .Take(1)
                                                    .Join(_context.ParticipantStatus,
                                                        psh => psh.StatusID,
                                                        ps => ps.ID,
                                                        (psh, ps) => ps.Description)
                                                .FirstOrDefault(),
                IsAdmin = p.IsAdmin
            });
            return participants;
        }

        public async Task<ParticipantViewModel> PostParticipant(string username)
        {
            await _context.Participant.AddAsync(new Participant
            {
                Username = username
            });
            await _context.ParticipantStatusHistory.AddAsync(new ParticipantStatusHistory
            {
                ParticipantUsername = username,
                StatusID = 4, //default to cleared
                StartDate = DateTime.Now,
                //No defined end date for creation
            });
            await _context.SaveChangesAsync();
            var participant = GetParticipantByUsername(username);
            return participant;
        }

        public async Task<ParticipantViewModel> UpdateParticipant(string username, bool isAdmin)
        {
            var participant = GetParticipantByUsername(username);
            participant.IsAdmin = isAdmin;
            await _context.SaveChangesAsync();
            return participant;
        }
        public async Task<ParticipantActivityCreatedViewModel> UpdateParticipantActivity(string username, ParticipantActivityPatchViewModel updatedParticipant)
        {           
            var participantActivity = _context.ParticipantActivity
                                        .FirstOrDefault(pa => pa.ParticipantUsername == username 
                                            && pa.ActivityID == updatedParticipant.ActivityID);

            participantActivity.PrivTypeID = updatedParticipant.ActivityPrivID ?? participantActivity.PrivTypeID;
            participantActivity.IsFreeAgent = updatedParticipant.IsFreeAgent ?? participantActivity.IsFreeAgent;
                                                
        
            await _context.SaveChangesAsync();
            return participantActivity;
        }
        public async Task<ParticipantStatusCreatedViewModel> UpdateParticipantStatus(string username, ParticipantStatusPatchViewModel participantStatus)
        {
            // End previous status
            var prevStatus = _context.ParticipantStatusHistory
                               .Where(psh => psh.ParticipantUsername == username)
                               .OrderByDescending(psh => psh.ID)
                               .FirstOrDefault();
            prevStatus.EndDate = DateTime.Now;

            var status = new ParticipantStatusHistory
            {
                ParticipantUsername = username,
                StatusID = participantStatus.StatusID,
                StartDate = DateTime.Now,
                EndDate = participantStatus.EndDate
            };
            await _context.ParticipantStatusHistory.AddAsync(status);
            await _context.SaveChangesAsync();
            return status;
        }
    }

}

