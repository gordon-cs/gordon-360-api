using Gordon360.Database.CCT;
using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services
{
    /// <summary>
    /// Service Class that facilitates data transactions between the ActivitiesController and the ACT_INFO database model.
    /// ACT_INFO is basically a copy of the ACT_CLUB_DEF domain model in TmsEPrd but with extra fields that we want to store (activity image, blurb etc...)
    /// Activity Info and ACtivity may be talked about interchangeably.
    /// </summary>
    public class ActivityService : IActivityService
    {
        private CCTContext _context;

        public ActivityService(CCTContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Fetches a single activity record whose id matches the id provided as an argument
        /// </summary>
        /// <param name="id">The activity code</param>
        /// <returns>ActivityViewModel if found, null if not found</returns>
        public ActivityInfoViewModel Get(string id)
        {
            var query = _context.ACT_INFO.Find(id);
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
        /// <param name="id">The session code</param>
        /// <returns>ActivityViewModel IEnumerable. If nothing is found, an empty IEnumerable is returned.</returns>
        public async Task<IEnumerable<ActivityInfoViewModel>> GetActivitiesForSession(string id)
        {
            var query = await _context.Procedures.ACTIVE_CLUBS_PER_SESS_IDAsync(id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "No Activities for this session was not found." };
            }

            // Transform the ActivityViewModel (ACT_CLUB_DEF) into ActivityInfoViewModel
            return query.Select(x =>
            {
                var record = _context.ACT_INFO.Find(x.ACT_CDE);
                if (record == null)
                {
                    throw new ResourceNotFoundException() { ExceptionMessage = "The Activity Info was not found." };
                }

                return new ActivityInfoViewModel
                {
                    ActivityCode = x.ACT_CDE.Trim(),
                    ActivityDescription = x.ACT_DESC ?? "",
                    ActivityBlurb = record.ACT_BLURB ?? "",
                    ActivityURL = record.ACT_URL ?? "",
                    ActivityImagePath = record.ACT_IMG_PATH.Trim() ?? "",
                    ActivityType = record.ACT_TYPE.Trim() ?? "",
                    ActivityTypeDescription = record.ACT_TYPE_DESC.Trim() ?? "",
                    ActivityJoinInfo = record.ACT_JOIN_INFO ?? ""
                };
            });
        }
        /// <summary>
        /// Fetches the Activity types of activities that are active during the session whose code is specified as parameter.
        /// </summary>
        /// <param name="id">The session code</param>
        /// <returns>ActivityViewModel IEnumerable. If nothing is found, an empty IEnumerable is returned.</returns>
        public async Task<IEnumerable<string>> GetActivityTypesForSession(string id)
        {
            // Stored procedure returns column ACT_TYPE_DESC
            var query = await _context.Procedures.DISTINCT_ACT_TYPEAsync(id);
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
        /// <param name="sessionCode">The activity code for the activity in question</param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsOpen(string id, string sessionCode)
        {
            // Check to see if there are any memberships where END_DTE is not null
            if (_context.MEMBERSHIP.Where(m => m.ACT_CDE.Equals(id) && m.SESS_CDE.Equals(sessionCode) && m.PART_CDE != "GUEST" && m.END_DTE != null).Any())
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
        public IEnumerable<string> GetOpenActivities(string sess_cde, int id)
        {
            var query = from mem in _context.MEMBERSHIP.Where(m => m.END_DTE == null &&
                m.SESS_CDE.Equals(sess_cde) && m.ID_NUM == id && m.GRP_ADMIN == true)
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
        /// <param name="id">The user's id</param>
        /// <param name="sess_cde">The session we want to get the closed activities for</param>
        /// <returns>The collection of activity codes for open activities</returns>
        public IEnumerable<string> GetClosedActivities(string sess_cde, int id)
        {
            var query = _context.MEMBERSHIP.Where(m => m.END_DTE != null &&
                m.SESS_CDE.Equals(sess_cde) && m.ID_NUM == id && m.GRP_ADMIN == true);

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
        /// <param name="activity">The activity info resource with the updated information</param>
        /// <param name="id">The id of the activity info to be updated</param>
        /// <returns>The updated activity info resource</returns>
        public ACT_INFO Update(string id, ACT_INFO activity)
        {
            var original = _context.ACT_INFO.Find(id);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity Info was not found." };
            }

            validateActivityInfo(activity);

            // One can only update certain fields within a membrship
            original.ACT_BLURB = activity.ACT_BLURB;
            original.ACT_URL = activity.ACT_URL;
            original.ACT_JOIN_INFO = activity.ACT_JOIN_INFO;

            _context.SaveChanges();

            return original;

        }

        /// <summary>
        /// Closes out a specific activity for a specific session
        /// </summary>
        /// <param name="id">The activity code for the activity that will be closed</param>
        /// <param name="sess_cde">The session code for the session where the activity is being closed</param>
        public void CloseOutActivityForSession(string id, string sess_cde)
        {
            var memberships = _context.MEMBERSHIP.Where(x => x.ACT_CDE == id && x.SESS_CDE == sess_cde);

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
        /// <param name="id">The activity code for the activity that will be closed</param>
        /// <param name="sess_cde">The session code for the session where the activity is being closed</param>
        public void OpenActivityForSession(string id, string sess_cde)
        {
            var memberships = _context.MEMBERSHIP.Where(x => x.ACT_CDE == id && x.SESS_CDE == sess_cde);

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
        /// Sets the path for the activity image.
        /// </summary>
        /// <param name="id">The activity code</param>
        /// <param name="path"></param>
        public void UpdateActivityImage(string id, string path)
        {
            var original = _context.ACT_INFO.Find(id);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity Info was not found." };
            }

            original.ACT_IMG_PATH = path;

            _context.SaveChanges();
        }
        /// <summary>
        /// Reset the path for the activity image
        /// </summary>
        /// <param name="id">The activity code</param>
        public void ResetActivityImage(string id)
        {
            var original = _context.ACT_INFO.Find(id);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity Info was not found." };
            }

            /* @TODO: fix images
            original.ACT_IMG_PATH = System.Web.Configuration.WebConfigurationManager.AppSettings["DEFAULT_ACTIVITY_IMAGE_PATH"];
            */
            _context.SaveChanges();
        }
        // Helper method to validate an activity info post. Throws an exception that gets caught later if something is not valid.
        // Returns true if all is well. The return value is not really used though. This could be of type void.
        private bool validateActivityInfo(ACT_INFO activity)
        {
            var activityExists = _context.ACT_INFO.Where(x => x.ACT_CDE == activity.ACT_CDE).Count() > 0;
            if (!activityExists)
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };

            return true;

        }

        /// <summary>
        /// change activty privacy
        /// </summary>
        /// <param name="id">The activity code</param>
        /// <param name="p">activity private or not</param>
        public void TogglePrivacy(string id, bool p)
        {
            var original = _context.ACT_INFO.Find(id);

            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity Info was not found." };
            }

            original.PRIVACY = p;

            _context.SaveChanges();
        }


    }
}