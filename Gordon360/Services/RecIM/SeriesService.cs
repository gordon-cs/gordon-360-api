using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gordon360.Extensions.System;

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
                .Include(s => s.Schedule)
                .Where(s => s.StatusID != 0)
                    .Select(s => new SeriesExtendedViewModel
                    {
                        ID = s.ID,
                        Name = s.Name,
                        StartDate = s.StartDate.SpecifyUtc(),
                        EndDate = s.EndDate.SpecifyUtc(),
                        Type = s.Type.Description,
                        Status = s.Status.Description,
                        ActivityID = s.ActivityID,
                        Match = _matchService.GetMatchesBySeriesID(s.ID),
                        TeamStanding = _context.SeriesTeam
                            .Where(st => st.SeriesID == s.ID && st.Team.StatusID != 0)
                            .Select(st => new TeamRecordViewModel
                            {
                                SeriesID = st.ID,
                                Name = _context.Team
                                        .FirstOrDefault(t => t.ID == st.TeamID)
                                        .Name,
                                WinCount = st.WinCount,
                                LossCount = st.LossCount,
                                TieCount = _context.SeriesTeam
                                        .Where(_st => _st.TeamID == st.TeamID && _st.SeriesID == s.ID)
                                        .Count() - st.WinCount - st.LossCount
                            }).OrderByDescending(st => st.WinCount).AsEnumerable(),
                        Schedule = s.Schedule
                    });
            if (active)
            {
                series = series.Where(s => s.StartDate < DateTime.UtcNow
                                        && s.EndDate > DateTime.UtcNow);
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
            var series = newSeries.ToSeries(activityInheritiedSeriesScheduleID);

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
                                .OrderByDescending(st => st.WinCount)
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
            int scheduleID = _context.Series.Find(seriesID)?.ScheduleID ?? 0;
            return _context.SeriesSchedule.Find(scheduleID);
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
            // sanitize start/end time dates
            var defaultDate = new DateTime(1998,12,23);
            var start = defaultDate.Add(seriesSchedule.DailyStartTime.TimeOfDay);
            var end = defaultDate.Add(seriesSchedule.DailyEndTime.TimeOfDay);

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
                StartTime = start,
                EndTime = end
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
            var series = _context.Series
                .Include(s => s.SeriesTeam)
                .FirstOrDefault(s => s.ID == seriesID);

            var seriesTeams = series.SeriesTeam;
            
            //if the series is deleted, throw exception
            if (series.StatusID == 0) throw new UnprocessibleEntity { ExceptionMessage = "Series has been deleted" };

            series.Name = update.Name ?? series.Name;
            series.StartDate = update.StartDate ?? series.StartDate;
            series.EndDate = update.EndDate ?? series.EndDate;
            series.StatusID = update.StatusID ?? series.StatusID;
            series.ScheduleID = update.ScheduleID ?? series.ScheduleID;
            
            //update teams
            if (update.TeamIDs is not null)
            {
                var updatedSeriesTeams = update.TeamIDs.ToList();
                foreach (var team in seriesTeams)
                {
                    if (!update.TeamIDs.Any(id => id == team.TeamID))
                        _context.SeriesTeam.Remove(team);
                    else
                        updatedSeriesTeams.Remove(team.TeamID);
                }

                foreach (var teamID in updatedSeriesTeams)
                    await _context.SeriesTeam.AddAsync(
                        new SeriesTeam
                        {
                            TeamID = teamID,
                            SeriesID = seriesID,
                            WinCount = 0,
                            LossCount = 0
                        });
            }
            
            await _context.SaveChangesAsync();
            return series;
        }

        public async Task UpdateSeriesTeamStats(SeriesTeamPatchViewModel update)
        {
            var st = _context.SeriesTeam.Find(update.ID);
            st.WinCount = update.WinCount ?? st.WinCount;
            st.LossCount = update.LossCount ?? st.LossCount;

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
                    WinCount = 0,
                    LossCount = 0
                };
                await _context.SeriesTeam.AddAsync(seriesTeam);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<SeriesViewModel> DeleteSeriesCascadeAsync(int seriesID)
        {
            //delete series
            var series = _context.Series
                .Include(s => s.SeriesTeam)
                .Include(s => s.Match)
                    .ThenInclude(s => s.MatchTeam)
                .FirstOrDefault(s => s.ID == seriesID);
            series.StatusID = 0;

            var seriesTeam = series.SeriesTeam;
            var matches = series.Match;

            //delete series teams (series team does not need to be fully deleted, a wipe is sufficient)
            foreach (var st in seriesTeam)
            {
                st.WinCount = 0;
                st.LossCount = 0;
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
        public async Task<IEnumerable<MatchViewModel>?> ScheduleMatchesAsync(int seriesID, UploadScheduleRequest request)
        {
            var series = _context.Series
                .Include(s => s.Type)
                .FirstOrDefault(s => s.ID == seriesID);
            // if the series is deleted, throw exception
            if (series.StatusID == 0) throw new UnprocessibleEntity { ExceptionMessage = "Series has been deleted" };

            // ensure series has surfaces to autoschedule on
            if (!_context.SeriesSurface.Any(ss => ss.SeriesID == seriesID)) throw new UnprocessibleEntity { ExceptionMessage = "Series has no specified surfaces" };

            var typeCode = series.Type.TypeCode;

            return typeCode switch
            {
                "RR" => await ScheduleRoundRobin(seriesID, request),
                "SE" => await ScheduleSingleElimination(seriesID),
                "DE" => await ScheduleDoubleElimination(seriesID),
                "L" => await ScheduleLadderAsync(seriesID, request),
            _ => null
            };
        }

        private async Task<IEnumerable<MatchViewModel>> ScheduleRoundRobin(int seriesID, UploadScheduleRequest request)
        {
            var createdMatches = new List<MatchViewModel>();
            var series = _context.Series
                .Include(s => s.SeriesSurface)
                .Include(s => s.Schedule)
                .FirstOrDefault(s => s.ID == seriesID);
            var teams = _context.SeriesTeam
                .Where(st => st.SeriesID == seriesID)
                .Select(st => st.TeamID)
                .ToList();
            int numCycles = request.RoundRobinMatchCapacity ?? teams.Count;
            //algorithm requires odd number of teams
            teams.Add(0);//0 is not a valid true team ID thus will act as dummy team

            SeriesScheduleViewModel schedule = series.Schedule;
            var availableSurfaces = series.SeriesSurface.ToArray();

            //day = starting datetime accurate to minute and seconds based on scheduler
            var day = series.StartDate;
            day = new DateTime(day.Year, day.Month, day.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second)
                .SpecifyUtc()
                .ConvertUtcToEst();
            string dayOfWeek = day.DayOfWeek.ToString();
            // scheduleEndTime is needed to combat the case where a schedule goes through midnight. (where EndTime < StartTime)
            var end = schedule.EndTime.SpecifyUtc().ConvertUtcToEst().TimeOfDay;
            var start = schedule.StartTime.SpecifyUtc().ConvertUtcToEst().TimeOfDay;
            DateTime scheduleEndTime = day.AddDays(new DateTime().Add(end) < new DateTime().Add(start) ? 1 : 0);
            scheduleEndTime = new DateTime(scheduleEndTime.Year, scheduleEndTime.Month, scheduleEndTime.Day, schedule.EndTime.Hour, schedule.EndTime.Minute, schedule.EndTime.Second);

            int surfaceIndex = 0;
            for (int cycles = 0; cycles < numCycles; cycles++)
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
                    while (!schedule.AvailableDays[dayOfWeek] || day.AddMinutes(schedule.EstMatchTime + 15) > scheduleEndTime)
                    {
                        // this check covers the basis that the while loop is initialized by only the condition that current schedule time has passed end time
                        // thus to ensure that we aren't skipping consecutive days if we pass midnight, we need to NOT add a day if we're still on an accepted day of the week
                        if (!schedule.AvailableDays[dayOfWeek])
                            day = day.AddDays(1);
                        day = new DateTime(day.Year, day.Month, day.Day).Add(start);
                        scheduleEndTime = scheduleEndTime.AddDays(1);
                        dayOfWeek = day.DayOfWeek.ToString();
                        surfaceIndex = 0;
                    }

                    var teamIDs = new List<int>() { teams[i], teams[j] };
                    if (!teamIDs.Contains(0))
                    {
                        var createdMatch = await _matchService.PostMatchAsync(new MatchUploadViewModel
                        {
                            StartTime = day.ConvertEstToUtc(),//convert back to UTC for post
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

        private async Task<IEnumerable<MatchViewModel>> ScheduleLadderAsync(int seriesID, UploadScheduleRequest request)
        {
            //created return
            var createdMatches = new List<MatchViewModel>();

            //queries
            var series = _context.Series
                .Include(s => s.SeriesTeam)
                .Include(s => s.SeriesSurface)
                .Include(s => s.Schedule)
                .FirstOrDefault(s => s.ID == seriesID);
            var teams = series.SeriesTeam
                .Select(st => st.TeamID)
                .ToList();

            //scheduler based variables
            var availableSurfaces = series.SeriesSurface.ToArray();
            SeriesScheduleViewModel schedule = series.Schedule;

            var day = series.StartDate;
            day = new DateTime(day.Year, day.Month, day.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second)
                .SpecifyUtc()
                .ConvertUtcToEst();
            string dayOfWeek = day.DayOfWeek.ToString();
            // scheduleEndTime is needed to combat the case where a schedule goes through midnight. (where EndTime < StartTime)
            var end = schedule.EndTime.SpecifyUtc().ConvertUtcToEst().TimeOfDay;
            var start = schedule.StartTime.SpecifyUtc().ConvertUtcToEst().TimeOfDay;
            DateTime scheduleEndTime = day.AddDays(new DateTime().Add(end) < new DateTime().Add(start) ? 1 : 0);
            scheduleEndTime = new DateTime(scheduleEndTime.Year, scheduleEndTime.Month, scheduleEndTime.Day, schedule.EndTime.Hour, schedule.EndTime.Minute, schedule.EndTime.Second);

            //local variables
            var numMatchesRemaining = request.NumberOfLadderMatches ?? 1;
            var numTeamsRemaining = teams.Count;
            var surfaceIndex = 0;
            var teamIndex = 0;
            // numMatchesRemaining used for other calculation, unusuable as condition
            for (int i = 0; i < (request.NumberOfLadderMatches ?? 1); i++)
            {
                if (surfaceIndex == availableSurfaces.Length)
                {
                    surfaceIndex = 0;
                    day = day.AddMinutes(schedule.EstMatchTime + 15);//15 minute buffer between matches as suggested by customer
                }

                while (!schedule.AvailableDays[dayOfWeek] || day.AddMinutes(schedule.EstMatchTime + 15) > scheduleEndTime)
                {
                    // this check covers the basis that the while loop is initialized by only the condition that current schedule time has passed end time
                    // thus to ensure that we aren't skipping consecutive days if we pass midnight, we need to NOT add a day if we're still on an accepted day of the week
                    if (!schedule.AvailableDays[dayOfWeek])
                        day = day.AddDays(1);
                    day = new DateTime(day.Year, day.Month, day.Day).Add(start);
                    scheduleEndTime = scheduleEndTime.AddDays(1);

                    dayOfWeek = day.DayOfWeek.ToString();
                    surfaceIndex = 0;
                }

                var teamIDs = new List<int>();
                int numTeamsInMatch = numTeamsRemaining / numMatchesRemaining;
                while (numTeamsInMatch > 0)
                {
                    teamIDs.Add(teams[teamIndex]);
                    teamIndex++;
                    numTeamsInMatch--;
                }

                var match = new MatchUploadViewModel
                {
                    StartTime = day.ConvertEstToUtc(),//convert back to UTC for post
                    SeriesID = seriesID,
                    SurfaceID = availableSurfaces[surfaceIndex].SurfaceID,
                    TeamIDs = teamIDs
                };
                var res = await _matchService.PostMatchAsync(match);
                createdMatches.Add(res);

                surfaceIndex++;
                numMatchesRemaining--;
                numTeamsRemaining -= numTeamsInMatch;
            }
            return createdMatches;
        }

        private async Task<IEnumerable<MatchViewModel>> ScheduleDoubleElimination(int seriesID)
        {
            throw new NotImplementedException();
        }
        private async Task<IEnumerable<MatchViewModel>> ScheduleSingleElimination(int seriesID)
        {
            var createdMatches = new List<MatchViewModel>();
            var series = _context.Series
                .Include(s => s.Schedule)
                .FirstOrDefault(s => s.ID == seriesID);
            var teams = _context.SeriesTeam
               .Where(st => st.SeriesID == seriesID)
               .OrderByDescending(st => st.WinCount);

            // seriesschedule casts start and end time to utc
            SeriesScheduleViewModel schedule = series.Schedule;

            var availableSurfaces = _context.SeriesSurface
                                        .Where(ss => ss.SeriesID == seriesID)
                                        .ToArray();

            var day = series.StartDate;
            day = new DateTime(day.Year, day.Month, day.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second)
                .SpecifyUtc()
                .ConvertUtcToEst();
            string dayOfWeek = day.DayOfWeek.ToString();
            int surfaceIndex = 0;
            // scheduleEndTime is needed to combat the case where a schedule goes through midnight. (where EndTime < StartTime)
            var end = schedule.EndTime.SpecifyUtc().ConvertUtcToEst().TimeOfDay;
            var start = schedule.StartTime.SpecifyUtc().ConvertUtcToEst().TimeOfDay;
            DateTime scheduleEndTime = day.AddDays(new DateTime().Add(end) < new DateTime().Add(start) ? 1 : 0);
            scheduleEndTime = new DateTime(scheduleEndTime.Year, scheduleEndTime.Month, scheduleEndTime.Day, schedule.EndTime.Hour, schedule.EndTime.Minute, schedule.EndTime.Second);


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
                while (!schedule.AvailableDays[dayOfWeek] || day.AddMinutes(schedule.EstMatchTime + 15) > scheduleEndTime)

                {
                    // this check covers the basis that the while loop is initialized by only the condition that current schedule time has passed end time
                    // thus to ensure that we aren't skipping consecutive days if we pass midnight, we need to NOT add a day if we're still on an accepted day of the week
                    if (!schedule.AvailableDays[dayOfWeek])
                        day = day.AddDays(1);
                    day = new DateTime(day.Year, day.Month, day.Day).Add(start);
                    scheduleEndTime = scheduleEndTime.AddDays(1);

                    dayOfWeek = day.DayOfWeek.ToString();
                    surfaceIndex = 0;
                }
                var createdMatch = await _matchService.UpdateMatchAsync(matchID,
                    new MatchPatchViewModel
                    {
                        StartTime = day.ConvertEstToUtc(),//convert back to UTC for post
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
                day = new DateTime(day.Year, day.Month, day.Day).Add(start);
                scheduleEndTime = scheduleEndTime.AddDays(1);

                dayOfWeek = day.DayOfWeek.ToString();
                surfaceIndex = 0;
                for (int i = 0; i < teamsInNextRound / 2; i++)
                {
                    if (surfaceIndex == availableSurfaces.Length)
                    {
                        surfaceIndex = 0;
                        day = day.AddMinutes(schedule.EstMatchTime + 15);
                    }
                    while (!schedule.AvailableDays[dayOfWeek] || day.AddMinutes(schedule.EstMatchTime + 15) > scheduleEndTime)
                    {
                        // this check covers the basis that the while loop is initialized by only the condition that current schedule time has passed end time
                        // thus to ensure that we aren't skipping consecutive days if we pass midnight, we need to NOT add a day if we're still on an accepted day of the week
                        if (!schedule.AvailableDays[dayOfWeek])
                            day = day.AddDays(1);
                        day = new DateTime(day.Year, day.Month, day.Day).Add(start);
                        scheduleEndTime = scheduleEndTime.AddDays(1);

                        dayOfWeek = day.DayOfWeek.ToString();
                        surfaceIndex = 0;
                    }
                    var createdMatch = await _matchService.PostMatchAsync(new MatchUploadViewModel
                    {
                        StartTime = day.ConvertEstToUtc(),//convert back to UTC for post
                        SeriesID = series.ID,
                        SurfaceID = availableSurfaces[surfaceIndex].SurfaceID, //temporary before 25live integration
                        TeamIDs = new List<int>().AsEnumerable() //no teams
                    });
                    createdMatches.Add(createdMatch);
                    surfaceIndex++;
                }
                teamsInNextRound /= 2;
            }
            return createdMatches;
        }

        /// <summary>
        /// Goal of this function is to generate a single elimination round.
        /// On the first possible round, this would imply handling teams with byes to ensure
        /// that the second round will be in a power of 2 (so that no further rounds need byes). 
        /// 
        /// These functions may need to be modified later as there may be more efficient ways to handle scheduling
        /// with the context that surfaces need to be booked ahead of time on 25Live
        /// </summary>
        /// <returns>Matches created as well as number of teams in the next round</returns>
        public async Task<EliminationRound> ScheduleElimRoundAsync(IEnumerable<SeriesTeam> involvedTeams)
        {
            int numTeams = involvedTeams.Count();
            int remainingTeamCount = involvedTeams.Count();
            int numByes = 0;
            var series = _context.Series.Find(involvedTeams.First().SeriesID);

            var teams = involvedTeams.Reverse();

            while (!(((numTeams + numByes) != 0) && (((numTeams + numByes) & ((numTeams + numByes) - 1)) == 0))) //while not power of 2
            {
                await UpdateSeriesTeamStats(new SeriesTeamPatchViewModel
                {
                    ID = teams.Last().ID,
                    WinCount = 1 //Bye round
                });
                teams = teams.Take(--remainingTeamCount);
                numByes++;
            }

            var teamPairings = EliminationRoundTeamPairsAsync(teams);
            var matches = new List<MatchViewModel>();

            foreach (var teamPair in teamPairings)
            {
                var createdMatch = await _matchService.PostMatchAsync(new MatchUploadViewModel
                {
                    StartTime = series.StartDate, //default time, autoscheduling will modify
                    SeriesID = series.ID,
                    SurfaceID = 1, //temporary before 25live integration
                    TeamIDs = teamPair
                });
                matches.Add(createdMatch);
            }
            return new EliminationRound
            {
                TeamsInNextRound = teamPairings.Count() + numByes,
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