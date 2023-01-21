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

        public IEnumerable<LookupViewModel> GetActivityLookup(string type)
        {
            switch (type)
            {
                case "status":
                    return _context.ActivityStatus
                                .Select(s => new LookupViewModel
                                {
                                    ID = s.ID,
                                    Description = s.Description
                                })
                                .AsEnumerable();
                case "activity":
                    return _context.ActivityType
                                .Select(a => new LookupViewModel
                                {
                                    ID = a.ID,
                                    Description = a.Description
                                })
                                .AsEnumerable();
                default:
                    return null;

            }
        }
        public IEnumerable<ActivityExtendedViewModel> GetActivities()
        {
            var activities = _context.Activity
                            .Select(a => new ActivityExtendedViewModel
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
                                        .Select(s => new SeriesExtendedViewModel
                                        {
                                            ID = s.ID,
                                            Name = s.Name,
                                            StartDate = s.StartDate,
                                            EndDate = s.EndDate,
                                            Type = _context.SeriesType
                                                    .FirstOrDefault(st => st.ID == s.TypeID)
                                                    .Description
                                        }).ToList(),
                                TypeID = a.TypeID,
                            });
            return activities;
        }
        public IEnumerable<ActivityExtendedViewModel> GetActivitiesByTime(DateTime? time)
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
        public ActivityExtendedViewModel? GetActivityByID(int activityID)
        {
            var activity = _context.Activity.Where(a => a.ID == activityID)
                            .Select(a => new ActivityExtendedViewModel
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
                                TypeID = a.TypeID,
                                Series = _seriesService.GetSeriesByActivityID(a.ID),
                                Team = a.Team.Select(t => new TeamExtendedViewModel
                                {
                                    ID = t.ID,
                                    Name = t.Name,
                                    Status = _context.TeamStatus
                                                .FirstOrDefault(ts => ts.ID == t.StatusID)
                                                .Description,
                                    Activity = new ActivityExtendedViewModel {
                                        ID = t.ActivityID, 
                                        Name = a.Name
                                    },
                                    Logo = t.Logo
                                })
                            
                            })
                            .FirstOrDefault();
            return activity;
        }
        public async Task<ActivityViewModel> UpdateActivityAsync(int activityID, ActivityPatchViewModel updatedActivity)
        {
            var activity = await _context.Activity.FindAsync(activityID);
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
            activity.TypeID = updatedActivity.TypeID ?? activity.TypeID;

            await _context.SaveChangesAsync();
            return activity;
        }
        public async Task<ActivityViewModel> PostActivityAsync(ActivityUploadViewModel a)
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
                Completed = false, //default not completed
                TypeID = a.TypeID,
            };
            await _context.Activity.AddAsync(activity);
            await _context.SaveChangesAsync();

            return activity;
        }

        public async Task<ParticipantActivityViewModel> PostParticipantActivityAsync(string username, int activityID, int privTypeID, bool isFreeAgent)
        {
            var participantActivity = new ParticipantActivity
            {
                ActivityID = activityID,
                ParticipantUsername = username,
                PrivTypeID = privTypeID,
                IsFreeAgent = isFreeAgent,
            };
            await _context.ParticipantActivity.AddAsync(participantActivity);
            await _context.SaveChangesAsync();

            return participantActivity;
        }

        public bool IsReferee(string username, int activityID)
        {
            return _context.ParticipantActivity.Any(pa =>
                pa.ParticipantUsername == username 
                && pa.ActivityID == activityID 
                && pa.PrivTypeID == 2); // PrivType: 2 => Referee
        }
    }
}

