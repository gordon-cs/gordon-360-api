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

        public TeamViewModel? GetTeamByID(int teamID)
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

        public Task PostTeam(Team team)
        {
            throw new NotImplementedException();
        }

        public Task UpdateParticipantRole(int teamID, int participantID, RoleType participantRole)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTeam(Team updatedTeam)
        {
            throw new NotImplementedException();
        }
    }
}
