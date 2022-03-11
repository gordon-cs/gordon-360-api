using Gordon360.Database.CCT;
using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Static.Methods;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services
{
    /// <summary>
    /// Service Class that facilitates data transactions between the MembershipsController and the Membership database model.
    /// </summary>
    public class MembershipService : IMembershipService
    {
        private readonly CCTContext _context;

        public MembershipService(CCTContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new Membership record to storage. Since we can't establish foreign key constraints and relationships on the database side,
        /// we do it here by using the validateMembership() method.
        /// </summary>
        /// <param name="membership">The membership to be added</param>
        /// <returns>The newly added Membership object</returns>
        public MEMBERSHIP Add(MEMBERSHIP membership)
        {
            // validate returns a boolean value.
            validateMembership(membership);
            isPersonAlreadyInActivity(membership);

            // Get session begin date of the membership
            var sessionCode = _context.CM_SESSION_MSTR.Where(x => x.SESS_CDE.Equals(membership.SESS_CDE)).FirstOrDefault();
            membership.BEGIN_DTE = (DateTime)sessionCode.SESS_BEGN_DTE;

            // The Add() method returns the added membership.
            var payload = _context.MEMBERSHIP.Add(membership);

            // There is a unique constraint in the Database on columns (ID_NUM, PART_LVL, SESS_CDE and ACT_CDE)
            if (payload == null)
            {
                throw new ResourceCreationException() { ExceptionMessage = "There was an error creating the membership. Verify that a similar membership doesn't already exist." };
            }
            _context.SaveChanges();

            return membership;

        }

        /// <summary>
        /// Delete the membership whose id is specified by the parameter.
        /// </summary>
        /// <param name="id">The membership id</param>
        /// <returns>The membership that was just deleted</returns>
        public MEMBERSHIP Delete(int id)
        {
            var result = _context.MEMBERSHIP.Find(id);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Membership was not found." };
            }
            _context.MEMBERSHIP.Remove(result);
            _context.SaveChanges();

            return result;
        }

        /// <summary>	
        /// Fetch the membership whose id is specified by the parameter	
        /// </summary>	
        /// <param name="id">The membership id</param>	
        /// <returns>MembershipViewModel if found, null if not found</returns>	
        public MEMBERSHIP GetSpecificMembership(int id)
        {
            MEMBERSHIP result = _context.MEMBERSHIP.Find(id);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Membership was not found." };
            }

            return result;
        }

        /// <summary>
        /// Fetches all membership records from storage.
        /// </summary>
        /// <returns>MembershipViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public async Task<IEnumerable<MembershipViewModel>> GetAll()
        {

            var allMemberships = await _context.Procedures.ALL_MEMBERSHIPSAsync();

            return allMemberships.OrderByDescending(m => m.StartDate).Select(m => new MembershipViewModel
            {
                MembershipID = m.MembershipID,
                ActivityCode = m.ActivityCode.Trim(),
                ActivityDescription = m.ActivityDescription.Trim(),
                ActivityImagePath = m.ActivityImagePath,
                SessionCode = m.SessionCode.Trim(),
                SessionDescription = m.SessionDescription.Trim(),
                IDNumber = m.IDNumber,
                FirstName = m.FirstName.Trim(),
                LastName = m.LastName.Trim(),
                Participation = m.Participation.Trim(),
                ParticipationDescription = m.ParticipationDescription.Trim(),
                StartDate = m.StartDate,
                EndDate = m.EndDate,
                Description = m.Description,
                GroupAdmin = m.GroupAdmin,
                Privacy = m.Privacy
            });
        }

        /// <summary>
        /// Fetches the memberships associated with the activity whose code is specified by the parameter.
        /// </summary>
        /// <param name="id">The activity code.</param>
        /// <returns>MembershipViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public async Task<IEnumerable<MembershipViewModel>> GetMembershipsForActivity(string id)
        {
            var memberships = await _context.Procedures.MEMBERSHIPS_PER_ACT_CDEAsync(id);

            return memberships.OrderByDescending(x => x.StartDate).Select(m => new MembershipViewModel
            {
                MembershipID = m.MembershipID,
                ActivityCode = m.ActivityCode.Trim(),
                ActivityDescription = m.ActivityDescription.Trim(),
                ActivityImagePath = m.ActivityImagePath,
                SessionCode = m.SessionCode.Trim(),
                SessionDescription = m.SessionDescription.Trim(),
                IDNumber = m.IDNumber,
                FirstName = m.FirstName.Trim(),
                LastName = m.LastName.Trim(),
                Mail_Location = m.Mail_Location,
                Participation = m.Participation.Trim(),
                ParticipationDescription = m.ParticipationDescription.Trim(),
                StartDate = m.StartDate,
                EndDate = m.EndDate,
                Description = m.Description,
                GroupAdmin = m.GroupAdmin,
                AccountPrivate = m.AccountPrivate
            });
        }

        /// <summary>
        /// Fetches the group admin (who have edit privileges of the page) of the activity whose activity code is specified by the parameter.
        /// </summary>
        /// <param name="id">The activity code.</param>
        /// <returns>MembershipViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public async Task<IEnumerable<MembershipViewModel>> GetGroupAdminMembershipsForActivity(string id)
        {
            var memberships = await GetMembershipsForActivity(id);
            return memberships.Where(m => m.GroupAdmin == true);
        }

        /// <summary>
        /// Fetches the leaders of the activity whose activity code is specified by the parameter.
        /// </summary>
        /// <param name="id">The activity code.</param>
        /// <returns>MembershipViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public async Task<IEnumerable<MembershipViewModel>> GetLeaderMembershipsForActivity(string id)
        {
            var leaderRole = Helpers.GetLeaderRoleCodes();
            var memberships = await GetMembershipsForActivity(id);
            return memberships.Where(x => x.Participation == leaderRole);
        }

        /// <summary>
        /// Fetches the advisors of the activity whose activity code is specified by the parameter.
        /// </summary>
        /// <param name="id">The activity code.</param>
        /// <returns>MembershipViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public async Task<IEnumerable<MembershipViewModel>> GetAdvisorMembershipsForActivity(string id)
        {

            var advisorRole = Helpers.GetAdvisorRoleCodes();
            var memberships = await GetMembershipsForActivity(id);
            return memberships.Where(x => x.Participation == advisorRole);
        }

        /// <summary>
        /// Fetches all the membership information linked to the student whose id appears as a parameter.
        /// </summary>
        /// <param name="id">The student id.</param>
        /// <returns>A MembershipViewModel IEnumerable. If nothing is found, an empty IEnumerable is returned.</returns>
        public async Task<IEnumerable<MembershipViewModel>> GetMembershipsForStudent(string id)
        {
            var studentExists = _context.ACCOUNT.Where(x => x.gordon_id.Trim() == id).Any();
            if (!studentExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
            }

            var memberships = await _context.Procedures.MEMBERSHIPS_PER_STUDENT_IDAsync(int.Parse(id));

            return memberships.OrderByDescending(x => x.StartDate).Select(m => new MembershipViewModel
            {
                MembershipID = m.MembershipID,
                ActivityCode = m.ActivityCode.Trim(),
                ActivityDescription = m.ActivityDescription.Trim(),
                ActivityImagePath = m.ActivityImagePath,
                SessionCode = m.SessionCode.Trim(),
                SessionDescription = m.SessionDescription.Trim(),
                IDNumber = m.IDNumber,
                FirstName = m.FirstName.Trim(),
                LastName = m.LastName.Trim(),
                Participation = m.Participation.Trim(),
                ParticipationDescription = m.ParticipationDescription.Trim(),
                StartDate = m.StartDate,
                EndDate = m.EndDate,
                Description = m.Description,
                ActivityType = m.ActivityType.Trim(),
                ActivityTypeDescription = m.ActivityTypeDescription.Trim(),
                GroupAdmin = m.GroupAdmin,
                Privacy = m.Privacy,
            });
        }

        /// <summary>
        /// Fetches the number of followers associated with the activity whose code is specified by the parameter.
        /// </summary>
        /// <param name="id">The activity code.</param>
        /// <returns>int.</returns>
        public async Task<int> GetActivityFollowersCount(string id)
        {
            var memberships = await GetMembershipsForActivity(id);
            return memberships.Where(x => x.Participation == "GUEST").Count();
        }

        /// <summary>
        /// Fetches the number of memberships associated with the activity whose code is specified by the parameter.
        /// </summary>
        /// <param name="id">The activity code.</param>
        /// <returns>int.</returns>
        public async Task<int> GetActivityMembersCount(string id)
        {
            var memberships = await GetMembershipsForActivity(id);
            return memberships.Where(x => x.Participation != "GUEST").Count();
        }

        /// <summary>
        /// Fetches the number of followers associated with the activity and session whose codes are specified by the parameter.
        /// </summary>
        /// <param name="id">The activity code.</param>
        /// <param name="sess_cde">The session code</param>
        /// <returns>int.</returns>
        public async Task<int> GetActivityFollowersCountForSession(string id, string sess_cde)
        {
            var memberships = await GetMembershipsForActivity(id);
            return memberships.Where(x => x.Participation == "GUEST" && x.SessionCode == sess_cde).Count();
        }

        /// <summary>
        /// Fetches the number of memberships associated with the activity and session whose codes are specified by the parameter.
        /// </summary>
        /// <param name="id">The activity code.</param>
        /// <param name="sess_cde">The session code</param>
        /// <returns>int.</returns>
        public async Task<int> GetActivityMembersCountForSession(string id, string sess_cde)
        {
            var memberships = await GetMembershipsForActivity(id);
            return memberships.Where(x => x.Participation != "GUEST" && x.SessionCode == sess_cde).Count();
        }

        /// <summary>
        /// Updates the membership whose id is given as the first parameter to the contents of the second parameter.
        /// </summary>
        /// <param name="id">The membership id.</param>
        /// <param name="membership">The updated membership.</param>
        /// <returns>The newly modified membership.</returns>
        public MEMBERSHIP Update(int id, MEMBERSHIP membership)
        {
            var original = _context.MEMBERSHIP.Find(id);
            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Membership was not found." };
            }

            validateMembership(membership);

            // One can only update certain fields within a membrship
            //original.BEGIN_DTE = membership.BEGIN_DTE;
            original.COMMENT_TXT = membership.COMMENT_TXT;
            //original.END_DTE = membership.END_DTE;
            original.PART_CDE = membership.PART_CDE;
            original.SESS_CDE = membership.SESS_CDE;

            _context.SaveChanges();

            return original;

        }
        /// <summary>
        /// Switches the group-admin property of the person whose membership id is given
        /// </summary>
        /// <param name="id">The membership id.</param>
        /// <param name="membership">The corresponding membership object</param>
        /// <returns>The newly modified membership.</returns>
        public MEMBERSHIP ToggleGroupAdmin(int id, MEMBERSHIP membership)
        {
            var original = _context.MEMBERSHIP.Find(id);
            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Membership was not found." };
            }

            validateMembership(membership);

            var isGuest = original.PART_CDE == "GUEST";

            if (isGuest)
                throw new ArgumentException("A guest cannot be assigned as an admin.", "Participation Level");

            original.GRP_ADMIN = !original.GRP_ADMIN;

            _context.SaveChanges();

            return original;
        }

        /// <summary>
        /// Switches the privacy property of the person whose membership id is given
        /// </summary>
        /// <param name="id">The membership id.</param>
        /// <param name="p">membership private or not</param>
        /// <returns>The newly modified membership.</returns>
        public void TogglePrivacy(int id, bool p)
        {
            var original = _context.MEMBERSHIP.Find(id);
            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Membership was not found." };
            }

            original.PRIVACY = p;

            _context.SaveChanges();
        }


        /// <summary>
        /// Helper method to Validate a membership
        /// </summary>
        /// <param name="membership">The membership to validate</param>
        /// <returns>True if the membership is valid. Throws ResourceNotFoundException if not. Exception is caught in an Exception Filter</returns>
        private async Task<bool> validateMembership(MEMBERSHIP membership)
        {
            var personExists = _context.ACCOUNT.Where(x => x.gordon_id.Trim() == membership.ID_NUM.ToString()).Any();
            if (!personExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Person was not found." };
            }
            var participationExists = _context.PART_DEF.Where(x => x.PART_CDE.Trim() == membership.PART_CDE).Any();
            if (!participationExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Participation was not found." };
            }
            var sessionExists = _context.CM_SESSION_MSTR.Where(x => x.SESS_CDE.Trim() == membership.SESS_CDE).Any();
            if (!sessionExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Session was not found." };
            }
            var activityExists = _context.ACT_INFO.Where(x => x.ACT_CDE.Trim() == membership.ACT_CDE).Any();
            if (!activityExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
            }

            if (!(await _context.Procedures.ACTIVE_CLUBS_PER_SESS_IDAsync(membership.SESS_CDE)).Any(a => a.ACT_CDE.Trim() == membership.ACT_CDE))
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity is not available for this session." };
            }

            return true;
        }

        private bool isPersonAlreadyInActivity(MEMBERSHIP membershipRequest)
        {
            var personAlreadyInActivity = _context.MEMBERSHIP.Any(x => x.SESS_CDE == membershipRequest.SESS_CDE &&
                x.ACT_CDE == membershipRequest.ACT_CDE && x.ID_NUM == membershipRequest.ID_NUM);

            if (personAlreadyInActivity)
            {
                throw new ResourceCreationException() { ExceptionMessage = "The Person is already part of the activity." };
            }

            return true;
        }

        /// <summary>	
        /// Determines whether or not the given student is a Group Admin of some activity	
        /// </summary>
        /// <param name="studentID">The student id</param>	
        /// <returns>true if student is a Group Admin, else false</returns>	
        public Boolean IsGroupAdmin(int studentID)
        {
            return _context.MEMBERSHIP.Any(membership => membership.ID_NUM == studentID && membership.GRP_ADMIN == true);
        }
    }
}