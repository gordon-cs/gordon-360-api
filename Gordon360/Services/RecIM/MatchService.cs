using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Static.Methods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Match = Gordon360.Models.CCT.Match;

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

        public MatchViewModel GetSimpleMatchViewByID(int matchID)
        {
            var res = _context.Match.Find(matchID);
            return res;
        }

        // ID 0 is default deleted, we don't want to return the deleted status on lookup
        public IEnumerable<LookupViewModel>? GetMatchLookup(string type)
        {
            return type switch
            {
                "status" => _context.MatchStatus.Where(query => query.ID != 0)
                        .Select(s => new LookupViewModel
                        {
                            ID = s.ID,
                            Description = s.Description
                        })
                        .AsEnumerable(),
                "teamstatus" => _context.MatchTeamStatus.Where(query => query.ID != 0)
                        .Select(s => new LookupViewModel
                        {
                            ID = s.ID,
                            Description = s.Description
                        })
                        .AsEnumerable(),
                _ => null
            };
        }

        /// <summary>
        /// this function is used because ASP somehow refuses to cast IEnumerables or recognize IEnumerables
        /// within other queries. The only solution is to return each individual instance and have the original
        /// query handle the enumeration.
        /// </summary>
        public MatchExtendedViewModel GetMatchForTeamByMatchID(int matchID)
        {
            var match = _context.MatchTeam
                .Where(mt => mt.MatchID == matchID && mt.StatusID != 0)
                .Include(mt => mt.Status)
                .Include(mt => mt.Match)
                    .ThenInclude(mt => mt.Series)
                        .ThenInclude(mt => mt.Activity)
                .Include(mt => mt.Match)
                    .ThenInclude(mt => mt.MatchTeam)
                        .ThenInclude(mt => mt.Status)
                .Include(mt => mt.Match)
                    .ThenInclude(mt => mt.MatchTeam)
                        .ThenInclude(mt => mt.Match)
                .Select(mt => new MatchExtendedViewModel
                {
                    ID = mt.MatchID,
                    Scores = mt.Match.MatchTeam
                        .Select(mt => (TeamMatchHistoryViewModel)mt).AsEnumerable(),
                    StartTime = mt.Match.StartTime.SpecifyUtc(),
                    Status = mt.Match.Status.Description,
                    Surface = mt.Match.Surface.Description,
                    Team = mt.Match.MatchTeam
                        .Select(_mt => new TeamExtendedViewModel
                        {
                            ID = _mt.TeamID,
                            Name = _mt.Team.Name,
                        })
                        .AsEnumerable(),
                    Activity = mt.Match.Series.Activity,

                })
                .FirstOrDefault();
            return match;
        }
 
        public MatchExtendedViewModel GetMatchByID(int matchID)
        {
            var match = _context.Match
                .Include(m => m.Series)
                    .ThenInclude(m => m.Activity)
                        .ThenInclude(m => m.Team)
                            .ThenInclude(m => m.Activity)
                .Include(m => m.MatchTeam)
                    .ThenInclude(m => m.Match)
                .Include(m => m.MatchTeam)
                    .ThenInclude(m => m.Status)
                .Include(m => m.MatchTeam)
                    .ThenInclude(m => m.Team) 
                        .ThenInclude(m => m.Status)
                .Include(m => m.MatchTeam)
                    .ThenInclude(m => m.Team)
                        .ThenInclude(m => m.SeriesTeam)
                .Where(m => m.ID == matchID && m.StatusID != 0)
                .Select(m => new MatchExtendedViewModel
                {
                    ID = matchID,
                    Scores = m.MatchTeam
                        .Select(mt => (TeamMatchHistoryViewModel)mt)
                        .AsEnumerable(),
                    StartTime = m.StartTime.SpecifyUtc(),
                    Surface = m.Surface.Description,
                    Status = m.Status.Description,
                    Attendance = m.MatchParticipant
                        .Select(mp => new ParticipantExtendedViewModel
                        {
                            Username = mp.ParticipantUsername
                        }).AsEnumerable(),
                    
                    SeriesID = m.SeriesID,
                    // Team will eventually be handled by TeamService 
                    Activity = _context.Activity
                        .Where(a => a.ID == m.Series.ActivityID)
                        .Select(a => new ActivityExtendedViewModel
                        {
                            ID = a.ID,
                            Name = a.Name,
                            Team = a.Team.Select(t => new TeamExtendedViewModel
                            {
                                ID = t.ID,
                                Name = t.Name,
                                Logo = t.Logo
                            })
                        })
                        .FirstOrDefault(),
                    Team = m.MatchTeam.Select(mt => new TeamExtendedViewModel
                    {
                        ID = mt.TeamID,
                        Name = mt.Team.Name,
                        Status = mt.Status.Description,
                        Participant = mt.Team.ParticipantTeam
                            .Select(pt => new ParticipantExtendedViewModel
                            {
                                Username = pt.ParticipantUsername,
                                Email = _accountService.GetAccountByUsername(pt.ParticipantUsername).Email,
                                Role = pt.RoleType.Description
                            }),
                        MatchHistory = _context.MatchTeam.Where(_mt => _mt.ID == mt.ID)
                            .Join(
                                _context.MatchTeam.Where(o_mt => o_mt.TeamID != mt.TeamID),
                                own_mt => own_mt.MatchID,
                                other_mt => other_mt.MatchID,
                                (own_mt, other_mt) => new TeamMatchHistoryViewModel
                                {
                                    TeamID = own_mt.TeamID,
                                    MatchID = own_mt.MatchID,
                                    Opponent = other_mt.Team,
                                    TeamScore = own_mt.Score,
                                    OpposingTeamScore = other_mt.Score,
                                    MatchStatusID = own_mt.Match.StatusID,
                                    MatchStartTime = own_mt.Match.StartTime.SpecifyUtc(),  
                                }
                            ),
                        TeamRecord = mt.Team.SeriesTeam.Select(st => (TeamRecordViewModel)st).AsEnumerable(),
                    })
                }).FirstOrDefault();
            return match;
        }


        public IEnumerable<MatchExtendedViewModel> GetMatchesBySeriesID(int seriesID)
        {
            var matches = _context.Match
                .Include(m => m.MatchTeam)
                    .ThenInclude(m => m.Match)
                .Include(m => m.MatchTeam)
                    .ThenInclude(m => m.Status)
                .Where(m => m.SeriesID == seriesID && m.StatusID != 0)
                .Select(m => new MatchExtendedViewModel
                {
                    ID = m.ID,
                    Scores = m.MatchTeam
                        .Select(mt => (TeamMatchHistoryViewModel)mt)
                        .AsEnumerable(),
                    StartTime = m.StartTime.SpecifyUtc(),
                    Surface = m.Surface.Description,
                    Status = m.Status.Description,
                    Team = m.MatchTeam.Select(mt => new TeamExtendedViewModel
                    {
                        ID = mt.TeamID,
                        Name = mt.Team.Name,
                        TeamRecord = _context.SeriesTeam
                            .Where(st => st.SeriesID == m.SeriesID && st.TeamID == mt.TeamID)
                            .Select(st => new TeamRecordViewModel
                            {
                                WinCount = st.WinCount,
                                LossCount = st.LossCount,
                            })
                    })
                });
            return matches;
        }

        public async Task<SurfaceViewModel> PostSurfaceAsync(SurfaceUploadViewModel newSurface)
        {
            var surface = newSurface.ToSurface();
            await _context.Surface.AddAsync(surface);
            await _context.SaveChangesAsync();

            return surface;
        }

        public async Task<SurfaceViewModel> UpdateSurfaceAsync(int surfaceID, SurfaceUploadViewModel updatedSurface)
        {
            var surface = _context.Surface.Find(surfaceID);
            //inherit description if possible
            surface.Name = updatedSurface.Name ?? surface.Name ?? surface.Description ?? updatedSurface.Description;
            surface.Description = updatedSurface.Description ?? surface.Description ?? surface.Name ?? updatedSurface.Name;
            await _context.SaveChangesAsync();

            return surface;
        }

        public IEnumerable<SurfaceViewModel> GetSurfaces()
        {
            return _context.Surface.Where(s => s.ID != 0).Select(s => (SurfaceViewModel)s);
        }

        public async Task DeleteSurfaceAsync(int surfaceID)
        {
            var surface = _context.Surface
                .Include(s => s.Match)
                .Include(s => s.SeriesSurface)
                .FirstOrDefault(s => s.ID == surfaceID);
            var matches = surface.Match;
            var seriesSurfaces = surface.SeriesSurface;

            //point all matches to unknown surface
            foreach (var match in matches)
                match.SurfaceID = 0;
            //point all seriessurface to unknown surface
            foreach (var ss in seriesSurfaces)
                ss.SurfaceID = 0;

            _context.Surface.Remove(surface);
            await _context.SaveChangesAsync();
        }

        public async Task<MatchViewModel> PostMatchAsync(MatchUploadViewModel newMatch)
        {
            var match = new Match
            {
                SeriesID = newMatch.SeriesID,
                StartTime = newMatch.StartTime,
                SurfaceID = newMatch.SurfaceID ?? 1, //unknown surface id
                StatusID = 1 //default unconfirmed
            }; ;
            await _context.Match.AddAsync(match);
            await _context.SaveChangesAsync();
            foreach (var teamID in newMatch.TeamIDs)
            {
                await CreateMatchTeamMappingAsync(teamID, match.ID);
            }
            await _context.SaveChangesAsync();
            return match;
        }

        public async Task CreateMatchTeamMappingAsync(int teamID, int matchID)
        {
            var matchTeam = new MatchTeam
            {
                TeamID = teamID,
                MatchID = matchID,
                StatusID = 2, //default confirmed
                Score = 0,
                SportsmanshipScore = 5 //default max
            };
            await _context.MatchTeam.AddAsync(matchTeam);
        }

        public async Task<MatchTeamViewModel> UpdateTeamStatsAsync(int matchID, MatchStatsPatchViewModel vm)
        {
            var teamstats = _context.MatchTeam.FirstOrDefault(mt => mt.MatchID == matchID && mt.TeamID == vm.TeamID);
            teamstats.Score = vm.Score ?? teamstats.Score;
            teamstats.SportsmanshipScore = vm.SportsmanshipScore ?? teamstats.SportsmanshipScore;
            teamstats.StatusID = vm.StatusID ?? teamstats.StatusID;
            await _context.SaveChangesAsync();
            return teamstats;

        }

        public async Task<MatchViewModel> UpdateMatchAsync(int matchID, MatchPatchViewModel vm)
        {
            var match = _context.Match.Find(matchID);
            match.StartTime = vm.StartTime ?? match.StartTime;
            match.StatusID = vm.StatusID ?? match.StatusID;
            match.SurfaceID = vm.SurfaceID ?? match.SurfaceID;

            if (vm.TeamIDs is not null)
            {
                List<int> updatedTeams = vm.TeamIDs.ToList();
                var removedTeams = _context.MatchTeam.Where(mt => mt.MatchID == matchID && !updatedTeams.Any(t_id => mt.TeamID == t_id));
                _context.MatchTeam.RemoveRange(removedTeams);

                var existingTeams = _context.MatchTeam.Where(mt => mt.MatchID == matchID && updatedTeams.Any(t_id => mt.TeamID == t_id));
                var teamsToAdd = updatedTeams.Where(id => !existingTeams.Any(t => t.TeamID == id));
                foreach (int id in teamsToAdd)
                {
                    await CreateMatchTeamMappingAsync(id, matchID);
                }
            }

            await _context.SaveChangesAsync();
            return match;
        }

        public IEnumerable<ParticipantAttendanceViewModel> GetMatchAttendance(int matchID)
        {
            var match = _context.Match.Find(matchID);
            var res = new List<ParticipantAttendanceViewModel>();
            if (match is null) return res;

            foreach(MatchTeam mt in match.MatchTeam)
            {
                var attendance = _context.MatchParticipant
                    .Where(mp => mp.TeamID == mt.TeamID && mp.MatchID == mt.MatchID)
                    .Select(a => (MatchAttendance)a);

                res.Add(new ParticipantAttendanceViewModel
                {
                    TeamID = mt.TeamID,
                    Attendance = attendance,
                });

            }
            return res;
        }

        public async Task<MatchViewModel> DeleteMatchCascadeAsync(int matchID)
        {
            //deletematch
            var match = _context.Match
                .Include(m => m.MatchTeam)
                .FirstOrDefault(m => m.ID == matchID);
            match.StatusID = 0; //deleted status

            //delete matchteam
            foreach (var mt in match.MatchTeam)
                mt.StatusID = 0; //deleted status
            
            await _context.SaveChangesAsync();
            return match;
        }
    }

}

