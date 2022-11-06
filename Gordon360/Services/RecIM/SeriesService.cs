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

namespace Gordon360.Services.RecIM
{
    public class SeriesService : ISeriesService
    {
        private readonly CCTContext _context;
        private readonly IMatchService _matchService;

        public SeriesService(CCTContext context, IMatchService matchService)
        {
            _context = context;
            _matchService = matchService;
        }

        public IEnumerable<SeriesViewModel> GetSeries(bool active = false)
        {
            var series = _context.Series
                            .Select(s => new SeriesViewModel
                            {
                                ID = s.ID,
                                Name = s.Name,
                                StartDate = s.StartDate,
                                EndDate = s.EndDate,
                                Type = _context.SeriesType
                                            .FirstOrDefault(st => st.ID == s.TypeID)
                                            .Description,
                                Status = _context.SeriesStatus
                                            .FirstOrDefault(ss => ss.ID == s.StatusID)
                                            .Description,
                                ActivityID = s.ActivityID,
                                Match = _matchService.GetMatchBySeriesID(s.ID),
                                TeamStanding = _context.SeriesTeam
                                .Where(st => st.SeriesID == s.ID)
                                .Select(st => new TeamRecordViewModel
                                {
                                    ID = st.ID,
                                    Name = _context.Team
                                            .FirstOrDefault(t => t.ID == st.TeamID)
                                            .Name,
                                    Win = st.Win,
                                    Loss = st.Loss ?? 0,
                                    Tie = _context.SeriesTeam
                                            .Where(total => total.TeamID == st.TeamID && total.SeriesID == s.ID)
                                            .Count() - st.Win - (st.Loss ?? 0)
                                }).OrderByDescending(st => st.Win).AsEnumerable()
                            });
            if (active) {
                series = series.Where(s => s.StartDate < DateTime.Now
                                        && s.EndDate > DateTime.Now);
            }
            return series;
        }
        public IEnumerable<SeriesViewModel> GetSeriesByActivityID(int activityID)
        {
            var series = _context.Series
                .Where(s => s.ActivityID == activityID)
                .Select(s => new SeriesViewModel
                {
                    ID = s.ID,
                    Name = s.Name,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    Type = _context.SeriesType
                                .FirstOrDefault(st => st.ID == s.TypeID)
                                .Description,
                    Status = _context.SeriesStatus
                                .FirstOrDefault(ss => ss.ID == s.StatusID)
                                .Description,
                    ActivityID = s.ActivityID,
                    Match = _matchService.GetMatchBySeriesID(s.ID),
                    TeamStanding = _context.SeriesTeam
                    .Where(st => st.SeriesID == s.ID)
                    .Select(st => new TeamRecordViewModel
                    {
                        ID = st.ID,
                        Name = _context.Team
                                .FirstOrDefault(t => t.ID == st.TeamID)
                                .Name,
                        Win = st.Win,
                        Loss = st.Loss ?? 0,
                        //Tie = _context.SeriesTeam
                        //        .Where(total => total.TeamID == st.TeamID && total.SeriesID == s.ID)
                        //        .Count() - st.Win - (st.Loss ?? 0)
                    }).OrderByDescending(st => st.Win).AsEnumerable()
                });
            return series;
        }
        public SeriesViewModel GetSeriesByID(int seriesID)
        {
            return GetSeries().FirstOrDefault(s => s.ID == seriesID);
        }
        public async Task<int> PostSeries(SeriesUploadViewModel newSeries, int? referenceSeriesID)
        {
            var series = new Series
            {
                Name = newSeries.Name,
                StartDate = newSeries.StartDate,
                EndDate = newSeries.EndDate,
                ActivityID = newSeries.ActivityID,
                TypeID = newSeries.TypeID,
                StatusID = 1 //default unconfirmed series
            };
            await _context.Series.AddAsync(series);
            await _context.SaveChangesAsync();

            int seriesID = referenceSeriesID ?? series.ID;
            IEnumerable<int> teams = _context.SeriesTeam
                                .Where(st => st.SeriesID == seriesID)
                                .OrderByDescending(st => st.Win)
                                .Select(st => st.TeamID);

            if (newSeries.NumberOfTeamsAdmitted is not null)
            {
                teams = teams.Take(newSeries.NumberOfTeamsAdmitted ?? 0);//will never be null but 0 is to silence error
            }
           
            await CreateSeriesTeamMapping(teams, series.ID);
            return series.ID;
        }
        public async Task UpdateSeries(SeriesPatchViewModel update)
        {
            var s = await _context.Series.FindAsync(update.ID);
            s.Name = update.Name ?? s.Name;
            s.StartDate = update.StartDate ?? s.StartDate;
            s.EndDate = update.EndDate ?? s.EndDate;
            s.Description = update.Description ?? s.Description;
            s.StatusID = update.StatusID ?? s.StatusID;

            await _context.SaveChangesAsync();
        }
        public async Task UpdateSeriesTeamStats(SeriesTeamPatchViewModel update)
        {
            var st = await _context.SeriesTeam.FindAsync(update.ID);
            st.Win = update.Win ?? st.Win;
            st.Loss = update.Loss ?? st.Loss;

            await _context.SaveChangesAsync();
        }

        private async Task CreateSeriesTeamMapping(IEnumerable<int> teams, int seriesID)
        {
            foreach (var teamID in teams)
            {
                var seriesTeam = new SeriesTeam
                {
                    TeamID = teamID,
                    SeriesID = seriesID,
                    Win = 0,
                    Loss = 0
                };
                await _context.SeriesTeam.AddAsync(seriesTeam);
            }
            await _context.SaveChangesAsync();
        }

