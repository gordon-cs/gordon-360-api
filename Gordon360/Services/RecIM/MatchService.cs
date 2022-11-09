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
using Azure.Identity;

namespace Gordon360.Services.RecIM
{
    public class MatchService : IMatchService
    {
        private readonly CCTContext _context;
        private readonly IParticipantService _participantService;


        public MatchService(CCTContext context, IParticipantService participantService)
        {
            _context = context;
            _participantService = participantService;
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
                                            Username = _participantService.GetAccountByParticipantID(mp.ParticipantID).AD_Username
                                        }).AsEnumerable(),
                            // Team will eventually be handled by TeamService 
                            Team = m.MatchTeam.Select(mt => new TeamViewModel
                            {
                                ID = mt.TeamID,
                                Name = _context.Team
                                   .FirstOrDefault(t => t.ID == mt.TeamID)
                                   .Name,
                                Participant = mt.Team.ParticipantTeam
                                    .Select(pt => new ParticipantViewModel
                                            {
                                                Username = _participantService.GetAccountByParticipantID(pt.ParticipantID).AD_Username,
                                                Email = _participantService.GetAccountByParticipantID(pt.ParticipantID).email,
                                                Role = _context.RoleType
                                                .FirstOrDefault(rt => rt.ID ==pt.RoleType)
                                                .Description
                                            }),
                                MatchHistory = _context.Match
                                    .Where(mh => mh.StatusID == 6)
                                        .Join(_context.MatchTeam
                                            .Where(matchteam => matchteam.TeamID == mt.TeamID)
                                            .Join(
                                                _context.MatchTeam.Where(matchteam => matchteam.TeamID != mt.TeamID),
                                                mt0 => mt0.MatchID,
                                                mt1 => mt1.MatchID,
                                                (mt0, mt1) => new
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
                                        (match, matchTeamJoin) => new TeamMatchHistoryViewModel
                                        {
                                            MatchID = match.ID,
                                            Opponent = _context.Team.Where(t => t.ID == matchTeamJoin.OpposingID)
                                                .Select(o => new TeamViewModel
                                                {
                                                    ID = o.ID,
                                                    Name = o.Name,
                                                    Logo = o.Logo
                                                }).FirstOrDefault(),
                                            OwnScore = matchTeamJoin.OwnScore,
                                            OpposingScore = matchTeamJoin.OpposingScore,
                                            Status = matchTeamJoin.Status,
                                            MatchStatusID = match.StatusID ?? 1,
                                            Time = match.Time
                                        })
                                    .OrderByDescending(mh => mh.Time)
                                    .Take(5),
                                TeamRecord = _context.SeriesTeam
                                           .Where(st => st.SeriesID == m.SeriesID && st.TeamID == mt.TeamID)
                                           .Select(st => new TeamRecordViewModel
                                           {
                                               Win = st.Win,
                                               Loss = st.Loss ?? 0,
                                           })
                            })
                        }).FirstOrDefault();
            return match; 
        }
       
        public IEnumerable<TeamMatchHistoryViewModel> GetMatchHistoryByTeamID(int teamID)
        {
            var vm = _context.Match
                            .Join(_context.MatchTeam
                                .Where(mt => mt.TeamID == teamID)

                                    .Join(
                                        _context.MatchTeam.Where(mt => mt.TeamID != teamID),
                                        mt0 => mt0.MatchID,
                                        mt1 => mt1.MatchID,
                                        (mt0, mt1) => new
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
                                (match, matchTeamJoin) => new TeamMatchHistoryViewModel
                                {
                                    MatchID = match.ID,
                                    Opponent = _context.Team.Where(t => t.ID == matchTeamJoin.OpposingID)
                                        .Select(o => new TeamViewModel
                                        {
                                            ID = o.ID,
                                            Name = o.Name,
                                            Logo = o.Logo
                                        }).FirstOrDefault(),
                                    OwnScore = matchTeamJoin.OwnScore,
                                    OpposingScore = matchTeamJoin.OpposingScore,
                                    Status = matchTeamJoin.Status,
                                    MatchStatusID = match.StatusID ?? 1,
                                    Time = match.Time
                                });
            return vm;
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
        public async Task PostMatch(MatchUploadViewModel m)
        {
            var match = new Match
            {
                SeriesID = m.SeriesID,
                Time = m.StartTime,
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
        public async Task UpdateTeamStats(MatchTeamPatchViewModel vm)
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

