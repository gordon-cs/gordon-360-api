using Gordon360.Models.CCT.Context;
using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.ViewModels;
using Gordon360.Static.Methods;
using Microsoft.EntityFrameworkCore;
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
        private IAccountService _accountService;

        public MembershipService(CCTContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        /// <summary>
        /// Adds a new Membership record to storage. Since we can't establish foreign key constraints and relationships on the database side,
        /// we do it here by using the validateMembership() method.
        /// </summary>
        /// <param name="membershipUpload">The membership to be added</param>
        /// <returns>The newly added membership object as a MembershipView</returns>
        public async Task<MembershipView> AddAsync(MembershipUploadViewModel membershipUpload)
        {
            ValidateMembership(membershipUpload);
            IsPersonAlreadyInActivity(membershipUpload);

            var sessionBeginDate = _context.CM_SESSION_MSTR
                .Where(x => x.SESS_CDE.Equals(membershipUpload.Session))
                .FirstOrDefault()?.SESS_BEGN_DTE ?? DateTime.Now;

            int gordonId = int.Parse(_accountService.GetAccountByUsername(membershipUpload.Username).GordonID);

            MEMBERSHIP m = membershipUpload.ToMembership(gordonId, sessionBeginDate);
            var payload = await _context.MEMBERSHIP.AddAsync(m);

            if (payload?.Entity == null)
            {
                throw new ResourceCreationException() { ExceptionMessage = "There was an error creating the membership." };
            } else
            {
                await _context.SaveChangesAsync();
                return GetMembershipViewById(payload.Entity.MEMBERSHIP_ID);
            }


        }

        /// <summary>
        /// Delete the membership whose id is specified by the parameter.
        /// </summary>
        /// <param name="membershipID">The membership id</param>
        /// <returns>The membership that was just deleted as a MembershipView</returns>
        public MembershipView Delete(int membershipID)
        {
            var result = _context.MEMBERSHIP.Find(membershipID);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Membership was not found." };
            }

            MembershipView toReturn = GetMembershipViewById(result.MEMBERSHIP_ID);

            _context.MEMBERSHIP.Remove(result);
            _context.SaveChanges();

            return toReturn;
        }

        /// <summary>	
        /// Fetch the membership whose id is specified by the parameter	
        /// </summary>	
        /// <param name="membershipID">The membership id</param>	
        /// <returns>The found membership as a MembershipView</returns>	
        public MembershipView GetSpecificMembership(int membershipID)
        {
            MembershipView result = _context.MembershipView.FirstOrDefault(m => m.MembershipID == membershipID);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Membership was not found." };
            }

            return result;
        }

        /// <summary>
        /// Fetches the memberships associated with the activity whose code is specified by the parameter.
        /// </summary>
        /// <param name="activityCode">The activity code</param>
        /// <param name="sessionCode">Optional code of session to get memberships for</param>
        /// <param name="groupAdmin">Optional filter for group admins</param>
        /// <param name="participationTypes">Optional filter for involvement participation type (MEMBR, ADV, LEAD, GUEST)</param>
        /// <returns>An IEnumerable of the matching MembershipView objects</returns>
        public IEnumerable<MembershipView> GetMembershipsForActivity(
            string activityCode,
            string? sessionCode = null,
            bool? groupAdmin = null,
            string[]? participationTypes = null

        ) {

            sessionCode ??= Helpers.GetCurrentSession(_context);

            IEnumerable<MembershipView> memberships = _context.MembershipView
                .Where(m =>
                    m.SessionCode.Trim() == sessionCode
                    && m.ActivityCode == activityCode);
            if (groupAdmin is not null)
            {
                memberships = memberships.Where(m => m.GroupAdmin == groupAdmin);
            }

            if (participationTypes?.Length > 0)
            {
                memberships = memberships.Where(m => participationTypes.Contains(m.Participation));
            }

            return memberships.OrderByDescending(m => m.StartDate);
        }

        /// <summary>
        /// Fetches the group admin (who have edit privileges of the page) of the activity whose activity code is specified by the parameter.
        /// </summary>
        /// <param name="activityCode">The activity code</param>
        /// <param name="sessionCode">The session code</param>
        /// <returns>An IEnumerable of the matching MembershipView objects</returns>
        public IEnumerable<MembershipView> GetGroupAdminMembershipsForActivity(string activityCode, string? sessionCode = null)
        {
            sessionCode ??= Helpers.GetCurrentSession(_context);
            return _context.MembershipView.Where(m => m.ActivityCode == activityCode && m.SessionCode == sessionCode && m.GroupAdmin == true);
        }

        /// <summary>
        /// Fetches the leaders of the activity whose activity code is specified by the parameter.
        /// </summary>
        /// <param name="activityCode">The activity code</param>
        /// <returns>An IEnumerable of the matching MembershipView objects</returns>
        public IEnumerable<MembershipView> GetLeaderMembershipsForActivity(string activityCode)
        {
            var leaderRole = Helpers.GetLeaderRoleCodes();
            return _context.MembershipView.Where(m => m.ActivityCode == activityCode && m.Participation == leaderRole);
        }

        /// <summary>
        /// Fetches the advisors of the activity whose activity code is specified by the parameter.
        /// </summary>
        /// <param name="activityCode">The activity code</param>
        /// <returns>An IEnumerable of the matching MembershipView objects</returns>
        public IEnumerable<MembershipView> GetAdvisorMembershipsForActivity(string activityCode)
        {

            var advisorRole = Helpers.GetAdvisorRoleCodes();
            return _context.MembershipView.Where(m => m.ActivityCode == activityCode && m.Participation == advisorRole);
        }

        /// <summary>
        /// Fetches all the membership information linked to the student whose id appears as a parameter.
        /// </summary>
        /// <param name="username">The student's AD Username</param>
        /// <returns>An IEnumerable of the matching MembershipView objects</returns>
        public IEnumerable<MembershipView> GetMembershipsByUser(string username)
        {
            var account = _accountService.GetAccountByUsername(username);

            return _context.MembershipView.Where(m => m.Username == account.ADUserName).OrderByDescending(x => x.StartDate);
        }

        /// <summary>
        /// Fetches the number of memberships associated with the activity whose code is specified by the parameter.
        /// </summary>
        /// <param name="activityCode">The activity code</param>
        /// <param name="sessionCode">The session code</param>
        /// <returns>The count of current members for the activity</returns>
        public int GetActivityMembersCount(string activityCode, string? sessionCode = null)
        {
            sessionCode ??= Helpers.GetCurrentSession(_context);
            return _context.MembershipView.Where(m => m.ActivityCode == activityCode && m.Participation != ParticipationType.Guest.Value && m.SessionCode == sessionCode).Count();
        }

        /// <summary>
        /// Fetches the number of followers associated with the activity and session whose codes are specified by the parameter.
        /// </summary>
        /// <param name="activityCode">The activity code</param>
        /// <param name="sessionCode">The session code</param>
        /// <returns>The count of current guests (aka subscribers) for the activity</returns>
        public int GetActivitySubscribersCountForSession(string activityCode, string? sessionCode = null)
        {
            sessionCode ??= Helpers.GetCurrentSession(_context);
            return _context.MembershipView.Where(m => m.ActivityCode == activityCode && m.Participation == ParticipationType.Guest.Value && m.SessionCode == sessionCode).Count();
        }

        /// <summary>
        /// Fetches the number of memberships associated with the activity and session whose codes are specified by the parameter.
        /// </summary>
        /// <param name="activityCode">The activity code.</param>
        /// <param name="sessionCode">The session code</param>
        /// <returns>The count of current members for the activity</returns>
        public int GetActivityMembersCountForSession(string activityCode, string? sessionCode = null)
        {
            sessionCode ??= Helpers.GetCurrentSession(_context);
            return _context.MembershipView.Where(m => m.ActivityCode == activityCode && m.Participation != ParticipationType.Guest.Value && m.SessionCode == sessionCode).Count();
        }

        /// <summary>
        /// Updates the membership whose id is given as the first parameter to the contents of the second parameter.
        /// </summary>
        /// <param name="membershipID">The id of the membership to update</param>
        /// <param name="membership">The updated membership</param>
        /// <returns>The newly modified membership as a MembershipView object</returns>
        public async Task<MembershipView> UpdateAsync(int membershipID, MembershipUploadViewModel membership)
        {
            var original = await _context.MEMBERSHIP.FirstOrDefaultAsync(m => m.MEMBERSHIP_ID == membershipID);
            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Membership was not found." };
            }

            // One can only update certain fields within a membrship
            original.COMMENT_TXT = membership.CommentText;
            original.PART_CDE = membership.Participation;

            if (membership.Participation == ParticipationType.Guest.Value)
            {
                await SetGroupAdminAsync(membershipID, false);
            }

            await _context.SaveChangesAsync();

            return GetMembershipViewById(original.MEMBERSHIP_ID);

        }
        /// <summary>
        /// Switches the group-admin property of the person whose membership id is given
        /// </summary>
        /// <param name="membershipID">The corresponding membership object</param>
        /// <param name="isGroupAdmin">The new value of group admin</param>
        /// <returns>The newly modified membership as a MembershipView object</returns>
        public async Task<MembershipView> SetGroupAdminAsync(int membershipID, bool isGroupAdmin)
        {
            var original = await _context.MEMBERSHIP.FirstOrDefaultAsync(m => m.MEMBERSHIP_ID == membershipID);
            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Membership was not found." };
            }

            if (original.PART_CDE == "GUEST" && isGroupAdmin)
                throw new ArgumentException("A guest cannot be assigned as an admin.", "Participation Level");

            original.GRP_ADMIN = isGroupAdmin;

            await _context.SaveChangesAsync();

            return GetMembershipViewById(original.MEMBERSHIP_ID);
        }

        /// <summary>
        /// Switches the privacy property of the person whose membership id is given
        /// </summary>
        /// <param name="membershipID">The membership object passed</param>
        /// <param name="isPrivate">The new value of privacy</param>
        /// <returns>The newly modified membership as a MembershipView object</returns>
        public async Task<MembershipView> SetPrivacyAsync(int membershipID, bool isPrivate)
        {
            var original = await _context.MEMBERSHIP.FirstOrDefaultAsync(m => m.MEMBERSHIP_ID == membershipID);
            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Membership was not found." };
            }

            original.PRIVACY = isPrivate;

            await _context.SaveChangesAsync();

            return GetMembershipViewById(original.MEMBERSHIP_ID);
        }


        /// <summary>
        /// Helper method to Validate a membership
        /// </summary>
        /// <param name="membership">The membership to validate</param>
        /// <returns>True if the membership is valid. Throws an Exception if not. Exception is caught in an Exception Filter</returns>
        public bool ValidateMembership(MembershipUploadViewModel membership)
        {
            var personExists = _accountService.GetAccountByUsername(membership.Username);
            if (personExists != null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Person was not found." };
            }
            var participationExists = _context.PART_DEF.Any(x => x.PART_CDE.Trim() == membership.Participation);
            if (!participationExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Participation was not found." };
            }
            var sessionExists = _context.CM_SESSION_MSTR.Any(x => x.SESS_CDE.Trim() == membership.Session);
            if (!sessionExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Session was not found." };
            }
            var activityExists = _context.ACT_INFO.Any(x => x.ACT_CDE.Trim() == membership.Activity);
            if (!activityExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
            }

            if (!_context.InvolvementOffering.Any(i => i.SessionCode == membership.Session && i.ActivityCode.Trim() == membership.Activity))
            {
                throw new BadInputException() { ExceptionMessage = "The Activity is not available for this session." };
            }

            return true;
        }

        public bool IsPersonAlreadyInActivity(MembershipUploadViewModel membershipRequest)
        {
            var personAlreadyInActivity = _context.MembershipView.Any(x => x.SessionCode == membershipRequest.Session &&
                x.ActivityCode == membershipRequest.Activity && x.Username == membershipRequest.Username);

            if (personAlreadyInActivity)
            {
                throw new ResourceCreationException() { ExceptionMessage = "The Person is already part of the activity." };
            }

            return true;
        }

        /// <summary>	
        /// Determines whether or not the given user is a Group Admin of some activity	
        /// </summary>
        /// <param name="username">Username of the user to check</param>	
        /// <returns>true if student is a Group Admin, else false</returns>	
        public bool IsGroupAdmin(string username)
        {
            return _context.MembershipView.Any(membership => membership.Username == username && membership.GroupAdmin == true);
        }

        public IEnumerable<EmailViewModel> MembershipEmails(string activityCode, string sessionCode, ParticipationType? participationCode = null)
        {
            var memberships = _context.MembershipView.Where(m => m.ActivityCode == activityCode && m.SessionCode == sessionCode);

            if (participationCode != null)
            {
                if (participationCode == ParticipationType.GroupAdmin)
                {
                    memberships = memberships.Where(m => m.GroupAdmin == true);
                }
                else
                {
                    memberships = memberships.Where(m => m.Participation == participationCode.Value);
                }
            }

            return memberships.AsEnumerable().Select(m =>
            {
                var account = _accountService.GetAccountByUsername(m.Username);
                return new EmailViewModel
                {
                    Email = account.Email,
                    FirstName = account.FirstName,
                    LastName = account.LastName
                };
            });
        }

        public class ParticipationType
        {
            private ParticipationType(string value) { Value = value; }

            public string Value { get; private set; }

            public static ParticipationType Leader { get { return new ParticipationType("LEAD"); } }
            public static ParticipationType Guest { get { return new ParticipationType("GUEST"); } }
            public static ParticipationType Member { get { return new ParticipationType("MEMBR"); } }
            public static ParticipationType Advisor { get { return new ParticipationType("ADV"); } }

            // NOTE: Group admin is not strictly a participation type, it's a separate role that Advisors and Leaders can have, with a separate flag in the database
            // BUT, it's convenient to treat it as a participation type in several places throughout the API
            public static ParticipationType GroupAdmin { get { return new ParticipationType("GRP_ADMIN"); } }
        }

        /// <summary>	
        /// Finds the matching MembershipView object from an existing MEMBERSHIP object
        /// </summary>
        /// <param name="membershipId">The MEMBERSHIP to match on MembershipID</param>	
        /// <returns>The found MembershipView object corresponding to the MEMBERSHIP by ID</returns>	
        public MembershipView GetMembershipViewById(int membershipId)
        {
            var foundMembership = _context.MembershipView.FirstOrDefault(m => m.MembershipID == membershipId);

            if (foundMembership == null)
            {
                throw new ResourceNotFoundException();
            }

            return foundMembership;
        }
    }
}