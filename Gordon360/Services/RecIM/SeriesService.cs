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
                                Match = s.Match.Select(m => new MatchViewModel
                                {
                                    ID = m.ID,
                                    Time = m.Time,
                                    Status = _context.MatchStatus
                                                .FirstOrDefault(ms => ms.ID == m.StatusID)
                                                .Description,
                                    Team = m.MatchTeam.Select(mt => new TeamViewModel
                                    {
                                        ID = mt.TeamID,
                                        Name = _context.Team
                                                .FirstOrDefault(t => t.ID == mt.TeamID)
                                                .Name,
                                        TeamRecord = _context.SeriesTeam
                                                        .Where(st => st.SeriesID == s.ID && st.TeamID == mt.TeamID)
                                                        .Select(st => new TeamRecordViewModel
                                                        {
                                                            Win = st.Win,
                                                            Loss = st.Loss ?? 0,
                                                            //Tie = _context.SeriesTeam
                                                            //        .Where(l => l.TeamID == st.TeamID
                                                            //                && l.SeriesID == s.ID
                                                            //                )
                                                            //        .Count() - st.Win - (st.Loss ?? 0)
                                                        })
                                    })
                                }),
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
        public SeriesViewModel GetSeriesByID(int seriesID)
        {
            return GetSeries().FirstOrDefault(s => s.ID == seriesID);
        }
        public async Task<int> PostSeries(CreateSeriesViewModel newSeries, int? referenceSeriesID)
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
            
            
            await CreateSeriesTeamMapping(teams, series.ID);
            return series.ID;
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

