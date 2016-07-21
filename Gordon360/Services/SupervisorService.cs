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
using Gordon360.Exceptions.CustomExceptions;

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
            // validates the supervisor. See comments for validation in MembershipService and MembershipRequestService
            validateSupervisor(supervisor);

            // The Add() method returns the added supervisor.
            var payload = _unitOfWork.SupervisorRepository.Add(supervisor);
            if (payload == null)
            {
                throw new ResourceCreationException() { ExceptionMessage = "There was an error creating the supervisor. Verify that a similar supervisor doesn't already exist." };
            }
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
                throw new ResourceNotFoundException() { ExceptionMessage = "The Supervisor was not found." };
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
                throw new ResourceNotFoundException() { ExceptionMessage = "The Supervisor was not found." };
            }

            var idParam = new SqlParameter("@SUP_ID", id);
            var result = RawSqlQuery<SupervisorViewModel>.query("SUPERVISOR_PER_SUP_ID @SUP_ID", idParam).FirstOrDefault();

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


            return result;
        }

        /// <summary>
        /// Fetches the supervisors of the activity whose activity code is specified by the parameter.
        /// </summary>
        /// <param name="id">The activity code.</param>
        /// <returns>SupervisorViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public IEnumerable<SupervisorViewModel> GetSupervisorsForActivity(string id)
        {
           
            var idParam = new SqlParameter("@ACT_CDE", id);
            var result = RawSqlQuery<SupervisorViewModel>.query("SUPERVISORS_PER_ACT_CDE @ACT_CDE", idParam);
            // No trimming here because we made the Supervisor Table, and we made sure to use varchar(n).
            return result;
        }

        /// <summary>
        /// Fetches all supervisor records from storage.
        /// </summary>
        /// <returns>SupervisorViewModel IEnumerable. If no records were found, an empty IEnumerable is returned.</returns>
        public IEnumerable<SupervisorViewModel> GetAll()
        {

            var result = RawSqlQuery<SupervisorViewModel>.query("ALL_SUPERVISORS");

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
                return trim;
            });
            return trimmedResult;
        }

        public SUPERVISOR Update(int id, SUPERVISOR supervisor)
        {
            var original = _unitOfWork.SupervisorRepository.GetById(id);
            if (original == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Supervisor was not found." };
            }

            validateSupervisor(supervisor);

            original.SUP_ID = supervisor.SUP_ID;
            original.ACT_CDE = supervisor.ACT_CDE;
            original.SESS_CDE = supervisor.SESS_CDE;
            original.ID_NUM = supervisor.ID_NUM;

            _unitOfWork.Save();

            return original;
        }

        /// <summary>
        /// Helper method to check for the validity of a supervisor.
        /// </summary>
        /// <param name="supervisor">The supervisor to validate</param>
        /// <returns>True if the supervisor is valid. False if it isn't</returns>
        private bool validateSupervisor(SUPERVISOR supervisor)
        {
            var personExists = _unitOfWork.AccountRepository.Where(x => x.gordon_id == supervisor.ID_NUM.ToString()).Count() > 0;
            if (!personExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Person was not found." };
            }
            var sessionExists = _unitOfWork.SessionRepository.Where(x => x.SESS_CDE.Trim() == supervisor.SESS_CDE).Count() > 0;
            if (!sessionExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Session was not found." };
            }
            var activityExists = _unitOfWork.ActivityRepository.Where(x => x.ACT_CDE.Trim() == supervisor.ACT_CDE).Count() > 0;
            if (!activityExists)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity was not found." };
            }

            var activitiesThisSession = RawSqlQuery<ACT_CLUB_DEF>.query("ACTIVE_CLUBS_PER_SESS_ID @SESS_CDE", new SqlParameter("SESS_CDE", SqlDbType.VarChar) { Value = supervisor.SESS_CDE });

            bool offered = false;
            foreach (var activityResult in activitiesThisSession)
            {
                if (activityResult.ACT_CDE.Trim() == supervisor.ACT_CDE)
                {
                    offered = true;
                    break;
                }
            }

            if (!offered)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Activity is not available for this session." };
            }
            return true;
        }
    }
}