        public async Task ScheduleMatches(int seriesID)
        {
            var typeCode = _context.SeriesType
                .FirstOrDefault(st =>
                    st.ID == _context.Series
                    .FirstOrDefault(s => s.ID == seriesID)
                    .TypeID
                ).TypeCode;
            if (typeCode == "rr")
            {
                await ScheduleRoundRobin(seriesID);
            }
            if (typeCode == "se")
            {
                await ScheduleSingleElimination(seriesID);
            }
            if (typeCode == "de")
            {
                await ScheduleDoubleElimination(seriesID);
            }
            if (typeCode == "l")
            {
                await ScheduleLadder(seriesID);
            }
        }
        private async Task ScheduleRoundRobin(int seriesID)
        {
            var teams = _context.SeriesTeam
                .Where(st => st.ID == seriesID)
                .ToArray();
            var series = _context.Series.FirstOrDefault(s => s.ID == seriesID); 
            for (int i = 0; i < teams.Length-1; i++)
            {
                for (int j = i + 1; j < teams.Length; j++)
                {
                    await _matchService.PostMatch(new MatchUploadViewModel
                    {
                        StartTime = series.StartDate,//temporary default
                        SeriesID = seriesID,
                        SurfaceID = 0,
                        TeamIDs = new List<int>() { teams[i].TeamID, teams[j].TeamID }.AsEnumerable()
                    });
                }
            }
        }
        private async Task ScheduleLadder(int seriesID)
        {
            var teams = _context.SeriesTeam
                .Where(st => st.ID == seriesID)
                .Select(st => st.ID);
            var series = _context.Series
                .FirstOrDefault(s => s.ID == seriesID);
            var match = new MatchUploadViewModel
            {
                StartTime = series.StartDate,
                SeriesID = seriesID,
                //to be replaced by proper match surface scheduler
                SurfaceID = 1,
                TeamIDs = teams
            };
            await _matchService.PostMatch(match);
        }
        private async Task ScheduleDoubleElimination(int seriesID)
        {
            var series = _context.Series.FirstOrDefault(s => s.ID == seriesID);
            //Teams are defaulted to be ordered by Wins if there was a reference series
            var teams = _context.SeriesTeam
               .Where(st => st.ID == seriesID);

            //schedule first round
            int teamsInWinners = await ScheduleEliminationRound(teams, null);

            throw new NotImplementedException();

        }
        private async Task ScheduleSingleElimination(int seriesID)
        {
            var series = _context.Series.FirstOrDefault(s => s.ID == seriesID);
            //Teams are defaulted to be ordered by Wins if there was a reference series
            var teams = _context.SeriesTeam
               .Where(st => st.ID == seriesID);

            //schedule first round
            int teamsInNextRound = await ScheduleEliminationRound(teams, null);

            //create matches for remaining rounds (possible implementation of including round number optional field)
            while (teamsInNextRound > 1)
            {
                for (int i = 0; i < teamsInNextRound/2; i++)
                {
                    await _matchService.PostMatch(new MatchUploadViewModel
                    {
                        StartTime = series.StartDate, //temporary before autoscheduling
                        SeriesID = series.ID,
                        SurfaceID = 1, //temporary before 25live integration
                        TeamIDs = new List<int>().AsEnumerable() //no teams
                    });
                }
                teamsInNextRound /= 2;
            }
        }
        public async Task<int> ScheduleEliminationRound(IEnumerable<SeriesTeam> originalTeams, IEnumerable<Match>? matches)
        {
            int numTeams = originalTeams.Count();
            int remainingTeamCount = originalTeams.Count();
            int numBuys = 0; //num matches for next round
            var series = _context.Series.FirstOrDefault(s => s.ID == originalTeams.First().SeriesID);

            var teams = originalTeams.Reverse();
            if (remainingTeamCount % 2 != 0) 
            {
                await UpdateSeriesTeamStats(new SeriesTeamPatchViewModel
                {
                    ID = teams.Last().ID,
                    Win = 1 //Buy round
                });
                teams = teams.Take(--remainingTeamCount);
                numBuys++;
            }
          
            while (!(((numTeams + numBuys) != 0) && (((numTeams + numBuys) & ((numTeams + numBuys) - 1)) == 0))) //while not power of 2
            {
                await UpdateSeriesTeamStats(new SeriesTeamPatchViewModel
                {
                    ID = teams.Last().ID,
                    Win = 1 //Buy round
                });
                teams = teams.Take(--remainingTeamCount);
                numBuys++;
            }

            var teamPairings = EliminationRoundPairs(teams);
            if (matches is null)
            {
                foreach(var teamPair in teamPairings)
                {
                    await _matchService.PostMatch(new MatchUploadViewModel
                    {
                        StartTime = series.StartDate, //temporary before autoscheduling
                        SeriesID = series.ID,
                        SurfaceID = 1, //temporary before 25live integration
                        TeamIDs = teamPair
                    });
                }
            }
            return teamPairings.Count() + numBuys;
        }
        private IEnumerable<IEnumerable<int>> EliminationRoundPairs(IEnumerable<SeriesTeam> teams)
        {
            var res = new List<IEnumerable<int>>();
            var teamsArr = teams.ToArray();

            for (int i = 0; i < teamsArr.Length/2; i++)
            {
                int j = (teamsArr.Length / 2 - 1) - i;
                res.Add(new List<int>
                {
                    teamsArr[i].TeamID,
                    teamsArr[j].TeamID
                }.AsEnumerable());
            }
            return res.AsEnumerable();
        }
    }

}

