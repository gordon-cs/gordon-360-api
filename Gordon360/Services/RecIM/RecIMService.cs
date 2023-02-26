using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Models.CCT.Context;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;

namespace Gordon360.Services.RecIM
{
    public class RecIMService : IRecIMService
    {
        private readonly CCTContext _context;

        public RecIMService(CCTContext context)
        {
            _context = context;
        }

        public async Task DeleteActivityCascade(int activityID)
        {
            //delete participant involvement
            var participantActivity = _context.ParticipantActivity.Where(pa => pa.ActivityID == activityID).ToList();
            foreach (var pa in participantActivity)
                pa.PrivTypeID = 0;
            await _context.SaveChangesAsync();


            // delete teams
            var activityTeams = _context.Team.Where(t => t.ActivityID == activityID).Select(t => t.ID);
            foreach (var team in activityTeams)
            {
                await DeleteTeamCascadeAsync(team);
            }
            
            // delete series
            var activitySeries = _context.Series.Where(s => s.ActivityID == activityID).Select(s => s.ID).ToList();
            foreach (var series in activitySeries)
            {
                await DeleteSeriesCascadeAsync(series); 
            }

            //delete activity
            var activity = _context.Activity.FirstOrDefault(a => a.ID == activityID);
            activity.StatusID = 0;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTeamCascadeAsync(int teamID)
        {
            var team = _context.Team.FirstOrDefault(t => t.ID == teamID);
            team.StatusID = 0;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSeriesCascadeAsync(int seriesID)
        {
            //delete series teams (series team does not need to be fully deleted, a wipe is sufficient)
            var seriesTeam = _context.SeriesTeam.Where(st => st.SeriesID == seriesID);
            foreach (var st in seriesTeam)
            {
                st.Win = 0;
                st.Loss = 0;
            }
            //delete matches
            var matches = _context.Match.Where(m => m.SeriesID == seriesID).Select(m => m.ID).ToList();
            foreach (var match in matches)
            {
                await DeleteMatchCascadeAsync(match);
            }
            //delete series
            var series = _context.Series.FirstOrDefault(s => s.ID == seriesID);
            series.StatusID = 0;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMatchCascadeAsync(int matchID)
        {
            //delete matchteam
            var matchteam = _context.MatchTeam.Where(mt => mt.MatchID == matchID);
            foreach (var mt in matchteam)
                mt.StatusID = 0;
            //deletematch
            var match = _context.Match.FirstOrDefault(m => m.ID == matchID);
            match.StatusID = 0;

            await _context.SaveChangesAsync();
        }
    }
}

