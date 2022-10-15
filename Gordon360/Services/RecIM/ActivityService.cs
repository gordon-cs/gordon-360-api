﻿using Gordon360.Models.CCT;
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


        public IEnumerable<Activity> GetActivities()
        {
            var activities = _context.Activity
                            .Include(l => l.Team)
                            .Include(l => l.Series);
            return activities;
        }
        public IEnumerable<Activity> GetActivitiesByTime(DateTime? time)
        {
            if (time is null)
            {
                return GetActivities().Where(a => !a.Completed); 
            } else
            {
                return GetActivities().Where(a => a.RegistrationEnd > time);
            }
        }
        public Activity? GetActivityByID(int activityID)
        {
            var result = _context.Activity
                            .Where(l => l.ID == activityID)
                            .Include(l => l.Team)
                            .Include(l => l.Series)
                                .ThenInclude(s => s.Match)
                            .Include(l => l.ParticipantActivity
                                .Join(_context.Participant,
                                    ul => ul.ParticipantID,
                                    u => u.ID,
                                    (ul, u) => u))
                            .FirstOrDefault();
            return result;
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

