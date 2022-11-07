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
using Gordon360.Authorization;

namespace Gordon360.Services.RecIM
{
    public class TeamService : ITeamService
    {
        private readonly CCTContext _context;
        private readonly IMatchService _matchService;

        public TeamService(CCTContext context, IMatchService matchService)
        {
            _context = context;
            _matchService = matchService;
        }

        public Task AddUserToTeam(int teamID, int participantID)
        {
            throw new NotImplementedException();
        }

        public TeamViewModel GetTeamByID(int teamID)
        {

            var team = _context.Team
                .Where(t => t.ID == teamID)
                .Select(t => new TeamViewModel
                {
                    ID = teamID,
                    Name = t.Name,
                    Status = _context.TeamStatus
                            .FirstOrDefault(s => s.ID == t.StatusID)
                            .Description,
                    Private = t.Private,
                    Logo = t.Logo,
                    Match = t.MatchTeam
                        .Select(mt => _matchService.GetMatchByID(mt.MatchID)),
                    Participant = t.ParticipantTeam
                        .Select(pt => new ParticipantViewModel
                        {
                            ADUserName = _context.ACCOUNT
                                .FirstOrDefault(a => a.account_id == pt.ParticipantID)
                                .AD_Username,
                            Email = _context.ACCOUNT
                                .FirstOrDefault(a => a.account_id == pt.ParticipantID)
                                 .email,
                            Role = _context.RoleType
                                .FirstOrDefault(rt => rt.ID == pt.RoleType)
                                .Description,
                        }),
                    MatchHistory = (from mt in _context.MatchTeam
                                    join opmt in _context.MatchTeam
                                    on mt.MatchID equals opmt.MatchID
                                    where mt.TeamID == teamID && mt.TeamID != opmt.TeamID
                                    select new TeamMatchHistoryViewModel
                                    {
                                        opponent = _context.Team
                                             .Where(opt => opt.ID == opmt.TeamID)
                                             .Select(opt => new TeamViewModel
                                             {
                                                 ID = opt.ID,
                                                 Name = opt.Name,
                                                 Logo = opt.Logo,
                                             }).FirstOrDefault(),
                                        OwnScore = mt.Score,
                                        OpposingScore = opmt.Score,
                                        Status = mt.Score > opmt.Score
                                                 ? "Win"
                                                 : mt.Score < opmt.Score
                                                     ? "Loss"
                                                     : "Tie",
                                        Time = _context.Match
                                             .FirstOrDefault(m => m.ID == mt.MatchID)
                                             .Time,
                                    }).OrderByDescending(history => history.Time).AsEnumerable(),
                    TeamRecord = t.SeriesTeam
                        .Select(st => new TeamRecordViewModel
                        {
                            ID = st.ID,
                            Name = t.Name,
                            Win = st.Win,
                            // for now Loss is calculated with query, just for no dummy data added to database
                            Loss = t.MatchTeam
                                .Where(mt => mt.Match.StatusID == 6
                                && mt.Score < _context.MatchTeam
                                .FirstOrDefault(opmt => opmt.MatchID == mt.MatchID
                                && opmt.TeamID != teamID)
                                .Score)
                                .Count(),
                            Tie = t.MatchTeam
                                .Where(mt => mt.Match.StatusID == 6
                                && mt.Score == _context.MatchTeam
                                .FirstOrDefault(opmt => opmt.MatchID == mt.MatchID
                                && opmt.TeamID != teamID)
                                .Score)
                                .Count(),
                        }),
                }).FirstOrDefault();
            return team;
        }

        public async Task<int> PostTeam(TeamUploadViewModel t, string adUsername)
        {
            var participantID = _context.ACCOUNT
                                .FirstOrDefault(a => a.AD_Username == adUsername)
                                .account_id;

            var team = new Team
            {
                Name = t.Name,
                StatusID = t.StatusID,
                ActivityID = t.ActivityID,
                Private = t.Private,
                Logo = t.Logo,
            };
            await _context.Team.AddAsync(team);
            await _context.SaveChangesAsync();

            await AddUserToTeam(team.ID, participantID);
            await UpdateParticipantRole(team.ID, participantID, 5);

            return team.ID;
        }

        public async Task UpdateParticipantRole(int teamID, int participantID, int participantRoleID)
        {
            var participantTeam = _context.ParticipantTeam.FirstOrDefault(pt => pt.ParticipantID == participantID && pt.TeamID == teamID);
            participantTeam.RoleType = participantRoleID;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateTeam(TeamPatchViewModel update)
        {
            var t = await _context.Team.FindAsync(update.ID);
            t.Name = update.Name ?? t.Name;
            t.StatusID = update.StatusID ?? t.StatusID;
            t.Private = update.Private;
            t.Logo = update.Logo ?? t.Logo;

            await _context.SaveChangesAsync();
        }
        public async Task AddUsersToTeam(int teamID, string adUsername)
        {
            var participantID = _context.ACCOUNT
                                .FirstOrDefault(a => a.AD_Username == adUsername)
                                .account_id;

            var participantTeam = new ParticipantTeam
            {
                TeamID = teamID,
                ParticipantID = participantID,
                SignDate = DateTime.Now,
                RoleType = 3, //default: 3 -> member
            };
            await _context.ParticipantTeam.AddAsync(participantTeam);

            await _context.SaveChangesAsync();
        }

        public bool IsTeamCaptain(string adUsername, int teamID)
        {
            var participantID = _context.ACCOUNT
                                .FirstOrDefault(a => a.AD_Username == adUsername)
                                .account_id;
            return _context.ParticipantTeam.FirstOrDefault(t => t.TeamID == teamID && t.ParticipantID == participantID).RoleType == 5;
        }
    }
}

