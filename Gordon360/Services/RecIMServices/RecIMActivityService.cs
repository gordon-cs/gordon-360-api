using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Gordon360.Services.RecIMServices
{
    public class RecIMActivityService : IRecIMActivityService
    {
        private readonly CCTContext _context;

        public RecIMActivityService(CCTContext context)
        {
            _context = context;
        }


        public IEnumerable<Activity> GetActivities()
        {
            var leagues = _context.Activity
                            .Include(l => l.Team)
                            .Include(l => l.Series)
                            .AsEnumerable();
            return leagues;
        }
        public IEnumerable<Activity> GetActivitiesByTime(DateTime? time)
        {
            var leagues = _context.Activity
                            .Where(l => time == null 
                                        ? !l.Completed 
                                        : l.RegistrationEnd > time)
                            .Include(l => l.Team)
                            .Include(l => l.Series)
                            .AsEnumerable();
            return leagues;
        }
        public Activity GetActivityByID(int activityID)
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
                                    (ul,u) => u))
                            .FirstOrDefault();
            return result;
        }
        public async Task UpdateActivity(Activity updatedActivity)
        {
            int activityID = updatedActivity.ID;
            var activity = _context.Activity
                            .FirstOrDefault(l => l.ID == updatedActivity.ID);
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
            activity.StatusID = updatedActivity.StatusID == null
                                ? activity.StatusID 
                                : updatedActivity.StatusID;
            activity.MinCapacity = updatedActivity.MinCapacity == null
                                   ? activity.MinCapacity
                                   : updatedActivity.MinCapacity;
            activity.MaxCapacity = updatedActivity.MaxCapacity == null
                                   ? activity.MaxCapacity
                                   : updatedActivity.MaxCapacity;
            activity.MaxCapacity = updatedActivity.MaxCapacity == null
                                   ? activity.MaxCapacity
                                   : updatedActivity.MaxCapacity;
            activity.SoloRegistration = updatedActivity.SoloRegistration;
            activity.Completed = updatedActivity.Completed;

            _context.SaveChanges();
        }
        public async Task PostActivity(Activity newActivity)
        {
            _context.Activity.Add(newActivity);
            _context.SaveChanges();
        }
       
    }

 }

