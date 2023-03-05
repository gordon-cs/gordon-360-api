using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
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

        public IEnumerable<LookupViewModel>? GetSeriesLookup(string type)
        {
            return type switch
            {
                "status" => _context.SeriesStatus.Where(query => query.ID != 0)
                    .Select(s => new LookupViewModel
                    {
                        ID = s.ID,
                        Description = s.Description
                    })
                    .AsEnumerable(),
                "series" => _context.SeriesType.Where(query => query.ID != 0)
                    .Select(s => new LookupViewModel
                    {
                        ID = s.ID,
                        Description = s.Description
                    })
                    .AsEnumerable(),
                _ => null
            };
        }

        public IEnumerable<SeriesExtendedViewModel> GetSeries(bool active = false)
        {
            var series = _context.Series
                .Where(s => s.StatusID != 0)
                    .Select(s => new SeriesExtendedViewModel
                    {
                        ID = s.ID,
                        Name = s.Name,
                        StartDate = s.StartDate,
                        EndDate = s.EndDate,
                        Type = s.Type.Description,
                        Status = s.Status.Description,
                        ActivityID = s.ActivityID,
                        Match = _matchService.GetMatchesBySeriesID(s.ID),
                        TeamStanding = _context.SeriesTeam
                        .Where(st => st.SeriesID == s.ID && st.Team.StatusID != 0)
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
            return GetSeries().Where(s => s.ActivityID == activityID).ToList();
        }

        public SeriesExtendedViewModel GetSeriesByID(int seriesID)
        {
            return GetSeries().FirstOrDefault(s => s.ID == seriesID);
        }

        public async Task<SeriesViewModel> PostSeriesAsync(SeriesUploadViewModel newSeries, int? referenceSeriesID)
        {
            //if activity has no start date
            var activity = _context.Activity.FirstOrDefault(a => a.ID == newSeries.ActivityID);
            if (activity.StartDate is null) activity.StartDate = newSeries.StartDate;

            // activity will inherit new end date if series ends after activity ends, OR if activity end date is null
            if (activity.EndDate is null || activity.EndDate < newSeries.EndDate) activity.EndDate = newSeries.EndDate;

            // inherit activity series schedule id if own scheduleID is null
            var activityInheritiedSeriesScheduleID = activity.SeriesScheduleID ?? 0;
            var series = new Series
            {
                Name = newSeries.Name,
                StartDate = newSeries.StartDate,
                EndDate = newSeries.EndDate,
                ActivityID = newSeries.ActivityID,
                TypeID = newSeries.TypeID,
                StatusID = 1, //default unconfirmed series
                ScheduleID = newSeries.ScheduleID ?? activityInheritiedSeriesScheduleID //updated when admin is ready to set up the schedule
            };
            await _context.Series.AddAsync(series);
            await _context.SaveChangesAsync();

            IEnumerable<int> teams = new List<int>();
            if (referenceSeriesID is null)
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
                teams = teams.Take(newSeries.NumberOfTeamsAdmitted ?? 0);//will never be null after the if check but 0 is to silence error
            }

            await CreateSeriesTeamMappingAsync(teams, series.ID);
            return series;
        }

        public SeriesScheduleViewModel GetSeriesScheduleByID(int seriesID)
        {
            int scheduleID = _context.Series.FirstOrDefault(s => s.ID == seriesID)?.ScheduleID ?? 0;
            return _context.SeriesSchedule.FirstOrDefault(ss => ss.ID == scheduleID);
        }

        public async Task<SeriesScheduleViewModel> PutSeriesScheduleAsync(SeriesScheduleUploadViewModel seriesSchedule)
        {
            //check for exact schedule existing
            var existingSchedule = _context.SeriesSchedule.FirstOrDefault(ss =>
                ss.StartTime.TimeOfDay == seriesSchedule.DailyStartTime.TimeOfDay &&
                ss.EndTime.TimeOfDay == seriesSchedule.DailyEndTime.TimeOfDay &&
                ss.EstMatchTime == seriesSchedule.EstMatchTime &&
                ss.Sun == seriesSchedule.AvailableDays.Sun &&
                ss.Mon == seriesSchedule.AvailableDays.Mon &&
                ss.Tue == seriesSchedule.AvailableDays.Tue &&
                ss.Wed == seriesSchedule.AvailableDays.Wed &&
                ss.Thu == seriesSchedule.AvailableDays.Thu &&
                ss.Fri == seriesSchedule.AvailableDays.Fri &&
                ss.Sat == seriesSchedule.AvailableDays.Sat
            );
            if (existingSchedule is not null)
            {
                if (seriesSchedule.SeriesID is not null)
                {
                    var series = _context.Series.Find(seriesSchedule.SeriesID);
                    //if the series is deleted, throw exception
                    if (series.StatusID == 0) throw new UnprocessibleEntity { ExceptionMessage = "Series has been deleted" };

                    if (series != null) return existingSchedule;
                    series.ScheduleID = existingSchedule.ID;
                    await _context.SaveChangesAsync();
                }
                return existingSchedule;
            }
            // if schedule does not exist
            var schedule = new SeriesSchedule
            {
                Sun = seriesSchedule.AvailableDays.Sun,
                Mon = seriesSchedule.AvailableDays.Mon,
                Tue = seriesSchedule.AvailableDays.Tue,
                Wed = seriesSchedule.AvailableDays.Wed,
                Thu = seriesSchedule.AvailableDays.Thu,
                Fri = seriesSchedule.AvailableDays.Fri,
                Sat = seriesSchedule.AvailableDays.Sat,
                EstMatchTime = seriesSchedule.EstMatchTime,
                StartTime = seriesSchedule.DailyStartTime,
                EndTime = seriesSchedule.DailyEndTime
            };
            await _context.SeriesSchedule.AddAsync(schedule);
            await _context.SaveChangesAsync();

            if (seriesSchedule.SeriesID is not null)
            {
                var series = _context.Series.Find(seriesSchedule.SeriesID);
                series.ScheduleID = schedule.ID;
            }
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<SeriesViewModel> UpdateSeriesAsync(int seriesID, SeriesPatchViewModel update)
        {
            var s = _context.Series.Find(seriesID);
            //if the series is deleted, throw exception
            if (s.StatusID == 0) throw new UnprocessibleEntity { ExceptionMessage = "Series has been deleted" };

            s.Name = update.Name ?? s.Name;
            s.StartDate = update.StartDate ?? s.StartDate;
            s.EndDate = update.EndDate ?? s.EndDate;
            s.StatusID = update.StatusID ?? s.StatusID;

            await _context.SaveChangesAsync();
            return s;
        }

        public async Task UpdateSeriesTeamStats(SeriesTeamPatchViewModel update)
        {
            var st = _context.SeriesTeam.Find(update.ID);
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

        public async Task<SeriesViewModel> DeleteSeriesCascadeAsync(int seriesID)
        {
            //delete series
            var series = _context.Series.Find(seriesID);
            series.StatusID = 0;

            var seriesTeam = series.SeriesTeam;
            var matches = series.Match;

            //delete series teams (series team does not need to be fully deleted, a wipe is sufficient)
            foreach (var st in seriesTeam)
            {
                st.Win = 0;
                st.Loss = 0;
            }
            //delete matches
            foreach (var match in matches)
            {
                //delete matchteam
                foreach (var mt in match.MatchTeam)
                    mt.StatusID = 0;
                //deletematch
                match.StatusID = 0;
            }
            
            await _context.SaveChangesAsync();
            return series;
        }

        /// <summary>
        /// Scheduler does not currently handle overlaps
        /// eventually:
        /// - ensure that matches that occur within 1 hour do not share the same surface
        ///    unless they're in the same series
        /// </summary>
        /// <param name="seriesID"></param>
        /// <returns>Created Match objects</returns>
        public async Task<IEnumerable<MatchViewModel>?> ScheduleMatchesAsync(int seriesID)
        {
            var series = _context.Series.Find(seriesID);
            //if the series is deleted, throw exception
            if (series.StatusID == 0) throw new UnprocessibleEntity { ExceptionMessage = "Series has been deleted" };

            var typeCode = series.Type.TypeCode;

            return typeCode switch
            {
                "RR" => await ScheduleRoundRobin(seriesID),
                "SE" => await ScheduleSingleElimination(seriesID),
                "DE" => await ScheduleDoubleElimination(seriesID),
                "L" => await ScheduleLadderAsync(seriesID),
            _ => null
            };
        }

        private async Task<IEnumerable<MatchViewModel>> ScheduleRoundRobin(int seriesID)
        {
            var createdMatches = new List<MatchViewModel>();
            var series = _context.Series.Find(seriesID);
            var teams = _context.SeriesTeam
                .Where(st => st.SeriesID == seriesID)
                .Select(st => st.TeamID)
                .ToList();

            //algorithm requires odd number of teams
            teams.Add(0);//0 is not a valid true team ID thus will act as dummy team

            SeriesScheduleViewModel schedule = _context.SeriesSchedule
                            .FirstOrDefault(ss => ss.ID ==
                                _context.Series
                                    .FirstOrDefault(s => s.ID == seriesID)
                                    .ScheduleID);
            var availableSurfaces = _context.SeriesSurface
                                        .Where(ss => ss.SeriesID == seriesID)
                                        .ToArray();

            //day = starting datetime accurate to minute and seconds based on scheduler
            var day = series.StartDate;
            day = new DateTime(day.Year, day.Month, day.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second);
            string dayOfWeek = day.DayOfWeek.ToString();

            int surfaceIndex = 0;
            for (int cycles = 0; cycles < teams.Count; cycles++)
            {
                int i = 0;
                int j = teams.Count - 1;
                while (i < j) //middlepoint algorithm to match opposite teams
                {
                    if (surfaceIndex == availableSurfaces.Length)
                    {
                        surfaceIndex = 0;
                        day = day.AddMinutes(schedule.EstMatchTime + 15);//15 minute buffer between matches as suggested by customer
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

                    var teamIDs = new List<int>() { teams[i], teams[j] };
                    if (!teamIDs.Contains(0))
                    {
                        var createdMatch = await _matchService.PostMatchAsync(new MatchUploadViewModel
                        {
                            StartTime = day,
                            SeriesID = seriesID,
                            SurfaceID = availableSurfaces[surfaceIndex].SurfaceID,
                            TeamIDs = teamIDs
                        });
                        createdMatches.Add(createdMatch);
                        surfaceIndex++;
                    }
                    i++;
                    j--;
                }
                var temp = teams[0];
                teams.Add(temp);
                teams.RemoveAt(0);
            }
            return createdMatches;
        }

        //rudamentary implementation (only allows all teams into 1 match)
        private async Task<IEnumerable<MatchViewModel>> ScheduleLadderAsync(int seriesID)
        {
            var createdMatches = new List<MatchViewModel>();
            var series = _context.Series.Find(seriesID);
            var teams = series.SeriesTeam
                .Select(st => st.TeamID);
            var availableSurfaces = series.SeriesSurface.ToArray();
            var surfaceIndex = 0;
            var match = new MatchUploadViewModel
            {
                StartTime = series.StartDate,
                SeriesID = seriesID,
                SurfaceID = availableSurfaces is null ? 1 : availableSurfaces[surfaceIndex].SurfaceID,
                TeamIDs = teams
            };
            //surfaceIndex++; //surfaceIndex can be incremented if we plan to rework ladder match logic (to make more than 1 match)

            var res = await _matchService.PostMatchAsync(match);
            createdMatches.Add(res);
            return createdMatches;
        }

        private async Task<IEnumerable<MatchViewModel>> ScheduleDoubleElimination(int seriesID)
        {
            throw new NotImplementedException();
        }
        private async Task<IEnumerable<MatchViewModel>> ScheduleSingleElimination(int seriesID)
        {
            var createdMatches = new List<MatchViewModel>();
            var series = _context.Series.Find(seriesID);
            var teams = series.SeriesTeam.OrderByDescending(st => st.Win);

            SeriesScheduleViewModel schedule = series.Schedule;

            var availableSurfaces = _context.SeriesSurface
                                        .Where(ss => ss.SeriesID == seriesID)
                                        .ToArray();
            var day = series.StartDate;
            day = new DateTime(day.Year, day.Month, day.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second);
            string dayOfWeek = day.DayOfWeek.ToString();
            int surfaceIndex = 0;

            //schedule first round
            var elimScheduler = await ScheduleElimRoundAsync(teams);
            int teamsInNextRound = elimScheduler.TeamsInNextRound;
            var matchIDs = elimScheduler.Match.Select(es => es.ID);
            foreach (var matchID in matchIDs)
            {
                if (surfaceIndex == availableSurfaces.Length)
                {
                    surfaceIndex = 0;
                    day = day.AddMinutes(schedule.EstMatchTime + 15);
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
                var createdMatch = await _matchService.UpdateMatchAsync(matchID,
                    new MatchPatchViewModel
                    {
                        Time = day,
                        SurfaceID = availableSurfaces[surfaceIndex].SurfaceID
                    });
                createdMatches.Add(createdMatch);
                surfaceIndex++;
            }
            //create matches for remaining rounds (possible implementation of including round number optional field)
            while (teamsInNextRound > 1)
            {
                //reset between rounds + prime the pump from first round
                day = day.AddDays(1);
                day = new DateTime(day.Year, day.Month, day.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second);
                dayOfWeek = day.DayOfWeek.ToString();
                surfaceIndex = 0;
                for (int i = 0; i < teamsInNextRound / 2; i++)
                {
                    if (surfaceIndex == availableSurfaces.Length)
                    {
                        surfaceIndex = 0;
                        day = day.AddMinutes(schedule.EstMatchTime + 15);
                    }
                    while (!schedule.AvailableDays[dayOfWeek] || day.AddMinutes(schedule.EstMatchTime + 15).TimeOfDay > schedule.EndTime.TimeOfDay)
                    {
                        day = day.AddDays(1);
                        day = new DateTime(day.Year, day.Month, day.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second);
                        dayOfWeek = day.DayOfWeek.ToString();
                        surfaceIndex = 0;
                    }
                    var createdMatch = await _matchService.PostMatchAsync(new MatchUploadViewModel
                    {
                        StartTime = day,
                        SeriesID = series.ID,
                        SurfaceID = availableSurfaces[surfaceIndex].SurfaceID, //temporary before 25live integration
                        TeamIDs = new List<int>().AsEnumerable() //no teams
                    });
                    createdMatches.Add(createdMatch);
                }
                teamsInNextRound /= 2;
            }
            return createdMatches;
        }

        /// <summary>
        /// Goal of this function is to generate a single elimination round.
        /// On the first possible round, this would imply handling teams with buys to ensure
        /// that the second round will be in a power of 2 (so that no further rounds need buys). 
        /// 
        /// These functions may need to be modified later as there may be more efficient ways to handle scheduling
        /// with the context that surfaces need to be booked ahead of time on 25Live
        /// </summary>
        /// <returns>Matches created as well as number of teams in the next round</returns>
        public async Task<EliminationRound> ScheduleElimRoundAsync(IEnumerable<SeriesTeam> involvedTeams)
        {
            int numTeams = involvedTeams.Count();
            int remainingTeamCount = involvedTeams.Count();
            int numBuys = 0;
            var series = _context.Series.Find(involvedTeams.First().SeriesID);

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

            var teamPairings = EliminationRoundTeamPairsAsync(teams);
            var matches = new List<MatchViewModel>();

            foreach (var teamPair in teamPairings)
            {
                var createdMatch = await _matchService.PostMatchAsync(new MatchUploadViewModel
                {
                    StartTime = series.StartDate, //temporary before autoscheduling
                    SeriesID = series.ID,
                    SurfaceID = 1, //temporary before 25live integration
                    TeamIDs = teamPair
                });
                matches.Add(createdMatch);
            }
            return new EliminationRound
            {
                TeamsInNextRound = teamPairings.Count() + numBuys,
                Match = matches
            };
        }
        public async Task<EliminationRound> ScheduleElimRoundAsync(IEnumerable<Match>? matches)
        {
            throw new NotImplementedException();
        }

        private static IEnumerable<IEnumerable<int>> EliminationRoundTeamPairsAsync(IEnumerable<SeriesTeam> teams)
        {
            var res = new List<IEnumerable<int>>();
            var teamsArr = teams.ToArray();
            int i = 0;
            int j = teamsArr.Length - 1;

            while (i < j)
            {
                res.Add(new List<int>
                {
                    teamsArr[i].TeamID,
                    teamsArr[j].TeamID
                }.AsEnumerable());
                i++;
                j--;
            }
            return res.AsEnumerable();
        }
    }
}