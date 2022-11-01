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
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Gordon360.Services
{
    /// <summary>
    /// Service Class that facilitates data transactions between the ActivitiesController and the ACT_INFO database model.
    /// ACT_INFO is basically a copy of the ACT_CLUB_DEF domain model in TmsEPrd but with extra fields that we want to store (activity image, blurb etc...)
    /// Activity Info and ACtivity may be talked about interchangeably.
    /// </summary>
    public class ActivityService : IActivityService
    {
        private readonly CCTContext _context;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IServer _server;

        public ActivityService(CCTContext context, IConfiguration config, IWebHostEnvironment webHostEnvironment, IServer server)
        {
            _context = context;
            _config = config;
            _webHostEnvironment = webHostEnvironment;
            _server = server;
        }
        /// <summary>
        /// Fetches a single activity record whose id matches the id provided as an argument
        /// </summary>
        /// <param name="activityCode">The activity code</param>
        /// <returns>ActivityViewModel if found, null if not found</returns>
        public ActivityInfoViewModel Get(string activityCode)
        {
            var query = _context.ACT_INFO.Find(activityCode);
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
            var query = await _context.Procedures.ACTIVE_CLUBS_PER_SESS_IDAsync(sessionCode);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "No Activities for this session was not found." };
            }

            // Transform the ActivityViewModel (ACT_CLUB_DEF) into ActivityInfoViewModel
            return query.Join(_context.ACT_INFO, act => act.ACT_CDE, actInfo => actInfo.ACT_CDE, (act, actInfo) => new ActivityInfoViewModel
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
            var query = await _context.Procedures.DISTINCT_ACT_TYPEAsync(sessionCode);
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
            var query = _context.ACT_INFO;
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
            if (_context.MEMBERSHIP.Where(m => m.ACT_CDE.Equals(activityCode) && m.SESS_CDE.Equals(sessionCode) && m.PART_CDE != "GUEST" && m.END_DTE != null).Any())
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets a collection of all the current open activities, by finding which activities have 
        /// memberships with an END_DTE that is null
        /// </summary>
        /// <returns>The collection of activity codes for open activities</returns>
        public IEnumerable<string> GetOpenActivities(string sess_cde)
        {
            var query = from mem in _context.MEMBERSHIP.Where(m => m.END_DTE == null && m.PART_CDE != "GUEST" && m.SESS_CDE == sess_cde)
                        group mem by mem.ACT_CDE into activities
                        select activities;

            // Convert the query result into a simple list of strings
            List<string> activity_codes = new List<string>();
            foreach (var a in query)
            {
                activity_codes.Add(a.Key.Trim());
            }


            return activity_codes;

            //TODO: this works for all the activities that actually have members. But if an acivity has no members, it
            // will not show up as closed or open.

        }

        /// <summary>
        /// Gets a collection of all the current open activities for which a given user is group admin, by finding which activities have 
        /// memberships with an END_DTE that is null
        /// </summary>
        /// <returns>The collection of activity codes for open activities</returns>
        public IEnumerable<string> GetOpenActivities(string sess_cde, int gordonID)
        {
            var query = from mem in _context.MEMBERSHIP.Where(m => m.END_DTE == null &&
                m.SESS_CDE.Equals(sess_cde) && m.ID_NUM == gordonID && m.GRP_ADMIN == true)
                        group mem by mem.ACT_CDE into activities
                        select activities;

            // Convert the query result into a simple list of strings
            List<string> activity_codes = new List<string>();
            foreach (var a in query)
            {
                activity_codes.Add(a.Key.Trim());
            }


            return activity_codes;

            //TODO: this works for all the activities that actually have members. But if an acivity has no members, it
            // will not show up as closed or open.

        }

        /// <summary>
        /// Gets a collection of all the current activities already closed out, by finding which activities have 
        /// memberships with an END_DTE that is not null
        /// </summary>
        /// <returns>The collection of activity codes for open activities</returns>
        public IEnumerable<string> GetClosedActivities(string sess_cde)
        {
            var query = from mem in _context.MEMBERSHIP.Where(m => m.END_DTE != null && m.PART_CDE != "GUEST" && m.SESS_CDE == sess_cde)
                        group mem by mem.ACT_CDE into activities
                        orderby activities.Key
                        select activities;

            List<string> activity_codes = new List<string>();

            foreach (var a in query)
            {
                activity_codes.Add(a.Key.Trim());
            }

            return activity_codes;

        }

        /// <summary>
        /// Gets a collection of all the current closed activities for which a given user is group admin, by finding which activities have 
        /// memberships with an END_DTE that is not null
        /// </summary>
        /// <param name="gordonID">The user's id</param>
        /// <param name="sess_cde">The session we want to get the closed activities for</param>
        /// <returns>The collection of activity codes for open activities</returns>
        public IEnumerable<string> GetClosedActivities(string sess_cde, int gordonID)
        {
            var query = _context.MEMBERSHIP.Where(m => m.END_DTE != null &&
                m.SESS_CDE.Equals(sess_cde) && m.ID_NUM == gordonID && m.GRP_ADMIN == true);

            // Convert the query result into a simple list of strings
            List<string> activity_codes = new List<string>();
            foreach (var a in query)
            {
                activity_codes.Add(a.ACT_CDE);
            }


            return activity_codes;

            //TODO: this works for all the activities that actually have members. But if an acivity has no members, it
            // will not show up as closed or open.

        }

        /// <summary>
        /// Updates the Activity Info 
        /// </summary>
        /// <param name="involvement">The activity info resource with the updated information</param>
        /// <param name="activityCode">The id of the activity info to be updated</param>
        /// <returns>The updated activity info resource</returns>
        public ACT_INFO Update(string activityCode, InvolvementUpdateViewModel involvement)
        {
            var original = _context.ACT_INFO.Find(activityCode);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity Info was not found." };
            }

            // One can only update certain fields within a membrship
            original.ACT_BLURB = involvement.Description;
            original.ACT_URL = involvement.Url;
            original.ACT_JOIN_INFO = involvement.JoinInfo;

            _context.SaveChanges();

            return original;
        }

        /// <summary>
        /// Closes out a specific activity for a specific session
        /// </summary>
        /// <param name="activityCode">The activity code for the activity that will be closed</param>
        /// <param name="sess_cde">The session code for the session where the activity is being closed</param>
        public void CloseOutActivityForSession(string activityCode, string sess_cde)
        {
            var memberships = _context.MEMBERSHIP.Where(x => x.ACT_CDE == activityCode && x.SESS_CDE == sess_cde);

            if (!memberships.Any())
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "No members found for this activity in this session." };
            }

            var session = _context.CM_SESSION_MSTR.Where(x => x.SESS_CDE == sess_cde).FirstOrDefault();

            foreach (var mem in memberships)
            {
                mem.END_DTE = session.SESS_END_DTE;
            }

            _context.SaveChanges();
        }

        /// <summary>
        /// Open a specific activity for a specific session
        /// </summary>
        /// <param name="activityCode">The activity code for the activity that will be closed</param>
        /// <param name="sess_cde">The session code for the session where the activity is being closed</param>
        public void OpenActivityForSession(string activityCode, string sess_cde)
        {
            var memberships = _context.MEMBERSHIP.Where(x => x.ACT_CDE == activityCode && x.SESS_CDE == sess_cde);

            if (!memberships.Any())
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "No members found for this activity in this session." };
            }
            foreach (var mem in memberships)
            {
                mem.END_DTE = null;
            }

            _context.SaveChanges();
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
            var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "browseable", "uploads", involvement.ACT_CDE.Trim(), filename);
            var baseUrl = _server.Features.Get<IServerAddressesFeature>()?.Addresses.FirstOrDefault(a => a.Contains("https"));
            var url = $"{baseUrl}/browseable/uploads/{involvement.ACT_CDE.Trim()}/{filename}";

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
            await _context.SaveChangesAsync();

            return involvement;
        }

        /// <summary>
        /// Reset the path for the activity image
        /// </summary>
        /// <param name="activityCode">The activity code</param>
        public void ResetActivityImage(string activityCode)
        {
            var original = _context.ACT_INFO.Find(activityCode);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity Info was not found." };
            }

            original.ACT_IMG_PATH = _config["DEFAULT_ACTIVITY_IMAGE_PATH"];
            _context.SaveChanges();
        }

        /// <summary>
        /// change activty privacy
        /// </summary>
        /// <param name="activityCode">The activity code</param>
        /// <param name="isPrivate">activity private or not</param>
        public void TogglePrivacy(string activityCode, bool isPrivate)
        {
            var original = _context.ACT_INFO.Find(activityCode);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity Info was not found." };
            }

            original.PRIVACY = isPrivate;

            _context.SaveChanges();
        }


    }
}