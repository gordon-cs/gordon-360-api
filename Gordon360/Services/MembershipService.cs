using Gordon360.Authorization;
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
using Gordon360.Enums;
using Gordon360.Extensions.System;

namespace Gordon360.Services
{
    /// <summary>
    /// Service Class that facilitates data transactions between the MembershipsController and the Membership database model.
    /// </summary>
    public class MembershipService : IMembershipService
    {
        private readonly CCTContext _context;
        private readonly IAccountService _accountService;
        private readonly IActivityService _activityService;

        public MembershipService(CCTContext context, IAccountService accountService, IActivityService activityService)
        {
            _context = context;
            _accountService = accountService;
            _activityService = activityService;
        }

        /// <summary>
        /// Fetches the memberships associated with the activity whose code is specified by the parameter.
        /// </summary>
        /// <param name="activityCode">Optional activity code filter</param>
        /// <param name="username">Optional username filter</param>
        /// <param name="sessionCode">Optional session code, defaults to current session. Use "*" for all sessions</param>
        /// <param name="participationTypes">Optional filter for involvement participation types (MEMBR, ADV, LEAD, GUEST, GRP_ADMIN)</param>
        /// <returns>An IEnumerable of the matching MembershipView objects</returns>
        public IEnumerable<MembershipView> GetMemberships(
            string? activityCode = null,
            string? username = null,
            string? sessionCode = null,
            List<string>? participationTypes = null
        )
        {
            IQueryable<MembershipView> memberships = _context.MembershipView;
            if (username is not null) memberships = memberships.Where(m => EF.Functions.Like(m.Username, username));

            if (activityCode is not null) memberships = memberships.Where(m => m.ActivityCode == activityCode);

            // Null sessionCode defaults to current session
            sessionCode ??= Helpers.GetCurrentSession(_context);
            // session code "*" means all sessions
            if (sessionCode != "*")
            {
                memberships = memberships.Where(m => m.SessionCode.Trim() == sessionCode);
            }

            if (participationTypes?.Count > 0)
            {
                var groupAdmin = Participation.GroupAdmin.GetCode();
                var includesGroupAdmin = participationTypes.Contains(groupAdmin) == true;
                if (includesGroupAdmin) participationTypes.Remove(groupAdmin);

                memberships = memberships.Where(m => participationTypes.Contains(m.Participation) || (includesGroupAdmin && m.GroupAdmin == true));
            }

            return memberships.OrderByDescending(m => m.StartDate);
        }

        /// <summary>	
        /// Fetch the membership whose id is specified by the parameter	
        /// </summary>	
        /// <param name="membershipID">The membership id</param>	
        /// <returns>The found membership as a MembershipView</returns>	
        public MembershipView GetSpecificMembership(int membershipID)
        {
            var result = _context.MembershipView.FirstOrDefault(m => m.MembershipID == membershipID);

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Membership was not found." };
            }

            return result;
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
            }
            else
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

            if (membership.Participation == Participation.Guest.GetCode())
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
            if (personExists == null)
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

        /// <summary>
        /// Filters out memberships that are private with respect to the given viewer
        /// </summary>
        /// <param name="memberships">Enumerable of memberships to filter</param>
        /// <param name="viewerUsername">username of viewer</param>
        /// <returns>The membership enumerable with private memberships removed</returns>
        public IEnumerable<MembershipView> RemovePrivateMemberships(IEnumerable<MembershipView> memberships, string viewerUsername)
            => memberships.Where(m =>
                {
                    var act = _activityService.Get(m.ActivityCode);
                    var isPublic = !(act.Privacy == true || m.Privacy == true);
                    if (isPublic)
                    {
                        return true;
                    }
                    else
                    {
                        // If the current authenticated user is an admin of this group, then include the membership
                        return GetMemberships(
                            activityCode: m.ActivityCode,
                            username: viewerUsername,
                            sessionCode: m.SessionCode)
                        .Any(m => m.Participation != Participation.Guest.GetCode());
                    }
                });
    }
}