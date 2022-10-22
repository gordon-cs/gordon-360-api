﻿using Gordon360.Models.CCT;
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
    public class ActivityService : IActivityService
    {
        private readonly CCTContext _context;

        public ActivityService(CCTContext context)
        {

            _context = context;
        }


        public IEnumerable<ActivityViewModel> GetActivities()
        {
            var activities = _context.Activity
                            .Select(a => new ActivityViewModel
                            {
                                ID = a.ID,
                                Name = a.Name,
                                RegistrationStart = a.RegistrationStart,
                                RegistrationEnd = a.RegistrationEnd,
                                Sport = _context.Sport
                                        .FirstOrDefault(s => s.ID == a.SportID),
                                Status = _context.ActivityStatus
                                        .FirstOrDefault(s => s.ID == a.StatusID)
                                        .Description,
                                MinCapacity = a.MinCapacity,
                                MaxCapacity = a.MaxCapacity,
                                SoloRegistration = a.SoloRegistration,
                                Logo = a.Logo,
                                Completed = a.Completed,
                                Series = a.Series
                                        .Select(s => new SeriesViewModel
                                        {
                                            ID = s.ID,
                                            Name = s.Name,
                                            StartDate = s.StartDate,
                                            EndDate = s.EndDate,
                                            Type = _context.SeriesType
                                                    .FirstOrDefault(st => st.ID == s.TypeID)
                                                    .Description
                                        }).ToList()
                            });
            return activities;
        }
        public IEnumerable<ActivityViewModel> GetActivitiesByTime(DateTime? time)
        {
            var vm = new ActivityViewModel();
            if (time is null)
            {

                return GetActivities().Where(a => !a.Completed);
            }
            else
            {
                return GetActivities().Where(a => a.RegistrationEnd > time);
            }
        }
        public ActivityViewModel? GetActivityByID(int activityID)
        {
            var activity = _context.Activity.Where(a => a.ID == activityID)
                            .Select(a => new ActivityViewModel
                            {
                                ID = a.ID,
                                Name = a.Name,
                                RegistrationStart = a.RegistrationStart,
                                RegistrationEnd = a.RegistrationEnd,
                                Sport = _context.Sport
                                        .FirstOrDefault(s => s.ID == a.SportID),
                                Status = _context.ActivityStatus
                                        .FirstOrDefault(s => s.ID == a.StatusID)
                                        .Description,
                                MinCapacity = a.MinCapacity,
                                MaxCapacity = a.MaxCapacity,
                                SoloRegistration = a.SoloRegistration,
                                Logo = a.Logo,
                                Completed = a.Completed,
                                Series = a.Series.Select(s => new SeriesViewModel
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
                                                                Win = st.Wins,
                                                                Loss = _context.SeriesTeam
                                                                        .Where(l => l.TeamID == st.TeamID && l.SeriesID == s.ID)
                                                                        .Count() - st.Wins
                                                            })
                                        })
                                    }),
                                    TeamStanding = s.SeriesTeam.Select(st => new TeamRecordViewModel
                                    {
                                        ID = st.ID,
                                        Name = _context.Team
                                                .FirstOrDefault(t => t.ID == st.TeamID)
                                                .Name,
                                        Win = st.Wins,
                                        Loss = _context.SeriesTeam
                                                .Where(l => l.TeamID == st.TeamID && l.SeriesID == s.ID)
                                                .Count() - st.Wins
                                    }).OrderByDescending(st => st.Win)
                                }),
                                Team = a.Team.Select(t => new TeamViewModel
                                {
                                    ID = t.ID,
                                    Name = t.Name,
                                    Status = _context.TeamStatus
                                                .FirstOrDefault(ts => ts.ID == t.StatusID)
                                                .Description,
                                                
                                    Logo = t.Logo
                                })
                            
                            })
                            .FirstOrDefault();
            return activity;
        }
        public async Task UpdateActivity(Activity updatedActivity)
        {
            var activity = await _context.Activity.FindAsync(updatedActivity.ID);
            activity.Name = updatedActivity.Name == null ? activity.Name : updatedActivity.Name;
            activity.Logo = updatedActivity.Logo == null ? activity.Logo : updatedActivity.Logo;
            activity.RegistrationStart = updatedActivity.RegistrationStart == default
                                            ? activity.RegistrationStart
                                            : updatedActivity.RegistrationStart;
            activity.RegistrationEnd = updatedActivity.RegistrationEnd == default
                                  ? activity.RegistrationEnd
                                  : updatedActivity.RegistrationEnd;
            activity.TypeID = updatedActivity.TypeID == default
                                ? activity.TypeID
                                : updatedActivity.TypeID;
            activity.SportID = updatedActivity.SportID == default
                                ? activity.SportID
                                : updatedActivity.SportID;
            activity.StatusID = updatedActivity.StatusID ?? activity.StatusID;
            activity.MinCapacity = updatedActivity.MinCapacity ?? activity.MinCapacity;
            activity.MaxCapacity = updatedActivity.MaxCapacity ?? activity.MaxCapacity;
            activity.MaxCapacity = updatedActivity.MaxCapacity ?? activity.MaxCapacity;
            activity.SoloRegistration = updatedActivity.SoloRegistration;
            activity.Completed = updatedActivity.Completed;

            await _context.SaveChangesAsync();
        }
        public async Task<int> PostActivity(Activity newActivity)
        {
            await _context.Activity.AddAsync(newActivity);
            await _context.SaveChangesAsync();
            return newActivity.ID;
        }

    }

}

