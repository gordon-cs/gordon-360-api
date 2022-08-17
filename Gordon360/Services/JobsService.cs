using Gordon360.Exceptions;
using Gordon360.Models.CCT.Context;
using Gordon360.Models.StudentTimesheets;
using Gordon360.Models.StudentTimesheets.Context;
using Gordon360.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Gordon360.Services
{
    /// <summary>
    /// Service Class that facilitates data transactions between the JobsController and the student_timesheets + paid_shifts database model.
    /// </summary>
    public class JobsService : IJobsService
    {
        private readonly StudentTimesheetsContext _context;
        private readonly CCTContext _CCTContext;

        public JobsService(StudentTimesheetsContext context, CCTContext cctContext)
        {
            _context = context;
            _CCTContext = cctContext;
        }

        public IEnumerable<StudentTimesheetsViewModel> getSavedShiftsForUser(int ID_NUM)
        {
            return _context.student_timesheets
                .Where(t => t.ID_NUM == ID_NUM && t.status != "PAID")
                .OrderBy(t => t.eml)
                .ThenBy(t => t.shift_start_datetime)
                .ThenBy(t => t.status)
                .Select<student_timesheets, StudentTimesheetsViewModel>(t => t);
        }

        public async Task SaveShiftForUserAsync(int studentID, int jobID, DateTime shiftStart, DateTime shiftEnd, string hoursWorked, string shiftNotes, string lastChangedBy)
        {
            await _context.Procedures.student_timesheets_insert_shiftAsync(studentID, jobID, shiftStart, shiftEnd, hoursWorked, shiftNotes, lastChangedBy);
        }

        public StudentTimesheetsViewModel EditShift(int rowID, DateTime shiftStart, DateTime shiftEnd, string hoursWorked, string username)
        {

            var result = _context.student_timesheets.Find(rowID);
            result.status = "Saved";
            result.shift_start_datetime = shiftStart;
            result.shift_end_datetime = shiftEnd;
            result.hours_worked = decimal.Parse(hoursWorked);
            result.last_changed_by = username;
            result.comments = null;
            _context.SaveChanges();

            return result;
        }

        public void DeleteShiftForUser(int rowID, int studentID)
        {
            _context.student_timesheets.Remove(new student_timesheets { ID = rowID, ID_NUM = studentID });
            _context.SaveChanges();
        }

        public async Task SubmitShiftForUserAsync(int studentID, int jobID, DateTime shiftEnd, int submittedTo, string lastChangedBy)
        {
            await _context.Procedures.student_timesheets_submit_job_shiftAsync(studentID, jobID, shiftEnd, submittedTo, lastChangedBy);
        }

        public async Task<IEnumerable<SupervisorViewModel>> GetsupervisorNameForJobAsync(int supervisorID)
        {
            var result = await _context.Procedures.student_timesheets_select_supervisor_nameAsync(supervisorID);
            return result.Select(s => new SupervisorViewModel { FIRST_NAME = s.first_name, LAST_NAME = s.last_name, PREFERRED_NAME = s.preferred_name });
        }

        public async Task<IEnumerable<ActiveJobViewModel>> GetActiveJobsAsync(DateTime shiftStart, DateTime shiftEnd, int studentID)
        {
            var result = await _context.Procedures.student_timesheets_select_emls_for_ajax_selectboxAsync(shiftStart, shiftEnd, studentID);
            return result.Select(j => new ActiveJobViewModel { EMLID = j.EmlID, POSTITLE = j.postitle });
        }

        public async Task<IEnumerable<OverlappingShiftIdViewModel>> EditShiftOverlapCheckAsync(int studentID, DateTime shiftStart, DateTime shiftEnd, int rowID)
        {
            var result = await _context.Procedures.student_timesheets_edit_shift_already_worked_these_hoursAsync(studentID, shiftStart, shiftEnd, rowID);
            return result.Select(x => new OverlappingShiftIdViewModel { ID = x.ID });
        }

        public async Task<IEnumerable<OverlappingShiftIdViewModel>> CheckForOverlappingShiftAsync(int studentID, DateTime shiftStart, DateTime shiftEnd)
        {
            var result = await _context.Procedures.student_timesheets_already_worked_these_hoursAsync(studentID, shiftStart, shiftEnd);
            return result.Select(x => new OverlappingShiftIdViewModel { ID = x.ID });
        }


        public async Task<ClockInViewModel> ClockInAsync(bool state, string id)
        {
            var result = await _CCTContext.Procedures.INSERT_TIMESHEETS_CLOCK_IN_OUTAsync(int.Parse(id), state);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            return new ClockInViewModel { currentState = state };
        }

        public async Task<IEnumerable<ClockInViewModel>> ClockOutAsync(string id)
        {
            var result = await _CCTContext.Procedures.GET_TIMESHEETS_CLOCK_IN_OUTAsync(int.Parse(id));
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            return (IEnumerable<ClockInViewModel>)result;

        }

        public async Task<ClockInViewModel> DeleteClockInAsync(string id)
        {
            var result = await _CCTContext.Procedures.DELETE_CLOCK_INAsync(id);

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            return new ClockInViewModel();
        }
    }
}
