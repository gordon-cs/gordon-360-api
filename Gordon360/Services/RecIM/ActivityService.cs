using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels.RecIM;
using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Hosting;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore;
using Gordon360.Extensions.System;


namespace Gordon360.Services.RecIM
{
    public class ActivityService : IActivityService
    {
        private readonly CCTContext _context;
        private readonly ISeriesService _seriesService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ServerUtils _serverUtils;

        public ActivityService(CCTContext context, ISeriesService seriesService, IWebHostEnvironment webHostEnvironment, ServerUtils serverUtils)
        {
            _context = context;
            _seriesService = seriesService;
            _webHostEnvironment = webHostEnvironment;
            _serverUtils = serverUtils;
        }

        public IEnumerable<LookupViewModel>? GetActivityLookup(string type)
        {
            return type switch
            {
                "status" => _context.ActivityStatus.Where(query => query.ID != 0)
                                .Select(s => new LookupViewModel
                                {
                                    ID = s.ID,
                                    Description = s.Description
                                })
                                .AsEnumerable(),
                "activity" => _context.ActivityType.Where(query => query.ID != 0)
                                .Select(a => new LookupViewModel
                                {
                                    ID = a.ID,
                                    Description = a.Description
                                })
                                .AsEnumerable(),
                _ => null

            };
        }

        public IEnumerable<ActivityExtendedViewModel> GetActivities()
        {
            var activities = _context.Activity.Where(a => a.StatusID != 0)
                .Include(a => a.Series)
                    .ThenInclude(a => a.Type)
                .Include(a => a.Series)
                    .ThenInclude(a => a.Status)
                .Include(a => a.Series)
                    .ThenInclude(a => a.Schedule)
                .Select(a => new ActivityExtendedViewModel
                {
                    ID = a.ID,
                    Name = a.Name,
                    RegistrationStart = a.RegistrationStart.SpecifyUtc(),
                    RegistrationEnd = a.RegistrationEnd.SpecifyUtc(),
                    RegistrationOpen = DateTime.UtcNow > a.RegistrationStart.SpecifyUtc()
                        && DateTime.UtcNow < a.RegistrationEnd.SpecifyUtc(),
                    Sport = a.Sport,
                    Status = a.Status.Description,
                    MinCapacity = a.MinCapacity,
                    MaxCapacity = a.MaxCapacity,
                    SoloRegistration = a.SoloRegistration,
                    Logo = a.Logo,
                    Completed = a.Completed,
                    Series = a.Series.Where(s => s.StatusID != 0)
                            .Select(s => (SeriesExtendedViewModel)s),
                    Type = a.Type.Description,
                    StartDate = a.StartDate.SpecifyUtc(),
                    EndDate = a.EndDate.SpecifyUtc(),
                    SeriesScheduleID = a.SeriesScheduleID,
                });
            return activities;
        }
        public IEnumerable<ActivityExtendedViewModel> GetActiveActivities(bool isActive)
        {
            return GetActivities().Where(a => !a.Completed == isActive);
        }

        public ActivityExtendedViewModel? GetActivityByID(int activityID)
        {
            var activity = _context.Activity.Where(a => a.ID == activityID && a.StatusID != 0)
                            .Select(a => new ActivityExtendedViewModel
                            {
                                ID = a.ID,
                                Name = a.Name,
                                RegistrationStart = a.RegistrationStart.SpecifyUtc(),
                                RegistrationEnd = a.RegistrationEnd.SpecifyUtc(),
                                RegistrationOpen = DateTime.UtcNow > a.RegistrationStart.SpecifyUtc()
                                    && DateTime.UtcNow < a.RegistrationEnd.SpecifyUtc(),
                                Sport = a.Sport,
                                Status = a.Status.Description,
                                MinCapacity = a.MinCapacity,
                                MaxCapacity = a.MaxCapacity,
                                SoloRegistration = a.SoloRegistration,
                                Logo = a.Logo,
                                Completed = a.Completed,
                                Type = a.Type.Description,
                                StartDate = a.StartDate.SpecifyUtc(),
                                EndDate = a.EndDate.SpecifyUtc(),
                                Series = _seriesService.GetSeriesByActivityID(a.ID), // more expensive route with more data compared to implicit cast of GetActivities()
                                Team = a.Team.Where(t => t.StatusID != 0)
                                    .Select(t => new TeamExtendedViewModel
                                    {
                                        ID = t.ID,
                                        Name = t.Name,
                                        Status = t.Status.Description,
                                        Activity = new ActivityExtendedViewModel {
                                            ID = t.ActivityID, 
                                            Name = a.Name
                                        },
                                        Logo = t.Logo,
                                        TeamRecord = _context.SeriesTeam
                                            .Include(st => st.Team)
                                            .Where(st => st.TeamID == t.ID)
                                            .Select(st => (TeamRecordViewModel) st)
                                    }),
                                SeriesScheduleID = a.SeriesScheduleID,
                            })
                            .FirstOrDefault();
            return activity;
        }

