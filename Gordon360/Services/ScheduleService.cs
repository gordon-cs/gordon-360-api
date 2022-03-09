using System;
using System.Collections.Generic;
using System.Linq;
using Gordon360.Models;
using Gordon360.Models.ViewModels;
using Gordon360.Repositories;
using Gordon360.Services.ComplexQueries;
using System.Data.SqlClient;
using System.Data;
using Gordon360.Exceptions.CustomExceptions;
using Gordon360.Static.Methods;
using System.Diagnostics;
using Gordon360.Database.CCT;
using System.Threading.Tasks;

namespace Gordon360.Services
{
    /// <summary>
    /// Service Class that facilitates data transactions between the SchedulesController and the Schedule part of the database model.
    /// </summary>
    public class ScheduleService : IScheduleService
    {
        private CCTContext _context;

        public ScheduleService(CCTContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Fetch the schedule item whose id and session code is specified by the parameter
        /// </summary>
        /// <param name="id">The id of the student</param>
        /// <returns>StudentScheduleViewModel if found, null if not found</returns>
        public async Task<IEnumerable<ScheduleViewModel>> GetScheduleStudent(string id)
        {
            var query = _context.ACCOUNT.FirstOrDefault(x => x.gordon_id == id);

            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Schedule was not found." };
            }


            var currentSession = await Helpers.GetCurrentSession();
            var result = await _context.Procedures.STUDENT_COURSES_BY_ID_NUM_AND_SESS_CDEAsync(int.Parse(id), currentSession.SessionCode);

            return (IEnumerable<ScheduleViewModel>)result;
        }


        /// <summary>
        /// Fetch the schedule item whose id and session code is specified by the parameter
        /// </summary>
        /// <param name="id">The id of the instructor</param>
        /// <returns>StudentScheduleViewModel if found, null if not found</returns>
        public async Task<IEnumerable<ScheduleViewModel>> GetScheduleFaculty(string id)
        {
            var query = _context.ACCOUNT.FirstOrDefault(x => x.gordon_id == id);
            //var currentSessionCode = Helpers.GetCurrentSession().SessionCode;
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Schedule was not found." };
            }

            var currentSession = await Helpers.GetCurrentSession();
            var result = await _context.Procedures.INSTRUCTOR_COURSES_BY_ID_NUM_AND_SESS_CDEAsync(int.Parse(id), currentSession.SessionCode);

            return (IEnumerable<ScheduleViewModel>)result;
        }

        /// <summary>
        /// Determine whether the specified user can read student schedules, based on the logic in the stored procedure
        /// </summary>
        /// <param name="username">The username to determine it for</param>
        /// <returns>Whether the specified user can read student schedules</returns>
        public bool CanReadStudentSchedules(string username)
        {
            var usernameParam = new SqlParameter("@username", username);

            return RawSqlQuery<int>.query("CAN_READ_STUDENT_SCHEDULES @username", usernameParam).FirstOrDefault() == 1;
        }
    }
}