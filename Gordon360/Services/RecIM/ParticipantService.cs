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

        public ParticipantService(CCTContext context)
        {

            _context = context;
        }

        public ParticipantViewModel GetParticipantByUsername(string username)
        {
            int participantID = Int32.Parse(_context.ACCOUNT
                                    .FirstOrDefault(a => a.AD_Username == username)
                                    .gordon_id);
            var participant = _context.Participant
                                .Where(p => p.ID == participantID)
                                .Select(p => new ParticipantViewModel
                                {
                                    Username = username,
                                    Email = _context.ACCOUNT
                                                .FirstOrDefault(a => a.AD_Username == username)
                                                .email,
                                    Status = _context.ParticipantStatusHistory
                                                .Where(psh => psh.ParticipantID == p.ID)
                                                .OrderByDescending(psh => psh.ID)
                                                .Take(1)
                                                    .Join(_context.ParticipantStatus,
                                                        psh => psh.StatusID,
                                                        ps => ps.ID,
                                                        (psh, ps) => ps.Description)
                                                .FirstOrDefault()
                                })
                                .FirstOrDefault();

            return participant;
        }
        public IEnumerable<ParticipantStatusViewModel> GetParticipantStatusHistory(string username)
        {
            int participantID = Int32.Parse(_context.ACCOUNT
                                    .FirstOrDefault(a => a.AD_Username == username)
                                    .gordon_id);
            var status = _context.ParticipantStatusHistory
                            .Where(psh => psh.ParticipantID == participantID)
                            .OrderByDescending(psh => psh.ID)
                                .Join(_context.ParticipantStatus,
                                    psh => psh.StatusID,
                                    ps => ps.ID,
                                    (psh, ps) => new ParticipantStatusViewModel
                                    {
                                        Username = username,
                                        StatusDescription = ps.Description,
                                        StartDate = psh.StartDate,
                                        EndDate = psh.EndDate
                                    }).AsEnumerable();
            return status;
        }
        public IEnumerable<TeamViewModel> GetParticipantTeams(string username)
        {
            //to be handled by teamservice
            int participantID = Int32.Parse(_context.ACCOUNT
                                    .FirstOrDefault(a => a.AD_Username == username)
                                    .gordon_id);
            var teams = _context.ParticipantTeam
                            .Where(pt => pt.ParticipantID == participantID)
                                .Join(_context.Team,
                                    pt => pt.TeamID,
                                    t => t.ID,
                                    (pt, t) => new TeamViewModel
                                    {
                                        ID = t.ID,
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
            var p = _context.Participant.AsEnumerable();
            var res = new List<ParticipantViewModel>();
            foreach (var user in p)
            {
                var stringID = $"{user.ID}";
                var account = (ParticipantViewModel)_context.ACCOUNT
                    .FirstOrDefault(a => a.gordon_id == stringID);
                res.Add(account);
            }        
            return res;
        }

        public async Task PostParticipant(int participantID)
        {
            await _context.Participant.AddAsync(new Participant
            {
                ID = participantID
            });
            await _context.ParticipantStatusHistory.AddAsync(new ParticipantStatusHistory
            {
                ParticipantID = participantID,
                StatusID = 4, //default to cleared
                StartDate = DateTime.Now,
                //No defined end date for creation
            });
            await _context.SaveChangesAsync();
        }
        public async Task UpdateParticipant(ParticipantPatchViewModel updatedParticipant)
        {
            int participantID = Int32.Parse(_context.ACCOUNT
                                                .FirstOrDefault(a => a.AD_Username == updatedParticipant.Username)
                                                .gordon_id);
            if (updatedParticipant.ActivityID is not null)
            {
                var participantActivity = _context.ParticipantActivity
                                            .FirstOrDefault(pa => pa.ParticipantID == participantID 
                                                && pa.ActivityID == updatedParticipant.ActivityID);
                var priv = _context.PrivType
                                .FirstOrDefault(pt => pt.Description == updatedParticipant.ActivityPrivType);

                participantActivity.PrivTypeID = priv is null 
                                                ? participantActivity.PrivTypeID
                                                : priv.ID;
                participantActivity.isFreeAgent = updatedParticipant.isFreeAgent ?? participantActivity.isFreeAgent;
                                                
            }
            if (updatedParticipant.TeamID is not null)
            {
                var participantTeam = _context.ParticipantTeam
                                        .FirstOrDefault(pt => pt.TeamID == updatedParticipant.TeamID
                                            && pt.ParticipantID == participantID);
                var role = _context.RoleType
                            .FirstOrDefault(pt => pt.Description == updatedParticipant.TeamRole);
            }
            await _context.SaveChangesAsync();
        }
        public async Task UpdateParticipantStatus(ParticipantStatusPatchViewModel participantStatus)
        {
            var status = new ParticipantStatusHistory
            {
                ParticipantID = Int32.Parse(_context.ACCOUNT
                                    .FirstOrDefault(a => a.AD_Username == participantStatus.Username)
                                    .gordon_id),
                StatusID = _context.ParticipantStatus
                                .FirstOrDefault(ps => ps.Description == participantStatus.StatusDescription)
                                .ID,
                StartDate = DateTime.Now,
                EndDate = participantStatus.EndDate
            };
            await _context.ParticipantStatusHistory.AddAsync(status);

            var prevStatus = _context.ParticipantStatusHistory
                                .Where(psh => psh.ParticipantID == status.ParticipantID)
                                .OrderByDescending(psh => psh.ID)
                                .FirstOrDefault();
            prevStatus.EndDate = DateTime.Now;

            await _context.SaveChangesAsync();
        }
    }

}

