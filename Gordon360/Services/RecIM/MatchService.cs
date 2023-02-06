﻿using Gordon360.Models.CCT;
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
using Microsoft.AspNetCore.Server.IIS;

namespace Gordon360.Services.RecIM
{
    public class MatchService : IMatchService
    {
        private readonly CCTContext _context;
        private readonly IAccountService _accountService;


        public MatchService(CCTContext context,  IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        public IEnumerable<LookupViewModel> GetMatchLookup(string type)
        {
            switch (type)
            {
                case "status":
                {
                    var res = _context.MatchStatus
                        .Select(s => new LookupViewModel
                        {
                            ID = s.ID,
                            Description = s.Description
                        })
                        .AsEnumerable();
                    return res;
                }
                case "teamstatus":
                {
                    var res = _context.MatchTeamStatus
                        .Select(s => new LookupViewModel
                        {
                            ID = s.ID,
                            Description = s.Description
                        })
                        .AsEnumerable();
                    return res;
                }
                case "surface":
                {
                    var res = _context.Surface
                        .Select(s => new LookupViewModel
                        {
                            ID = s.ID,
                            Description = s.Description
                        })
                        .AsEnumerable();
                    return res;
                }
                default:
                    return null;
            }
        }

        //this function is used because ASP somehow refuses to cast IEnumerables or recognize IEnumerables
        //within other queries. The only solution is to return each individual instance and have the original
        //query handle the enumeration.
        public IEnumerable<MatchExtendedViewModel> GetMatchesForTeamID(int teamID)
        {
            // This query returns the activityID and activity name of the team by teamID
            var activity = (from a in _context.Activity
                           where a.ID == (from t in _context.Team
                                          where t.ID == teamID
                                          select t
                                          )
                                          .FirstOrDefault()
                                          .ActivityID
                           select new ActivityExtendedViewModel
                           {
                               ID = a.ID,
                               Name = a.Name,
                           })
                           .FirstOrDefault();

            // This query returns a list of matches the team is involved by teamID
            var match = from t in _context.Team
                            join mt in _context.MatchTeam
                            on t.ID equals mt.TeamID
                            where t.ID == teamID
                         select new MatchExtendedViewModel
                         {
                             ID = mt.MatchID,
                             Scores = (from s in _context.MatchTeam
                                       where s.MatchID == mt.MatchID
                                       select new TeamMatchHistoryViewModel
                                       {
                                           TeamID = mt.TeamID,
                                           TeamScore = mt.Score,
                                       })
                                       .AsEnumerable(),
                             Status = (from ms in _context.MatchStatus
                                       where ms.ID == (from m in _context.Match
                                                       where m.ID == mt.MatchID
                                                       select m
                                                       )
                                                       .FirstOrDefault()
                                                       .StatusID
                                       select ms
                                       )
                                       .FirstOrDefault()
                                       .Description,
                             Team = (from _mt in _context.MatchTeam
                                     where _mt.MatchID == mt.MatchID
                                     select new TeamExtendedViewModel
                                     {
                                        ID = mt.TeamID,
                                         Name = (from t in _context.Team
                                                 where t.ID == mt.TeamID
                                                 select t
                                                 )
                                                 .FirstOrDefault()
                                                 .Name,
                                     })
                                     .AsEnumerable(),
                             Activity = activity,
                         };

            return match;
        }
 
        public MatchExtendedViewModel GetMatchByID(int matchID)
        {
            var match = _context.Match
                        .Where(m => m.ID == matchID)
                        .Select(m => new MatchExtendedViewModel
                        {
                            ID = matchID,
                            Scores = _context.MatchTeam
                                        .Where(mt => mt.MatchID == matchID)
                                        .Select(mt => new TeamMatchHistoryViewModel
                                        {
                                            TeamID = mt.TeamID,
                                            TeamScore = mt.Score
                                        }).AsEnumerable(),
                            Time = m.Time,
                            Surface = _context.Surface
                                        .FirstOrDefault(s => s.ID == m.SurfaceID)
                                        .Description,
                            Status = _context.MatchStatus
                                        .FirstOrDefault(ms => ms.ID == m.StatusID)
                                        .Description,
                            Attendance = _context.MatchParticipant
                                        .Where(mp => mp.MatchID == matchID)
                                        .Select(mp => new ParticipantExtendedViewModel
                                        {
                                            Username = mp.ParticipantUsername
                                        }).AsEnumerable(),
                            Activity = _context.Activity
                                        .Where(a => a.ID == _context.Series
                                                        .FirstOrDefault(s => s.ID == _context.Match
                                                                                    .FirstOrDefault(m => m.ID == matchID)
                                                        .SeriesID)
                                        .ActivityID)
                                        .Select(a => new ActivityExtendedViewModel
                                        {
                                            ID = a.ID,
                                            Name = a.Name
                                        })
                                        .FirstOrDefault(),
                            // Team will eventually be handled by TeamService 
                            Team = m.MatchTeam.Select(mt => new TeamExtendedViewModel
                            {
                                ID = mt.TeamID,
                                Name = _context.Team
                                   .FirstOrDefault(t => t.ID == mt.TeamID)
                                   .Name,
                                Participant = mt.Team.ParticipantTeam
                                    .Select(pt => new ParticipantExtendedViewModel
                                            {
                                                Username = pt.ParticipantUsername,
                                                Email = _accountService.GetAccountByUsername(pt.ParticipantUsername).Email,
                                                Role = _context.RoleType
                                                .FirstOrDefault(rt => rt.ID == pt.RoleTypeID)
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
                                                    TeamID = mt0.TeamID,
                                                    MatchID = mt0.MatchID,
                                                    Score = mt0.Score,
                                                    OpposingID = mt1.TeamID,
                                                    OpposingTeamScore = mt1.Score,
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
                                            TeamID = matchTeamJoin.TeamID,
                                            MatchID = match.ID,
                                            Opponent = _context.Team.Where(t => t.ID == matchTeamJoin.OpposingID)
                                                .Select(o => new TeamExtendedViewModel
                                                {
                                                    ID = o.ID,
                                                    Name = o.Name,
                                                    Logo = o.Logo
                                                }).FirstOrDefault(),
                                            TeamScore = matchTeamJoin.Score,
                                            OpposingTeamScore = matchTeamJoin.OpposingTeamScore,
                                            Status = matchTeamJoin.Status,
                                            MatchStatusID = match.StatusID,
                                            Time = match.Time
                                        })
                                    .OrderByDescending(mh => mh.Time)
                                    .Take(5),
                                TeamRecord = _context.SeriesTeam
                                           .Where(st => st.SeriesID == m.SeriesID && st.TeamID == mt.TeamID)
                                           .Select(st => new TeamRecordViewModel
                                           {
                                               Win = st.Win,
                                               Loss = st.Loss,
                                           }),
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
                                            TeamID = mt0.TeamID,
                                            MatchID = mt0.MatchID,
                                            Score = mt0.Score,
                                            OpposingID = mt1.TeamID,
                                            OpposingTeamScore = mt1.Score,
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
                                    TeamID = matchTeamJoin.TeamID,
                                    MatchID = match.ID,
                                    Opponent = _context.Team.Where(t => t.ID == matchTeamJoin.OpposingID)
                                        .Select(o => new TeamExtendedViewModel
                                        {
                                            ID = o.ID,
                                            Name = o.Name,
                                            Logo = o.Logo
                                        }).FirstOrDefault(),
                                    TeamScore = matchTeamJoin.Score,
                                    OpposingTeamScore = matchTeamJoin.OpposingTeamScore,
                                    Status = matchTeamJoin.Status,
                                    MatchStatusID = match.StatusID,
                                    Time = match.Time
                                }).AsEnumerable();
            return vm;
        }
        public IEnumerable<MatchExtendedViewModel> GetMatchBySeriesID(int seriesID)
        {
            var match = _context.Match
                        .Where(m => m.SeriesID == seriesID)
                        .Select(m => new MatchExtendedViewModel
                        {
                            ID = m.ID,
                            Time = m.Time,
                            Surface = _context.Surface
                                        .FirstOrDefault(s => s.ID == m.SurfaceID)
                                        .Description,
                            Status = _context.MatchStatus
                                        .FirstOrDefault(ms => ms.ID == m.StatusID)
                                        .Description,
                            Team = m.MatchTeam.Select(mt => new TeamExtendedViewModel
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
                                               Loss = st.Loss,
                                           })
                            })
                        });
            return match;
        }
        public async Task<MatchViewModel> PostMatchAsync(MatchUploadViewModel m)
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
                await CreateMatchTeamMappingAsync(teamID, match.ID);
            }
            await _context.SaveChangesAsync();
            return match;
        }

        private async Task CreateMatchTeamMappingAsync(int teamID, int matchID)
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
        public async Task<MatchParticipantViewModel> AddParticipantAttendanceAsync(string username, int matchID)
        {
            var matchParticipant = new MatchParticipant
            {
                ParticipantUsername = username,
                MatchID = matchID
            };
            await _context.MatchParticipant.AddAsync(matchParticipant);
            await _context.SaveChangesAsync();
            return matchParticipant;
        }
        public async Task<MatchTeamViewModel> UpdateTeamStatsAsync(int matchID, MatchStatsPatchViewModel vm)
        {
            var teamstats = _context.MatchTeam.FirstOrDefault(mt => mt.MatchID == matchID && mt.TeamID == vm.TeamID);
            teamstats.Score = vm.Score ?? teamstats.Score;
            teamstats.Sportsmanship = vm.Sportsmanship ?? teamstats.Sportsmanship;

            if (vm.Status is not null)
            {
                teamstats.StatusID = _context.MatchTeamStatus
                    .FirstOrDefault(mts => mts.Description == vm.Status)
                    .ID;
            }
            await _context.SaveChangesAsync();
            return teamstats;

        }
        public async Task<MatchViewModel> UpdateMatchAsync(int matchID, MatchPatchViewModel vm)
        {
            var match = _context.Match.FirstOrDefault(m => m.ID == matchID);
            match.Time = vm.Time ?? match.Time;
            match.StatusID = vm.StatusID ?? match.StatusID;
            match.SurfaceID = vm.SurfaceID ?? match.SurfaceID;
            await _context.SaveChangesAsync();
            return match;
        }
    }

}

