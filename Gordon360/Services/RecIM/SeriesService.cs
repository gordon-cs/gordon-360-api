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
using Microsoft.Graph;

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
        public IEnumerable<LookupViewModel> GetSeriesLookup(string type)
        {
            if (type == "status")
            {
                var res = _context.SeriesStatus
                    .Select(s => new LookupViewModel
                    {
                        ID = s.ID,
                        Description = s.Description
                    })
                    .AsEnumerable();
                return res;
            }
            if (type == "series")
            {
                var res = _context.SeriesType
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
            public IEnumerable<SeriesExtendedViewModel> GetSeries(bool active = false)
        {
            var series = _context.Series
                            .Select(s => new SeriesExtendedViewModel
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
                                    Loss = st.Loss,
                                    Tie = _context.SeriesTeam
                                            .Where(total => total.TeamID == st.TeamID && total.SeriesID == s.ID)
                                            .Count() - st.Win - st.Loss
                                }).OrderByDescending(st => st.Win).AsEnumerable()
                            });
            if (active)
            {
                series = series.Where(s => s.StartDate < DateTime.Now
                                        && s.EndDate > DateTime.Now);
            }
            return series;
        }
        public IEnumerable<SeriesExtendedViewModel> GetSeriesByActivityID(int activityID)
        { 
            var series = _context.Series
                .Where(s => s.ActivityID == activityID)
                .Select(s => new SeriesExtendedViewModel
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
                        Loss = st.Loss,
                        //Tie = _context.SeriesTeam
                        //        .Where(total => total.TeamID == st.TeamID && total.SeriesID == s.ID)
                        //        .Count() - st.Win - (st.Loss ?? 0)
                    }).OrderByDescending(st => st.Win).AsEnumerable()
                });
            return series;
        }
        public SeriesExtendedViewModel GetSeriesByID(int seriesID)
        {
            return GetSeries().FirstOrDefault(s => s.ID == seriesID);
        }
        public async Task<SeriesViewModel> PostSeriesAsync(SeriesUploadViewModel newSeries, int? referenceSeriesID)
        {
            var series = new Series
            {
                Name = newSeries.Name,
                StartDate = newSeries.StartDate,
                EndDate = newSeries.EndDate,
                ActivityID = newSeries.ActivityID,
                TypeID = newSeries.TypeID,
                StatusID = 1, //default unconfirmed series
                ScheduleID = 2 //temporary while autoscheduling is not completed
            };
            await _context.Series.AddAsync(series);
            await _context.SaveChangesAsync();

            IEnumerable<int> teams = new List<int>();
            if (referenceSeriesID is not null)
            {
                teams = _context.Team
                        .Where(t => t.ActivityID == series.ActivityID)
                        .Select(t => t.ID)
                        .AsEnumerable();
            }
            else
            {
                teams = _context.SeriesTeam
                                .Where(st => st.SeriesID == referenceSeriesID)
                                .OrderByDescending(st => st.Win)
                                .Select(st => st.TeamID);
            }
            if (newSeries.NumberOfTeamsAdmitted is not null)
            {
                teams = teams.Take(newSeries.NumberOfTeamsAdmitted ?? 0);//will never be null but 0 is to silence error
            }
            
            await CreateSeriesTeamMappingAsync(teams, series.ID);
            return series;
        }
        public async Task<SeriesViewModel> UpdateSeriesAsync(int seriesID, SeriesPatchViewModel update)
        {
            var s = await _context.Series.FindAsync(seriesID);
            s.Name = update.Name ?? s.Name;
            s.StartDate = update.StartDate ?? s.StartDate;
            s.EndDate = update.EndDate ?? s.EndDate;
            s.StatusID = update.StatusID ?? s.StatusID;

            await _context.SaveChangesAsync();
            return s;
        }
        public async Task UpdateSeriesTeamStats(SeriesTeamPatchViewModel update)
        {
            var st = await _context.SeriesTeam.FindAsync(update.ID);
            st.Win = update.Win ?? st.Win;
            st.Loss = update.Loss ?? st.Loss;

            await _context.SaveChangesAsync();
        }

        private async Task CreateSeriesTeamMappingAsync(IEnumerable<int> teams, int seriesID)
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

        // Scheduler does not currently handle overlaps
        // eventually:
        // - ensure that matches that occur within 1 hour do not share the same surface
        //    unless they're in the same series
        public async Task ScheduleMatchesAsync(int seriesID)
        {
            var typeCode = _context.SeriesType
                .FirstOrDefault(st =>
                    st.ID == _context.Series
                    .FirstOrDefault(s => s.ID == seriesID)
                    .TypeID
                ).TypeCode;
            switch (typeCode)
            {
                case "rr":
                    await ScheduleRoundRobin(seriesID);
                    break;
                case "se":
                    await ScheduleSingleElimination(seriesID);
                    break;
                case "de":
                    await ScheduleDoubleElimination(seriesID);
                    break;
                case "l":
                    await ScheduleLadderAsync(seriesID);
                    break;
                default:
                    break;
            }
        }
        private async Task ScheduleRoundRobin(int seriesID)
        {
            var series = _context.Series.FirstOrDefault(s => s.ID == seriesID);
            var teams = _context.SeriesTeam
                .Where(st => st.ID == seriesID)
                .ToList();
            SeriesScheduleViewModel schedule = _context.SeriesSchedule
                            .FirstOrDefault(ss => ss.ID ==
                                _context.Series
                                    .FirstOrDefault(s => s.ID == seriesID)
                                    .ScheduleID);
            var availableSurfaces = _context.SeriesSurface
                                        .Where(ss => ss.SeriesID == seriesID)
                                        .ToList();

            //day = starting datetime accurate to minute and seconds based on scheduler
            var day = series.StartDate;
            day = new DateTime(day.Year, day.Month, day.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second);
            string dayOfWeek = day.DayOfWeek.ToString();
            
            int surfaceIndex = 0;
            for (int cycles = 0; cycles < teams.Count - 1; cycles++)
            {
                int i = 0;
                int j = teams.Count - 1;
                while (i < j) //middlepoint algorithm to match opposite teams
                {
                    if (surfaceIndex == availableSurfaces.Count - 1)
                    {
                        surfaceIndex = 0;
                        day.AddMinutes(schedule.EstMatchTime + 15);//15 minute buffer between matches as suggested by customer
                    }

                    //ensure matchtime is in an available day will be a "bug" if the match goes beyond 12AM
                    //minor bug as it just means that some games will be scheduled on the next possible day
                    //even if they are "hypothetically" able to play on the original day
                    while (!schedule.AvailableDays[dayOfWeek] ||
                        day.AddMinutes(schedule.EstMatchTime + 15).TimeOfDay > schedule.EndTime.TimeOfDay
                        )
                    {
                        day = day.AddDays(1);
                        day = new DateTime(day.Year, day.Month, day.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second);
                        dayOfWeek = day.DayOfWeek.ToString();
                        surfaceIndex = 0;
                    }

                    await _matchService.PostMatchAsync(new MatchUploadViewModel
                    {
                        StartTime = day,
                        SeriesID = seriesID,
                        SurfaceID = availableSurfaces[surfaceIndex].SurfaceID,
                        TeamIDs = new List<int>() { teams[i].TeamID, teams[j].TeamID }.AsEnumerable()
                    });
                    surfaceIndex++;
                    i++;
                    j--;
                }
                var temp = teams[0];
                teams.RemoveAt(0);
                teams.Add(temp);
            }   
        }
        private async Task ScheduleLadderAsync(int seriesID)
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

            await _matchService.PostMatchAsync(match);
        }
        private async Task ScheduleDoubleElimination(int seriesID)
        {
            var series = _context.Series.FirstOrDefault(s => s.ID == seriesID);
            //Teams are defaulted to be ordered by Wins if there was a reference series
            var teams = _context.SeriesTeam
               .Where(st => st.ID == seriesID);

            //schedule first round
            var elimScheduler = await ScheduleElimRound(teams);
            int teamsInWinners = elimScheduler.TeamsInNextRound;
            int teamsInLosers = teams.Count() - teamsInWinners;

            //create matches for losers bracket
            int numBuys = 0;
            int teamCount = (int)teamsInLosers; //casting to avoid reference value
            while (!(((teamsInLosers + numBuys) != 0) && (((teamsInLosers + numBuys) & ((teamsInLosers + numBuys) - 1)) == 0))) //while not power of 2
            {
                await _matchService.PostMatchAsync(new MatchUploadViewModel
                {
                    StartTime = series.StartDate, //temporary before autoscheduling
                    SeriesID = series.ID,
                    SurfaceID = 1, //temporary before 25live integration
                    TeamIDs = new List<int>().AsEnumerable() //no teams
                });
                teamCount--;
                numBuys++;
            }
            //teams in losers has weird logic where each team actually represents a match that needs to be made
            //since each team will be paired with a team from the upper bracket
            teamsInLosers = teamCount + numBuys;
            while (teamsInLosers > 1)
            {
                for (int i = 0; i < teamsInLosers; i++)
                {
                    await _matchService.PostMatchAsync(new MatchUploadViewModel
                    {
                        StartTime = series.StartDate, //temporary before autoscheduling
                        SeriesID = series.ID,
                        SurfaceID = 1, //temporary before 25live integration
                        TeamIDs = new List<int>().AsEnumerable() //no teams
                    });
                }
                teamsInLosers /= 2;
            }
            //create matches for winners bracket
            while (teamsInWinners > 0)
            {
                for (int i = 0; i < teamsInWinners / 2; i++)
                {
                    await _matchService.PostMatchAsync(new MatchUploadViewModel
                    {
                        StartTime = series.StartDate, //temporary before autoscheduling
                        SeriesID = series.ID,
                        SurfaceID = 1, //temporary before 25live integration
                        TeamIDs = new List<int>().AsEnumerable() //no teams
                    });
                }
                teamsInWinners /= 2;
            }
        }
        private async Task ScheduleSingleElimination(int seriesID)
        {
            var series = _context.Series.FirstOrDefault(s => s.ID == seriesID);
            //Teams are defaulted to be ordered by Wins if there was a reference series
            var teams = _context.SeriesTeam
               .Where(st => st.ID == seriesID);

            SeriesScheduleViewModel schedule = _context.SeriesSchedule
                            .FirstOrDefault(ss => ss.ID ==
                                _context.Series
                                    .FirstOrDefault(s => s.ID == seriesID)
                                    .ScheduleID);
            var availableSurfaces = _context.SeriesSurface
                                        .Where(ss => ss.SeriesID == seriesID)
                                        .ToArray();
            var day = series.StartDate;
            day = new DateTime(day.Year, day.Month, day.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second);
            string dayOfWeek = day.DayOfWeek.ToString();
            int surfaceIndex = 0;

            //schedule first round
            var elimScheduler = await ScheduleElimRound(teams);
            int teamsInNextRound = elimScheduler.TeamsInNextRound;
            foreach (var matchID in elimScheduler.MatchID)
            {
                if (surfaceIndex == availableSurfaces.Length)
                {
                    surfaceIndex = 0;
                    day.AddMinutes(schedule.EstMatchTime + 15);
                }
                while (!schedule.AvailableDays[dayOfWeek] ||
                    day.AddMinutes(schedule.EstMatchTime + 15).TimeOfDay > schedule.EndTime.TimeOfDay
                    )
                {
                    day = day.AddDays(1);
                    day = new DateTime(day.Year, day.Month, day.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second);
                    dayOfWeek = day.DayOfWeek.ToString();
                    surfaceIndex = 0;
                }
                await _matchService.UpdateMatchAsync(matchID,
                    new MatchPatchViewModel
                    {
                        Time = day,
                        SurfaceID = availableSurfaces[surfaceIndex].SurfaceID
                    });
                surfaceIndex++;
            }
            //create matches for remaining rounds (possible implementation of including round number optional field)
            while (teamsInNextRound > 1)
            {
                for (int i = 0; i < teamsInNextRound / 2; i++)
                {
                    if (surfaceIndex == availableSurfaces.Length)
                    {
                        surfaceIndex = 0;
                        day.AddMinutes(schedule.EstMatchTime + 15);
                    }
                    while (!schedule.AvailableDays[dayOfWeek] || day.AddMinutes(schedule.EstMatchTime + 15).TimeOfDay > schedule.EndTime.TimeOfDay)
                    {
                        day = day.AddDays(1);
                        day = new DateTime(day.Year, day.Month, day.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second);
                        dayOfWeek = day.DayOfWeek.ToString();
                        surfaceIndex = 0;
                    }
                    await _matchService.PostMatchAsync(new MatchUploadViewModel
                    {
                        StartTime = day,
                        SeriesID = series.ID,
                        SurfaceID = availableSurfaces[surfaceIndex].SurfaceID, //temporary before 25live integration
                        TeamIDs = new List<int>().AsEnumerable() //no teams
                    });
                }
                teamsInNextRound /= 2;
                //reset between rounds
                day = day.AddDays(1);
                day = new DateTime(day.Year, day.Month, day.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second);
                dayOfWeek = day.DayOfWeek.ToString();
                surfaceIndex = 0;
            }
        }

        /// <summary>
        /// Goal of this function is to be able to either create and initialize the elimination bracket
        /// if none exists already, and will overload with a matches enumerable to update existing matches
        /// to produce a next round
        /// 
        /// These functions may need to be modified later as there may be more efficient ways to handle scheduling
        /// with the context that surfaces need to be booked ahead of time on 25Live
        /// </summary>
        public async Task<EliminationRound> ScheduleElimRound(IEnumerable<SeriesTeam> involvedTeams)
        {
            int numTeams = involvedTeams.Count();
            int remainingTeamCount = involvedTeams.Count();
            int numBuys = 0;
            var series = _context.Series.FirstOrDefault(s => s.ID == involvedTeams.First().SeriesID);

            var teams = involvedTeams.Reverse();

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

            var teamPairings = EliminationRoundTeamPairs(teams);
            var matchIDs = new List<int>();

            foreach (var teamPair in teamPairings)
            {
                var createdMatch = await _matchService.PostMatchAsync(new MatchUploadViewModel
                {
                    StartTime = series.StartDate, //temporary before autoscheduling
                    SeriesID = series.ID,
                    SurfaceID = 1, //temporary before 25live integration
                    TeamIDs = teamPair
                });
                matchIDs.Add(createdMatch.ID);
            }
            return new EliminationRound
            {
                TeamsInNextRound = teamPairings.Count() + numBuys,
                MatchID = matchIDs
            };
        }
        public async Task<EliminationRound> ScheduleElimRound(IEnumerable<Match>? matches)
        {
            throw new NotImplementedException();
        }


        private IEnumerable<IEnumerable<int>> EliminationRoundTeamPairs(IEnumerable<SeriesTeam> teams)
        {
            var res = new List<IEnumerable<int>>();
            var teamsArr = teams.ToArray();

            for (int i = 0; i < teamsArr.Length / 2; i++)
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

