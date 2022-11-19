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
    public class ActivityService : IActivityService
    {
        private readonly CCTContext _context;
        private readonly ISeriesService _seriesService;

        public ActivityService(CCTContext context, ISeriesService seriesService)
        { 
            _context = context;
            _seriesService = seriesService;
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
                                RegistrationOpen = DateTime.Now > a.RegistrationStart && DateTime.Now < a.RegistrationEnd,
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
            if (time is null)
            {
                return GetActivities().Where(a => !a.Completed);
            }
            else
            {
                return GetActivities().Where(a => a.RegistrationEnd > time);
            }
        }
        public IEnumerable<ActivityViewModel> GetOpenActivities()
        {
            return GetActivities().Where(a => a.RegistrationOpen);
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
                                RegistrationOpen = DateTime.Now > a.RegistrationStart && DateTime.Now < a.RegistrationEnd,
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
                                Series = _seriesService.GetSeriesByActivityID(a.ID),
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
        public async Task UpdateActivity(ActivityPatchViewModel updatedActivity)
        {
            var activity = await _context.Activity.FindAsync(updatedActivity.ID);
            activity.Name = updatedActivity.Name ?? activity.Name;
            activity.Logo = updatedActivity.Logo ?? activity.Logo;
            activity.RegistrationStart = updatedActivity.RegistrationStart ?? activity.RegistrationStart;
            activity.RegistrationEnd = updatedActivity.RegistrationEnd ?? activity.RegistrationEnd;
            activity.SportID = updatedActivity.SportID ?? activity.SportID;
            activity.StatusID = updatedActivity.StatusID ?? activity.StatusID;
            activity.MinCapacity = updatedActivity.MinCapacity ?? activity.MinCapacity;
            activity.MaxCapacity = updatedActivity.MaxCapacity ?? activity.MaxCapacity;
            activity.MaxCapacity = updatedActivity.MaxCapacity ?? activity.MaxCapacity;
            activity.SoloRegistration = updatedActivity.SoloRegistration ?? activity.SoloRegistration;
            activity.Completed = updatedActivity.Completed ?? activity.Completed;

            await _context.SaveChangesAsync();
        }
        public async Task<int> PostActivity(ActivityUploadViewModel a)
        {
            var activity = new Activity
            {
                Name = a.Name,
                Logo = a.Logo,
                RegistrationStart = a.RegistrationStart,
                RegistrationEnd = a.RegistrationEnd,
                SportID = a.SportID,
                StatusID = 1, //default set to pending status
                MinCapacity = a.MinCapacity ?? 0,
                MaxCapacity = a.MaxCapacity,
                SoloRegistration = a.SoloRegistration,
                Completed = false //default not completed
            };
            await _context.Activity.AddAsync(activity);
            await _context.SaveChangesAsync();
            return activity.ID;
        }

    }

}

