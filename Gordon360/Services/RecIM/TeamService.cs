using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Exceptions;
using Gordon360.Models.CCT.Context;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.Graph;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.TermStore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Gordon360.Utilities;
using Microsoft.EntityFrameworkCore;
using Gordon360.Extensions.System;

namespace Gordon360.Services.RecIM
{
    public class TeamService : ITeamService
    {
        private readonly CCTContext _context;
        private readonly IMatchService _matchService;
        private readonly IParticipantService _participantService;
        private readonly IAccountService _accountService;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ServerUtils _serverUtils;

        public TeamService(CCTContext context, IEmailService emailService, IConfiguration config, IParticipantService participantSerivce, IMatchService matchService, IAccountService accountService, IWebHostEnvironment webHostEnvironment, ServerUtils serverUtils)
        {
            _context = context;
            _config = config;
            _matchService = matchService;
            _accountService = accountService;
            _participantService = participantSerivce;
            _emailService = emailService;
            _webHostEnvironment = webHostEnvironment;
            _serverUtils = serverUtils;
        }

        // status ID of 0 implies deleted
        public IEnumerable<LookupViewModel>? GetTeamLookup(string type)
        {
            return type switch
            {
                "status" => _context.TeamStatus.Where(query => query.ID != 0)
                    .Select(s => new LookupViewModel
                    {
                        ID = s.ID,
                        Description = s.Description
                    })
                    .AsEnumerable(),
                "role" => _context.RoleType.Where(query => query.ID != 0)
                    .Select(s => new LookupViewModel
                    {
                        ID = s.ID,
                        Description = s.Description
                    })
                    .AsEnumerable(),
                _ => null
            };
        }

        public double GetTeamSportsmanshipScore(int teamID)
        {
            var sportsmanshipScores = _context.MatchTeam
                                    .Where(mt => mt.Match.StatusID == 6 && mt.TeamID == teamID) //6 is completed
                                        .Select(mt => mt.SportsmanshipScore);
            if (sportsmanshipScores.Count() == 0)
                return 5;

            return sportsmanshipScores.Average();
        }

