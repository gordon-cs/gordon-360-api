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
    /// Service Class that facilitates data transactions between the SchedulesController and the Schedule part of the database model.
    /// </summary>
    public class ScheduleService : IScheduleService
    {
        private IUnitOfWork _unitOfWork;

        public ScheduleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Fetch the schedule item whose id and session code is specified by the parameter
        /// </summary>
        /// <param name="id">The id of the student</param>
        /// <param name="session">The session id</param>
        /// <returns>StudentScheduleViewModel if found, null if not found</returns>
        public IEnumerable<ScheduleViewModel> GetScheduleStudent(int id, string session)
        {
            var query = _unitOfWork.StudentScheduleRepository.GetById(id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Schedule was not found." };
            }
            var idParam = new SqlParameter("@id_num", id);
            var sessParam = new SqlParameter("@sess_cde", session);
            var result = RawSqlQuery<ScheduleViewModel>.query("STUDENT_COURSES_BY_ID_NUM_AND_SESS_CDE @id_num @sess_cde", idParam, sessParam); // TODO: write prepared statement

            if (result == null)
            {
                return null;
            }

            return result;
        }


        /// <summary>
        /// Fetch the schedule item whose id and session code is specified by the parameter
        /// </summary>
        /// <param name="id">The id of the instructor</param>
        /// <param name="session">The session id</param>
        /// <returns>StudentScheduleViewModel if found, null if not found</returns>
        public IEnumerable<ScheduleViewModel> GetScheduleFaculty(int id, string session)
        {
            var query = _unitOfWork.FacultyScheduleRepository.GetById(id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Schedule was not found." };
            }
            var idParam = new SqlParameter("@id_num", id);
            var sessParam = new SqlParameter("@sess_cde", session);
            var result = RawSqlQuery<ScheduleViewModel>.query("INSTRUCTOR_COURSES_BY_ID_NUM_AND_SESS_CDE @id_num @sess_cde", idParam, sessParam); // TODO: write prepared statement

            if (result == null)
            {
                return null;
            }

            return result;
        }
    }
}