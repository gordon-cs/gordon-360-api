﻿using Gordon360.Models.CCT;
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
using Gordon360.Static.Names;
using Microsoft.Graph;

namespace Gordon360.Services.RecIM;

public class SeriesService(CCTContext context, IMatchService matchService, IAffiliationService affiliationService) : ISeriesService
{
    public IEnumerable<LookupViewModel>? GetSeriesLookup(string type)
    {
        return type switch
        {
            "status" => context.SeriesStatus.Where(query => query.ID != 0)
                .Select(s => new LookupViewModel
                {
                    ID = s.ID,
                    Description = s.Description
                })
                .AsEnumerable(),
            "series" => context.SeriesType.Where(query => query.ID != 0)
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
        var series = context.Series
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
                    Points = s.Points,
                    WinnerID = s.WinnerID,
                    ActivityID = s.ActivityID,
                    Match = matchService.GetMatchesBySeriesID(s.ID),
                    TeamStanding = context.SeriesTeam
                        .Where(st => st.SeriesID == s.ID && st.Team.StatusID != 0)
                        .Select(st => new TeamRecordViewModel
                        {
                            SeriesID = st.SeriesID,
                            TeamID = st.TeamID,
                            Logo = st.Team.Logo,
                            Name = st.Team.Name,
                            WinCount = st.WinCount,
                            LossCount = st.LossCount,
                            TieCount = st.TieCount,
                            SportsmanshipRating = context.MatchTeam
                                    .Where(mt => mt.TeamID == st.TeamID && mt.Match.StatusID == 6)
                                    .Average(mt => mt.SportsmanshipScore) 
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
        var activity = context.Activity.FirstOrDefault(a => a.ID == newSeries.ActivityID);
        if (activity.StartDate is null) activity.StartDate = newSeries.StartDate;

        // activity will inherit new end date if series ends after activity ends, OR if activity end date is null
        if (activity.EndDate is null || activity.EndDate < newSeries.EndDate) activity.EndDate = newSeries.EndDate;

        // inherit activity series schedule id if own scheduleID is null
        var activityInheritiedSeriesScheduleID = activity.SeriesScheduleID ?? 0;
        var series = newSeries.ToSeries(activityInheritiedSeriesScheduleID);
        series.SeriesSurface.Add(
            new SeriesSurface
                {
                    SeriesID = series.ID,
                    SurfaceID = 1
                }
        );

        await context.Series.AddAsync(series);
        await context.SaveChangesAsync();

        IEnumerable<int> teams = new List<int>();
        if (referenceSeriesID is null)
        {
            teams = context.Team
                    .Where(t => t.ActivityID == series.ActivityID && t.StatusID != 0)
                    .Select(t => t.ID)
                    .AsEnumerable();
        }
        else
        {
            teams = context.SeriesTeam
                .Where(st => st.SeriesID == referenceSeriesID && st.Team.StatusID != 0)
                .OrderByDescending(st => st.WinCount)
                .ThenBy(st => st.Team.MatchTeam.Average(mt => mt.SportsmanshipScore))
                .Select(st => st.TeamID);
        }
        if (newSeries.NumberOfTeamsAdmitted is int topTeams)
        {
            teams = teams.Take(topTeams);
        }

        await CreateSeriesTeamMappingAsync(teams, series.ID);


        return series;
    }

    public SeriesScheduleExtendedViewModel GetSeriesScheduleByID(int seriesID)
    {
        int scheduleID = context.Series.Find(seriesID)?.ScheduleID ?? 0;
        SeriesScheduleExtendedViewModel res =  context.SeriesSchedule.Find(scheduleID);
        res.SurfaceIDs = context.SeriesSurface.Where(ss => ss.SeriesID == seriesID).Select(s => s.SurfaceID);
        return res;
    }

    public async Task<SeriesScheduleViewModel> PutSeriesScheduleAsync(SeriesScheduleUploadViewModel seriesSchedule)
    {

        if (seriesSchedule.SeriesID is int seriesID)
        {
            var series = context.Series.Find(seriesID);
            //update surfaces
            if (seriesSchedule.AvailableSurfaceIDs.Count() == 0) throw new UnprocessibleEntity { ExceptionMessage = "The schedule must have a surface" };

            var updatedSurfaces = seriesSchedule.AvailableSurfaceIDs.ToList();
            var seriesSurfaces = context.SeriesSurface.Where(s => s.SeriesID == seriesID);
            foreach (var surface in seriesSurfaces)
            {
                if (!seriesSchedule.AvailableSurfaceIDs.Any(id => id == surface.ID))
                    context.SeriesSurface.Remove(surface);
                else
                    updatedSurfaces.Remove(surface.ID);
            }

            foreach (var surfaceID in updatedSurfaces)
                await context.SeriesSurface.AddAsync(
                    new SeriesSurface
                    {
                        SeriesID = seriesID,
                        SurfaceID = surfaceID
                    });

        }
        //check for exact schedule existing
        var existingSchedule = context.SeriesSchedule.FirstOrDefault(ss =>
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
                var series = context.Series.Find(seriesSchedule.SeriesID);
                //if the series is deleted, throw exception
                if (series.StatusID == 0) throw new UnprocessibleEntity { ExceptionMessage = "Series has been deleted" };

                if (series is null) return existingSchedule;
                series.ScheduleID = existingSchedule.ID;
                await context.SaveChangesAsync();
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
        await context.SeriesSchedule.AddAsync(schedule);
        await context.SaveChangesAsync();

        if (seriesSchedule.SeriesID is not null)
        {
            var series = context.Series.Find(seriesSchedule.SeriesID);
            series.ScheduleID = schedule.ID;
        }
        await context.SaveChangesAsync();
        return schedule;
    }

    public IEnumerable<AffiliationPointsViewModel> GetSeriesWinners(int seriesID)
    {
        return context.AffiliationPoints
            .Where(ap => ap.SeriesID == seriesID)
            .Select(ap => (AffiliationPointsViewModel)ap)
            .AsEnumerable();
    }

    public async Task HandleAdditionalSeriesWinnersAsync(AffiliationPointsUploadViewModel vm)
    {
        if (context.AffiliationPoints.Any(ap => ap.TeamID == vm.TeamID && ap.SeriesID == vm.SeriesID) && vm.Points is null)
            await RemoveSeriesWinnersAsync(vm);
        else
            await AddAdditionalSeriesWinnerAsync(vm);
    }

    private async Task AddAdditionalSeriesWinnerAsync(AffiliationPointsUploadViewModel vm)
    {
        string? affiliation = context.Team
                    .FirstOrDefault(t => t.ID == vm.TeamID)?
                    .Affiliation;

        if (affiliation is string a)
            await affiliationService.AddPointsToAffilliationAsync(a,
                new AffiliationPointsUploadViewModel
                {
                    TeamID = vm.TeamID,
                    SeriesID = vm.SeriesID,
                    Points = vm.Points
                });
    }

    public async Task RemoveSeriesWinnersAsync(AffiliationPointsUploadViewModel vm)
    {
        var res = context.AffiliationPoints
            .Where(ap => ap.SeriesID == vm.SeriesID && ap.TeamID == vm.TeamID);
        context.AffiliationPoints.RemoveRange(res);
        await context.SaveChangesAsync();
    }

    public async Task<SeriesViewModel> UpdateSeriesAsync(int seriesID, SeriesPatchViewModel update)
    {
        var series = context.Series
            .Include(s => s.SeriesTeam)
            .FirstOrDefault(s => s.ID == seriesID);

        var seriesTeams = series.SeriesTeam;
    
        //if the series is deleted, throw exception
        if (series.StatusID == 0) throw new UnprocessibleEntity { ExceptionMessage = "Series has been deleted" };

        series.Name = update.Name ?? series.Name;
        series.StartDate = update.StartDate ?? series.StartDate;
        series.EndDate = update.EndDate ?? series.EndDate;
        series.ScheduleID = update.ScheduleID ?? series.ScheduleID;
        series.WinnerID = update.WinnerID ?? series.WinnerID;
        series.Points = update.Points ?? series.Points;

        // add or subtract points to halls/affiliations logic

        if (series.StatusID == 2 && update.StatusID == 3 ) //in-progress -> completed
        {
            var calculatedWinner = context.SeriesTeamView
                .Where(st => st.SeriesID == seriesID)
                .OrderByDescending(st => st.WinCount)
                .ThenByDescending(st => st.SportsmanshipRating)
                .FirstOrDefault();

            if (calculatedWinner is SeriesTeamView w)
            {
                series.WinnerID = w.TeamID;
                await AddAdditionalSeriesWinnerAsync(
                    new AffiliationPointsUploadViewModel
                    {
                        TeamID = w.TeamID,
                        SeriesID = seriesID,
                        Points = series.Points
                    });
            }

        }
        if (series.StatusID == 3 && update.StatusID is not null) //completed -> anything
        {
            //remove all hall points attributed to this series
            var seriesAffiliationPoints = context.AffiliationPoints.Where(ap => ap.SeriesID == seriesID);
            context.AffiliationPoints.RemoveRange(seriesAffiliationPoints);
        }
    
        series.StatusID = update.StatusID ?? series.StatusID;


        //update teams
        if (update.TeamIDs is not null)
        {
            var updatedSeriesTeams = update.TeamIDs.ToList();
            foreach (var team in seriesTeams)
            {
                if (!update.TeamIDs.Any(id => id == team.TeamID))
                {
                    // error check (if teams that being removed are already in a match, block the patch
                    if (context.MatchTeam.Where(mt => mt.Match.SeriesID == seriesID && mt.Match.StatusID != 0).Any(mt => mt.TeamID == team.TeamID))
                        throw new UnprocessibleEntity { ExceptionMessage = $"Team {team.ID} is already in a Match in this Series and cannot be removed." };
                    context.SeriesTeam.Remove(team);
                } 
                else
                    updatedSeriesTeams.Remove(team.TeamID);
            }

            foreach (var teamID in updatedSeriesTeams)
                await context.SeriesTeam.AddAsync(
                    new SeriesTeam
                    {
                        TeamID = teamID,
                        SeriesID = seriesID,
                        WinCount = 0,
                        LossCount = 0
                    });
        }
    
        await context.SaveChangesAsync();
        return series;
    }

    // used for autoscheduler
    public async Task UpdateSeriesTeamStatsAsync(SeriesTeamPatchViewModel update)
    {
        var st = context.SeriesTeam.Find(update.ID);
        st.WinCount = update.WinCount ?? st.WinCount;
        st.LossCount = update.LossCount ?? st.LossCount;

        await context.SaveChangesAsync();
    }

    public async Task<TeamRecordViewModel> UpdateSeriesTeamRecordAsync(int seriesID, TeamRecordPatchViewModel update)
    {
        var teamRecord = context.SeriesTeam.FirstOrDefault(st => st.TeamID == update.TeamID && st.SeriesID == seriesID);
        if (teamRecord is null) throw new ResourceNotFoundException();

        teamRecord.WinCount = update.WinCount ?? teamRecord.WinCount;
        teamRecord.LossCount = update.LossCount ?? teamRecord.LossCount;
        teamRecord.TieCount = update.TieCount ?? teamRecord.TieCount;

        await context.SaveChangesAsync();
        return teamRecord;
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
            await context.SeriesTeam.AddAsync(seriesTeam);
        }
        await context.SaveChangesAsync();
    }

    public async Task<SeriesViewModel> DeleteSeriesCascadeAsync(int seriesID)
    {
        //delete series
        var series = context.Series
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
    
        await context.SaveChangesAsync();
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
        var series = context.Series
            .Include(s => s.Type)
            .FirstOrDefault(s => s.ID == seriesID);
        // if the series is deleted, throw exception
        if (series.StatusID == 0) throw new UnprocessibleEntity { ExceptionMessage = "Series has been deleted" };

        // ensure series has surfaces to autoschedule on
        if (!context.SeriesSurface.Any(ss => ss.SeriesID == seriesID)) throw new UnprocessibleEntity { ExceptionMessage = "Series has no specified surfaces" };

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

    public SeriesAutoSchedulerEstimateViewModel GetScheduleMatchesEstimateAsync(int seriesID, UploadScheduleRequest request)
    {
        var series = context.Series
            .Include(s => s.Type)
            .FirstOrDefault(s => s.ID == seriesID);
        // if the series is deleted, throw exception
        if (series.StatusID == 0) throw new UnprocessibleEntity { ExceptionMessage = "Series has been deleted" };

        // ensure series has surfaces to autoschedule on
        if (!context.SeriesSurface.Any(ss => ss.SeriesID == seriesID)) throw new UnprocessibleEntity { ExceptionMessage = "Series has no specified surfaces" };

        var typeCode = series.Type.TypeCode;

        return typeCode switch
        {
            "RR" => ScheduleRoundRobinEst(seriesID, request),
            "SE" => ScheduleSingleEliminationEst(seriesID),
            "DE" => ScheduleDoubleEliminationEst(seriesID),
            "L" =>  ScheduleLadderEst(seriesID, request),
        _ => null
        };
    }

    private SeriesAutoSchedulerEstimateViewModel ScheduleRoundRobinEst(int seriesID, UploadScheduleRequest request)
    {
        int createdMatches = 0;
        var series = context.Series
            .Include(s => s.SeriesSurface)
            .Include(s => s.Schedule)
            .FirstOrDefault(s => s.ID == seriesID);
        var teams = context.SeriesTeam
            .Include(s => s.Team)
            .Where(st => st.SeriesID == seriesID && st.Team.StatusID != 0)
            .Select(st => st.TeamID)
            .ToList();

        int numCycles = request.RoundRobinMatchCapacity ?? teams.Count + 1 - teams.Count % 2;
        //algorithm requires odd number of teams
        if (teams.Count() % 2 == 0)
            teams.Add(-1);//-1 is not a valid true team ID thus will act as dummy team

        SeriesScheduleViewModel schedule = series.Schedule;
        var availableSurfaces = series.SeriesSurface.ToArray();

        //day = starting datetime accurate to minute and seconds based on scheduler
        var day = series.StartDate;
        day = new DateTime(day.Year, day.Month, day.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second)
            .SpecifyUtc();
        string dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
        // scheduleEndTime is needed to combat the case where a schedule goes through midnight. (where EndTime < StartTime)
        var end = schedule.EndTime.SpecifyUtc().TimeOfDay;
        var start = schedule.StartTime.SpecifyUtc().TimeOfDay;
        var shouldAddDay = new DateTime().Add(end) < new DateTime().Add(start) ? 1 : 0;
        DateTime scheduleEndTime = day.AddDays(shouldAddDay);
        scheduleEndTime = new DateTime(scheduleEndTime.Year, scheduleEndTime.Month, scheduleEndTime.Day, schedule.EndTime.Hour, schedule.EndTime.Minute, schedule.EndTime.Second)
            .SpecifyUtc();

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
                    day = day.AddDays(1);
                    day = new DateTime(day.Year, day.Month, day.Day).Add(start).SpecifyUtc();
                    scheduleEndTime = new DateTime(day.Year, day.Month, day.Day, schedule.EndTime.Hour, schedule.EndTime.Minute, schedule.EndTime.Second)
                        .AddDays(shouldAddDay)
                        .SpecifyUtc();
                    dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
                    surfaceIndex = 0;
                }

                var teamIDs = new List<int>() { teams[i], teams[j] };
                if (!teamIDs.Contains(-1))
                {
                    createdMatches++;
                    surfaceIndex++;
                }
                i++;
                j--;
            }
            var temp = teams[0];
            teams.Add(temp);
            teams.RemoveAt(0);
        }
        return new SeriesAutoSchedulerEstimateViewModel()
        {
            SeriesID = seriesID,
            Name = series.Name,
            EndDate = day,
            GamesCreated = createdMatches
        };
    }

    private async Task<IEnumerable<MatchViewModel>> ScheduleRoundRobin(int seriesID, UploadScheduleRequest request)
    {
        var createdMatches = new List<MatchViewModel>();
        var series = context.Series
            .Include(s => s.SeriesSurface)
            .Include(s => s.Schedule)
            .FirstOrDefault(s => s.ID == seriesID);
        var teams = context.SeriesTeam
            .Include(s => s.Team)
            .Where(st => st.SeriesID == seriesID && st.Team.StatusID != 0)
            .Select(st => st.TeamID)
            .ToList();

        int numCycles = request.RoundRobinMatchCapacity ?? teams.Count + 1 - teams.Count%2;
        //algorithm requires odd number of teams
        if (teams.Count() % 2 == 0)
            teams.Add(-1);//-1 is not a valid true team ID thus will act as dummy team

        SeriesScheduleViewModel schedule = series.Schedule;
        var availableSurfaces = series.SeriesSurface.ToArray();

        //day = starting datetime accurate to minute and seconds based on scheduler
        var day = series.StartDate;
        day = new DateTime(day.Year, day.Month, day.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second)
            .SpecifyUtc();
        string dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
        // scheduleEndTime is needed to combat the case where a schedule goes through midnight. (where EndTime < StartTime)
        var end = schedule.EndTime.SpecifyUtc().TimeOfDay;
        var start = schedule.StartTime.SpecifyUtc().TimeOfDay;
        var shouldAddDay = new DateTime().Add(end) < new DateTime().Add(start) ? 1 : 0;
        DateTime scheduleEndTime = day.AddDays(shouldAddDay);
        scheduleEndTime = new DateTime(scheduleEndTime.Year, scheduleEndTime.Month, scheduleEndTime.Day, schedule.EndTime.Hour, schedule.EndTime.Minute, schedule.EndTime.Second)
            .SpecifyUtc();

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
                    day = day.AddDays(1);
                    day = new DateTime(day.Year, day.Month, day.Day).Add(start).SpecifyUtc();
                    scheduleEndTime = new DateTime(day.Year, day.Month, day.Day, schedule.EndTime.Hour, schedule.EndTime.Minute, schedule.EndTime.Second)
                        .AddDays(shouldAddDay)
                        .SpecifyUtc();
                    dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
                    surfaceIndex = 0;
                }

                var teamIDs = new List<int>() { teams[i], teams[j] };
                if (!teamIDs.Contains(-1))
                {
                    var createdMatch = await matchService.PostMatchAsync(new MatchUploadViewModel
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

    private SeriesAutoSchedulerEstimateViewModel ScheduleLadderEst(int seriesID, UploadScheduleRequest request)
    {
        int createdMatches = 0;

        //queries
        var series = context.Series
            .Include(s => s.SeriesTeam)
            .Include(s => s.SeriesSurface)
            .Include(s => s.Schedule)
            .FirstOrDefault(s => s.ID == seriesID);
        var teams = context.SeriesTeam
            .Include(s => s.Team)
            .Where(st => st.SeriesID == seriesID && st.Team.StatusID != 0)
            .Select(st => st.TeamID)
            .ToList();

        //scheduler based variables
        var availableSurfaces = series.SeriesSurface.ToArray();
        SeriesScheduleViewModel schedule = series.Schedule;

        var day = series.StartDate;
        day = new DateTime(day.Year, day.Month, day.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second)
            .SpecifyUtc();
        string dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
        // scheduleEndTime is needed to combat the case where a schedule goes through midnight. (where EndTime < StartTime)
        var end = schedule.EndTime.SpecifyUtc().TimeOfDay;
        var start = schedule.StartTime.SpecifyUtc().TimeOfDay;
        DateTime scheduleEndTime = day.AddDays(new DateTime().Add(end) < new DateTime().Add(start) ? 1 : 0);
        scheduleEndTime = new DateTime(scheduleEndTime.Year, scheduleEndTime.Month, scheduleEndTime.Day, schedule.EndTime.Hour, schedule.EndTime.Minute, schedule.EndTime.Second)
            .SpecifyUtc();

        //local variables
        var numMatchesRemaining = request.NumberOfLadderMatches ?? 1;
        var surfaceIndex = 0;
        var teamIndex = 0;
        for (int i = 0; i < (request.NumberOfLadderMatches ?? 1); i++)
        {
            if (surfaceIndex == availableSurfaces.Length)
            {
                surfaceIndex = 0;
                day = day.AddMinutes(schedule.EstMatchTime + 15);//15 minute buffer between matches as suggested by customer
            }

            while (!schedule.AvailableDays[dayOfWeek] || day.AddMinutes(schedule.EstMatchTime + 15) > scheduleEndTime)
            {
                day = day.AddDays(1);
                day = new DateTime(day.Year, day.Month, day.Day).Add(start).SpecifyUtc();
                scheduleEndTime = scheduleEndTime.AddDays(1);

                dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
                surfaceIndex = 0;
            }

            var teamIDs = new List<int>();
            int numTeamsInMatch = teams.Count / (request.NumberOfLadderMatches ?? 1);
            while (numTeamsInMatch > 0 && teamIndex < teams.Count)
            {
                teamIDs.Add(teams[teamIndex]);
                teamIndex++;
                numTeamsInMatch--;
            }

            createdMatches++;
            surfaceIndex++;
            numMatchesRemaining--;
        }

        return new SeriesAutoSchedulerEstimateViewModel()
        {
            SeriesID = seriesID,
            Name = series.Name,
            EndDate = day,
            GamesCreated = createdMatches
        };
    }

    private async Task<IEnumerable<MatchViewModel>> ScheduleLadderAsync(int seriesID, UploadScheduleRequest request)
    {
        //created return
        var createdMatches = new List<MatchViewModel>();

        //queries
        var series = context.Series
            .Include(s => s.SeriesTeam)
            .Include(s => s.SeriesSurface)
            .Include(s => s.Schedule)
            .FirstOrDefault(s => s.ID == seriesID);
        var teams = context.SeriesTeam
            .Include(s => s.Team)
            .Where(st => st.SeriesID == seriesID && st.Team.StatusID != 0)
            .Select(st => st.TeamID)
            .ToList();

        //scheduler based variables
        var availableSurfaces = series.SeriesSurface.ToArray();
        SeriesScheduleViewModel schedule = series.Schedule;

        var day = series.StartDate;
        day = new DateTime(day.Year, day.Month, day.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second)
            .SpecifyUtc();
        string dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
        // scheduleEndTime is needed to combat the case where a schedule goes through midnight. (where EndTime < StartTime)
        var end = schedule.EndTime.SpecifyUtc().TimeOfDay;
        var start = schedule.StartTime.SpecifyUtc().TimeOfDay;
        DateTime scheduleEndTime = day.AddDays(new DateTime().Add(end) < new DateTime().Add(start) ? 1 : 0);
        scheduleEndTime = new DateTime(scheduleEndTime.Year, scheduleEndTime.Month, scheduleEndTime.Day, schedule.EndTime.Hour, schedule.EndTime.Minute, schedule.EndTime.Second)
            .SpecifyUtc();

        //local variables
        var numMatchesRemaining = request.NumberOfLadderMatches ?? 1;
        var surfaceIndex = 0;
        var teamIndex = 0;
        for (int i = 0; i < (request.NumberOfLadderMatches ?? 1); i++)
        {
            if (surfaceIndex == availableSurfaces.Length)
            {
                surfaceIndex = 0;
                day = day.AddMinutes(schedule.EstMatchTime + 15);//15 minute buffer between matches as suggested by customer
            }

            while (!schedule.AvailableDays[dayOfWeek] || day.AddMinutes(schedule.EstMatchTime + 15) > scheduleEndTime)
            {
                day = day.AddDays(1);
                day = new DateTime(day.Year, day.Month, day.Day).Add(start).SpecifyUtc();
                scheduleEndTime = scheduleEndTime.AddDays(1);

                dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
                surfaceIndex = 0;
            }

            var teamIDs = new List<int>();
            int numTeamsInMatch = teams.Count / (request.NumberOfLadderMatches ?? 1);
            while (numTeamsInMatch > 0 && teamIndex < teams.Count)
            {
                teamIDs.Add(teams[teamIndex]);
                teamIndex++;
                numTeamsInMatch--;
            }
            var match = new MatchUploadViewModel
            {
                StartTime = day,
                SeriesID = seriesID,
                SurfaceID = availableSurfaces[surfaceIndex].SurfaceID,
                TeamIDs = teamIDs
            };
            var res = await matchService.PostMatchAsync(match);
            createdMatches.Add(res);

            surfaceIndex++;
            numMatchesRemaining--;
        }
        return createdMatches;
    }

    private SeriesAutoSchedulerEstimateViewModel ScheduleDoubleEliminationEst(int seriesID)
    {
        int createdMatches = 0;
        var series = context.Series
            .Include(s => s.Schedule)
            .FirstOrDefault(s => s.ID == seriesID);
        var teams = context.SeriesTeam
            .Include(s => s.Team)
            .Where(st => st.SeriesID == seriesID && st.Team.StatusID != 0)
            .OrderByDescending(st => st.WinCount);


        // seriesschedule casts start and end time to utc
        SeriesScheduleViewModel schedule = series.Schedule;

        var availableSurfaces = context.SeriesSurface
                                    .Where(ss => ss.SeriesID == seriesID)
                                    .ToArray();

        var day = series.StartDate;
        day = new DateTime(day.Year, day.Month, day.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second)
            .SpecifyUtc();
        string dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
        // scheduleEndTime is needed to combat the case where a schedule goes through midnight. (where EndTime < StartTime)
        var end = schedule.EndTime.SpecifyUtc().TimeOfDay;
        var start = schedule.StartTime.SpecifyUtc().TimeOfDay;
        DateTime scheduleEndTime = day.AddDays(new DateTime().Add(end) < new DateTime().Add(start) ? 1 : 0);
        scheduleEndTime = new DateTime(scheduleEndTime.Year, scheduleEndTime.Month, scheduleEndTime.Day, schedule.EndTime.Hour, schedule.EndTime.Minute, schedule.EndTime.Second)
            .SpecifyUtc();

        int surfaceIndex = 0;

        //schedule play-in + first round
        var elimScheduler = ScheduleElimRoundEst(teams);
        int teamsInNextRound = elimScheduler.TeamsInNextRound;
        var matchIDs = elimScheduler.Match.Select(m => m.ID);
        int numPlayInMatches = elimScheduler.NumByeTeams == 0 ? 0 : matchIDs.Count() - elimScheduler.TeamsInNextRound; // uncomment this if we decide that there should be a break day between non-byes and official bracket
        int numOfBracketParticipatingTeams = elimScheduler.NumByeTeams == 0 ? elimScheduler.TeamsInNextRound * 2 : elimScheduler.TeamsInNextRound; // play-in teams do not count
        // at this point first round matches have been made, bye round matches have been made


        foreach (var matchID in matchIDs)
        {
            if (surfaceIndex == availableSurfaces.Length)
            {
                surfaceIndex = 0;
                day = day.AddMinutes(schedule.EstMatchTime + 15);
            }
            while (!schedule.AvailableDays[dayOfWeek] || day.AddMinutes(schedule.EstMatchTime + 15) > scheduleEndTime)

            {
                day = day.AddDays(1);
                day = new DateTime(day.Year, day.Month, day.Day).Add(start);
                scheduleEndTime = scheduleEndTime.AddDays(1);

                dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
                surfaceIndex = 0;
            }
            createdMatches++;
            surfaceIndex++;
        }

        //create matches for remaining rounds 
        int roundNumber = 2; //scheduleElimRound will auto schedule the first 2 rounds for bracketing
        while (teamsInNextRound > 1)
        {
            //reset between rounds + prime the pump from first round
            day = day.AddDays(1);
            day = new DateTime(day.Year, day.Month, day.Day).Add(start);
            scheduleEndTime = scheduleEndTime.AddDays(1);

            dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
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
                    day = day.AddDays(1);
                    day = new DateTime(day.Year, day.Month, day.Day).Add(start);
                    scheduleEndTime = scheduleEndTime.AddDays(1);

                    dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
                    surfaceIndex = 0;
                }
                createdMatches++;
                surfaceIndex++;
            }
            roundNumber++;
            teamsInNextRound /= 2;
        }
        //current solution: losers bracket will be played in the time between winners finals -> grandfinals
        int j = 0;
        roundNumber = 0;
        while (numOfBracketParticipatingTeams > 0)
        {
            //reset between rounds + prime the pump from first round
            day = day.AddDays(1);
            day = new DateTime(day.Year, day.Month, day.Day).Add(start);
            scheduleEndTime = scheduleEndTime.AddDays(1);

            dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
            surfaceIndex = 0;


            // - losers bracket alternates between matches among the teams of the lower bracket and teams that JUST lost from the winners side
            for (int i = 0; i < numOfBracketParticipatingTeams / (j % 2 == 0 ? 2 : 1); i++)
            {
                if (surfaceIndex == availableSurfaces.Length)
                {
                    surfaceIndex = 0;
                    day = day.AddMinutes(schedule.EstMatchTime + 15);
                }
                while (!schedule.AvailableDays[dayOfWeek] || day.AddMinutes(schedule.EstMatchTime + 15) > scheduleEndTime)
                {
                    day = day.AddDays(1);
                    day = new DateTime(day.Year, day.Month, day.Day).Add(start);
                    scheduleEndTime = scheduleEndTime.AddDays(1);

                    dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
                    surfaceIndex = 0;

                    createdMatches++;
                    surfaceIndex++;
                }
            }
            numOfBracketParticipatingTeams /= (j % 2 == 0 ? 2 : 1);
            j++;
            roundNumber++;
        }

        // GRAND FINALS (played on the same day [if possible] as the losers bracket)
        for (int i = 1; i < 3; i++)
        {
            if (surfaceIndex == availableSurfaces.Length)
            {
                surfaceIndex = 0;
                day = day.AddMinutes(schedule.EstMatchTime + 15);
            }
            while (!schedule.AvailableDays[dayOfWeek] || day.AddMinutes(schedule.EstMatchTime + 15) > scheduleEndTime)
            {
                day = day.AddDays(1);
                day = new DateTime(day.Year, day.Month, day.Day).Add(start);
                scheduleEndTime = scheduleEndTime.AddDays(1);

                dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
                surfaceIndex = 0;
            }

            createdMatches++;
            surfaceIndex++;
        }

        return new SeriesAutoSchedulerEstimateViewModel()
        {
            SeriesID = seriesID,
            Name = series.Name,
            EndDate = day,
            GamesCreated = createdMatches
        };
    }

    /// <summary>
    /// Current double elimination autoscheduling will consider "play-in" as a "play-in" tournament
    /// Losers of the play in do not count for the double elimination bracket.
    /// </summary>
    private async Task<IEnumerable<MatchViewModel>> ScheduleDoubleElimination(int seriesID)
    {
        var createdMatches = new List<MatchViewModel>();
        var series = context.Series
            .Include(s => s.Schedule)
            .FirstOrDefault(s => s.ID == seriesID);
        var teams = context.SeriesTeam
            .Include(s => s.Team)
            .Where(st => st.SeriesID == seriesID && st.Team.StatusID != 0)
            .OrderByDescending(st => st.WinCount);


        // seriesschedule casts start and end time to utc
        SeriesScheduleViewModel schedule = series.Schedule;

        var availableSurfaces = context.SeriesSurface
                                    .Where(ss => ss.SeriesID == seriesID)
                                    .ToArray();

        var day = series.StartDate;
        day = new DateTime(day.Year, day.Month, day.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second)
            .SpecifyUtc();
        string dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
        // scheduleEndTime is needed to combat the case where a schedule goes through midnight. (where EndTime < StartTime)
        var end = schedule.EndTime.SpecifyUtc().TimeOfDay;
        var start = schedule.StartTime.SpecifyUtc().TimeOfDay;
        DateTime scheduleEndTime = day.AddDays(new DateTime().Add(end) < new DateTime().Add(start) ? 1 : 0);
        scheduleEndTime = new DateTime(scheduleEndTime.Year, scheduleEndTime.Month, scheduleEndTime.Day, schedule.EndTime.Hour, schedule.EndTime.Minute, schedule.EndTime.Second)
            .SpecifyUtc();

        int surfaceIndex = 0;

        //schedule play-in + first round
        var elimScheduler = await ScheduleElimRoundAsync(teams);
        int teamsInNextRound = elimScheduler.TeamsInNextRound;
        var matchIDs = elimScheduler.Match.Select(m => m.ID);
        int numPlayInMatches = elimScheduler.NumByeTeams == 0 ? 0 : matchIDs.Count() - elimScheduler.TeamsInNextRound; // uncomment this if we decide that there should be a break day between non-byes and official bracket
        int numOfBracketParticipatingTeams = elimScheduler.NumByeTeams == 0 ? elimScheduler.TeamsInNextRound * 2 : elimScheduler.TeamsInNextRound; // play-in teams do not count
        // at this point first round matches have been made, bye round matches have been made


        foreach (var matchID in matchIDs)
        {
            if (surfaceIndex == availableSurfaces.Length)
            {
                surfaceIndex = 0;
                day = day.AddMinutes(schedule.EstMatchTime + 15);
            }
            while (!schedule.AvailableDays[dayOfWeek] || day.AddMinutes(schedule.EstMatchTime + 15) > scheduleEndTime)

            {
                day = day.AddDays(1);
                day = new DateTime(day.Year, day.Month, day.Day).Add(start);
                scheduleEndTime = scheduleEndTime.AddDays(1);

                dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
                surfaceIndex = 0;
            }
            var createdMatch = await matchService.UpdateMatchAsync(matchID,
                new MatchPatchViewModel
                {
                    StartTime = day,
                    SurfaceID = availableSurfaces[surfaceIndex].SurfaceID
                });
            createdMatches.Add(createdMatch);
            surfaceIndex++;
        }

        //create matches for remaining rounds 
        int roundNumber = 2; //scheduleElimRound will auto schedule the first 2 rounds for bracketing
        while (teamsInNextRound > 1)
        {
            //reset between rounds + prime the pump from first round
            day = day.AddDays(1);
            day = new DateTime(day.Year, day.Month, day.Day).Add(start);
            scheduleEndTime = scheduleEndTime.AddDays(1);

            dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
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
                    day = day.AddDays(1);
                    day = new DateTime(day.Year, day.Month, day.Day).Add(start);
                    scheduleEndTime = scheduleEndTime.AddDays(1);

                    dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
                    surfaceIndex = 0;
                }

                var createdMatch = await matchService.PostMatchAsync(new MatchUploadViewModel
                {
                    StartTime = day,
                    SeriesID = series.ID,
                    SurfaceID = availableSurfaces[surfaceIndex].SurfaceID, 
                    TeamIDs = new List<int>().AsEnumerable() //no teams
                });
                createdMatches.Add(createdMatch);

                var matchBracketPlacement = new MatchBracket()
                {
                    MatchID = createdMatch.ID,
                    RoundNumber = roundNumber,
                    RoundOf = teamsInNextRound,
                    SeedIndex = i,
                    IsLosers = false 
                };
                await context.MatchBracket.AddAsync(matchBracketPlacement);
                await context.SaveChangesAsync();
                surfaceIndex++;
            }
            roundNumber++;
            teamsInNextRound /= 2;
        }
        //current solution: losers bracket will be played in the time between winners finals -> grandfinals
        int j = 0;
        roundNumber = 0;
        while (numOfBracketParticipatingTeams > 0)
        {
            //reset between rounds + prime the pump from first round
            day = day.AddDays(1);
            day = new DateTime(day.Year, day.Month, day.Day).Add(start);
            scheduleEndTime = scheduleEndTime.AddDays(1);

            dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
            surfaceIndex = 0;


            // - losers bracket alternates between matches among the teams of the lower bracket and teams that JUST lost from the winners side
            for (int i = 0; i < numOfBracketParticipatingTeams / (j % 2 == 0 ? 2 : 1); i++)
            {
                if (surfaceIndex == availableSurfaces.Length)
                {
                    surfaceIndex = 0;
                    day = day.AddMinutes(schedule.EstMatchTime + 15);
                }
                while (!schedule.AvailableDays[dayOfWeek] || day.AddMinutes(schedule.EstMatchTime + 15) > scheduleEndTime)
                {
                    day = day.AddDays(1);
                    day = new DateTime(day.Year, day.Month, day.Day).Add(start);
                    scheduleEndTime = scheduleEndTime.AddDays(1);

                    dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
                    surfaceIndex = 0;
                }

                var createdMatch = await matchService.PostMatchAsync(new MatchUploadViewModel
                {
                    StartTime = day,
                    SeriesID = series.ID,
                    SurfaceID = availableSurfaces[surfaceIndex].SurfaceID, 
                    TeamIDs = new List<int>().AsEnumerable() //no teams
                });
                createdMatches.Add(createdMatch);

                var matchBracketPlacement = new MatchBracket()
                {
                    MatchID = createdMatch.ID,
                    RoundNumber = roundNumber,
                    RoundOf = numOfBracketParticipatingTeams,
                    SeedIndex = i,
                    IsLosers = true
                };
                await context.MatchBracket.AddAsync(matchBracketPlacement);
                await context.SaveChangesAsync();
                surfaceIndex++;
            }
            numOfBracketParticipatingTeams /= (j % 2 == 0 ? 2 : 1);
            j++;
            roundNumber++;
        }

