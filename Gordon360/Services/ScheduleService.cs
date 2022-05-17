using Gordon360.Database.CCT;
using Gordon360.Exceptions;
using Gordon360.Models.ViewModels;
using Gordon360.Static.Methods;
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

        public ScheduleService(CCTContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Fetch the schedule item whose id and session code is specified by the parameter
        /// </summary>
        /// <param name="username">The AD Username of the student</param>
        /// <returns>StudentScheduleViewModel if found, null if not found</returns>
        public async Task<IEnumerable<ScheduleViewModel>> GetScheduleStudentAsync(string username)
        {
            var account = _context.ACCOUNT.FirstOrDefault(x => x.AD_Username == username);

            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Schedule was not found." };
            }


            var currentSession = await Helpers.GetCurrentSessionAsync();
            var result = await _context.Procedures.STUDENT_COURSES_BY_ID_NUM_AND_SESS_CDEAsync(int.Parse(account.gordon_id), currentSession.SessionCode);

            return (IEnumerable<ScheduleViewModel>)result;
        }


        /// <summary>
        /// Fetch the schedule item whose id and session code is specified by the parameter
        /// </summary>
        /// <param name="username">The AD Username of the instructor</param>
        /// <returns>StudentScheduleViewModel if found, null if not found</returns>
        public async Task<IEnumerable<ScheduleViewModel>> GetScheduleFacultyAsync(string username)
        {
            var account = _context.ACCOUNT.FirstOrDefault(x => x.AD_Username == username);
            //var currentSessionCode = Helpers.GetCurrentSession().SessionCode;
            if (account == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The Schedule was not found." };
            }

            var currentSession = await Helpers.GetCurrentSessionAsync();
            var result = await _context.Procedures.INSTRUCTOR_COURSES_BY_ID_NUM_AND_SESS_CDEAsync(int.Parse(account.gordon_id), currentSession.SessionCode);

            return (IEnumerable<ScheduleViewModel>)result;
        }
    }
}