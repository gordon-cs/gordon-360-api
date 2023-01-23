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
        public IEnumerable<LookupViewModel> GetParticipantLookup(string type)
        {
            if (type == "status")
            {
                var res = _context.ParticipantStatus
                            .Select(s => new LookupViewModel
                            {
                                ID = s.ID,
                                Description = s.Description
                            })
                            .AsEnumerable();
                return res;
            }
            if (type == "activitypriv")
            {
                var res = _context.PrivType
                            .Select(s => new LookupViewModel
                            {
                                ID = s.ID,
                                Description = s.Description
                            })
                            .AsEnumerable();
                return res;
            }
            return null;
        }
        public ACCOUNT GetAccountByParticipantID(int ID)
        {
            return _context.ACCOUNT
                        .FirstOrDefault(a => a.gordon_id == $"{ID}");
        }
        public ParticipantExtendedViewModel GetParticipantByUsername(string username)
        {
            var account = _accountService.GetAccountByUsername(username);
            var participant = _context.Participant
                                .Where(p => p.Username == username)
                                .Select(p => new ParticipantExtendedViewModel
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
                                                    .Select(pn => (ParticipantNotificationExtendedViewModel)pn)
                                                    .AsEnumerable(),
                                    IsAdmin = p.IsAdmin
                                })
                                .FirstOrDefault();
            return participant;
        }

        public async Task<ParticipantNotificationViewModel> SendParticipantNotificationAsync(string username, ParticipantNotificationUploadViewModel notificationVM)
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
        public IEnumerable<ParticipantStatusExtendedViewModel> GetParticipantStatusHistory(string username)
        {
            var status = _context.ParticipantStatusHistory
                            .Where(psh => psh.ParticipantUsername == username)
                            .OrderByDescending(psh => psh.ID)
                                .Join(_context.ParticipantStatus,
                                    psh => psh.StatusID,
                                    ps => ps.ID,
                                    (psh, ps) => new ParticipantStatusExtendedViewModel
                                    {
                                        Username = username,
                                        Status = ps.Description,
                                        StartDate = psh.StartDate,
                                        EndDate = psh.EndDate
                                    }).AsEnumerable();
            return status;
        }
        public IEnumerable<TeamExtendedViewModel> GetParticipantTeams(string username)
        {
       
            //to be handled by teamservice
            var teams = _context.ParticipantTeam
                            .Where(pt => pt.ParticipantUsername == username)
                                .Join(_context.Team,
                                    pt => pt.TeamID,
                                    t => t.ID,
                                    (pt, t) => new TeamExtendedViewModel
                                    {
                                        ID = t.ID,
                                        Activity = _context.Activity.FirstOrDefault(a => a.ID == t.ActivityID),
                                        Name = t.Name,
                                        Status = _context.TeamStatus
                                                    .FirstOrDefault(ts => ts.ID == t.StatusID)
                                                    .Description,
                                        Logo = t.Logo
                                    });
            return teams;
        }

        public IEnumerable<ParticipantExtendedViewModel> GetParticipants()
        {
            //temporary, slow and will be adjusted after we implement views in the DB
            var participants = _context.Participant.Select(p => new ParticipantExtendedViewModel
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

        public async Task<ParticipantExtendedViewModel> PostParticipantAsync(string username)
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

        public async Task<ParticipantExtendedViewModel> UpdateParticipantAsync(string username, bool isAdmin)
        {
            var participant = GetParticipantByUsername(username);
            participant.IsAdmin = isAdmin;
            await _context.SaveChangesAsync();
            return participant;
        }

        public async Task<ParticipantActivityViewModel> UpdateParticipantActivityAsync(string username, ParticipantActivityPatchViewModel updatedParticipant)
        {           
            var participantActivity = _context.ParticipantActivity
                                        .FirstOrDefault(pa => pa.ParticipantUsername == username 
                                            && pa.ActivityID == updatedParticipant.ActivityID);

            participantActivity.PrivTypeID = updatedParticipant.ActivityPrivID ?? participantActivity.PrivTypeID;
            participantActivity.IsFreeAgent = updatedParticipant.IsFreeAgent ?? participantActivity.IsFreeAgent;
                                                
        
            await _context.SaveChangesAsync();
            return participantActivity;
        }

        public async Task<ParticipantStatusViewModel> UpdateParticipantStatusAsync(string username, ParticipantStatusPatchViewModel participantStatus)
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

        public bool IsParticipant(string username)
        {
            return _context.Participant.Any(p => p.Username == username);
        }

        public bool IsAdmin(string username)
        {
            return _context.Participant.Any(p => p.Username == username && p.IsAdmin == true);
        }
    }

}

