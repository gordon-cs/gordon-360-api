using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Models.CCT.Context;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gordon360.Authorization;
using Gordon360.Models.ViewModels;
using Microsoft.EntityFrameworkCore.Internal;
using System.Net;
using System.Net.Mail;
using System.Globalization;

namespace Gordon360.Services.RecIM
{
    public class TeamService : ITeamService
    {
        private readonly CCTContext _context;
        private readonly IMatchService _matchService;
        private readonly IParticipantService _participantService;
        private readonly IAccountService _accountService;
        private readonly IConfiguration _config;

        public TeamService(CCTContext context, IConfiguration config, IMatchService matchService, IParticipantService participantService, IAccountService accountService)
        {
            _context = context;
            _config = config;
            _matchService = matchService;
            _participantService = participantService;
            _accountService = accountService;
        }
        public IEnumerable<LookupViewModel> GetTeamLookup(string type)
        {
            if (type == "status")
            {
                var res = _context.TeamStatus
                    .Select(s => new LookupViewModel
                    {
                        ID = s.ID,
                        Description = s.Description
                    })
                    .AsEnumerable();
                return res;
            }
            if (type == "role")
            {
                var res = _context.RoleType
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
        public double GetTeamSportsmanshipScore(int teamID)
        {
            var sportsmanshipScores = _context.Match
                                    .Where(m => m.StatusID == 6)
                                        .Join(_context.MatchTeam
                                            .Where(mt => mt.TeamID == teamID),
                                            m => m.ID,
                                            mt => mt.MatchID,
                                            (m, mt) => new { score = mt.Sportsmanship }
                                        );
            if (sportsmanshipScores.Count() == 0)
            {
                return 5;
            }
            return sportsmanshipScores.Average(ss => ss.score);
        }
        public IEnumerable<TeamExtendedViewModel> GetTeams(bool active)
        {
            var teamQuery = active 
                ? _context.Team.Where(t => !t.Activity.Completed == active)
                : _context.Team;    

            var teams = teamQuery
                .Select(t => new TeamExtendedViewModel
                {
                    ID = t.ID,
                    Activity = _context.Activity.FirstOrDefault(a => a.ID == t.ActivityID),
                    Name = t.Name,
                    Status = _context.TeamStatus
                                            .FirstOrDefault(ts => ts.ID == t.StatusID)
                                            .Description,
                    Logo = t.Logo,
                    Participant = t.ParticipantTeam
                        .Select(pt => new ParticipantExtendedViewModel
                        {
                            Username = pt.ParticipantUsername,
                            Email = _accountService.GetAccountByUsername(pt.ParticipantUsername).Email,
                            Role = _context.RoleType
                                                .FirstOrDefault(rt => rt.ID == pt.RoleTypeID)
                                                .Description,
                        }),
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
                                                    && opmt.TeamID != t.ID)
                                                    .Score)
                                                    .Count(),
                                                Tie = t.MatchTeam
                                                    .Where(mt => mt.Match.StatusID == 6
                                                    && mt.Score == _context.MatchTeam
                                                    .FirstOrDefault(opmt => opmt.MatchID == mt.MatchID
                                                    && opmt.TeamID != t.ID)
                                                    .Score)
                                                    .Count(),
                                            }).AsEnumerable(),
                    Sportsmanship = _context.Match
                                    .Where(m => m.StatusID == 6)
                                        .Join(_context.MatchTeam
                                            .Where(mt => mt.TeamID == t.ID),
                                            m => m.ID,
                                            mt => mt.MatchID,
                                            (m, mt) => new { score = mt.Sportsmanship }
                                        ).Count() == 0 
                                        ? 5
                                        : _context.Match
                                    .Where(m => m.StatusID == 6)
                                        .Join(_context.MatchTeam
                                            .Where(mt => mt.TeamID == t.ID),
                                            m => m.ID,
                                            mt => mt.MatchID,
                                            (m, mt) => new { score = mt.Sportsmanship }
                                        ).Average(ss => ss.score) 
                })
                .AsEnumerable();
            return teams;
        }
        public TeamExtendedViewModel GetTeamByID(int teamID)
        {
            var team = _context.Team
                            .Where(t => t.ID == teamID)
                            .Select(t => new TeamExtendedViewModel
                            {
                                ID = teamID,
                                Activity = _context.Activity.FirstOrDefault(a => a.ID == t.ActivityID),
                                Name = t.Name,
                                Status = _context.TeamStatus
                                            .FirstOrDefault(ts => ts.ID == t.StatusID)
                                            .Description,
                                Logo = t.Logo,
                                Match = t.MatchTeam
                                            .Select(mt => _matchService.GetMatchForTeamByMatchID(mt.MatchID)),
                                Participant = t.ParticipantTeam
                                                .Select(pt => new ParticipantExtendedViewModel
                                                {
                                                    Username = pt.ParticipantUsername,
                                                    Email = _accountService.GetAccountByUsername(pt.ParticipantUsername).Email,
                                                    Role = _context.RoleType
                                                                        .FirstOrDefault(rt => rt.ID == pt.RoleTypeID)
                                                                        .Description,
                                                }),
                                MatchHistory = _context.Match
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
                                                                OwnScore = mt0.Score,
                                                                OpposingTeamID = mt1.TeamID,
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
                                                                Opponent = _context.Team.Where(t => t.ID == matchTeamJoin.OpposingTeamID)
                                                                    .Select(o => new TeamExtendedViewModel
                                                                    {
                                                                        ID = o.ID,
                                                                        Name = o.Name,
                                                                        Logo = o.Logo
                                                                    }).FirstOrDefault(),
                                                                TeamScore = matchTeamJoin.OwnScore,
                                                                OpposingTeamScore = matchTeamJoin.OpposingTeamScore,
                                                                Status = matchTeamJoin.Status,
                                                                MatchStatusID = match.StatusID,
                                                                Time = match.Time
                                                            }
                                                ).AsEnumerable(),
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
                                            }).AsEnumerable(),
                                Sportsmanship = GetTeamSportsmanshipScore(teamID)



                            }).FirstOrDefault();
            return team;
        }

        // return type is wrong
        public IEnumerable<TeamInviteViewModel> GetTeamInvites(string username)
        {
            var teamRequestToJoin = _context.ParticipantTeam
                    .Where(pt => pt.ParticipantUsername == username && pt.RoleTypeID == 2)
                    .Join(_context.Team
                        .Join(_context.Activity,
                            t => t.ActivityID,
                            a => a.ID,
                            (t, a) => new
                            {
                                ActivityID = t.ActivityID,
                                ActivityName = a.Name,
                                TeamID = t.ID,
                                TeamName = t.Name,
                            }
                        ),
                        pt => pt.TeamID,
                        t => t.TeamID,
                        (pt, t) => new TeamInviteViewModel
                        {
                            ActivityID = t.ActivityID,
                            ActivityName = t.ActivityName,
                            TeamID = t.TeamID,
                            TeamName = t.TeamName,
                        }
                    )
                    .AsEnumerable();

            return teamRequestToJoin;
        }
        
        public async Task<TeamViewModel> PostTeamAsync(TeamUploadViewModel t, string username)
        {
            
            var team = new Team
            {
                Name = t.Name,
                StatusID = 1,
                ActivityID = t.ActivityID,
                Logo = t.Logo,
            };
            await _context.Team.AddAsync(team);
            await _context.SaveChangesAsync();

            var captain = new ParticipantTeamUploadViewModel
            {
                Username = username,
                RoleTypeID = 5
            };
            await AddParticipantToTeamAsync("",team.ID, captain);

            var existingSeries = _context.Series.Where(s => s.ActivityID == t.ActivityID).OrderBy(s => s.StartDate)?.FirstOrDefault();
            if (existingSeries is not null) {
                var seriesTeam = new SeriesTeam
                {
                    TeamID = team.ID,
                    SeriesID = existingSeries.ID,
                    Win = 0,
                    Loss = 0,
                };
                await _context.SeriesTeam.AddAsync(seriesTeam);
                await _context.SaveChangesAsync();
            }

            return team;
        }

        public async Task<ParticipantTeamViewModel> UpdateParticipantRoleAsync(int teamID, ParticipantTeamUploadViewModel participant)
        {
            var participantTeam = _context.ParticipantTeam.FirstOrDefault(pt => pt.ParticipantUsername == participant.Username && pt.TeamID == teamID);
            var roleID = participant.RoleTypeID ?? 3;

            //if user is currently requested to join and have just accepted to join the team, user should have other instances of themselves removed from other teams
            if (participantTeam.RoleTypeID == 2 && roleID == 3)
            {
                var activityID = _context.Team.FirstOrDefault(t => t.ID == teamID).ActivityID;
                var otherInstances = _context.ParticipantTeam
                    .Where(pt => pt.ID != participantTeam.ID && pt.ParticipantUsername == participantTeam.ParticipantUsername)
                    .Join(_context.Team.Where(t => t.ActivityID == activityID),
                    pt => pt.TeamID,
                    t => t.ID,
                    (pt, t) => pt)
                    .ToList();
                _context.ParticipantTeam.RemoveRange(otherInstances);
            }

            participantTeam.RoleTypeID = roleID;
            await _context.SaveChangesAsync();

            return participantTeam;
        }
        public async Task DeleteTeamParticipantAsync(int teamID, string username)
        {
            var teamParticipant = _context.ParticipantTeam.FirstOrDefault(pt => pt.TeamID == teamID && pt.ParticipantUsername == username);
            _context.ParticipantTeam.Remove(teamParticipant);
            await _context.SaveChangesAsync();
        }

        public async Task<TeamViewModel> UpdateTeamAsync(int teamID, TeamPatchViewModel update)
        {
            var t = await _context.Team.FindAsync(teamID);
            t.Name = update.Name ?? t.Name;
            t.StatusID = update.StatusID ?? t.StatusID;
            t.Logo = update.Logo ?? t.Logo;

            await _context.SaveChangesAsync();

            return t;
        }

        private async Task SendInviteEmail(int teamID, string inviteeUsername, string inviterUsername)
        {
     
        }
        
        public async Task<ParticipantTeamViewModel> AddParticipantToTeamAsync(string inviterUsername, int teamID, ParticipantTeamUploadViewModel participant)
        {
            var participantTeam = new ParticipantTeam
            {
                TeamID = teamID,
                ParticipantUsername = participant.Username,
                SignDate = DateTime.Now,
                RoleTypeID = participant.RoleTypeID ?? 3, //default: 3 -> member
            };
            await _context.ParticipantTeam.AddAsync(participantTeam);
            await _context.SaveChangesAsync();
            
            if (participant.RoleTypeID == 2) //if this is an invite, send an email
            {
                await SendInviteEmail(teamID, participant.Username, inviterUsername);
            }
            return participantTeam;
        }

        public bool HasUserJoined(int activityID, string username)
        {
            // get all the partipantTeam from the teams with the activityID
            var participantTeams = _context.Activity
                        .Where(a => a.ID == activityID)
                        .Join(_context.Team
                            .Join(_context.ParticipantTeam.Where(pt => pt.RoleTypeID % 6 > 2),
                                t => t.ID,
                                pt => pt.TeamID,
                                (t, pt) => new {
                                    ActivityID = activityID,
                                    TeamName = t.Name,
                                    Participant = pt.ParticipantUsername,

                                }),
                            a => a.ID,
                            t => t.ActivityID,
                            (a, t) => new {
                                teamName = t.TeamName,
                                Participant = t.Participant,
                            }
                        ).AsEnumerable();

            return participantTeams.Any(pt => pt.Participant == username);
        }
        public bool IsTeamCaptain(string username, int teamID)
        {
            return _context.ParticipantTeam.Any(t =>
                        t.TeamID == teamID
                        && t.ParticipantUsername == username
                        && (
                            t.RoleTypeID == 5       // RoleType: 5 => captain
                            || t.RoleTypeID == 4    // RoleType: 4 => co-captain
                        )
            );
        }
    }
}

