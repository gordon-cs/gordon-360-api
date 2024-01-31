using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using Gordon360.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Gordon360.Enums;

namespace Gordon360.Services;

/// <summary>
/// Service Class that facilitates data transactions between the ActivitiesController and the ACT_INFO database model.
/// ACT_INFO is basically a copy of the ACT_CLUB_DEF domain model in TmsEPrd but with extra fields that we want to store (activity image, blurb etc...)
/// Activity Info and ACtivity may be talked about interchangeably.
/// </summary>
public class ActivityService(CCTContext context,
                             IConfiguration config,
                             IWebHostEnvironment webHostEnvironment,
                             ServerUtils serverUtils) : IActivityService
{
    /// <summary>
    /// Fetches a single activity record whose id matches the id provided as an argument
    /// </summary>
    /// <param name="activityCode">The activity code</param>
    /// <returns>ActivityViewModel if found, null if not found</returns>
    public ActivityInfoViewModel Get(string activityCode)
    {
        var query = context.ACT_INFO.Find(activityCode);
        if (query == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
        }
        ActivityInfoViewModel result = query;
        return result;
    }

    /// <summary>
    /// Fetches the Activities that are active during the session whose code is specified as parameter.
    /// </summary>
    /// <param name="sessionCode">The session code</param>
    /// <returns>ActivityViewModel IEnumerable. If nothing is found, an empty IEnumerable is returned.</returns>
    public async Task<IEnumerable<ActivityInfoViewModel>> GetActivitiesForSessionAsync(string sessionCode)
    {
        var query = await context.Procedures.ACTIVE_CLUBS_PER_SESS_IDAsync(sessionCode);
        if (query == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "No Activities for this session was not found." };
        }

        // Transform the ActivityViewModel (ACT_CLUB_DEF) into ActivityInfoViewModel
        return query.Join(context.ACT_INFO, act => act.ACT_CDE, actInfo => actInfo.ACT_CDE, (act, actInfo) => new ActivityInfoViewModel
        {
            ActivityCode = act.ACT_CDE.Trim(),
            ActivityDescription = act.ACT_DESC ?? "",
            ActivityBlurb = actInfo.ACT_BLURB ?? "",
            ActivityURL = actInfo.ACT_URL ?? "",
            ActivityImagePath = actInfo.ACT_IMG_PATH.Trim() ?? "",
            ActivityType = actInfo.ACT_TYPE.Trim() ?? "",
            ActivityTypeDescription = actInfo.ACT_TYPE_DESC.Trim() ?? "",
            ActivityJoinInfo = actInfo.ACT_JOIN_INFO ?? ""
        });
    }

    /// <summary>
    /// Fetches the Activity types of activities that are active during the session whose code is specified as parameter.
    /// </summary>
    /// <param name="sessionCode">The session code</param>
    /// <returns>ActivityViewModel IEnumerable. If nothing is found, an empty IEnumerable is returned.</returns>
    public async Task<IEnumerable<string>> GetActivityTypesForSessionAsync(string sessionCode)
    {
        // Stored procedure returns column ACT_TYPE_DESC
        var query = await context.Procedures.DISTINCT_ACT_TYPEAsync(sessionCode);
        if (query == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "No Activities for this session was not found." };
        }

        // Remove white space
        return query.Select(x => x.ACT_TYPE_DESC.Trim());
    }


    /// <summary>
    /// Fetches all activity records from storage.
    /// </summary>
    /// <returns>ActivityViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
    public IEnumerable<ActivityInfoViewModel> GetAll()
    {
        var query = context.ACT_INFO;
        var result = query.Select<ACT_INFO, ActivityInfoViewModel>(x => x);
        return result;
    }

    /// <summary>
    /// Checks to see if a specified activity is still open for this session
    /// Note: the way we know that an activity is open or closed is by the column END_DTE in MEMBERSHIP table
    /// When an activity is closed out, the END_DTE is set to the date on which the closing happened
    /// Otherwise, the END_DTE for all memberships of the activity will be null for that session
    /// </summary>
    /// <param name="activityCode">The activity code for the activity in question</param>
    /// <param name="sessionCode">Code of the session to check</param>
    /// <returns></returns>
    public bool IsOpen(string activityCode, string sessionCode)
    {
        // Check to see if there are any memberships where END_DTE is not null
        if (context.MEMBERSHIP.Where(m => m.ACT_CDE.Equals(activityCode) && m.SESS_CDE.Equals(sessionCode) && m.PART_CDE != "GUEST" && m.END_DTE != null).Any())
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Gets all activities for a session with an open status matching the `getOpen` param
    /// An activity is closed if it has a non-null `END_DTE`, and open otherwise
    /// </summary>
    /// <param name="sess_cde">The session code of the session to find activities for</param>
    /// <param name="getOpen">Whether to get open or closed activites</param>
    /// <returns>Collection of activities for the specified session with a status matching the `getOpen` param</returns>
    public IQueryable<ActivityInfoViewModel> GetActivitiesByStatus(string sess_cde, bool getOpen)
    {
        // TODO: this works for all the activities that actually have members. But if an acivity has no members, it
        // will not show up as closed or open.

        IQueryable<MEMBERSHIP> memberships = context.MEMBERSHIP
            .Where(m =>
                m.PART_CDE != Participation.Guest.GetCode()
                && m.SESS_CDE == sess_cde);

        memberships = getOpen ? memberships.Where(m => m.END_DTE == null) : memberships.Where(m => m.END_DTE != null);

        IQueryable<string> activityCodes = memberships
            .GroupBy(m => m.ACT_CDE)
            .Select(m => m.Key);

        IQueryable<ActivityInfoViewModel> activities = context.ACT_INFO
            .Where(a => activityCodes.Contains(a.ACT_CDE))
            .Select<ACT_INFO, ActivityInfoViewModel>(a => a);

        return activities;
    }

    /// <summary>
    /// Updates the Activity Info 
    /// </summary>
    /// <param name="involvement">The activity info resource with the updated information</param>
    /// <param name="activityCode">The id of the activity info to be updated</param>
    /// <returns>The updated activity info resource</returns>
    public ACT_INFO Update(string activityCode, InvolvementUpdateViewModel involvement)
    {
        var original = context.ACT_INFO.Find(activityCode);

        if (original == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The Activity Info was not found." };
        }

        // One can only update certain fields within a membrship
        original.ACT_BLURB = involvement.Description;
        original.ACT_URL = involvement.Url;
        original.ACT_JOIN_INFO = involvement.JoinInfo;

        context.SaveChanges();

        return original;
    }

    /// <summary>
    /// Closes out a specific activity for a specific session
    /// </summary>
    /// <param name="activityCode">The activity code for the activity that will be closed</param>
    /// <param name="sess_cde">The session code for the session where the activity is being closed</param>
    public void CloseOutActivityForSession(string activityCode, string sess_cde)
    {
        var memberships = context.MEMBERSHIP.Where(x => x.ACT_CDE == activityCode && x.SESS_CDE == sess_cde);

        if (!memberships.Any())
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "No members found for this activity in this session." };
        }

        var session = context.CM_SESSION_MSTR.Where(x => x.SESS_CDE == sess_cde).FirstOrDefault();

        foreach (var mem in memberships)
        {
            mem.END_DTE = session.SESS_END_DTE;
        }

        context.SaveChanges();
    }

    /// <summary>
    /// Open a specific activity for a specific session
    /// </summary>
    /// <param name="activityCode">The activity code for the activity that will be closed</param>
    /// <param name="sess_cde">The session code for the session where the activity is being closed</param>
    public void OpenActivityForSession(string activityCode, string sess_cde)
    {
        var memberships = context.MEMBERSHIP.Where(x => x.ACT_CDE == activityCode && x.SESS_CDE == sess_cde);

        if (!memberships.Any())
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "No members found for this activity in this session." };
        }
        foreach (var mem in memberships)
        {
            mem.END_DTE = null;
        }

        context.SaveChanges();
    }

    /// <summary>
    /// Updates the image for the spcefied involvement
    /// </summary>
    /// <param name="involvement">The involvement to update the image of</param>
    /// <param name="image">The new image</param>
    /// <returns>The involvement with the updated image path</returns>
    public async Task<ACT_INFO> UpdateActivityImageAsync(ACT_INFO involvement, IFormFile image)
    {
        // Put current DateTime in filename so the browser knows it's a new file and refreshes cache
        var filename = $"canvasImage_{DateTime.Now:yyyy-MM-dd_HH-mm-ss-fff}.png";
        var involvement_code = involvement.ACT_CDE.Trim();
        var imagePath = Path.Combine(webHostEnvironment.ContentRootPath, "browseable", "uploads", involvement_code, filename);

        var serverAddress = serverUtils.GetAddress();
        if (serverAddress is not string) throw new Exception("Could not upload Involvement Image: Server Address is null");

        var url = $"{serverAddress}browseable/uploads/{involvement_code}/{filename}";

        //delete old image file if it exists.
        if (Path.GetDirectoryName(imagePath) is string directory && Directory.Exists(directory))
        {
            foreach (FileInfo file in new DirectoryInfo(directory).GetFiles())
            {
                file.Delete();
            }
        }

        ImageUtils.UploadImageAsync(imagePath, image);

        involvement.ACT_IMG_PATH = url;
        await context.SaveChangesAsync();

        return involvement;
    }

    /// <summary>
    /// Reset the path for the activity image
    /// </summary>
    /// <param name="activityCode">The activity code</param>
    public void ResetActivityImage(string activityCode)
    {
        var original = context.ACT_INFO.Find(activityCode);

        if (original == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The Activity Info was not found." };
        }

        original.ACT_IMG_PATH = config["DEFAULT_ACTIVITY_IMAGE_PATH"];
        context.SaveChanges();
    }

    /// <summary>
    /// change activty privacy
    /// </summary>
    /// <param name="activityCode">The activity code</param>
    /// <param name="isPrivate">activity private or not</param>
    public void TogglePrivacy(string activityCode, bool isPrivate)
    {
        var original = context.ACT_INFO.Find(activityCode);

        if (original == null)
        {
            throw new ResourceNotFoundException() { ExceptionMessage = "The Activity Info was not found." };
        }

        original.PRIVACY = isPrivate;

        context.SaveChanges();
    }


}