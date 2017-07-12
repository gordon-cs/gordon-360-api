using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services.ComplexQueries;
using System.Data.SqlClient;
using System.Data;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Static.Methods;
using System.Diagnostics;

namespace Gordon360.Services
{
    /// <summary>
    /// Service Class that facilitates data transactions between the MembershipsController and the Membership database model.
    /// </summary>
    public class MembershipService : IMembershipService
    {
        private IUnitOfWork _unitOfWork;

        public MembershipService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            var sessionCode = _unitOfWork.SessionRepository.Find(x => x.SESS_CDE.Equals(membership.SESS_CDE)).FirstOrDefault();
            membership.BEGIN_DTE = (DateTime) sessionCode.SESS_BEGN_DTE;

            // The Add() method returns the added membership.
            var payload = _unitOfWork.MembershipRepository.Add(membership);

            // There is a unique constraint in the Database on columns (ID_NUM, PART_LVL, SESS_CDE and ACT_CDE)
            if (payload == null)
            {
                throw new ResourceCreationException() { ExceptionMessage = "There was an error creating the membership. Verify that a similar membership doesn't already exist." };
            }
            _unitOfWork.Save();

            return payload;

        }

        /// <summary>
        /// Delete the membership whose id is specified by the parameter.
        /// </summary>
        /// <param name="id">The membership id</param>
        /// <returns>The membership that was just deleted</returns>
        public MEMBERSHIP Delete(int id)
        {
            var result = _unitOfWork.MembershipRepository.GetById(id);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Membership was not found." };
            }
            result = _unitOfWork.MembershipRepository.Delete(result);

            _unitOfWork.Save();