        // GRAND FINALS (played on the same day [if possible] as the losers bracket)
        for (int i = 1; i < 3; i++)
        {
            if (surfaceIndex == availableSurfaces.Length)
            {
                surfaceIndex = 0;
                day = day.AddMinutes(schedule.EstMatchTime + 15);
            }
            while (!schedule.AvailableDays[dayOfWeek] || day.AddMinutes(schedule.EstMatchTime + 15) > scheduleEndTime)
            {
                day = day.AddDays(1);
                day = new DateTime(day.Year, day.Month, day.Day).Add(start);
                scheduleEndTime = scheduleEndTime.AddDays(1);

                dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
                surfaceIndex = 0;
            }
            var createdMatch = await matchService.PostMatchAsync(new MatchUploadViewModel
            {
                StartTime = day,
                SeriesID = series.ID,
                SurfaceID = availableSurfaces[surfaceIndex].SurfaceID, 
                TeamIDs = new List<int>().AsEnumerable() //no teams
            });
            createdMatches.Add(createdMatch);

            var matchBracketPlacement = new MatchBracket()
            {
                MatchID = createdMatch.ID,
                RoundNumber = roundNumber + i,
                RoundOf = 2,
                SeedIndex = 0,
                IsLosers = true
            };
            await context.MatchBracket.AddAsync(matchBracketPlacement);
            await context.SaveChangesAsync();
            surfaceIndex++;
        }

