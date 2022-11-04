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
using System.Text.RegularExpressions;

namespace Gordon360.Services.RecIM
{
    public class ParticipantService : IParticipantService
    {
        private readonly CCTContext _context;

        public ParticipantService(CCTContext context)
        {

            _context = context;
        }

        public ParticipantViewModel GetParticipantByID(int participantID)
        {
            var participant = _context.ACCOUNT
                                .Join(_context.Participant
                                    .Where(p => p.ID == participantID),
                                a => Int32.Parse(a.gordon_id),
                                p => p.ID,
                                (a, p) => new ParticipantViewModel
                                {
                                    Username = a.AD_Username,
                                    Email = a.email,
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
        public IEnumerable<ParticipantStatusViewModel> GetParticipantStatusHistory(int participantID)
        {
            var status = _context.ParticipantStatusHistory
                            .Where(psh => psh.ParticipantID == participantID)
                            .OrderByDescending(psh => psh.ID)
                                .Join(_context.ParticipantStatus,
                                    psh => psh.StatusID,
                                    ps => ps.ID,
                                    (psh, ps) => new ParticipantStatusViewModel
                                    {
                                        Username = _context.ACCOUNT
                                                    .FirstOrDefault(a => Int32.Parse(a.gordon_id) == participantID)
                                                    .AD_Username,
                                        StatusDescription = ps.Description,
                                        StartDate = psh.StartDate,
                                        EndDate = psh.EndDate
                                    }).AsEnumerable();
            return status;
        }
        public IEnumerable<TeamViewModel> GetParticipantTeams(int participantID)
        {
            //to be handled by teamservice
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
            var participants = _context.ACCOUNT
                                .Join(_context.Participant,
                                a => Int32.Parse(a.gordon_id),
                                p => p.ID,
                                (a, p) =>  (ParticipantViewModel)a);
            
            return participants;
        }

        public async Task PostParticipant(int participantID)
        {
            await _context.AddAsync(new Participant
            {
                ID = participantID
            });
            _context.SaveChangesAsync();
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