            return result;
        }

        /// <summary>
        /// Fetch the membership whose id is specified by the parameter
        /// </summary>
        /// <param name="id">The membership id</param>
        /// <returns>MembershipViewModel if found, null if not found</returns>
        public MembershipViewModel Get(int id)
        {
            var query = _unitOfWork.MembershipRepository.GetById(id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Membership was not found." };
            }

            var idParam = new SqlParameter("@MEMBERSHIP_ID", id);
            var result = RawSqlQuery<MembershipViewModel>.query("MEMBERSHIPS_PER_MEMBERSHIP_ID @MEMBERSHIP_ID", idParam).FirstOrDefault();

            if (result == null)
            {
                return null;
            }
            // Getting rid of database-inherited whitespace
            result.ActivityCode = result.ActivityCode.Trim();
            result.ActivityDescription = result.ActivityDescription.Trim();
            result.SessionCode = result.SessionCode.Trim();
            result.SessionDescription = result.SessionDescription.Trim();
            result.IDNumber = result.IDNumber;
            result.FirstName = result.FirstName.Trim();
            result.LastName = result.LastName.Trim();
            result.Participation = result.Participation.Trim();
            result.ParticipationDescription = result.ParticipationDescription.Trim();

            return result;
        }

        /// <summary>
        /// Fetches all membership records from storage.
        /// </summary>
        /// <returns>MembershipViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public IEnumerable<MembershipViewModel> GetAll()
        {

            var result = RawSqlQuery<MembershipViewModel>.query("ALL_MEMBERSHIPS");
            // Trimming database generated whitespace ._.
            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.ActivityCode = x.ActivityCode.Trim();
                trim.ActivityDescription = x.ActivityDescription.Trim();
                trim.SessionCode = x.SessionCode.Trim();
                trim.SessionDescription = x.SessionDescription.Trim();
                trim.IDNumber = x.IDNumber;
                trim.FirstName = x.FirstName.Trim();
                trim.LastName = x.LastName.Trim();
                trim.Participation = x.Participation.Trim();
                trim.ParticipationDescription = x.ParticipationDescription.Trim();
                return trim;
            });
            return trimmedResult.OrderByDescending(x => x.StartDate);
        }

        /// <summary>
        /// Fetches the group admin (who have edit privileges of the page) of the activity whose activity code is specified by the parameter.
        /// </summary>
        /// <param name="id">The activity code.</param>
        /// <returns>MembershipViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public IEnumerable<MembershipViewModel> GetGroupAdminMembershipsForActivity(string id)
        {
            var idParam = new SqlParameter("@ACT_CDE", id);
            var result = RawSqlQuery<MembershipViewModel>.query("MEMBERSHIPS_PER_ACT_CDE @ACT_CDE", idParam);

            // Filter group admin
            result = result.Where(x => x.GroupAdmin.HasValue && x.GroupAdmin.Value == true);
            // Getting rid of whitespace inherited from the database .__.
            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.ActivityCode = x.ActivityCode.Trim();
                trim.ActivityDescription = x.ActivityDescription.Trim();
                trim.SessionCode = x.SessionCode.Trim();
                trim.SessionDescription = x.SessionDescription.Trim();
                trim.IDNumber = x.IDNumber;
                trim.FirstName = x.FirstName.Trim();
                trim.LastName = x.LastName.Trim();
                trim.Participation = x.Participation.Trim();
                trim.ParticipationDescription = x.ParticipationDescription.Trim();
                return trim;
            });

            return trimmedResult;

        }

        /// <summary>
        /// Fetches the leaders of the activity whose activity code is specified by the parameter.
        /// </summary>
        /// <param name="id">The activity code.</param>
        /// <returns>MembershipViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public IEnumerable<MembershipViewModel> GetLeaderMembershipsForActivity(string id)
        {
            var idParam = new SqlParameter("@ACT_CDE", id);
            var result = RawSqlQuery<MembershipViewModel>.query("MEMBERSHIPS_PER_ACT_CDE @ACT_CDE", idParam);
            // Filter leaders
            
            var leaderRoles = Helpers.GetLeaderRoleCodes();
            result = result.Where(x => leaderRoles == x.Participation.Trim());

            // Getting rid of whitespace inherited from the database .__.
            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.ActivityCode = x.ActivityCode.Trim();
                trim.ActivityDescription = x.ActivityDescription.Trim();
                trim.SessionCode = x.SessionCode.Trim();
                trim.SessionDescription = x.SessionDescription.Trim();
                trim.IDNumber = x.IDNumber;
                trim.FirstName = x.FirstName.Trim();
                trim.LastName = x.LastName.Trim();
                trim.Participation = x.Participation.Trim();
                trim.ParticipationDescription = x.ParticipationDescription.Trim();
                return trim;
            });
            
            return trimmedResult;
        }

        /// <summary>
        /// Fetches the advisors of the activity whose activity code is specified by the parameter.
        /// </summary>
        /// <param name="id">The activity code.</param>
        /// <returns>MembershipViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public IEnumerable<MembershipViewModel> GetAdvisorMembershipsForActivity(string id)
        {
            var idParam = new SqlParameter("@ACT_CDE", id);
            var result = RawSqlQuery<MembershipViewModel>.query("MEMBERSHIPS_PER_ACT_CDE @ACT_CDE", idParam);
            // Filter advisors

            var advisorRole = Helpers.GetAdvisorRoleCodes();
            result = result.Where(x => advisorRole == x.Participation.Trim());

            // Getting rid of whitespace inherited from the database .__.
            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.ActivityCode = x.ActivityCode.Trim();
                trim.ActivityDescription = x.ActivityDescription.Trim();
                trim.SessionCode = x.SessionCode.Trim();
                trim.SessionDescription = x.SessionDescription.Trim();
                trim.IDNumber = x.IDNumber;
                trim.FirstName = x.FirstName.Trim();
                trim.LastName = x.LastName.Trim();
                trim.Participation = x.Participation.Trim();
                trim.ParticipationDescription = x.ParticipationDescription.Trim();
                return trim;
            });

            return trimmedResult;
        }

        /// <summary>
        /// Fetches the memberships associated with the activity whose code is specified by the parameter.
        /// </summary>
        /// <param name="id">The activity code.</param>
        /// <returns>MembershipViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public IEnumerable<MembershipViewModel> GetMembershipsForActivity(string id)
        {
            var idParam = new SqlParameter("@ACT_CDE", id);
            var result = RawSqlQuery<MembershipViewModel>.query("MEMBERSHIPS_PER_ACT_CDE @ACT_CDE", idParam);
            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.ActivityCode = x.ActivityCode.Trim();
                trim.ActivityDescription = x.ActivityDescription.Trim();
                trim.SessionCode = x.SessionCode.Trim();
                trim.SessionDescription = x.SessionDescription.Trim();
                trim.IDNumber = x.IDNumber;
                trim.FirstName = x.FirstName.Trim();
                trim.LastName = x.LastName.Trim();
                trim.Participation = x.Participation.Trim();
                trim.ParticipationDescription = x.ParticipationDescription.Trim();
                trim.GroupAdmin = x.GroupAdmin;
                return trim;
            });
            return trimmedResult.OrderByDescending(x => x.StartDate);
        }

        /// <summary>
        /// Fetches all the membership information linked to the student whose id appears as a parameter.
        /// </summary>
        /// <param name="id">The student id.</param>
        /// <returns>A MembershipViewModel IEnumerable. If nothing is found, an empty IEnumberable is returned.</returns>
        public IEnumerable<MembershipViewModel> GetMembershipsForStudent(string id)
        {
            var studentExists = _unitOfWork.AccountRepository.Where(x => x.gordon_id.Trim() == id).Count() > 0;
            if (!studentExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
            }

            var idParam = new SqlParameter("@STUDENT_ID", id);
            var result = RawSqlQuery<MembershipViewModel>.query("MEMBERSHIPS_PER_STUDENT_ID @STUDENT_ID", idParam);
            
            // The Views that were given to were defined as char(n) instead of varchar(n)
            // They return with whitespace padding which messes up comparisons later on.
            // I'm using the IEnumerable Select method here to help get rid of that.
            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.ActivityCode = x.ActivityCode.Trim();
                trim.ActivityDescription = x.ActivityDescription.Trim();
                trim.SessionCode = x.SessionCode.Trim();
                trim.SessionDescription = x.SessionDescription.Trim();
                trim.Participation = x.Participation.Trim();
                trim.ParticipationDescription = x.ParticipationDescription.Trim();
                trim.FirstName = x.FirstName.Trim();
                trim.LastName = x.LastName.Trim();
                trim.IDNumber = x.IDNumber;
                return trim;
            });

            return trimmedResult.OrderByDescending(x => x.StartDate);
        }

        /// <summary>
        /// Fetches the number of followers associated with the activity whose code is specified by the parameter.
        /// </summary>
        /// <param name="id">The activity code.</param>
        /// <returns>int.</returns>
        public int GetActivityFollowersCount(string id)
        {
            var idParam = new SqlParameter("@ACT_CDE", id);
            var result = RawSqlQuery<MembershipViewModel>.query("MEMBERSHIPS_PER_ACT_CDE @ACT_CDE", idParam);

            return result.Where(x => x.Participation == "GUEST").Count();
        }

        /// <summary>
        /// Fetches the number of memberships associated with the activity whose code is specified by the parameter.
        /// </summary>
        /// <param name="id">The activity code.</param>
        /// <returns>int.</returns>
        public int GetActivityMembersCount(string id)
        {
            var idParam = new SqlParameter("@ACT_CDE", id);
            var result = RawSqlQuery<MembershipViewModel>.query("MEMBERSHIPS_PER_ACT_CDE @ACT_CDE", idParam);

            return result.Where(x => x.Participation != "GUEST").Count();
        }

        /// <summary>
        /// Fetches the number of followers associated with the activity and session whose codes are specified by the parameter.
        /// </summary>
        /// <param name="id">The activity code.</param>
        /// <param name="sess_cde">The session code</param>
        /// <returns>int.</returns>
        public int GetActivityFollowersCountForSession(string id, string sess_cde)
        {
            var idParam = new SqlParameter("@ACT_CDE", id);
            var result = RawSqlQuery<MembershipViewModel>.query("MEMBERSHIPS_PER_ACT_CDE @ACT_CDE", idParam);

            return result.Where(x => x.Participation == "GUEST" && x.SessionCode.Trim() == sess_cde).Count();
        }

        /// <summary>
        /// Fetches the number of memberships associated with the activity and session whose codes are specified by the parameter.
        /// </summary>
        /// <param name="id">The activity code.</param>
        /// <param name="sess_cde">The session code</param>
        /// <returns>int.</returns>
        public int GetActivityMembersCountForSession(string id, string sess_cde)
        {
            var idParam = new SqlParameter("@ACT_CDE", id);
            var result = RawSqlQuery<MembershipViewModel>.query("MEMBERSHIPS_PER_ACT_CDE @ACT_CDE", idParam);

            return result.Where(x => x.Participation != "GUEST" && x.SessionCode.Trim() == sess_cde).Count();
        }

        /// <summary>
        /// Updates the membership whose id is given as the first parameter to the contents of the second parameter.
        /// </summary>
        /// <param name="id">The membership id.</param>
        /// <param name="membership">The updated membership.</param>
        /// <returns>The newly modified membership.</returns>
        public MEMBERSHIP Update(int id, MEMBERSHIP membership)
        {
            var original = _unitOfWork.MembershipRepository.GetById(id);
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

            _unitOfWork.Save();

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
            var original = _unitOfWork.MembershipRepository.GetById(id);
            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Membership was not found." };
            }

            validateMembership(membership);

            var isGuest = original.PART_CDE == "GUEST";

            if (isGuest)
                throw new ArgumentException("A guest cannot be assigned as an admin.", "Participation Level" );

            var isAdmin = original.GRP_ADMIN ?? false;
            if (!isAdmin)
                original.GRP_ADMIN = true;
            else
                original.GRP_ADMIN = false;

            _unitOfWork.Save();

            return original;
        }


        /// <summary>
        /// Helper method to Validate a membership
        /// </summary>
        /// <param name="membership">The membership to validate</param>
        /// <returns>True if the membership is valid. Throws ResourceNotFoundException if not. Exception is cauth in an Exception Filter</returns>
        private bool validateMembership(MEMBERSHIP membership)
        {
            var personExists = _unitOfWork.AccountRepository.Where(x => x.gordon_id.Trim() == membership.ID_NUM.ToString()).Count() > 0;
            if (!personExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Person was not found." };
            }
            var participationExists = _unitOfWork.ParticipationRepository.Where(x => x.PART_CDE.Trim() == membership.PART_CDE).Count() > 0;
            if (!participationExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Participation was not found." };
            }
            var sessionExists = _unitOfWork.SessionRepository.Where(x => x.SESS_CDE.Trim() == membership.SESS_CDE).Count() > 0;
            if (!sessionExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Session was not found." };
            }
            var activityExists = _unitOfWork.ActivityRepository.Where(x => x.ACT_CDE.Trim() == membership.ACT_CDE).Count() > 0;
            if (!activityExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
            }

            var activitiesThisSession = RawSqlQuery<ActivityViewModel>.query("ACTIVE_CLUBS_PER_SESS_ID @SESS_CDE", new SqlParameter("SESS_CDE", SqlDbType.VarChar) { Value = membership.SESS_CDE });

            bool offered = false;
            foreach (var activityResult in activitiesThisSession)
            {
                if (activityResult.ACT_CDE.Trim() == membership.ACT_CDE)
                {
                    offered = true;
                }
            }

            if (!offered)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity is not available for this session." };
            }

            
            return true;
        }

        private bool isPersonAlreadyInActivity(MEMBERSHIP membershipRequest)
        {
            var personAlreadyInActivity = _unitOfWork.MembershipRepository.Where(x => x.SESS_CDE == membershipRequest.SESS_CDE &&
                x.ACT_CDE == membershipRequest.ACT_CDE && x.ID_NUM == membershipRequest.ID_NUM).Count() > 0;
            if (personAlreadyInActivity)
            {
                throw new ResourceCreationException() { ExceptionMessage = "The Person is already part of the activity." };
            }

            return true;
        }
    }
}