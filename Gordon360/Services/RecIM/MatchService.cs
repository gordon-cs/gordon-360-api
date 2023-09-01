using Gordon360.Exceptions;
using Gordon360.Extensions.System;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels.RecIM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Match = Gordon360.Models.CCT.Match;

namespace Gordon360.Services.RecIM;

public class MatchService : IMatchService
{
    private readonly CCTContext _context;
    private readonly IAccountService _accountService;
    private readonly IParticipantService _participantService;


    public MatchService(CCTContext context,  IAccountService accountService, IParticipantService participantService)
    {
        _context = context;
        _accountService = accountService;
        _participantService = participantService;
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
                    .Where(mt => mt.StatusID != 0)
                    .Select(mt => (TeamMatchHistoryViewModel)mt).AsEnumerable(),
                StartTime = mt.Match.StartTime.SpecifyUtc(),
                Status = mt.Match.Status.Description,
                Surface = mt.Match.Surface.Name,
                Team = mt.Match.MatchTeam.Where(mt => mt.StatusID != 0)
                    .Select(_mt => new TeamExtendedViewModel
                    {
                        ID = _mt.TeamID,
                        Name = _mt.Team.Name,
                        Logo = _mt.Team.Logo
                    })
                    .AsEnumerable(),
                Series = _context.Series
                    .Include(s => s.SeriesTeam)
                        .ThenInclude(s => s.Team)
                    .Where(s => s.ID == mt.Match.SeriesID)
                    .Select(s => new SeriesExtendedViewModel
                    {
                        ID = s.ID,
                        Name = s.Name,
                        Type = s.Type.Description,
                        TeamStanding = s.SeriesTeam.Select(st => (TeamRecordViewModel)st)
                    }).FirstOrDefault(),
                Activity = new ActivityExtendedViewModel
                {
                    ID = mt.Team.ActivityID,
                    Name = mt.Team.Activity.Name
                },

            })
            .FirstOrDefault();
        return match;
    }

    public MatchExtendedViewModel GetMatchByID(int matchID)
    {
        var teamCount = _context.MatchTeam.Where(mt => mt.MatchID == matchID && mt.StatusID != 0).Count();
        if (teamCount > 2)
        {
            var multiTeamMatch = _context.Match
            .Where(m => m.ID == matchID && m.StatusID != 0)
            .Select(m => new MatchExtendedViewModel
            {
                ID = matchID,
                Scores = m.MatchTeam.Where(mt => mt.StatusID != 0)
                    .Select(mt => new TeamMatchHistoryViewModel()
                    {
                        TeamID = mt.TeamID,
                        MatchID = mt.MatchID,
                        TeamScore = mt.Score,
                        Status = mt.Status.Description,
                        SportsmanshipScore = mt.SportsmanshipScore
                    })
                    .AsEnumerable(),
                StartTime = m.StartTime.SpecifyUtc(),
                Surface = m.Surface.Name,
                Status = m.Status.Description,
                Attendance = m.MatchParticipant
                    .Select(mp => new ParticipantExtendedViewModel
                    {
                        Username = mp.ParticipantUsername
                    }).AsEnumerable(),

                Series = _context.Series
                    .Where(s => s.ID == m.SeriesID)
                    .Select(s => new SeriesExtendedViewModel
                    {
                        ID = s.ID,
                        Name = s.Name,
                        Type = s.Type.Description,
                        TeamStanding = s.SeriesTeam.Where(st => st.Team.StatusID != 0)
                        .Select(
                            st => new TeamRecordViewModel()
                            {
                                SeriesID = st.SeriesID,
                                TeamID = st.TeamID,
                                Name = st.Team.Name,
                                WinCount = st.WinCount,
                                LossCount = st.LossCount,
                            }),
                    }).FirstOrDefault(),
                // Team will eventually be handled by TeamService 
                Activity = new ActivityExtendedViewModel
                {
                    ID = m.Series.ActivityID,
                    Name = m.Series.Activity.Name,
                    Logo = m.Series.Activity.Logo
                },
                Team = m.MatchTeam
                    .Where(mt => mt.StatusID != 0)
                    .Select(mt => new TeamExtendedViewModel
                    {
                        ID = mt.TeamID,
                        Name = mt.Team.Name,
                        Logo = mt.Team.Logo,
                        Status = mt.Status.Description,
                        Participant = mt.Team.ParticipantTeam
                            .Where(pt => !new int[] { 0, 1, 2 }.Contains(pt.RoleTypeID)) //roletype is either deleted, invalid, invited to join
                            .Select(pt => _participantService.GetParticipantByUsername(pt.ParticipantUsername, pt.RoleType.Description)),
                    })
            }).FirstOrDefault();
            return multiTeamMatch;
        }
        var twoTeamMatch = _context.Match
            .Where(m => m.ID == matchID && m.StatusID != 0)
            .Select(m => new MatchExtendedViewModel
            {
                ID = matchID,
                Scores = m.MatchTeam.Where(mt => mt.StatusID != 0)
                    .Select(mt => new TeamMatchHistoryViewModel()
                    {
                        TeamID = mt.TeamID,
                        MatchID = mt.MatchID,
                        TeamScore = mt.Score,
                        Status = mt.Status.Description,
                        SportsmanshipScore = mt.SportsmanshipScore
                    })
                    .AsEnumerable(),
                StartTime = m.StartTime.SpecifyUtc(),
                Surface = m.Surface.Name,
                Status = m.Status.Description,
                Attendance = m.MatchParticipant
                    .Select(mp => new ParticipantExtendedViewModel
                    {
                        Username = mp.ParticipantUsername
                    }).AsEnumerable(),
                
                Series = _context.Series
                    .Where(s => s.ID == m.SeriesID)
                    .Select(s => new SeriesExtendedViewModel
                    {
                        ID = s.ID,
                        Name = s.Name,
                        Type = s.Type.Description,
                        TeamStanding = s.SeriesTeam.Where(st => st.Team.StatusID != 0)
                        .Select(
                            st => new TeamRecordViewModel()
                            {
                                SeriesID = st.SeriesID,
                                TeamID = st.TeamID,
                                Name = st.Team.Name,
                                WinCount = st.WinCount,
                                LossCount = st.LossCount,
                            }),
                    }).FirstOrDefault(),
                // Team will eventually be handled by TeamService 
                Activity = new ActivityExtendedViewModel
                {
                    ID = m.Series.ActivityID,
                    Name = m.Series.Activity.Name
                },
                Team = m.MatchTeam
                    .Where(mt => mt.StatusID != 0 )
                    .Select(mt => new TeamExtendedViewModel
                    {
                        ID = mt.TeamID,
                        Name = mt.Team.Name,
                        Logo = mt.Team.Logo,
                        Status = mt.Status.Description,
                        Participant = mt.Team.ParticipantTeam
                            .Where(pt => !new int[] {0,1,2}.Contains(pt.RoleTypeID)) //roletype is either deleted, invalid, invited to join
                            .Select(pt => _participantService.GetParticipantByUsername(pt.ParticipantUsername, pt.RoleType.Description)),
                        MatchHistory = _context.MatchTeam.Where(_mt => _mt.TeamID == mt.TeamID && _mt.Match.StatusID == 6)
                            .OrderByDescending(mt => mt.Match.StartTime)
                            .Take(5)
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
                                    Status = own_mt.Score > other_mt.Score
                                            ? "Win"
                                            : own_mt.Score < other_mt.Score
                                                ? "Lose"
                                                : "Tie",
                                    MatchStatusID = own_mt.Match.StatusID,
                                    MatchStartTime = own_mt.Match.StartTime.SpecifyUtc(),
                                }
                            ),

                        TeamRecord = _context.SeriesTeam.Where(st => st.TeamID == mt.TeamID).Select( 
                            st => new TeamRecordViewModel()
                            {
                                SeriesID = st.SeriesID,
                                TeamID = st.TeamID,
                                Name = st.Team.Name,
                                WinCount = st.WinCount,
                                LossCount = st.LossCount,
                            }),
                    })
            }).FirstOrDefault();
        return twoTeamMatch;
    }


    public IEnumerable<MatchExtendedViewModel> GetMatchesBySeriesID(int seriesID)
    {
        var matches = _context.Match
            .Where(m => m.SeriesID == seriesID && m.StatusID != 0)
            .Include(m => m.MatchTeam)
                .ThenInclude(m => m.Match)
            .Include(m => m.MatchTeam)
                .ThenInclude(m => m.Status)
            .Select(m => new MatchExtendedViewModel
            {
                ID = m.ID,
                Scores = m.MatchTeam
                    .Select(mt => (TeamMatchHistoryViewModel)mt)
                    .AsEnumerable(),
                StartTime = m.StartTime.SpecifyUtc(),
                Surface = m.Surface.Name,
                Status = m.Status.Description,
                Team = m.MatchTeam.Where(mt => mt.StatusID != 0).Select(mt => new TeamExtendedViewModel
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
            SurfaceID = newMatch.SurfaceID ?? 1, //TBD surface id
            StatusID = 1 //default unconfirmed
        }; ;
        await _context.Match.AddAsync(match);
        await _context.SaveChangesAsync();
        foreach (var teamID in newMatch.TeamIDs)
        {
            if (teamID != -1) // do not create team mappings for fake teams
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
        var match = _context.Match.Find(matchID);

        if (match.SeriesID == 6) throw new UnprocessibleEntity() { ExceptionMessage = "Stats cannot be updated for a completed match" };

        teamstats.Score = vm.Score ?? teamstats.Score;
        teamstats.SportsmanshipScore = vm.SportsmanshipScore ?? teamstats.SportsmanshipScore;

        if (teamstats.StatusID == 4 && vm.StatusID != 4 ) //forfeit -> non forfeit
        {
            var opposingTeams = _context.MatchTeam.Where(mt => mt.MatchID == matchID && mt.TeamID != vm.TeamID).ToList();
            var seriesRecords = _context.SeriesTeam.Where(st => st.SeriesID == match.SeriesID);
            var ownRecord = seriesRecords.FirstOrDefault(st => st.TeamID == vm.TeamID);
            if (match.StatusID == 4 || match.StatusID != 6) //if forfeited or not completed 
            {
                ownRecord.LossCount--;
                if (opposingTeams.Count == 1) // if NOT a ladder match/only has 1 opponent (we can gift the win)
                {
                    match.StatusID = 2; // confirmed
                    var opposingRecord = seriesRecords.FirstOrDefault(st => st.TeamID == opposingTeams[0].TeamID);
                    var opposingStats = _context.MatchTeam.FirstOrDefault(mt => mt.MatchID == matchID && mt.TeamID != vm.TeamID);
                    opposingStats.Score = 0; // reset scores
                    opposingRecord.WinCount--;
                }
            }
        }

        if (teamstats.StatusID != 4 && vm.StatusID == 4) //non forfeit -> forfeit
        {
            var opposingTeams = _context.MatchTeam.Where(mt => mt.MatchID == matchID && mt.TeamID != vm.TeamID).ToList();
            // if there is only 1 opponent and they've already forfeited. Throw exception
            if (opposingTeams.Count == 1 && opposingTeams.First().StatusID == 4) throw new UnprocessibleEntity() { ExceptionMessage = "Both teams cannot forfeit." };

            if (match.StatusID != 4 && match.StatusID != 6) //not already set to forfeited or completed
            {
                var seriesRecords = _context.SeriesTeam.Where(st => st.SeriesID == match.SeriesID);
                teamstats.Score = 0; //remove team score of forfeit
                var ownRecord = seriesRecords.FirstOrDefault(st => st.TeamID == vm.TeamID);
                
                ownRecord.LossCount++;
                if (opposingTeams.Count == 1) // if NOT a ladder match/only has 1 opponent (we can gift the win)
                {
                    // forfeit match if there is only 1 other team
                    match.StatusID = 4; //forfeited
                    var opposingRecord = seriesRecords.FirstOrDefault(st => st.TeamID == opposingTeams[0].TeamID);
                    var opposingStats = _context.MatchTeam.FirstOrDefault(mt => mt.MatchID == matchID && mt.TeamID != vm.TeamID);
                    opposingRecord.WinCount++;
                    opposingStats.Score = 1;
                }
            }
        }

        teamstats.StatusID = vm.StatusID ?? teamstats.StatusID;

        await _context.SaveChangesAsync();
        return teamstats;

    }

    public async Task<MatchViewModel> UpdateMatchAsync(int matchID, MatchPatchViewModel vm)
    {
        var match = _context.Match.Find(matchID);

        if (match.StatusID == 4 || vm.StatusID == 4) throw new UnprocessibleEntity() { ExceptionMessage = "Please resolve the 'Forfeit' status within the Team's own status " };

        if (match.StatusID == 6 && vm.StatusID != 6) //completed -> not completed
        {
            var teams = _context.MatchTeam.Where(mt => mt.MatchID == matchID) //secondary sort by sportsmanship score (tiebreakers)
                .OrderByDescending(mt => mt.SportsmanshipScore)
                .ThenByDescending(mt => mt.Score);
            var seriesRecords = _context.SeriesTeam.Where(st => st.SeriesID == match.SeriesID);

            // not tie
            if (teams.First().Score - teams.Last().Score != 0)
            {
                //set winner
                var winner = seriesRecords.FirstOrDefault(st => st.TeamID == teams.First().TeamID);
                winner.WinCount--;
                winner.LossCount++; //done so that the foreach below does not need a conditional

                //set everyone 
                foreach (var team in teams)
                    seriesRecords.FirstOrDefault(st => st.TeamID == team.TeamID).LossCount--;
            } else
            {
                foreach (var team in teams)
                    seriesRecords.FirstOrDefault(st => st.TeamID == team.TeamID).TieCount--;
            }
        }

        if (match.StatusID != 6 && vm.StatusID == 6) //not completed -> completed
        {
            var teams = _context.MatchTeam.Where(mt => mt.MatchID == matchID) //secondary sort by sportsmanship score (tiebreakers)
                .OrderByDescending(mt => mt.SportsmanshipScore)
                .ThenByDescending(mt => mt.Score);
            var seriesRecords = _context.SeriesTeam.Where(st => st.SeriesID == match.SeriesID);

            // not tie
            if (teams.First().Score - teams.Last().Score != 0)
            {
                //set winner
                var winner = seriesRecords.FirstOrDefault(st => st.TeamID == teams.First().TeamID);
                winner.WinCount++;
                winner.LossCount--; //done so that the foreach below does not need a conditional

                //set everyone 
                foreach (var team in teams)
                    seriesRecords.FirstOrDefault(st => st.TeamID == team.TeamID).LossCount++;
            } else
            {
                foreach (var team in teams)
                    seriesRecords.FirstOrDefault(st => st.TeamID == team.TeamID).TieCount++;
            }
        }

        match.StartTime = vm.StartTime ?? match.StartTime;
        match.StatusID = vm.StatusID ?? match.StatusID;
        match.SurfaceID = vm.SurfaceID ?? match.SurfaceID;

        if (vm.TeamIDs is not null && match.StatusID != 6) //make sure that completed matches cant have their team list updated
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
        var match = _context.Match.Include(m => m.MatchTeam).FirstOrDefault(m => m.ID == matchID);
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

    public async Task<MatchAttendance> AddParticipantAttendanceAsync(int matchID, MatchAttendance attendee)
    {
        var teamID = attendee.TeamID ?? _context.MatchTeam
            .Where(mt => mt.MatchID == matchID)
            .Join(
                _context.ParticipantTeam.Where(pt => pt.ParticipantUsername == attendee.Username),
                mt => mt.TeamID,
                pt => pt.TeamID,
                (mt, pt) => mt
            ).FirstOrDefault()?.TeamID;


        if (teamID is int t_id)
        {
            var attemptFind = _context.MatchParticipant
                .FirstOrDefault(mp => mp.ParticipantUsername == attendee.Username && mp.TeamID == teamID && mp.MatchID == matchID);

            if (attemptFind is not null) return attemptFind;

            var newAttendee = new MatchParticipant
            {
                ParticipantUsername = attendee.Username,
                MatchID = matchID,
                TeamID = t_id
            };
            await _context.MatchParticipant.AddAsync(newAttendee);
            await _context.SaveChangesAsync();

            return newAttendee;
        }

        throw new ResourceNotFoundException() { ExceptionMessage = "Participant was not found in a team in this match" };
    }

    public async Task DeleteParticipantAttendanceAsync(int matchID, MatchAttendance attendee)
    {
        var teamID = attendee.TeamID ?? _context.MatchTeam
            .Where(mt => mt.MatchID == matchID)
            .Join(
                _context.ParticipantTeam.Where(pt => pt.ParticipantUsername == attendee.Username),
                mt => mt.TeamID,
                pt => pt.TeamID,
                (mt, pt) => mt
            ).FirstOrDefault()?.TeamID;

        var res = _context.MatchParticipant
            .FirstOrDefault(mp => mp.ParticipantUsername == attendee.Username && mp.TeamID == teamID && mp.MatchID == matchID);

        if (teamID is null || res is null) throw new ResourceNotFoundException() { ExceptionMessage = "Participant was not found in a team in this match" };

        _context.MatchParticipant.Remove(res);
        await _context.SaveChangesAsync();
    }


    public async Task<MatchViewModel> DeleteMatchCascadeAsync(int matchID)
    {
        //deletematch
        var match = _context.Match
            .Include(m => m.MatchTeam)
            .FirstOrDefault(m => m.ID == matchID);

        if (match.StatusID == 6) //deleting a completed match needs to reset the win/loss record
        {
            var teams = _context.MatchTeam.Where(mt => mt.MatchID == matchID) //secondary sort by sportsmanship score (tiebreakers)
                .OrderByDescending(mt => mt.SportsmanshipScore)
                .ThenByDescending(mt => mt.Score);
            var seriesRecords = _context.SeriesTeam.Where(st => st.SeriesID == match.SeriesID);

            // not tie
            if (teams.First().Score - teams.Last().Score != 0)
            {
                //set winner
                var winner = seriesRecords.FirstOrDefault(st => st.TeamID == teams.First().TeamID);
                winner.WinCount--;
                winner.LossCount++; //done so that the foreach below does not need a conditional

                //set everyone 
                foreach (var team in teams)
                    seriesRecords.FirstOrDefault(st => st.TeamID == team.TeamID).LossCount--;
            }
        }
        match.StatusID = 0; //deleted status

        //delete matchteam
        foreach (var mt in match.MatchTeam)
            mt.StatusID = 0; //deleted status
        
        await _context.SaveChangesAsync();
        return match;
    }

    public IEnumerable<MatchExtendedViewModel> GetAllMatches()
    {
        var matches = _context.Match
             .Where(m => m.StatusID != 0 && m.StatusID != 4 && m.StatusID !=6)
             .Include(m => m.MatchTeam)
                 .ThenInclude(m => m.Match)
             .Include(m => m.MatchTeam)
                 .ThenInclude(m => m.Status)
             .Select(m => new MatchExtendedViewModel
             {
                 ID = m.ID,
                 Activity = m.Series.Activity,
                 Scores = m.MatchTeam
                     .Select(mt => (TeamMatchHistoryViewModel)mt)
                     .AsEnumerable(),
                 StartTime = m.StartTime.SpecifyUtc(),
                 Surface = m.Surface.Name,
                 Status = m.Status.Description,
                 Team = m.MatchTeam.Where(mt => mt.StatusID != 0).Select(mt => new TeamExtendedViewModel
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
}