        return createdMatches;
    }

    private SeriesAutoSchedulerEstimateViewModel ScheduleSingleEliminationEst(int seriesID)
    {
        int createdMatches = 0;
        var series = context.Series
            .Include(s => s.Schedule)
            .FirstOrDefault(s => s.ID == seriesID);
        var teams = context.SeriesTeam
            .Include(s => s.Team)
            .Where(st => st.SeriesID == seriesID && st.Team.StatusID != 0)
            .OrderByDescending(st => st.WinCount);


        // seriesschedule casts start and end time to utc
        SeriesScheduleViewModel schedule = series.Schedule;

        var availableSurfaces = context.SeriesSurface
                                    .Where(ss => ss.SeriesID == seriesID)
                                    .ToArray();

        var day = series.StartDate;
        day = new DateTime(day.Year, day.Month, day.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second)
            .SpecifyUtc();
        string dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
        // scheduleEndTime is needed to combat the case where a schedule goes through midnight. (where EndTime < StartTime)
        var end = schedule.EndTime.SpecifyUtc().TimeOfDay;
        var start = schedule.StartTime.SpecifyUtc().TimeOfDay;
        DateTime scheduleEndTime = day.AddDays(new DateTime().Add(end) < new DateTime().Add(start) ? 1 : 0);
        scheduleEndTime = new DateTime(scheduleEndTime.Year, scheduleEndTime.Month, scheduleEndTime.Day, schedule.EndTime.Hour, schedule.EndTime.Minute, schedule.EndTime.Second)
            .SpecifyUtc();

        int surfaceIndex = 0;

        //schedule first round
        var elimScheduler = ScheduleElimRoundEst(teams);
        int teamsInNextRound = elimScheduler.TeamsInNextRound;
        var matchIDs = elimScheduler.Match.Select(es => es.ID);
        //int numPlayInMatches = elimScheduler.NumByeTeams == 0 ? 0 : matchIDs.Count() - teamsInNextRound; // uncomment this if we decide that there should be a break day between non-byes and official bracket
        foreach (var matchID in matchIDs)
        {
            if (surfaceIndex == availableSurfaces.Length)
            {
                surfaceIndex = 0;
                day = day.AddMinutes(schedule.EstMatchTime + 15);
            }
            while (!schedule.AvailableDays[dayOfWeek] || day.AddMinutes(schedule.EstMatchTime + 15) > scheduleEndTime)

            {
                day = day.AddDays(1);
                day = new DateTime(day.Year, day.Month, day.Day).Add(start);
                scheduleEndTime = scheduleEndTime.AddDays(1);

                dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
                surfaceIndex = 0;
            }
            createdMatches++;
            surfaceIndex++;
        }
        //create matches for remaining rounds 
        int roundNumber = 2; //scheduleElimRound will auto schedule the first 2 rounds for bracketing
        while (teamsInNextRound > 1)
        {
            //reset between rounds + prime the pump from first round
            day = day.AddDays(1);
            day = new DateTime(day.Year, day.Month, day.Day).Add(start);
            scheduleEndTime = scheduleEndTime.AddDays(1);

            dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
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
                    day = day.AddDays(1);
                    day = new DateTime(day.Year, day.Month, day.Day).Add(start);
                    scheduleEndTime = scheduleEndTime.AddDays(1);

                    dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
                    surfaceIndex = 0;
                }

                createdMatches++;
                surfaceIndex++;
            }
            roundNumber++;
            teamsInNextRound /= 2;
        }

        return new SeriesAutoSchedulerEstimateViewModel()
        {
            SeriesID = seriesID,
            Name = series.Name,
            EndDate = day,
            GamesCreated = createdMatches
        }; 
    }

    private async Task<IEnumerable<MatchViewModel>> ScheduleSingleElimination(int seriesID)
    {
        var createdMatches = new List<MatchViewModel>();
        var series = context.Series
            .Include(s => s.Schedule)
            .FirstOrDefault(s => s.ID == seriesID);
        var teams = context.SeriesTeam
            .Include(s => s.Team)
            .Where(st => st.SeriesID == seriesID && st.Team.StatusID != 0)
            .OrderByDescending(st => st.WinCount);
       

        // seriesschedule casts start and end time to utc
        SeriesScheduleViewModel schedule = series.Schedule;

        var availableSurfaces = context.SeriesSurface
                                    .Where(ss => ss.SeriesID == seriesID)
                                    .ToArray();

        var day = series.StartDate;
        day = new DateTime(day.Year, day.Month, day.Day, schedule.StartTime.Hour, schedule.StartTime.Minute, schedule.StartTime.Second)
            .SpecifyUtc();
        string dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
        // scheduleEndTime is needed to combat the case where a schedule goes through midnight. (where EndTime < StartTime)
        var end = schedule.EndTime.SpecifyUtc().TimeOfDay;
        var start = schedule.StartTime.SpecifyUtc().TimeOfDay;
        DateTime scheduleEndTime = day.AddDays(new DateTime().Add(end) < new DateTime().Add(start) ? 1 : 0);
        scheduleEndTime = new DateTime(scheduleEndTime.Year, scheduleEndTime.Month, scheduleEndTime.Day, schedule.EndTime.Hour, schedule.EndTime.Minute, schedule.EndTime.Second)
            .SpecifyUtc();

        int surfaceIndex = 0;

        //schedule first round
        var elimScheduler = await ScheduleElimRoundAsync(teams);
        int teamsInNextRound = elimScheduler.TeamsInNextRound;
        var matchIDs = elimScheduler.Match.Select(es => es.ID);
        //int numPlayInMatches = elimScheduler.NumByeTeams == 0 ? 0 : matchIDs.Count() - teamsInNextRound; // uncomment this if we decide that there should be a break day between non-byes and official bracket
        foreach (var matchID in matchIDs)
        {
            if (surfaceIndex == availableSurfaces.Length)
            {
                surfaceIndex = 0;
                day = day.AddMinutes(schedule.EstMatchTime + 15);
            }
            while (!schedule.AvailableDays[dayOfWeek] || day.AddMinutes(schedule.EstMatchTime + 15) > scheduleEndTime)

            {
                day = day.AddDays(1);
                day = new DateTime(day.Year, day.Month, day.Day).Add(start);
                scheduleEndTime = scheduleEndTime.AddDays(1);

                dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
                surfaceIndex = 0;
            }
            var createdMatch = await matchService.UpdateMatchAsync(matchID,
                new MatchPatchViewModel
                {
                    StartTime = day,
                    SurfaceID = availableSurfaces[surfaceIndex].SurfaceID
                });
            createdMatches.Add(createdMatch);
            surfaceIndex++;
        }
        //create matches for remaining rounds 
        int roundNumber = 2; //scheduleElimRound will auto schedule the first 2 rounds for bracketing
        while (teamsInNextRound > 1)
        {
            //reset between rounds + prime the pump from first round
            day = day.AddDays(1);
            day = new DateTime(day.Year, day.Month, day.Day).Add(start);
            scheduleEndTime = scheduleEndTime.AddDays(1);

            dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
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
                    day = day.AddDays(1);
                    day = new DateTime(day.Year, day.Month, day.Day).Add(start);
                    scheduleEndTime = scheduleEndTime.AddDays(1);

                    dayOfWeek = day.ConvertFromUtc(Time_Zones.EST).DayOfWeek.ToString();
                    surfaceIndex = 0;
                }

                var createdMatch = await matchService.PostMatchAsync(new MatchUploadViewModel
                {
                    StartTime = day,
                    SeriesID = series.ID,
                    SurfaceID = availableSurfaces[surfaceIndex].SurfaceID, //temporary before 25live integration
                    TeamIDs = new List<int>().AsEnumerable() //no teams
                });
                createdMatches.Add(createdMatch);

                var matchBracketPlacement = new MatchBracket()
                {
                    MatchID = createdMatch.ID,
                    RoundNumber = roundNumber,
                    RoundOf = teamsInNextRound,
                    SeedIndex = i,
                    IsLosers = false //Double elimination not handled yet
                };
                await context.MatchBracket.AddAsync(matchBracketPlacement);
                await context.SaveChangesAsync();
                surfaceIndex++;
            }
            roundNumber++;
            teamsInNextRound /= 2;
        }
        return createdMatches;
    }

    private EliminationRound ScheduleElimRoundEst(IEnumerable<SeriesTeam> involvedTeams, bool isLosers = false)
    {
        MatchViewModel dummyMatch = new()
        {
            ID = -1,
        };

        int numTeams = involvedTeams.Count();
        int remainingTeamCount = involvedTeams.Count();
        int numByes = 0;

        var matches = new List<MatchViewModel>();
        var byeTeams = new List<int>();
        // bye logic
        // Play-off round needs to be calculated to ensure that the first round is in a power of 2
        while (!(((numTeams + numByes) != 0) && (((numTeams + numByes) & ((numTeams + numByes) - 1)) == 0)))
        {
            byeTeams.Add(involvedTeams.Last().TeamID);
            involvedTeams = involvedTeams.Take(--remainingTeamCount);
            numByes++;
        }

        // logic for handling bye teams that play another bye team in the next round
        var teamPairings = EliminationRoundTeamPairsAsync(involvedTeams).ToList();
        var byePairings = new List<List<int>>();
        var secondRoundPlayInPairs = new List<List<int>>();

        if (teamPairings.Count() + byePairings.Count() < byeTeams.Count())
            while (teamPairings.Count() + byePairings.Count() < byeTeams.Count())
            {
                var byePair = byeTeams.TakeLast(2);
                byePairings.Add(new List<int> { byePair.First(), byePair.Last() });
                byeTeams.Remove(byePair.First());
                byeTeams.Remove(byePair.Last());
            }
        else
            for (int i = 0; i < (teamPairings.Count() - byeTeams.Count) / 2; i++)
            {
                secondRoundPlayInPairs.Add(new List<int>());
                if (isLosers) secondRoundPlayInPairs.Add(new List<int>()); // double elim losers side needs twice as many matches
            }

        // Play-in matches, need to be created first in order for scheduling times
        var playInMatches = new List<int>();
        foreach (var teamPair in teamPairings)
        {
            matches.Add(dummyMatch);
            playInMatches.Add(-1);
        }


        // Full bye matches (no pair, waiting for winner of play-in) second in order for bracket ordering
        var fullByeMatches = new List<int>();
        foreach (int teamID in byeTeams)
        {
            matches.Add(dummyMatch);
            fullByeMatches.Add(-1);
        }

        // Bye pair matches, consisting of purely pairs of teams in the "second round" who both had byes, 3rd in order for bracket ordering
        var byePairMatches = new List<int>();
        foreach (var teamPair in byePairings)
        {
            matches.Add(dummyMatch);
            byePairMatches.Add(-1);
            byePairMatches.Add(-1);
        }

        // Empty pairs, only happens if number of teams is greater than 24, where play-in matches outnumber bye matches. Last in order for bracket ordering.
        // or when teams are already in a power of 2
        foreach (var emptyMatchPair in secondRoundPlayInPairs)
        {
            matches.Add(dummyMatch);
        }

        // assign bracket information
        List<int> allMatches = fullByeMatches.Concat(byePairMatches.Concat(playInMatches)).ToList();

        return new EliminationRound
        {
            TeamsInNextRound = allMatches.Count() / 2,
            NumByeTeams = numByes,
            Match = matches
        };
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
    public async Task<EliminationRound> ScheduleElimRoundAsync(IEnumerable<SeriesTeam> involvedTeams, bool isLosers = false)
    {
        int numTeams = involvedTeams.Count();
        int remainingTeamCount = involvedTeams.Count();
        int numByes = 0;
        var series = context.Series.Find(involvedTeams.First().SeriesID);

        var teams = involvedTeams.Reverse();
        var matches = new List<MatchViewModel>();
        var byeTeams = new List<int>();
        // bye logic
        // Play-off round needs to be calculated to ensure that the first round is in a power of 2
        while (!(((numTeams + numByes) != 0) && (((numTeams + numByes) & ((numTeams + numByes) - 1)) == 0))) 
        {
            if (teams.Last().ID != -1)
                await UpdateSeriesTeamStatsAsync(new SeriesTeamPatchViewModel
                {
                    ID = teams.Last().ID,
                    WinCount = 1 //Bye round
                });
            byeTeams.Add(teams.Last().TeamID);
            teams = teams.Take(--remainingTeamCount);
            numByes++;
        }

        // logic for handling bye teams that play another bye team in the next round
        var teamPairings = EliminationRoundTeamPairsAsync(teams).ToList();
        var byePairings = new List<List<int>>();
        var fullBye = new List<List<int>>();
        var secondRoundPlayInPairs = new List<List<int>>();

        if (teamPairings.Count() + byePairings.Count() < byeTeams.Count())
            while (teamPairings.Count() + byePairings.Count() < byeTeams.Count())
            {
                var byePair = byeTeams.TakeLast(2);
                byePairings.Add(new List<int> { byePair.First(), byePair.Last() });
                byeTeams.Remove(byePair.First());
                byeTeams.Remove(byePair.Last());
            }
        else
            for (int i = 0; i < (teamPairings.Count() - byeTeams.Count) / 2; i++)
            {
                secondRoundPlayInPairs.Add(new List<int>());
                if (isLosers) secondRoundPlayInPairs.Add(new List<int>()); // double elim losers side needs twice as many matches
            }


        foreach (int teamID in byeTeams)
            fullBye.Add(new List<int> { teamID });


       // Play-in matches, need to be created first in order for scheduling times
       teamPairings.Reverse();
        var playInMatches = new List<int>();
        foreach (var teamPair in teamPairings)
        {
            var createdMatch = await matchService.PostMatchAsync(new MatchUploadViewModel
            {
                StartTime = series.StartDate, //default time, will be modified by autoscheduling
                SeriesID = series.ID,
                SurfaceID = 0, //default surface (undefined surface type) will be modified by autoscheduling
                TeamIDs = teamPair
            });
            matches.Add(createdMatch);
            playInMatches.Add(createdMatch.ID);
        }


        // Full bye matches (no pair, waiting for winner of play-in) second in order for bracket ordering
        var fullByeMatches = new List<int>();
        var secondRoundMatches = new List<int>();
        foreach (var bye in fullBye)
        {
            var createdMatch = await matchService.PostMatchAsync(new MatchUploadViewModel
            {
                StartTime = series.StartDate, //default time, will be modified by autoscheduling
                SeriesID = series.ID,
                SurfaceID = 0, //default surface (undefined surface type) will be modified by autoscheduling
                TeamIDs = bye
            });
            matches.Add(createdMatch);
            fullByeMatches.Add(-1);
            secondRoundMatches.Add(createdMatch.ID);
        }

        // Bye pair matches, consisting of purely pairs of teams in the "second round" who both had byes, 3rd in order for bracket ordering
        var byePairMatches = new List<int>();
        foreach (var teamPair in byePairings)
        {
            var createdMatch = await matchService.PostMatchAsync(new MatchUploadViewModel
            {
                StartTime = series.StartDate, //default time, will be modified by autoscheduling
                SeriesID = series.ID,
                SurfaceID = 0, //default surface (undefined surface type) will be modified by autoscheduling
                TeamIDs = teamPair
            });
            matches.Add(createdMatch);
            byePairMatches.Add(-1);
            byePairMatches.Add(-1);
            secondRoundMatches.Add(createdMatch.ID);
        }

        // Empty pairs, only happens if number of teams is greater than 24, where play-in matches outnumber bye matches. Last in order for bracket ordering.
        // or when teams are already in a power of 2
        foreach (var emptyMatchPair in secondRoundPlayInPairs)
        {
            var createdMatch = await matchService.PostMatchAsync(new MatchUploadViewModel
            {
                StartTime = series.StartDate, //default time, will be modified by autoscheduling
                SeriesID = series.ID,
                SurfaceID = 0, //default surface (undefined surface type) will be modified by autoscheduling
                TeamIDs = emptyMatchPair
            });
            matches.Add(createdMatch);
            secondRoundMatches.Add(createdMatch.ID);
        }

        // assign bracket information
        List<int> allMatches = fullByeMatches.Concat(byePairMatches.Concat(playInMatches)).ToList();

        // bracket place first round
        await CreateEliminationBracket(allMatches, 0, isLosers);
        // bracket place second round
        await CreateEliminationBracket(secondRoundMatches, 1, isLosers);

        return new EliminationRound
        {
            TeamsInNextRound = allMatches.Count()/2,
            NumByeTeams = numByes,
            Match = matches
        };
    }

    private async Task<IEnumerable<MatchBracketViewModel>> CreateEliminationBracket(List<int> matchesIDs, int roundNumber, bool isLosers)
    {
        var res = new List<MatchBracketViewModel>();
        int rounds = (int)Math.Log(matchesIDs.Count(), 2);
        var matchArr = matchesIDs.ToArray();
        var matchIndexes = new List<int> { 0 };

        for(int i = 0; i < rounds; i++)
        {
            var temp = new List<int>();
            var newLength = matchIndexes.Count() * 2 - 1;
            foreach (int index in matchIndexes)
            {
                temp.Add(index);
                temp.Add(newLength - index);
            }
            matchIndexes = temp;
        }
        var indexArr = matchIndexes.ToArray();
        int j = 0;
        foreach(int i in indexArr)
        {
            if (matchArr[i] != -1)
            {
                var matchBracketPlacement = new MatchBracket()
                {
                    MatchID = matchArr[i],
                    RoundNumber = roundNumber,
                    RoundOf = matchArr.Length * 2,
                    SeedIndex = j,
                    IsLosers = isLosers
                };
                await context.MatchBracket.AddAsync(matchBracketPlacement);
                res.Add(matchBracketPlacement);
            }
            j++;
        }
        await context.SaveChangesAsync();
        return res;
    }

    public IEnumerable<MatchBracketExportViewModel> GetSeriesBracketInformation(int seriesID)
    {
        /**
         * Match stores StartTime and Series information needed to handle calculations
         */
        var match = context.Match
            .Include(m => m.MatchBracket)
            .Where(m => m.SeriesID == seriesID && m.StatusID != 0)
            .Select(m => new MatchBracketViewModel
            {
                MatchID = m.MatchBracket.MatchID,
                RoundNumber = m.MatchBracket.RoundNumber,
                RoundOf = m.MatchBracket.RoundOf,
                SeedIndex = m.MatchBracket.SeedIndex,
                IsLosers = m.MatchBracket.IsLosers,
                StartTime = m.StartTime
            })
            .AsEnumerable()
            .OrderBy(mb => mb.RoundNumber)
            .ThenBy(mb => mb.SeedIndex);

        /* 
         * fill out each of round level:
         * round of 64 needs 32 matches (64 teams), 32/16...
         * (empty spots happen when there are bye rounds and are typically an issue in the first 2 rounds)
         */
        var combinedList = Enumerable.Empty<MatchBracketExtendedViewModel>().ToList();
        var fakeMatchID = -1;
        var i = 0;
        while (i < match.Count())
        {
            var j = 0;
            var currentRoundOf = match.ElementAt(i).RoundOf;
            var currentRoundNum = match.ElementAt(i).RoundNumber;

            while (j < currentRoundOf / 2) //roundOf is a term based on number of teams, matches are per 2 teams
            {
                var m = match.ElementAt(i);
                /**
                 *     Round 0 |  1  |  2  | ... n-1 (finals)
                 *  s      0 - 
                 *  e          \ _ 0
                 *  e          /     \
                 *  d      1 -        \ _ 0
                 *                    /     \
                 *  i      2 -       /       \
                 *  n          \ _ 1           .
                 *  d          /      .        .
                 *  e    n-1 -        .        .
                 *  x
                 *  
                 *  Each bracket has has an associated RoundOf which declares how many matches should exist in each
                 *  round: roundOf64 = 32 matches, roundOf32 = 16 matches ...
                 *  Each match in the bracket has an associated index. If there are "missing" indicies, we know a bye 
                 *  match needs to be filled in. 
                 *  
                 *  Check each element (sorted), if the index is incrementing by 1, if not, then we make a pseudo match
                 *  to be displayed on the UI.
                 */
                if (j == match.ElementAt(i).SeedIndex)
                {
                    var state = "SCHEDULED";
                    var teams = context.MatchTeam.Where(mt => mt.MatchID == m.MatchID && mt.StatusID != 0);

                    var teamList = Enumerable.Empty<TeamBracketExtendedViewModel>().ToList();
                    if (teams.Count() != 0)
                    {
                        foreach (var team in teams)
                            teamList.Add(new TeamBracketExtendedViewModel
                            {
                                TeamID = team.TeamID,
                                Score = team.Score.ToString(),
                                IsWinner = false,
                                TeamName = context.Team.Find(team.TeamID)?.Name ?? ""
                            });

                        if (teams.Count() == 2)
                        {
                            if (teams.ElementAt(0).Score != 0 || teams.ElementAt(1).Score != 0) state = "SCORE_DONE";

                            if (Convert.ToInt32(teamList.ElementAt(0).Score) > Convert.ToInt32(teamList.ElementAt(1).Score))
                                teamList.ElementAt(0).IsWinner = true;
                            if (Convert.ToInt32(teamList.ElementAt(0).Score) < Convert.ToInt32(teamList.ElementAt(1).Score))
                                teamList.ElementAt(1).IsWinner = true;
                        }
                    }

                    combinedList.Add(new MatchBracketExtendedViewModel
                    {
                        MatchID = m.MatchID,
                        NextMatchID = null,
                        RoundNumber = m.RoundNumber,
                        RoundOf = m.RoundOf,
                        State = state,
                        SeedIndex = m.SeedIndex,
                        IsLosers = m.IsLosers,
                        StartTime = m.StartTime,
                        Team = teamList
                    });
                    i++;
                }
                else
                {
                    combinedList.Add(new MatchBracketExtendedViewModel
                    {
                        MatchID = fakeMatchID--,
                        NextMatchID = null,
                        RoundNumber = currentRoundNum,
                        RoundOf = m.RoundOf,
                        State = "WALK_OVER",
                        SeedIndex = j,
                        IsLosers = m.IsLosers,
                        StartTime = m.StartTime,
                        Team = Enumerable.Empty<TeamBracketExtendedViewModel>()
                    });
                }
                j++;
            }
        }

        i = 0;
        foreach (var _match in combinedList)
        {
            var nextMatchSeedIndex = _match.SeedIndex >> 1;
            var nextMatch = combinedList.FirstOrDefault(m => m.RoundNumber == _match.RoundNumber + 1 && m.SeedIndex == nextMatchSeedIndex);
            _match.NextMatchID = nextMatch?.MatchID;

            if (_match.MatchID < 0)
            {
                /**
                 * 2 conditions
                 * 1) Match i is the only bye match
                 *      - Inherit the team in the next match that is not i+1
                 * 2) Both are bye matches
                 *      - Inherit Team at index 0,1 respectively
                 */
                var bye = nextMatch?.Team;
                if (_match.SeedIndex % 2 == 0)
                {
                    bye = nextMatch?.Team
                        .Where(t => !combinedList[i+1].Team.Any(t_ => t_.TeamID == t.TeamID));
                }
                else
                {
                    bye = nextMatch?.Team
                        .Where(t => !combinedList[i-1].Team.Any(t_ => t_.TeamID == t.TeamID));

                }
                combinedList[i].Team = [new TeamBracketExtendedViewModel()
                {
                    TeamID = bye.First().TeamID,
                    Score = "BYE",
                    IsWinner = true,
                    Status = "WALK_OVER",
                    TeamName = bye.First().TeamName
                }];
            }
            i++;
        }
      

        //final conversion to exact UI shape (more efficient than letting the UI handle the key/pair changes
        //concious decision to not make all the conversions in the extended viewmodel as it would disrupt our 
        //current api styling and naming.
        //I want to minimize style disruptions albeit at the cost of a tiny bit of performance
        var res = Enumerable.Empty<MatchBracketExportViewModel>().ToList();
        foreach( var m in combinedList )
        {
            var teams = Enumerable.Empty<TeamBracketExportViewModel>().ToList();
            foreach (var t in m.Team)
                teams.Add(new TeamBracketExportViewModel
                {
                    id = t.TeamID,
                    resultText = t.Score,
                    isWinner = t.IsWinner,
                    status = t.Status,
                    name = t.TeamName
                });
            res.Add(new MatchBracketExportViewModel
            {
                id = m.MatchID,
                name = null, //unused currently
                nextMatchId = m.NextMatchID,
                tournamentRoundText = $"{m.RoundNumber + 1}", //start at round 1 instead of 0 for UI readability
                state = m.State,
                startTime = m.StartTime,
                participants = teams,
                seedIndex = m.SeedIndex,
                isLosers = m.IsLosers
            });
        }

        return res;
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