using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels.RecIM;
using Microsoft.Extensions.Configuration;
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
                "surface" => _context.Surface.Where(query => query.ID != 0)
                        .Select(s => new LookupViewModel
                        {
                            ID = s.ID,
                            Description = s.Description
                        })
                        .AsEnumerable(),

                _ => null
            };
        }

        //this function is used because ASP somehow refuses to cast IEnumerables or recognize IEnumerables
        //within other queries. The only solution is to return each individual instance and have the original
        //query handle the enumeration.
        public MatchExtendedViewModel GetMatchForTeamByMatchID(int matchID)
        {
            var activity = _context.Match.Find(matchID).Series.Activity;

            var match = _context.MatchTeam
                .Where(mt => mt.MatchID == matchID && mt.StatusID != 0)
                .Select(mt => new MatchExtendedViewModel
                {
                    ID = mt.MatchID,
                    Scores = _context.MatchTeam
                        .Where(s => s.MatchID == mt.MatchID)
                        .Select(mt => new TeamMatchHistoryViewModel
                        {
                            TeamID = mt.TeamID,
                            TeamScore = mt.Score
                        }).AsEnumerable(),
                    Time = mt.Match.Time,
                    Status = mt.Match.Status.Description,
                    Surface = mt.Match.Surface.Description,
                    Team = _context.MatchTeam
                        .Where(_mt => _mt.MatchID == mt.MatchID)
                        .Select(_mt => new TeamExtendedViewModel
                        {
                            ID = mt.TeamID,
                            Name = mt.Team.Name,
                        })
                        .AsEnumerable(),
                    Activity = activity,

                })
                .FirstOrDefault();
            return match;
        }
 
        public MatchExtendedViewModel GetMatchByID(int matchID)
        {
            var match = _context.Match
                        .Where(m => m.ID == matchID && m.StatusID != 0)
                        .Select(m => new MatchExtendedViewModel
                        {
                            ID = matchID,
                            Scores = m.MatchTeam
                                        .Select(mt => new TeamMatchHistoryViewModel
                                        {
                                            TeamID = mt.TeamID,
                                            TeamScore = mt.Score,
                                            Status = mt.Status.Description,
                                            SportsmanshipRating = mt.Sportsmanship
                                        }).AsEnumerable(),
                            Time = m.Time,
                            Surface = m.Surface.Description,
                            Status = m.Status.Description,
                            Attendance = m.MatchParticipant
                                        .Select(mp => new ParticipantExtendedViewModel
                                        {
                                            Username = mp.ParticipantUsername
                                        }).AsEnumerable(),
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
                            SeriesID = m.SeriesID,
                            // Team will eventually be handled by TeamService 
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
                                MatchHistory = GetMatchHistoryByTeamID(mt.TeamID)
                                    .OrderByDescending(mh => mh.MatchStartTime)
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
                                .Where(mt => mt.TeamID == teamID && mt.StatusID != 0) // 0 = deleted

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
                                    MatchStartTime = match.Time
                                }).AsEnumerable();
            return vm;
        }

        public IEnumerable<MatchExtendedViewModel> GetMatchesBySeriesID(int seriesID)
        {
            var matches = _context.Match
                        .Where(m => m.SeriesID == seriesID && m.StatusID != 0)
                        .Select(m => new MatchExtendedViewModel
                        {
                            ID = m.ID,
                            Time = m.Time,
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
                                        Win = st.Win,
                                        Loss = st.Loss,
                                    })
                            })
                        });
            return matches;
        }

        public async Task<MatchViewModel> PostMatchAsync(MatchUploadViewModel m)
        {
            var match = new Match
            {
                SeriesID = m.SeriesID,
                Time = m.StartTime,
                SurfaceID = m.SurfaceID ?? 1, //unknown surface id
                StatusID = 1 //default unconfirmed
            };
            await _context.Match.AddAsync(match);

            foreach (var teamID in m.TeamIDs)
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
                Sportsmanship = 5 //default max
            };
            await _context.MatchTeam.AddAsync(matchTeam);
        }

        public async Task<MatchTeamViewModel> UpdateTeamStatsAsync(int matchID, MatchStatsPatchViewModel vm)
        {
            var teamstats = _context.MatchTeam.FirstOrDefault(mt => mt.MatchID == matchID && mt.TeamID == vm.TeamID);
            teamstats.Score = vm.Score ?? teamstats.Score;
            teamstats.Sportsmanship = vm.Sportsmanship ?? teamstats.Sportsmanship;
            teamstats.StatusID = vm.StatusID ?? teamstats.StatusID;
            await _context.SaveChangesAsync();
            return teamstats;

        }

        public async Task<MatchViewModel> UpdateMatchAsync(int matchID, MatchPatchViewModel vm)
        {
            var match = _context.Match.Find(matchID);
            match.Time = vm.Time ?? match.Time;
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

        public async Task DeleteMatchCascadeAsync(int matchID)
        {
            //delete matchteam
            var matchteam = _context.MatchTeam.Where(mt => mt.MatchID == matchID);
            foreach (var mt in matchteam)
                mt.StatusID = 0; //deleted status
            //deletematch
            var match = _context.Match.Find(matchID);
            match.StatusID = 0; //deleted status

            await _context.SaveChangesAsync();
        }
    }

}

