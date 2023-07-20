using Gordon360.Exceptions;
using Gordon360.Models.CCT;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.ViewModels;
using Gordon360.Static.Methods;
using Microsoft.Graph.CallRecords;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services
{
    /// <summary>
    /// Service Class that facilitates data transactions between the SchedulesController and the Schedule part of the database model.
    /// </summary>
    public class ScheduleService : IScheduleService
    {
        private readonly CCTContext _context;
        private readonly ISessionService _sessionService;

        public ScheduleService(CCTContext context)
        {
            _context = context;
            _sessionService = new SessionService(context);

        }

        /// <summary>
        /// Fetch the schedule item whose id and session code is specified by the parameter
        /// </summary>
        /// <param name="username">The AD Username of the student</param>
        /// <param name="sessionID">The selected session of the student</param>
        /// <returns>StudentScheduleViewModel if found, null if not found</returns>
        public async Task<IEnumerable<ScheduleViewModel>> GetScheduleStudentAsync(string username, string? sessionID = null)
        {
            var account = _context.ACCOUNT.FirstOrDefault(x => x.AD_Username == username);

            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Account was not found." };
            }

            var sessionCode = sessionID ?? Helpers.GetCurrentSession(_context);
            var result = await _context.Procedures.STUDENT_COURSES_BY_ID_NUM_AND_SESS_CDEAsync(int.Parse(account.gordon_id), sessionCode);

            return result.Select(x => new ScheduleViewModel
            {
                ID_NUM = x.ID_NUM,
                CRS_CDE = x.CRS_CDE,
                CRS_TITLE = x.CRS_TITLE,
                BLDG_CDE = x.BLDG_CDE,
                ROOM_CDE = x.ROOM_CDE,
                MONDAY_CDE = x.MONDAY_CDE,
                TUESDAY_CDE = x.TUESDAY_CDE,
                WEDNESDAY_CDE = x.WEDNESDAY_CDE,
                THURSDAY_CDE = x.THURSDAY_CDE,
                FRIDAY_CDE = x.FRIDAY_CDE,
                BEGIN_TIME = x.BEGIN_TIME,
                END_TIME = x.END_TIME
            });
        }


        /// <summary>
        /// Fetch the schedule item whose id and session code is specified by the parameter
        /// </summary>
        /// <param name="username">The AD Username of the instructor</param>
        /// <param name="sessionID">The selected session of the instructor</param>
        /// <returns>StudentScheduleViewModel if found, null if not found</returns>
        public async Task<IEnumerable<ScheduleViewModel>> GetScheduleFacultyAsync(string username, string? sessionID = null)
        {
            var account = _context.ACCOUNT.FirstOrDefault(x => x.AD_Username == username);
            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Schedule was not found." };
            }

            var sessionCode = sessionID ?? Helpers.GetCurrentSession(_context);
            var result = await _context.Procedures.INSTRUCTOR_COURSES_BY_ID_NUM_AND_SESS_CDEAsync(int.Parse(account.gordon_id), sessionCode);

            return result.Select(x => new ScheduleViewModel
            {
                ID_NUM = x.ID_NUM ?? default,
                CRS_CDE = x.CRS_CDE,
                CRS_TITLE = x.CRS_TITLE,
                BLDG_CDE = x.BLDG_CDE,
                ROOM_CDE = x.ROOM_CDE,
                MONDAY_CDE = x.MONDAY_CDE,
                TUESDAY_CDE = x.TUESDAY_CDE,
                WEDNESDAY_CDE = x.WEDNESDAY_CDE,
                THURSDAY_CDE = x.THURSDAY_CDE,
                FRIDAY_CDE = x.FRIDAY_CDE,
                BEGIN_TIME = x.BEGIN_TIME,
                END_TIME = x.END_TIME
            });
        }

        /// <summary>
        /// Fetch the session item whose id specified by the parameter
        /// </summary>
        /// <param name="username">The AD Username of the user</param>
        /// <returns>SessionCoursesViewModel if found, null if not found</returns>
        public async Task<IEnumerable<SessionCoursesViewModel>> GetAllCourses(string username)
        {
            var account = _context.ACCOUNT.FirstOrDefault(x => x.AD_Username == username);
            var allSessions = _sessionService.GetAll();
            var result = Enumerable.Empty<SessionCoursesViewModel>();

            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }


            foreach (SessionViewModel vm in allSessions)
            {
                result = result.Append(
                    new SessionCoursesViewModel
                    {
                        SessionCode = vm.SessionCode,
                        SessionDescription = vm.SessionDescription,
                        SessionBeginDate = vm.SessionBeginDate,
                        SessionEndDate = vm.SessionEndDate,
                        //The case for "ALUMNI" does not work at the moment currently, but it doesn't affect the code,
                        //and might be used in the future, so we decided to leave it in.
                        AllCourses = account.account_type == "STUDENT" || account.account_type == "ALUMNI" 
                            ? await GetScheduleStudentAsync(username, vm.SessionCode) 
                            : await GetScheduleFacultyAsync(username, vm.SessionCode),
                    });
            }
            return result;

        }
    }
}