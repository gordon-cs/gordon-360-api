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
using Gordon360.Models.ViewModels;

namespace Gordon360.Services.RecIM
{
    public class TeamService : ITeamService
    {
        private readonly CCTContext _context;
        private readonly IMatchService _matchService;
        private readonly IParticipantService _participantService;
        private readonly IAccountService _accountService;


        public TeamService(CCTContext context, IMatchService matchService, IParticipantService participantService, IAccountService accountService)
        {
            _context = context;
            _matchService = matchService;
            _participantService = participantService;
            _accountService = accountService;
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

        public TeamViewModel GetTeamByID(int teamID)
        {
            var team = _context.Team
                            .Where(t => t.ID == teamID)
                            .Select(t => new TeamViewModel
                            {
                                ID = teamID,
                                ActivityID = t.ActivityID,
                                Name = t.Name,
                                Status = _context.TeamStatus
                                            .FirstOrDefault(ts => ts.ID == t.StatusID)
                                            .Description,
                                Logo = t.Logo,
                                Match = t.MatchTeam
                                            .Select(mt => _matchService.GetMatchByID(mt.MatchID)),
                                Participant = t.ParticipantTeam
                                                .Select(pt => new ParticipantViewModel
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
                                            OwnID = mt0.TeamID,
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
                                    OwnID = matchTeamJoin.OwnID,
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
                                    MatchStatusID = match.StatusID,
                                    Time = match.Time
                                }).AsEnumerable(),
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

        public async Task<TeamCreatedViewModel> PostTeamAsync(TeamUploadViewModel t, string username)
        {
            var participantID = Int32.Parse(_accountService.GetAccountByUsername(username).GordonID);
            // return null if ParticipantActivity contains an instance of both t.ActivityID & participantID
            // ^handle this in the future (unable to implement right now as this would lock our accounts out)
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
            await AddUserToTeamAsync(team.ID, captain);

            return team;
        }

        public async Task<ParticipantTeamViewModel> UpdateParticipantRoleAsync(int teamID, ParticipantTeamUploadViewModel participant)
        {
            var participantTeam = _context.ParticipantTeam.FirstOrDefault(pt => pt.ParticipantUsername == participant.Username && pt.TeamID == teamID);
            participantTeam.RoleTypeID = participant.RoleTypeID ?? 3;

            await _context.SaveChangesAsync();

            return participantTeam;
        }

        public async Task<TeamCreatedViewModel> UpdateTeamAsync(int teamID, TeamPatchViewModel update)
        {
            var t = await _context.Team.FindAsync(teamID);
            t.Name = update.Name ?? t.Name;
            t.StatusID = update.StatusID ?? t.StatusID;
            t.Logo = update.Logo ?? t.Logo;

            await _context.SaveChangesAsync();

            return t;
        }
        public async Task<ParticipantTeamViewModel> AddUserToTeamAsync(int teamID, ParticipantTeamUploadViewModel participant)
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

            return participantTeam;
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

