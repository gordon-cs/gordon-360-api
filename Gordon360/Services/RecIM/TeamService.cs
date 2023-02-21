using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Team = Gordon360.Models.CCT.Team;
using Gordon360.Models.CCT.Context;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Globalization;
using Microsoft.Graph;
using Microsoft.AspNetCore.Server.IIS.Core;

namespace Gordon360.Services.RecIM
{
    public class TeamService : ITeamService
    {
        private readonly CCTContext _context;
        private readonly IMatchService _matchService;
        private readonly IParticipantService _participantService;
        private readonly IAccountService _accountService;
        private readonly IConfiguration _config;

        public TeamService(CCTContext context, IConfiguration config, IParticipantService participantSerivce, IMatchService matchService, IAccountService accountService)
        {
            _context = context;
            _config = config;
            _matchService = matchService;
            _accountService = accountService;
            _participantService = participantSerivce;
        }
        public IEnumerable<LookupViewModel> GetTeamLookup(string type)
        {
            if (type == "status")
            {
                var res = _context.TeamStatus.Where(query => query.ID != 0)
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
                var res = _context.RoleType.Where(query => query.ID != 0)
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

        public IEnumerable<TeamExtendedViewModel> GetTeamInvitesByParticipantUsername(string username)
        {
            var participantStatus = _participantService.GetParticipantByUsername(username).Status;
            if (participantStatus == "Banned" || participantStatus == "Suspended") 
                throw new UnauthorizedAccessException($"{username} is currented {participantStatus}. If you would like to dispute this, please contact Rec.IM@gordon.edu");
            
            var teamInvites = _context.ParticipantTeam
                    .Where(pt => pt.ParticipantUsername == username && pt.RoleTypeID == 2)
                    .Join(_context.Team
                        .Join(_context.Activity,
                            t => t.ActivityID,
                            a => a.ID,
                            (t, a) => new TeamExtendedViewModel
                            {
                                ID = t.ID,
                                Activity = a,
                                Name = t.Name,
                                Logo = t.Logo,
                            }
                        ),
                        pt => pt.TeamID,
                        t => t.ID,
                        (pt, t) => t
                    )
                    .AsEnumerable();

            return teamInvites;
;
        }
        
        public ParticipantTeamViewModel GetParticipantTeam(int teamID, string username)
        {
            var participantStatus = _participantService.GetParticipantByUsername(username).Status;
            if (participantStatus == "Banned" || participantStatus == "Suspended")
                throw new UnauthorizedAccessException($"{username} is currented {participantStatus}. If you would like to dispute this, please contact Rec.IM@gordon.edu");
            var participantTeam = _context.ParticipantTeam
                                    .Where(pt => pt.TeamID == teamID && pt.ParticipantUsername == username)
                                    .Select(pt => new ParticipantTeamViewModel
                                    {
                                        ID = pt.ID,
                                        TeamID = pt.TeamID,
                                        ParticipantUsername = pt.ParticipantUsername,
                                        SignDate = pt.SignDate,
                                        RoleTypeID = pt.RoleTypeID,
                                    })
                                    .FirstOrDefault();

            return participantTeam;
        }

        public async Task<TeamViewModel> PostTeamAsync(TeamUploadViewModel t, string username)
        {
            var participantStatus = _participantService.GetParticipantByUsername(username).Status;
            if (participantStatus == "Banned" || participantStatus == "Suspended")
                throw new UnauthorizedAccessException($"{username} is currented {participantStatus}. If you would like to dispute this, please contact Rec.IM@gordon.edu");
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
            await AddParticipantToTeamAsync(team.ID, captain);

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
        public async Task DeleteParticipantTeamAsync(int teamID, string username)
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
            var team = _context.Team.FirstOrDefault(t => t.ID == teamID);
            var activity = _context.Activity.FirstOrDefault(a => a.ID == team.ActivityID);
            var invitee = _accountService.GetAccountByUsername(inviteeUsername);
            var inviter = _accountService.GetAccountByUsername(inviterUsername);

            string from_email = _config["Emails:RecIM:Username"];
            string to_email = invitee.Email;
            string messageBody =
                 $"Hey {invitee.FirstName}!<br><br>" +
                $"{inviter.FirstName} {inviter.LastName} has invited you join <b>{team.Name}</b> for <b>{activity.Name}</b> <br>" +
                $"Registration closes on <i>{activity.RegistrationEnd.ToString("D", CultureInfo.GetCultureInfo("en-US"))}</i> <br>" +
                //$"check it out <a href='https://360.gordon.edu/recim'>here</a>! <br><br>" + //for production
                $"check it out <a href='https://360recim.gordon.edu/recim'>here</a>! <br><br>" +//for development
                $"Gordon Rec-IM";

            using var smtpClient = new SmtpClient()
            {
                Credentials = new NetworkCredential
                {
                    UserName = from_email,
                    Password = _config["Emails:RecIM:Password"]
                },
                Host = _config["SmtpHost"],
                EnableSsl = true,
                Port = 587,
            };

            var message = new MailMessage(from_email, to_email)
            {
                Subject = $"Gordon Rec-IM: {inviter.FirstName} {inviter.LastName} has invited you to a team!",
                Body = messageBody,
            };
            message.IsBodyHtml = true;

            smtpClient.Send(message);
        }
        
        public async Task<ParticipantTeamViewModel> AddParticipantToTeamAsync(int teamID, ParticipantTeamUploadViewModel participant, string? inviterUsername = null)
        {
            //new check for enabling non-recim participants to be invited
            if(!_context.Participant.Any(p => p.Username == participant.Username))
            {
                await _participantService.PostParticipantAsync(participant.Username, 1); //pending user
            }


            var participantTeam = new ParticipantTeam
            {
                TeamID = teamID,
                ParticipantUsername = participant.Username,
                SignDate = DateTime.Now,
                RoleTypeID = participant.RoleTypeID ?? 2, //3 -> Member, 2-> Requested Join
            };
            await _context.ParticipantTeam.AddAsync(participantTeam);
            await _context.SaveChangesAsync();
            if (participant.RoleTypeID == 2 && inviterUsername is not null) //if this is an invite, send an email
            {
                await SendInviteEmail(teamID, participant.Username, inviterUsername);
            }
            return participantTeam;
        }

        public bool HasUserJoined(int activityID, string username)
        {
            // get all the partipantTeam from the teams with the activityID
            var participantTeams = _context.Team.Where(t => t.ActivityID == activityID)
                .Join(_context.ParticipantTeam.Where(pt => pt.RoleTypeID % 6 > 2),
                t => t.ID,
                pt => pt.TeamID,
                (t, pt) => pt)
                .AsEnumerable();

            return participantTeams.Any(pt => pt.ParticipantUsername == username);
        }

        public bool HasTeamNameTaken(int activityID, string teamName)
        {
            return _context.Team.Any(t =>
                        t.ActivityID == activityID
                        && t.Name == teamName
            );
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

