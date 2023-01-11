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
            if (active) {
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

        public async Task ScheduleMatchesAsync(int seriesID)
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
                await ScheduleLadderAsync(seriesID);
            }
        }
        private Task ScheduleRoundRobin(int seriesID)
        {
            var teams = _context.SeriesTeam
                .Where(st => st.ID == seriesID)
                .AsEnumerable();

            //matrix can be used for all permutations, if check for self match
          
            throw new NotImplementedException();
        }
        /**
         * difference between single and double elimination is that single elim only
         * needs log(n) rounds with double elim needing to schedule twice that but 
         * handle the logic of losers bracket
         */
        private Task ScheduleSingleElimination(int seriesID)
        {
            var teams = _context.SeriesTeam
                .Where(st => st.ID == seriesID)
                .AsEnumerable();
            throw new NotImplementedException();
        }
        private Task ScheduleDoubleElimination(int seriesID)
        {
            var teams = _context.SeriesTeam
                .Where(st => st.ID == seriesID)
                .AsEnumerable();
            throw new NotImplementedException();
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
    }

}

