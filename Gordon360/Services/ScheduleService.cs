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
        /// <returns>StudentScheduleViewModel if found, null if not found</returns>
        public IEnumerable<ScheduleViewModel> GetScheduleStudent(string id)
        {
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            //var currentSessionCode = Helpers.GetCurrentSession().SessionCode;
            var currentSessionCode = "201901";
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Schedule was not found." };
            }
            var idInt = Int32.Parse(id);
            var idParam = new SqlParameter("@stu_num", idInt);
            var sessParam = new SqlParameter("@sess_cde", currentSessionCode);
            var result = RawSqlQuery<ScheduleViewModel>.query("STUDENT_COURSES_BY_ID_NUM_AND_SESS_CDE @stu_num, @sess_cde", idParam, sessParam);
            //var result = RawSqlQuery<ScheduleViewModel>.query("SELECT STUDENT_ID as ID_NUM, CRS_CDE, CRS_TITLE, BLDG_CDE, ROOM_CDE, MONDAY_CDE, TUESDAY_CDE, WEDNESDAY_CDE, THURSDAY_CDE, FRIDAY_CDE, BEGIN_TIME, END_TIME FROM TmsEPrd.dbo.GORD_CCT_COURSES WHERE yr_cde = 18 and trm_cde = 'SP' and Student_ID = 50153273");
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
        public IEnumerable<ScheduleViewModel> GetScheduleFaculty(string id)
        {
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            var currentSessionCode = Helpers.GetCurrentSession().SessionCode;
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Schedule was not found." };
            }
            var idInt = Int32.Parse(id);
            var idParam = new SqlParameter("@instructor_id", id);
            var sessParam = new SqlParameter("@sess_cde", currentSessionCode);
            var result = RawSqlQuery<ScheduleViewModel>.query("INSTRUCTOR_COURSES_BY_ID_NUM_AND_SESS_CDE @instructor_id, @sess_cde", idParam, sessParam); // TODO: write prepared statement

            if (result == null)
            {
                return null;
            }

            return result;
        }
    }
}