        public IEnumerable<TeamExtendedViewModel> GetTeams(bool active)
        {
            var teamQuery = active 
                ? _context.Team.Where(t => !t.Activity.Completed == active && t.StatusID != 0) // 0 is deleted
                : _context.Team;    

            var teams = teamQuery
                .Include(t => t.SeriesTeam)
                    .ThenInclude(t => t.Team)

                .Select(t => new TeamExtendedViewModel
                {
                    ID = t.ID,
                    Activity = t.Activity,
                    Name = t.Name,
                    Status = t.Status.Description,
                    Logo = t.Logo,
                    Participant = t.ParticipantTeam.Where(pt => pt.RoleTypeID != 0) // 0 is deleted
                        .Select(pt => new ParticipantExtendedViewModel
                        {
                            Username = pt.ParticipantUsername,
                            Email = _accountService.GetAccountByUsername(pt.ParticipantUsername).Email,
                            Role = pt.RoleType.Description,
                        }),
                    TeamRecord = t.SeriesTeam
                        .Select(st => (TeamRecordViewModel)st)
                        .AsEnumerable(),
                    SportsmanshipRating = _context.Match
                        .Where(m => m.StatusID == 6) //completed
                            .Join(t.MatchTeam,
                                m => m.ID,
                                mt => mt.MatchID,
                                (m, mt) => m
                            ).Count() == 0 // if there are no completed matches, default sportsmanshipRating to 5
                            ? 5
                            : _context.Match
                        .Where(m => m.StatusID == 6) //completed
                            .Join(t.MatchTeam,
                                m => m.ID,
                                mt => mt.MatchID,
                                (m, mt) => mt.SportsmanshipScore
                            ).Average() 
                })
                .AsEnumerable();
            return teams;
        }
        public TeamExtendedViewModel GetTeamByID(int teamID)
        {
            var team = _context.Team
                            .Where(t => t.ID == teamID && t.StatusID != 0)
                            .Include(t => t.Activity)
                                .ThenInclude(t => t.Type)
                            .Include(t => t.SeriesTeam)
                                .ThenInclude(t => t.Team)
                            .Select(t => new TeamExtendedViewModel
                            {
                                ID = teamID,
                                Activity = t.Activity,
                                Name = t.Name,
                                Status = t.Status.Description,
                                Logo = t.Logo,
                                Match = t.MatchTeam
                                    .Select(mt => _matchService.GetMatchForTeamByMatchID(mt.MatchID)).AsEnumerable(),
                                Participant = t.ParticipantTeam.Where(pt => pt.RoleTypeID != 0)
                                                .Select(pt => new ParticipantExtendedViewModel
                                                {
                                                    Username = pt.ParticipantUsername,
                                                    Email = _accountService.GetAccountByUsername(pt.ParticipantUsername).Email,
                                                    Role = pt.RoleType.Description,
                                                    GamesAttended = _context.MatchParticipant.Count(mp => mp.TeamID == teamID && mp.ParticipantUsername == pt.ParticipantUsername)
        }),
                                MatchHistory = _context.Match.Where(m => m.StatusID == 6 || m.StatusID == 4) // completed or forfeited status
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
                                                                MatchStartTime = match.StartTime.SpecifyUtc()
                                                            }
                                                ).AsEnumerable(),
                                TeamRecord = t.SeriesTeam
                                            .Select(st =>  (TeamRecordViewModel)st)
                                            .AsEnumerable(),
                                SportsmanshipRating = GetTeamSportsmanshipScore(teamID)
                            }).FirstOrDefault();
            return team;
        }

        public IEnumerable<TeamExtendedViewModel> GetTeamInvitesByParticipantUsername(string username)
        {
            var participantStatus = _participantService.GetParticipantByUsername(username).Status;
            if (participantStatus == "Banned" || participantStatus == "Suspended") 
                throw new UnauthorizedAccessException($"{username} is currented {participantStatus}. If you would like to dispute this, please contact Rec.IM@gordon.edu");

            var teamInvites = _context.ParticipantTeam
                    .Where(pt => pt.ParticipantUsername == username && pt.RoleTypeID == 2) //pending status
                    .Select(pt => new TeamExtendedViewModel
                    {
                        ID = pt.Team.ID,
                        Activity = pt.Team.Activity,
                        Name = pt.Team.Name,
                        Logo = pt.Team.Logo
                    })
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
                                        SignDate = pt.SignDate.SpecifyUtc(),
                                        RoleTypeID = pt.RoleTypeID,
                                    })
                                    .FirstOrDefault();

            return participantTeam;
        }

        public async Task<TeamViewModel> PostTeamAsync(TeamUploadViewModel newTeam, string username)
        {
            var participantStatus = _participantService.GetParticipantByUsername(username).Status;
            if (participantStatus == "Banned" || participantStatus == "Suspended")
                throw new UnauthorizedAccessException($"{username} is currented {participantStatus}. If you would like to dispute this, please contact Rec.IM@gordon.edu");
            
            if(_context.Activity.Find(newTeam.ActivityID).Team.Any(team => team.Name == newTeam.Name))
                throw new UnprocessibleEntity
                { ExceptionMessage = $"Team name {newTeam.Name} has already been taken by another team in this activity" };
            
            if (newTeam.Logo != null)
            {
                // ImageUtils.GetImageFormat checks whether the image type is valid (jpg/jpeg/png)
                var (extension, format, data) = ImageUtils.GetImageFormat(newTeam.Logo);

                // Use a unique alphanumeric GUID string as the file name
                var filename = $"{Guid.NewGuid().ToString("N")}.{extension}";
                var imagePath = GetImagePath(filename);
                var url = GetImageURL(filename);

                ImageUtils.UploadImage(imagePath, data, format);

                newTeam.Logo = url;
            }

            var team = newTeam.ToTeam();
            await _context.Team.AddAsync(team);
            await _context.SaveChangesAsync();

            var captain = new ParticipantTeamUploadViewModel
            {
                Username = username,
                RoleTypeID = 5 //captain
            };
            await AddParticipantToTeamAsync(team.ID, captain);

            var existingSeries = _context.Series.Where(s => s.ActivityID == newTeam.ActivityID).OrderBy(s => s.StartDate)?.FirstOrDefault();
            if (existingSeries is not null) {
                var seriesTeam = new SeriesTeam
                {
                    TeamID = team.ID,
                    SeriesID = existingSeries.ID,
                    WinCount = 0,
                    LossCount = 0,
                };
                await _context.SeriesTeam.AddAsync(seriesTeam);
                await _context.SaveChangesAsync();
            }

            return team;
        }

        public async Task<ParticipantTeamViewModel> UpdateParticipantRoleAsync(int teamID, ParticipantTeamUploadViewModel participant)
        {
            var participantTeam = _context.ParticipantTeam.FirstOrDefault(pt => pt.ParticipantUsername == participant.Username && pt.TeamID == teamID);
            var roleID = participant.RoleTypeID ?? 3; //update or default to member


            var activityID = _context.Team.Find(teamID).ActivityID;
            var otherInstances = _context.ParticipantTeam
                .Where(pt => pt.ID != participantTeam.ID && pt.ParticipantUsername == participantTeam.ParticipantUsername)
                .Join(_context.Team.Where(t => t.ActivityID == activityID),
                pt => pt.TeamID,
                t => t.ID,
                (pt, t) => pt);

            //if captain or inactive on another team, delete this participant team (admins can bypass)
            if (otherInstances.Any(pt => new int[] { 5,6 }.Contains(pt.RoleTypeID) && !_participantService.IsAdmin(participant.Username)))  
            {
                _context.ParticipantTeam.Remove(participantTeam);
                throw new ResourceCreationException() { ExceptionMessage = $"Participant is in an immutable role in this activity" };
            }

            //unconditionally will remove all other instances to ensure no edge case
            _context.ParticipantTeam.RemoveRange(otherInstances);
            participantTeam.RoleTypeID = roleID;
            await _context.SaveChangesAsync();

            return participantTeam;
        }
        public async Task<ParticipantTeamViewModel> DeleteParticipantTeamAsync(int teamID, string username)
        {
            var participantTeam = _context.ParticipantTeam.FirstOrDefault(pt => pt.TeamID == teamID && pt.ParticipantUsername == username);
            participantTeam.RoleTypeID = 0;
            await _context.SaveChangesAsync();

            return participantTeam;
        }

        public async Task<TeamViewModel> DeleteTeamCascadeAsync(int teamID)
        {
            var team = _context.Team.Find(teamID);
            team.StatusID = 0;
            await _context.SaveChangesAsync();

            return team;
        }

        public async Task<TeamViewModel> UpdateTeamAsync(int teamID, TeamPatchViewModel update)
        {
            var t =  _context.Team
                .Include(t => t.Activity)
                    .ThenInclude(t => t.Team)
                .FirstOrDefault(t => t.ID == teamID);
            if (update.Name is not null)
            {
                if (t.Activity.Team.Any(team => team.Name == update.Name && team.ID != teamID)) 
                    throw new UnprocessibleEntity 
                        { ExceptionMessage = $"Team name {update.Name} has already been taken by another team in this activity" };
            }
            t.Name = update.Name ?? t.Name;
            t.StatusID = update.StatusID ?? t.StatusID;
            
            if (update.Logo != null)
            {
                // ImageUtils.GetImageFormat checks whether the image type is valid (jpg/jpeg/png)
                var (extension, format, data) = ImageUtils.GetImageFormat(update.Logo);

                string? imagePath = null;
                // If old image exists, overwrite it with new image at same path
                if (t.Logo != null)
                {
                    imagePath = GetImagePath(Path.GetFileName(t.Logo));
                }
                // Otherwise, upload new image and save url to db
                else
                {
                    // Use a unique alphanumeric GUID string as the file name
                    var filename = $"{Guid.NewGuid().ToString("N")}.{extension}";
                    imagePath = GetImagePath(filename);
                    var url = GetImageURL(filename);
                    t.Logo = url;
                }

                ImageUtils.UploadImage(imagePath, data, format);
            }

            //If the image property is null, it means that either the user
            //chose to remove the previous image or that there was no previous
            //image (DeleteImage is designed to handle this).
            else if (t.Logo != null)
            {
                var imagePath = GetImagePath(Path.GetFileName(t.Logo));

                ImageUtils.DeleteImage(imagePath);
                t.Logo = update.Logo; //null
            }

            await _context.SaveChangesAsync();

            return t;
        }

        private async Task SendInviteEmail(int teamID, string inviteeUsername, string inviterUsername)
        {
            var team = _context.Team
                .Include(t => t.Activity)
                .FirstOrDefault(t => t.ID == teamID);
            var invitee = _accountService.GetAccountByUsername(inviteeUsername);
            var inviter = _accountService.GetAccountByUsername(inviterUsername);
            var password = _config["Emails:RecIM:Password"];
            string from_email = _config["Emails:RecIM:Username"];
            string to_email = invitee.Email;
            string messageBody =
                 $"Hey {invitee.FirstName}!<br><br>" +
                $"{inviter.FirstName} {inviter.LastName} has invited you join <b>{team.Name}</b> for <b>{team.Activity.Name}</b> <br>" +
                $"Registration closes on <i>{team.Activity.RegistrationEnd.ToString("D", CultureInfo.GetCultureInfo("en-US"))}</i> <br>" +
                //$"check it out <a href='https://360.gordon.edu/recim'>here</a>! <br><br>" + //for production
                $"check it out <a href='https://360recim.gordon.edu/recim'>here</a>! <br><br>" +//for development
                $"Gordon Rec-IM";
            string subject = $"Gordon Rec-IM: {inviter.FirstName} {inviter.LastName} has invited you to a team!";

            _emailService.SendEmails(new string[] {to_email},from_email,subject,messageBody,password);
        }
        
        public async Task<ParticipantTeamViewModel> AddParticipantToTeamAsync(int teamID, ParticipantTeamUploadViewModel participant, string? inviterUsername = null)
        {
            //new check for enabling non-recim participants to be invited
            if(!_context.Participant.Any(p => p.Username == participant.Username))
                await _participantService.PostParticipantAsync(participant.Username, 1); //pending user

            //check for participant is on the team
            if (_context.Team.FirstOrDefault(t => t.ID == teamID).ParticipantTeam.Any(pt => pt.ParticipantUsername == participant.Username 
                && pt.RoleTypeID != 0 && pt.RoleTypeID != 2)) //doesn't check for deleted or invited
                throw new UnprocessibleEntity { ExceptionMessage = $"Participant {participant.Username} is already in this team" };

            // if a participant is "deleted, modify the existing participantTeam instance
            var participantTeam = _context.ParticipantTeam.FirstOrDefault(pt => pt.TeamID == teamID && pt.ParticipantUsername == participant.Username && pt.RoleTypeID == 0);
            if (participantTeam is ParticipantTeam pt)
            {
                pt.SignDate = DateTime.UtcNow;
                pt.RoleTypeID = participant.RoleTypeID ?? 2;
            }
            else
            {
                participantTeam = new ParticipantTeam
                    {
                        TeamID = teamID,
                        ParticipantUsername = participant.Username,
                        SignDate = DateTime.UtcNow,
                        RoleTypeID = participant.RoleTypeID ?? 2, //3 -> Member, 2-> Requested Join
                    };
                await _context.ParticipantTeam.AddAsync(participantTeam);
            }
            
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
            var participantTeams = _context.Team.Where(t => t.ActivityID == activityID && t.StatusID != 0)
                .Join(_context.ParticipantTeam.Where(pt => pt.RoleTypeID % 6 > 2 && pt.RoleTypeID != 0), // 3,4,5 are member,co-captain,captain respectively
                t => t.ID,
                pt => pt.TeamID,
                (t, pt) => pt)
                .AsEnumerable();

            return participantTeams.Any(pt => pt.ParticipantUsername == username);
        }

        public bool HasTeamNameTaken(int activityID, string teamName)
        {
            return _context.Team.Where(t => t.StatusID != 0).Any(t =>
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

        public int GetTeamActivityID(int teamID)
        {
            return _context.Team.Find(teamID).ActivityID;
        }

        public int ParticipantAttendanceCount(int teamID, string username)
        {
            return _context.MatchParticipant.Count(mp => mp.TeamID == teamID && mp.ParticipantUsername == username);
        }

        public async Task<IEnumerable<MatchAttendance>> PutParticipantAttendanceAsync(int matchID, ParticipantAttendanceViewModel attendance)
        {
            var res = new List<MatchAttendance>();
            int teamID = attendance.TeamID;
            var attendees = new List<String>();
            foreach (MatchAttendance attendee in attendance.Attendance)
                attendees.Add(attendee.Username);


            var attendanceToRemove = _context.MatchParticipant.Where(mp => mp.MatchID == matchID && mp.TeamID == teamID
                && !attendees.Any(name => mp.ParticipantUsername == name));
            _context.MatchParticipant.RemoveRange(attendanceToRemove);

            var existingAttendance = _context.MatchParticipant.Where(mp => mp.MatchID == matchID && mp.TeamID == teamID 
                && attendees.Any(name => mp.ParticipantUsername == name));

            var attendanceToAdd = attendees.Where(name => !existingAttendance.Any(ea => ea.ParticipantUsername == name));
            foreach (string username in attendanceToAdd)
            {
                var created = new MatchParticipant
                {
                    MatchID = matchID,
                    ParticipantUsername = username,
                    TeamID = teamID
                };
                await _context.MatchParticipant.AddAsync(created);
                res.Add(created);
       
            }
            await _context.SaveChangesAsync();
            return (res);
        }

        private string GetImagePath(string filename)
        {
            return Path.Combine(_webHostEnvironment.ContentRootPath, "browseable", "uploads", "recim", "team", filename);
        }

        private string GetImageURL(string filename)
        {
            var serverAddress = _serverUtils.GetAddress();
            if (serverAddress is not string) throw new Exception("Could not upload Rec-IM Team Image: Server Address is null");

            if (serverAddress.Contains("localhost"))
                serverAddress += '/';
            var url = $"{serverAddress}browseable/uploads/recim/team/{filename}";
            return url;
        }
    }
}

