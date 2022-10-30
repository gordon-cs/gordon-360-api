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
using Match = Gordon360.Models.CCT.Match;

namespace Gordon360.Services.RecIM
{
    public class MatchService : IMatchService
    {
        private readonly CCTContext _context;

        public MatchService(CCTContext context)
        {

            _context = context;
        }
        public MatchViewModel GetMatchByID(int matchID)
        {
            var match = _context.Match
                        .Where(m => m.ID == matchID)
                        .Select(m => new MatchViewModel
                        {
                            ID = matchID,
                            Time = m.Time,
                            Surface = _context.Surface
                                        .FirstOrDefault(s => s.ID == m.SurfaceID)
                                        .Description,
                            Status = _context.MatchStatus
                                        .FirstOrDefault(ms => ms.ID == m.StatusID)
                                        .Description,
                            Attendance = _context.MatchParticipant
                                        .Where(mp => mp.MatchID == matchID)
                                        .Select(mp => new ParticipantViewModel
                                        {
                                            ADUserName = _context.ACCOUNT
                                                        .FirstOrDefault(a => a.account_id == mp.ParticipantID)
                                                        .AD_Username
                                        }).AsEnumerable(),
                            // Team will eventually be handled by TeamService 
                            Team = m.MatchTeam.Select(mt => new TeamViewModel
                            {
                                ID = mt.TeamID,
                                Name = _context.Team
                                   .FirstOrDefault(t => t.ID == mt.TeamID)
                                   .Name,
                                Participant = mt.Team.ParticipantTeam
                                    .Join(_context.ACCOUNT,
                                            pt => pt.ParticipantID,
                                            a => a.account_id,
                                            (pt,a) => new ParticipantViewModel
                                            {
                                                ADUserName = a.AD_Username,
                                                Email = a.email,
                                                Role = _context.RoleType
                                                .FirstOrDefault(rt => rt.ID ==pt.RoleType)
                                                .Description
                                            }),
                                MatchHistory = _context.Match.Where(m0 => m0.StatusID == 6)//6 is completed
                                    .Join(_context.MatchTeam.Where(q => q.TeamID == mt.TeamID).Join(
                                            _context.MatchTeam,
                                            mt0 => mt0.MatchID,
                                            mt1 => mt1.MatchID,
                                            (mt0,mt1) => new
                                            {
                                                MatchID = mt0.MatchID,
                                                OwnScore = mt0.Score,
                                                OpposingID = mt1.TeamID,
                                                OpposingScore = mt1.Score,
                                                Status = mt0.Score > mt1.Score 
                                                        ? "Win"
                                                        : mt0.Score < mt1.Score
                                                            ? "Lose"
                                                            : "Tie"
                                            }),
                                        match => match.ID,
                                        matchTeamJoin => matchTeamJoin.MatchID,
                                        (match,matchTeamJoin) => new TeamMatchHistoryViewModel
                                        {
                                            opponent = _context.Team.Where(t => t.ID == matchTeamJoin.OpposingID)
                                                .Select(o => new TeamViewModel
                                                {
                                                    ID = o.ID,
                                                    Name = o.Name,
                                                    Logo = o.Logo
                                                }).FirstOrDefault(),
                                            OwnScore = matchTeamJoin.OwnScore,
                                            OpposingScore = matchTeamJoin.OpposingScore,
                                            Status = matchTeamJoin.Status,
                                            Time = match.Time
                                        })
                                    .OrderByDescending(history => history.Time)
                                    .Take(5),
                                TeamRecord = _context.SeriesTeam
                                           .Where(st => st.SeriesID == m.SeriesID && st.TeamID == mt.TeamID)
                                           .Select(st => new TeamRecordViewModel
                                           {
                                               Win = st.Win,
                                               Loss = st.Loss ?? 0,
                                           })
                            }),
                        }).FirstOrDefault();
            return match; 
        }
        public IEnumerable<MatchViewModel> GetMatchBySeriesID(int seriesID)
        {
            var match = _context.Match
                        .Where(m => m.SeriesID == seriesID)
                        .Select(m => new MatchViewModel
                        {
                            ID = m.ID,
                            Time = m.Time,
                            Surface = _context.Surface
                                        .FirstOrDefault(s => s.ID == m.SurfaceID)
                                        .Description,
                            Status = _context.MatchStatus
                                        .FirstOrDefault(ms => ms.ID == m.StatusID)
                                        .Description,
                            Team = m.MatchTeam.Select(mt => new TeamViewModel
                            {
                                ID = mt.TeamID,
                                Name = _context.Team
                                   .FirstOrDefault(t => t.ID == mt.TeamID)
                                   .Name,
                                TeamRecord = _context.SeriesTeam
                                           .Where(st => st.SeriesID == m.SeriesID && st.TeamID == mt.TeamID)
                                           .Select(st => new TeamRecordViewModel
                                           {
                                               Win = st.Win,
                                               Loss = st.Loss ?? 0,
                                           })
                            })
                        });
            return match;
        }
        public async Task PostMatch(CreateMatchViewModel m)
        {
            var match = new Match
            {
                SeriesID = m.SeriesID,
                Time = m.Time,
                SurfaceID = m.SurfaceID ?? 0, //unknown surface id
                StatusID = 1 //default unconfirmed
            };
            await _context.Match.AddAsync(match);
            await _context.SaveChangesAsync();

            foreach(var teamID in m.TeamIDs)
            {
                await CreateMatchTeamMapping(teamID, match.ID);
            }
            await _context.SaveChangesAsync();
        }

        private async Task CreateMatchTeamMapping(int teamID, int matchID)
        {
            var matchTeam = new MatchTeam
            {
                TeamID = teamID,
                MatchID = matchID,
                StatusID = 1, //default unconfirmed
                Score = 0,
                Sportsmanship = 5 //default max
            };
            await _context.MatchTeam.AddAsync(matchTeam);
        }
        public async Task AddParticipantAttendance(int participantID, int matchID)
        {
            var matchParticipant = new MatchParticipant
            {
                ParticipantID = participantID,
                MatchID = matchID
            };
            await _context.MatchParticipant.AddAsync(matchParticipant);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateTeamStats(UpdateMatchTeamViewModel vm)
        {
            var teamstats = _context.MatchTeam.FirstOrDefault(mt => mt.MatchID == vm.MatchID && mt.TeamID == vm.TeamID);
            teamstats.Score = vm.Score ?? teamstats.Score;
            teamstats.Sportsmanship = vm.Sportsmanship ?? teamstats.Sportsmanship;

            if (vm.Status is not null)
            {
                teamstats.StatusID = _context.MatchTeamStatus
                    .FirstOrDefault(mts => mts.Description == vm.Status)
                    .ID;
            }
            await _context.SaveChangesAsync();
        }
    }

}

