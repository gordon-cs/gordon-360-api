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
    /// Service Class that facilitates data transactions between the JobsController and the student_timesheets + paid_shifts database model.
    /// </summary>
    public class JobsService : IJobsService
    {
        private IUnitOfWork _unitOfWork;

        public JobsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<StudentTimesheetsViewModel> saveShiftForUser(int studentID, int jobID, DateTime shiftStart, DateTime shiftEnd, string hoursWorked, string shiftNotes, string lastChangedBy)
        {
            IEnumerable<StudentTimesheetsViewModel> result = null;

            var id_num = new SqlParameter("@ID_NUM", studentID);
            var eml = new SqlParameter("@eml", jobID);
            var shiftStartDateTime = new SqlParameter("@shift_start_datetime", shiftStart);
            var shiftEndDateTime = new SqlParameter("@shift_end_datetime", shiftEnd);
            var hours_worked = new SqlParameter("@hours_worked", hoursWorked);
            var notes = new SqlParameter("@shift_notes", shiftNotes);
            var changedBy = new SqlParameter("@last_changed_by", lastChangedBy);

            Debug.WriteLine("Time in: " + shiftStart);
            Debug.WriteLine("Time out: " + shiftEnd);

            try
            {
                result = RawSqlQuery<StudentTimesheetsViewModel>.StudentTimesheetQuery("student_timesheets_insert_shift @ID_NUM, @eml, @shift_start_datetime, @shift_end_datetime, @hours_worked, @shift_notes, @last_changed_by", id_num, eml, shiftStartDateTime, shiftEndDateTime, hours_worked, notes, changedBy);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return result;
        }

        public IEnumerable<StudentTimesheetsViewModel> deleteShiftForUser(int rowID, int studentID)
        {
            IEnumerable<StudentTimesheetsViewModel> result = null;

            var row_id = new SqlParameter("@row_num", rowID);
            var ID_NUM = new SqlParameter("@ID_NUM", studentID);

            try
            {
                result = RawSqlQuery<StudentTimesheetsViewModel>.StudentTimesheetQuery("student_timesheets_delete_shift @row_num, @ID_NUM", row_id, ID_NUM);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return result;
        }

        public IEnumerable<StudentTimesheetsViewModel> submitShiftForUser(int studentID, int jobID, DateTime shiftEnd, int submittedTo, string lastChangedBy)
        {
            IEnumerable<StudentTimesheetsViewModel> result = null;

            var ID_NUM = new SqlParameter("@ID_NUM", studentID);
            var eml = new SqlParameter("@eml", jobID);
            var shiftEndDateTime = new SqlParameter("@shift_end_datetime", shiftEnd);
            var submitted_to = new SqlParameter("@submitted_to", submittedTo);
            var changedBy = new SqlParameter("@last_changed_by", lastChangedBy);

            try
            {
                result = RawSqlQuery<StudentTimesheetsViewModel>.StudentTimesheetQuery("student_timesheets_submit_job_shift @ID_NUM, @eml, @shift_end_datetime, @submitted_to, @last_changed_by", ID_NUM, eml, shiftEndDateTime, submitted_to, changedBy);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return result;
        }

        public IEnumerable<SupervisorViewModel> getsupervisorNameForJob(int supervisorID)
        {
            IEnumerable<SupervisorViewModel> result = null;

            var supervisor = new SqlParameter("@supervisor", supervisorID);

            try
            {
                result = RawSqlQuery<SupervisorViewModel>.StudentTimesheetQuery("student_timesheets_select_supervisor_name @supervisor", supervisor);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return result;
        }

        public IEnumerable<ActiveJobViewModel> getActiveJobs(DateTime shiftStart, DateTime shiftEnd, int studentID)
        {
            IEnumerable<ActiveJobViewModel> result = null;

            var start_datetime = new SqlParameter("@start_datetime", shiftStart);
            var end_datetime = new SqlParameter("@end_datetime", shiftEnd);
            var id_num = new SqlParameter("@ID_NUM", studentID);

            try
            {
                Debug.WriteLine("\n start: " + shiftStart);
                Debug.WriteLine("end " + shiftEnd);
                Debug.WriteLine("ID: " + studentID + "\n");
                Debug.WriteLine("executing jobs query");
                result = RawSqlQuery<ActiveJobViewModel>.StudentTimesheetQuery("student_timesheets_select_emls_for_ajax_selectbox @start_datetime, @end_datetime, @ID_NUM", start_datetime, end_datetime, id_num);
                Debug.WriteLine("postitle: " + result);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return result;
        }
    }
}