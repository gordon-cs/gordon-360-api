using System;
using System.Collections.Generic;
using System.Linq;
using Gordon360.Models.ViewModels;
using System.Data;
using Gordon360.Exceptions.CustomExceptions;
using System.Diagnostics;
using Gordon360.Database.CCT;
using Gordon360.Database.StudentTimesheets;
using Gordon360.Models.StudentTimesheets;
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

        public async void saveShiftForUser(int studentID, int jobID, DateTime shiftStart, DateTime shiftEnd, string hoursWorked, string shiftNotes, string lastChangedBy)
        {
            await _context.Procedures.student_timesheets_insert_shiftAsync(studentID, jobID, shiftStart, shiftEnd, hoursWorked, shiftNotes, lastChangedBy);
        }

        public StudentTimesheetsViewModel editShift(int rowID, DateTime shiftStart, DateTime shiftEnd, string hoursWorked, string username)
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

        public void deleteShiftForUser(int rowID, int studentID)
        {
            _context.student_timesheets.Remove(new student_timesheets { ID = rowID, ID_NUM = studentID });
            _context.SaveChanges();
        }

        public async void submitShiftForUser(int studentID, int jobID, DateTime shiftEnd, int submittedTo, string lastChangedBy)
        {
            await _context.Procedures.student_timesheets_submit_job_shiftAsync(studentID, jobID, shiftEnd, submittedTo, lastChangedBy);
        }

        public async Task<IEnumerable<SupervisorViewModel>> getsupervisorNameForJob(int supervisorID)
        {
            return (await _context.Procedures.student_timesheets_select_supervisor_nameAsync(supervisorID))
                .Select(s => new SupervisorViewModel { FIRST_NAME = s.first_name, LAST_NAME = s.last_name, PREFERRED_NAME = s.preferred_name });
        }

        public async Task<IEnumerable<ActiveJobViewModel>> getActiveJobs(DateTime shiftStart, DateTime shiftEnd, int studentID)
        {
            return (await _context.Procedures.student_timesheets_select_emls_for_ajax_selectboxAsync(shiftStart, shiftEnd, studentID)).Select(j => new ActiveJobViewModel { EMLID = j.EmlID, POSTITLE = j.postitle });
        }

        public IEnumerable<OverlappingShiftIdViewModel> editShiftOverlapCheck(int studentID, DateTime shiftStart, DateTime shiftEnd, int rowID)
        {
            IEnumerable<OverlappingShiftIdViewModel> result = null;

            try
            {
                result = (IEnumerable<OverlappingShiftIdViewModel>)_context.Procedures.student_timesheets_edit_shift_already_worked_these_hoursAsync(studentID, shiftStart, shiftEnd, rowID);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return result;
        }

        public IEnumerable<OverlappingShiftIdViewModel> checkForOverlappingShift(int studentID, DateTime shiftStart, DateTime shiftEnd)
        {
            IEnumerable<OverlappingShiftIdViewModel> result = null;

            try
            {
                result = (IEnumerable<OverlappingShiftIdViewModel>)_context.Procedures.student_timesheets_already_worked_these_hoursAsync(studentID, shiftStart, shiftEnd);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return result;
        }


        public ClockInViewModel ClockIn(bool state, string id)
        {
            var query = _CCTContext.ACCOUNT.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var result = _CCTContext.Procedures.INSERT_TIMESHEETS_CLOCK_IN_OUTAsync(int.Parse(id), state);
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            var currentState = state;

            ClockInViewModel y = new ClockInViewModel()
            {
                currentState = currentState
            };

            return y;
        }

        public IEnumerable<ClockInViewModel> ClockOut(string id)
        {
            var query = _CCTContext.ACCOUNT.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var result = _CCTContext.Procedures.GET_TIMESHEETS_CLOCK_IN_OUTAsync(int.Parse(id));
            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            return (IEnumerable<ClockInViewModel>)result;

        }

        public ClockInViewModel DeleteClockIn(string id)
        {
            var query = _CCTContext.ACCOUNT.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var result = _CCTContext.Procedures.DELETE_CLOCK_INAsync(id);


            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            return new ClockInViewModel();
        }
    }
}
