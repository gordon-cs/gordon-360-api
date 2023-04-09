using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Models.CCT.Context;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gordon360.Extensions.System;
using Microsoft.EntityFrameworkCore;

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

        public IEnumerable<LookupViewModel>? GetParticipantLookup(string type)
        {
            return type switch
            {
                "status" => _context.ParticipantStatus.Where(query => query.ID != 0)
                            .Select(s => new LookupViewModel
                            {
                                ID = s.ID,
                                Description = s.Description
                            })
                            .AsEnumerable(),
                "activitypriv" => _context.ParticipantStatus.Where(query => query.ID != 0)
                            .Select(s => new LookupViewModel
                            {
                                ID = s.ID,
                                Description = s.Description
                            })
                            .AsEnumerable(),
                _ => null
            };
        }

        public ParticipantExtendedViewModel GetParticipantByUsername(string username)
        {
            string? accountEmail = null;
            try
            {
                accountEmail = _accountService.GetAccountByUsername(username).Email;
            }
            catch
            {
                // if exception is thrown, we know that this was a manually added participant
            }
            var participant = _context.Participant
                                .Where(p => p.Username == username)
                                .Select(p => new ParticipantExtendedViewModel
                                {
                                    Username = username,
                                    Email = accountEmail,
                                    Status = p.ParticipantStatusHistory
                                                .OrderByDescending(psh => psh.ID)
                                                .FirstOrDefault()
                                                .Status
                                                .Description,
                                    Notification = _context.ParticipantNotification
                                                    .Where(pn => pn.ParticipantUsername == username && pn.EndDate > DateTime.UtcNow)
                                                    .OrderByDescending(pn => pn.DispatchDate)
                                                    .Select(pn => (ParticipantNotificationViewModel)pn)
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
                DispatchDate = DateTime.UtcNow
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
                                        StartDate = psh.StartDate.SpecifyUtc(),
                                        EndDate = psh.EndDate.SpecifyUtc()
                                    }).AsEnumerable();
            return status;
        }

        public IEnumerable<TeamExtendedViewModel> GetParticipantTeams(string username)
        {
                   var teams = _context.ParticipantTeam
                            .Where(pt => pt.ParticipantUsername == username && pt.RoleTypeID != 0 && pt.RoleTypeID != 2 )
                                .Join(_context.Team.Where(t => t.StatusID != 0),
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
                                        Logo = t.Logo,
                                        TeamRecord = _context.SeriesTeam
                                            .Include(st => st.Team)
                                            .Where(st => st.TeamID == t.ID)
                                            .Select(st => (TeamRecordViewModel)st)
                                    
                                    });
            return teams;
        }

        public IEnumerable<ParticipantExtendedViewModel> GetParticipants()
        {
            //temporary, slow and will be adjusted after we implement views in the DB
            var participants = _context.Participant.Select(p => new ParticipantExtendedViewModel
            {
                Username = p.Username,
                Status = p.ParticipantStatusHistory.OrderByDescending(psh => psh.ID).FirstOrDefault().Status.Description,
                IsAdmin = p.IsAdmin
            });
            return participants;
        }

        public async Task<ParticipantExtendedViewModel> PostParticipantAsync(string username, int? statusID)
        {
            // Find gender
            string user_gender = null;
            var student = _context.Student.Where(s => s.AD_Username == username).FirstOrDefault();
            var facstaff = _context.FacStaff.Where(fs => fs.AD_Username == username).FirstOrDefault();
            if (student is not null) user_gender = student.Gender;
            if (facstaff is not null) user_gender = facstaff.Gender;
            if (user_gender is null) user_gender = "Unspecified";

            await _context.Participant.AddAsync(new Participant
            {
                Username = username,
                SpecifiedGender = user_gender
            });
            await _context.ParticipantStatusHistory.AddAsync(new ParticipantStatusHistory
            {
                ParticipantUsername = username,
                StatusID = statusID ?? 4, //default to cleared
                StartDate = DateTime.UtcNow,
                //No defined end date for creation
            });
            await _context.SaveChangesAsync();
            var participant = GetParticipantByUsername(username);
            return participant;
        }

        public async Task<ParticipantExtendedViewModel> SetParticipantAdminStatusAsync(string username, bool isAdmin)
        {
            var participant = _context.Participant.Find(username);
            participant.IsAdmin = isAdmin;
            await _context.SaveChangesAsync();
            return new ParticipantExtendedViewModel
            {
                Username = participant.Username,
                IsAdmin = participant.IsAdmin,
                AllowEmails = participant.AllowEmails ?? true, //due to SQL having a default value, EFCore thinks that AllowEmails is nullable. It isn't.
                SpecifiedGender = participant.SpecifiedGender
            };
        }

        public async Task<ParticipantExtendedViewModel> UpdateParticipantAsync(string username, bool allowEmails)
        {
            var participant = _context.Participant.Find(username);
            participant.AllowEmails = allowEmails;
            await _context.SaveChangesAsync();
            return new ParticipantExtendedViewModel
            {
                Username = participant.Username,
                IsAdmin = participant.IsAdmin,
                AllowEmails = participant.AllowEmails ?? true, //due to SQL having a default value, EFCore thinks that AllowEmails is nullable. It isn't.
                SpecifiedGender = participant.SpecifiedGender
            };
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

        public async Task<ParticipantStatusHistoryViewModel> UpdateParticipantStatusAsync(string username, ParticipantStatusPatchViewModel participantStatus)
        {
            // End previous status
            var prevStatus = _context.ParticipantStatusHistory
                               .Where(psh => psh.ParticipantUsername == username)
                               .OrderByDescending(psh => psh.ID)
                               .FirstOrDefault();
            prevStatus.EndDate = DateTime.UtcNow;

            var status = new ParticipantStatusHistory
            {
                ParticipantUsername = username,
                StatusID = participantStatus.StatusID,
                StartDate = DateTime.UtcNow,
                EndDate = participantStatus.EndDate
            };
            await _context.ParticipantStatusHistory.AddAsync(status);
            await _context.SaveChangesAsync();
            return status;
        }

        public bool IsParticipant(string username)
        {
            if (_context.Participant.FirstOrDefault(p => p.Username == username) is null)
            {
                return false;
            }
            var isPending = _context.ParticipantStatusHistory
                    .Where(psh => psh.ParticipantUsername == username)
                    .OrderByDescending(psh => psh.ID)
                    .FirstOrDefault()
                    .StatusID == 1;

            // 1 is pending
            return !isPending;
        }

        public bool IsAdmin(string username)
        {
            return _context.Participant.Any(p => p.Username == username && p.IsAdmin == true);
        }
    }

}

