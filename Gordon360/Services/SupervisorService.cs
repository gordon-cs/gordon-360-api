using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using System.Data.SqlClient;
using System.Data;
using Gordon360.Services.ComplexQueries;

namespace Gordon360.Services
{
    /// <summary>
    /// Service class to facilitate data transactions between the controller and the database models.
    /// </summary>
    public class SupervisorService : ISupervisorService
    {
        private IUnitOfWork _unitOfWork;

        public SupervisorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Adds a new Supervisor record to storage. Since we can't establish foreign key constraints and relationships on the database side,
        /// we do it here by using the supervisorIsValid() method.
        /// </summary>
        /// <param name="supervisor">The supervisor to be added</param>
        /// <returns>The newly added SUPERVISOR object</returns>
        public SUPERVISOR Add(SUPERVISOR supervisor)
        {
            // supervisorIsValid() returns a boolean value.
            var isValidSupervisor = supervisorIsValid(supervisor);

            if (!isValidSupervisor)
            {
                return null;
            }

            // The Add() method returns the added supervisor.
            var payload = _unitOfWork.SupervisorRepository.Add(supervisor);
            _unitOfWork.Save();

            return payload;
        }

        /// <summary>
        /// Delete the supervisor whose id is specified by the parameter.
        /// </summary>
        /// <param name="id">The supervisor id</param>
        /// <returns>The supervisor that was just deleted</returns>
        public SUPERVISOR Delete(int id)
        {
            var result = _unitOfWork.SupervisorRepository.GetById(id);
            if (result == null)
            {
                // Controller checks for nulls and returns NotFound() when it sees one returned by a service.
                return null;
            }
            result = _unitOfWork.SupervisorRepository.Delete(result);

            _unitOfWork.Save();

            return result;
        }

        /// <summary>
        /// Fetch the supervisor whose id is specified by the parameter
        /// </summary>
        /// <param name="id">The supervisor id</param>
        /// <returns>SupervisorViewModel if found, null if not found</returns>
        public SupervisorViewModel Get(int id)
        {
            var query = _unitOfWork.SupervisorRepository.GetById(id);
            if (query == null)
            {
                return null;
            }

            var rawsqlquery = Constants.getSupervisorByIdQuery;
            var result = RawSqlQuery<SupervisorViewModel>.query(rawsqlquery, id).FirstOrDefault();

            if (result == null)
            {
                return null;
            }
            // Getting rid of database-inherited whitespace
            result.ActivityCode = result.ActivityCode.Trim();
            result.ActivityDescription = result.ActivityDescription.Trim();
            result.SessionCode = result.SessionCode.Trim();
            result.SessionDescription = result.SessionDescription.Trim();
            result.IDNumber = result.IDNumber.Trim();
            result.FirstName = result.FirstName.Trim();
            result.LastName = result.LastName.Trim();


            return result;
        }

        /// <summary>
        /// Fetches the supervisors of the activity whose activity code is specified by the parameter.
        /// </summary>
        /// <param name="id">The activity code.</param>
        /// <returns>SupervisorViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public IEnumerable<SupervisorViewModel> GetSupervisorsForActivity(string id)
        {
            var rawsqlquery = Constants.getSupervisorsForActivityQuery;
            var result = RawSqlQuery<SupervisorViewModel>.query(rawsqlquery, id);
            // No trimming here because we made the Supervisor Table, and we made sure to use varchar(n).
            return result;
        }

        /// <summary>
        /// Fetches all supervisor records from storage.
        /// </summary>
        /// <returns>SupervisorViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public IEnumerable<SupervisorViewModel> GetAll()
        {
            var rawsqlquery = Constants.getAllSupervisorsQuery;
            var result = RawSqlQuery<SupervisorViewModel>.query(rawsqlquery);
            // Trimming database generated whitespace ._.
            var trimmedResult = result.Select(x =>
            {
                var trim = x;
                trim.ActivityCode = x.ActivityCode.Trim();
                trim.ActivityDescription = x.ActivityDescription.Trim();
                trim.SessionCode = x.SessionCode.Trim();
                trim.SessionDescription = x.SessionDescription.Trim();
                trim.IDNumber = x.IDNumber.Trim();
                trim.FirstName = x.FirstName.Trim();
                trim.LastName = x.LastName.Trim();
                return trim;
            });
            return trimmedResult;
        }

        public SUPERVISOR Update(int id, SUPERVISOR supervisor)
        {
            var original = _unitOfWork.SupervisorRepository.GetById(id);
            if (original == null)
            {
                return null;
            }

            var isValidSupervisor = supervisorIsValid(supervisor);

            if (!isValidSupervisor)
            {
                return null;
            }
            original.SUP_ID = supervisor.SUP_ID;
            original.ACT_CDE = supervisor.ACT_CDE;
            original.SESSION_CDE = supervisor.SESSION_CDE;
            original.ID_NUM = supervisor.ID_NUM;

            _unitOfWork.Save();

            return original;
        }

        /// <summary>
        /// Helper method to check for the validity of a supervisor.
        /// </summary>
        /// <param name="supervisor">The supervisor to validate</param>
        /// <returns>True if the supervisor is valid. False if it isn't</returns>
        private bool supervisorIsValid(SUPERVISOR supervisor)
        {
            var personExists = _unitOfWork.AccountRepository.Where(x => x.gordon_id.Trim() == supervisor.ID_NUM).Count() > 0;
            var sessionExists = _unitOfWork.SessionRepository.Where(x => x.SESS_CDE.Trim() == supervisor.SESSION_CDE).Count() > 0;
            var activityExists = _unitOfWork.ActivityRepository.Where(x => x.ACT_CDE.Trim() == supervisor.ACT_CDE).Count() > 0;

            if (!personExists || !sessionExists || !activityExists)
            {
                return false;
            }

            var activitiesThisSession = _unitOfWork.ActivityPerSessionRepository.ExecWithStoredProcedure("ACTIVE_CLUBS_PER_SESS_ID @SESS_CDE", new SqlParameter("SESS_CDE", SqlDbType.VarChar) { Value = supervisor.SESSION_CDE });

            bool offered = false;
            foreach (var activityResult in activitiesThisSession)
            {
                if (activityResult.ACT_CDE.Trim() == supervisor.ACT_CDE)
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