        public async Task<ActivityViewModel> UpdateActivityAsync(int activityID, ActivityPatchViewModel updatedActivity)
        {
            var activity = _context.Activity.Find(activityID);
            if (activity.StatusID == 0) throw new UnprocessibleEntity { ExceptionMessage = "Activity has been deleted" };

            activity.Name = updatedActivity.Name ?? activity.Name;
            activity.RegistrationStart = updatedActivity.RegistrationStart ?? activity.RegistrationStart;
            activity.RegistrationEnd = updatedActivity.RegistrationEnd ?? activity.RegistrationEnd;
            activity.SportID = updatedActivity.SportID ?? activity.SportID;
            activity.StatusID = updatedActivity.StatusID ?? activity.StatusID;
            activity.MinCapacity = updatedActivity.MinCapacity ?? activity.MinCapacity;
            activity.MaxCapacity = updatedActivity.MaxCapacity ?? activity.MaxCapacity;
            activity.SoloRegistration = updatedActivity.SoloRegistration ?? activity.SoloRegistration;
            activity.Completed = updatedActivity.Completed ?? activity.Completed;
            activity.TypeID = updatedActivity.TypeID ?? activity.TypeID;
            activity.StartDate = updatedActivity.StartDate ?? activity.StartDate;
            activity.EndDate = updatedActivity.EndDate ?? activity.EndDate;
            activity.SeriesScheduleID = updatedActivity.SeriesScheduleID ?? activity.SeriesScheduleID;

            if (updatedActivity.IsLogoUpdate)
            {
                if (updatedActivity.Logo != null)
                {
                    // ImageUtils.GetImageFormat checks whether the image type is valid (jpg/jpeg/png)
                    var (extension, format, data) = ImageUtils.GetImageFormat(updatedActivity.Logo);

                    string? imagePath = null;
                    // If old image exists, overwrite it with new image at same path
                    if (activity.Logo != null)
                    {
                        imagePath = GetImagePath(Path.GetFileName(activity.Logo));
                    }
                    // Otherwise, upload new image and save url to db
                    else
                    {
                        // Use a unique alphanumeric GUID string as the file name
                        var filename = $"{Guid.NewGuid().ToString("N")}.{extension}";
                        imagePath = GetImagePath(filename);
                        var url = GetImageURL(filename);
                        activity.Logo = url;
                    }

                    ImageUtils.UploadImage(imagePath, data, format);
                }

                //If the image property is null, it means that either the user
                //chose to remove the previous image or that there was no previous
                //image (DeleteImage is designed to handle this).
                else if (activity.Logo != null)
                {
                    var imagePath = GetImagePath(Path.GetFileName(activity.Logo));

                    ImageUtils.DeleteImage(imagePath);
                    activity.Logo = updatedActivity.Logo; //null
                }
            }

            await _context.SaveChangesAsync();
            return activity;
        }

        public async Task<ActivityViewModel> PostActivityAsync(ActivityUploadViewModel newActivity)
        {
            var activity = newActivity.ToActivity();

            await _context.Activity.AddAsync(activity);
            await _context.SaveChangesAsync();

            return activity;
        }

        public async Task<ParticipantActivityViewModel> AddParticipantActivityInvolvementAsync(string username, int activityID, int privTypeID, bool isFreeAgent)
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
            return _context.ParticipantActivity.Where(pa => pa.PrivTypeID != 0).Any(pa =>
                pa.ParticipantUsername == username 
                && pa.ActivityID == activityID 
                && pa.PrivTypeID == 2); // PrivType: 2 => Referee
        }

        public bool ActivityTeamCapacityReached(int activityID)
        {
            var activity = _context.Activity.Find(activityID);

            //if the activity is deleted, throw exception
            if (activity.StatusID == 0) throw new UnprocessibleEntity { ExceptionMessage = "Activity has been deleted" };

            int? capacity = activity?.MaxCapacity;
            if (capacity is null) return false;
            int numTeams = activity.Team.Count();
            return numTeams >= capacity;
        }

        public bool ActivityRegistrationClosed(int activityID)
        {
            var a = _context.Activity.Find(activityID);
            return !(DateTime.UtcNow > a.RegistrationStart.SpecifyUtc()
                        && DateTime.UtcNow < a.RegistrationEnd.SpecifyUtc());
        }

        public async Task<ActivityViewModel> DeleteActivityCascade(int activityID)
        {
            var activity = _context.Activity
                .Include(a => a.ParticipantActivity)
                .Include(a => a.Team)
                .Include(a => a.Series)
                    .ThenInclude(a => a.Match)
                        .ThenInclude(a => a.MatchTeam)
                .FirstOrDefault(a => a.ID == activityID);
            var participantActivity = activity.ParticipantActivity;
            var activityTeams = activity.Team;
            var activitySeries = activity.Series;

            //delete activity
            activity.StatusID = 0;

            //delete participant involvement

            foreach (var pa in participantActivity)
                pa.PrivTypeID = 0;


            // delete teams
            
            foreach (var team in activityTeams)
            {
                team.StatusID = 0;
                var participantTeams = _context.ParticipantTeam.Where(pt => pt.TeamID == team.ID).ToList();
                foreach (var participantTeam in participantTeams)
                {
                    participantTeam.RoleTypeID = 0;
                }
            }
            
            // delete series
            foreach (var series in activitySeries)
            {
                //delete matches
                foreach (var match in series.Match)
                {
                    //delete matchteam
                    foreach (var mt in match.MatchTeam)
                        mt.StatusID = 0;
                    //deletematch
                    match.StatusID = 0;

                }
                //delete series
                series.StatusID = 0;
            }

            await _context.SaveChangesAsync();
            return activity;
        }
        private string GetImagePath(string filename)
        {
            return Path.Combine(_webHostEnvironment.ContentRootPath, "browseable", "uploads", "recim", "activity", filename);
        }

        private string GetImageURL(string filename)
        {
            var serverAddress = _serverUtils.GetAddress();
            if (serverAddress is not string) throw new Exception("Could not upload Rec-IM Activity Image: Server Address is null");

            if (serverAddress.Contains("localhost"))
                serverAddress += '/';
            var url = $"{serverAddress}browseable/uploads/recim/activity/{filename}";
            return url;
        }
    }
}

