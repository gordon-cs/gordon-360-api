using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCT_App.Models;
using CCT_App.Models.ViewModels;
using CCT_App.Repositories;
using CCT_App.Services.ComplexQueries;
using System.Data.SqlClient;
using System.Data;

namespace CCT_App.Services
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
        /// we do it here by using the membershipIsValid() method.
        /// </summary>
        /// <param name="membership">The membership to be added</param>
        /// <returns></returns>
        public Membership Add(Membership membership)
        {
            // membershipIsValid() returns a boolean value.
            var isValidMembership = membershipIsValid(membership);
            
            if(!isValidMembership)
            {
                return null;
            }

            // The Add() method returns the added membership.
            var payload = _unitOfWork.MembershipRepository.Add(membership);
            _unitOfWork.Save();

            return payload;

        }

        /// <summary>
        /// Delete the membership whose id is specified by the parameter.
        /// </summary>
        /// <param name="id">The membership id</param>
        /// <returns>The membership that was just deleted</returns>
        public Membership Delete(int id)
        {
            var result = _unitOfWork.MembershipRepository.GetById(id);
            if (result == null)
            {
                // Controller checks for nulls and returns NotFound() when it sees one returned by a service.
                return null;
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
                return null;
            }

            var rawsqlquery = Constants.getMembershipByIDQuery;
            var result = RawSqlQuery<MembershipViewModel>.query(rawsqlquery, id).FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Fetches all membership records from storage.
        /// </summary>
        /// <returns>MembershipViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public IEnumerable<MembershipViewModel> GetAll()
        {
            var rawsqlquery = Constants.getAllMembershipsQuery;
            var result = RawSqlQuery<MembershipViewModel>.query(rawsqlquery);
            return result;
        }

        /// <summary>
        /// Updates the membership whose id is given as the first parameter to the contents of the second parameter.
        /// </summary>
        /// <param name="id">The membership id.</param>
        /// <param name="membership">The updated membership.</param>
        /// <returns>The newly modified membership.</returns>
        public Membership Update(int id, Membership membership)
        {
            var original = _unitOfWork.MembershipRepository.GetById(id);
            if (original == null)
            {
                return null;
            }

            var isValidMembership = membershipIsValid(membership);

            if(!isValidMembership)
            {
                return null;
            }

            original.ACT_CDE = membership.ACT_CDE;
            original.BEGIN_DTE = membership.BEGIN_DTE;
            original.DESCRIPTION = membership.DESCRIPTION;
            original.END_DTE = membership.END_DTE;
            original.ID_NUM = membership.ID_NUM;
            original.JOB_NAME = membership.JOB_NAME;
            original.JOB_TIME = membership.JOB_TIME;
            original.MEMBERSHIP_ID = membership.MEMBERSHIP_ID;
            original.PART_LVL = membership.PART_LVL;
            original.SESSION_CDE = membership.SESSION_CDE;
            original.USER_NAME = membership.USER_NAME;

            _unitOfWork.Save();

            return original;

        }

        /// <summary>
        /// Helper method to check for the validity of a membership.
        /// </summary>
        /// <param name="membership">The membership to validate</param>
        /// <returns>True if the membership is valid. False if it isn't</returns>
        private bool membershipIsValid(Membership membership)
        {
            var personExists = _unitOfWork.AccountRepository.Where(x => x.gordon_id.Trim() == membership.ID_NUM).Count() > 0;
            var participationExists = _unitOfWork.ParticipationRepository.Where(x => x.PART_CDE.Trim() == membership.PART_LVL).Count() > 0;
            var sessionExists = _unitOfWork.SessionRepository.Where(x => x.SESS_CDE.Trim() == membership.SESSION_CDE).Count() > 0;
            var activityExists = _unitOfWork.ActivityRepository.Where(x => x.ACT_CDE.Trim() == membership.ACT_CDE).Count() > 0;

            if (!personExists || !participationExists || !sessionExists || !activityExists)
            {
                return false;
            }

            var activitiesThisSession = _unitOfWork.ActivityPerSessionRepository.ExecWithStoredProcedure("ACTIVE_CLUBS_PER_SESS_ID @SESS_CDE", new SqlParameter("SESS_CDE", SqlDbType.VarChar) { Value = membership.SESSION_CDE });

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
                return false;
            }
            return true;
        }

    }
}