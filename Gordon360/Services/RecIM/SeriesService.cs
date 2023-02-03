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
                ScheduleID = 0 //updated when admin is ready to set up the schedule
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

        public async Task<SeriesScheduleViewModel> PutSeriesScheduleAsync(SeriesScheduleUploadViewModel seriesSchedule)
        {
            var existingSchedule = _context.SeriesSchedule.FirstOrDefault(ss => 
                ss.StartTime.Hour == seriesSchedule.DailyStartTime.Hour &&
                ss.StartTime.Minute == seriesSchedule.DailyStartTime.Minute &&
                ss.EndTime.Hour == seriesSchedule.DailyEndTime.Hour &&
                ss.EndTime.Minute == seriesSchedule.DailyEndTime.Minute &&
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
                    var series = _context.Series.FirstOrDefault(s => s.ID == seriesSchedule.SeriesID);
                    series.ScheduleID = existingSchedule.ID;
                    await _context.SaveChangesAsync();
                }
                return existingSchedule;
            }

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
                var series = _context.Series.FirstOrDefault(s => s.ID == seriesSchedule.SeriesID);
                series.ScheduleID = schedule.ID;
            }
            await _context.SaveChangesAsync();
            return schedule;

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
        public async Task<IEnumerable<MatchViewModel>> ScheduleMatchesAsync(int seriesID)
        {
            var createdMatches = new List<MatchViewModel>();

            var series = _context.Series
                    .FirstOrDefault(s => s.ID == seriesID);
            var typeCode = _context.SeriesType
                .FirstOrDefault(st =>
                    st.ID == series.TypeID
                ).TypeCode;

            if (typeCode == "RR")
            {
                await ScheduleRoundRobin(seriesID);
            }
            if (typeCode == "SE")
            {
                await ScheduleSingleElimination(seriesID);
            }
            if (typeCode == "DE")
            {
                await ScheduleDoubleElimination(seriesID);
            }
            if (typeCode == "L")
            {
                await ScheduleLadderAsync(seriesID);
            }

            return createdMatches;
        }
        private async Task<IEnumerable<MatchViewModel>> ScheduleRoundRobin(int seriesID)
        {
            var createdMatches = new List<MatchViewModel>();
            var series = _context.Series.FirstOrDefault(s => s.ID == seriesID);
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
            var teams = _context.SeriesTeam
                .Where(st => st.ID == seriesID)
                .Select(st => st.ID);
            var series = _context.Series
                .FirstOrDefault(s => s.ID == seriesID);
            var availableSurfaces = _context.SeriesSurface
                                        .Where(ss => ss.SeriesID == seriesID)?
                                        .ToArray();
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
            throw new NotImplementedException();
            }
        }
}

