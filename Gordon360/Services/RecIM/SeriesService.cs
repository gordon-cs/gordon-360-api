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

        public IEnumerable<SeriesViewModel> GetSeries(bool active)
        {
            var series = _context.Series
                            .Select(s => new SeriesViewModel
                            {
                                ID = s.ID,
                                Name = s.Name,
                                StartDate = s.StartDate,
                                EndDate = s.EndDate,
                                Description = s.Description,
                                ActivityID = s.ActivityID,
                                Type = _context.SeriesType
                                        .FirstOrDefault(st => st.ID == s.TypeID)
                                        .Description,
                                Status = _context.SeriesStatus
                                        .FirstOrDefault(ss => ss.ID == s.StatusID)
                                        .Description,
                            });
            if (active) {
                series = series.Where(s => s.StartDate < DateTime.Now
                                        && s.EndDate > DateTime.Now);
            }
            return series;
        }
        public SeriesViewModel GetSeriesByID(int seriesID)
        {
            return null;
        }
        public async Task PostSeries(SeriesViewModel newSeries)
        {
            
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
                await ScheduleDoubleElimination(seriesID)
            }
            if (typeCode == "l")
            {
                await ScheduleLadder(seriesID);
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
         * needs log(n) matches with double elim needing to schedule twice that but 
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
        private async Task ScheduleLadder(int seriesID)
        {
            var teams = _context.SeriesTeam
                .Where(st => st.ID == seriesID)
                .Select(st => new
                {
                    ID = st.ID
                });
            var series = _context.Series
                .FirstOrDefault(s => s.ID == seriesID);
            var match = new CreateMatchViewModel
            {
                Time = series.StartDate,
                SportID = _context.Activity
                        .FirstOrDefault(a => a.ID == series.ActivityID)
                        .SportID,
                //to be replaced by proper match surface scheduler
                SurfaceID = 1,
                TeamIDs = teams
            };
            await _matchService.PostMatch(match); 
        }
    }

}

