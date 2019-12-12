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

        public IEnumerable<StudentTimesheetsViewModel> saveShiftsForUser(int studentID, int jobID, DateTime shiftStart, DateTime shiftEnd, string hoursWorked, string shiftNotes, string lastChangedBy)
        {
            Debug.WriteLine("Writing all values in shift object to console: \n");
            Debug.WriteLine(studentID);
            Debug.WriteLine(jobID);
            Debug.WriteLine(shiftStart);
            Debug.WriteLine(shiftEnd);
            Debug.WriteLine(hoursWorked);
            Debug.WriteLine(shiftNotes);
            Debug.WriteLine(lastChangedBy);

            IEnumerable<StudentTimesheetsViewModel> result = null;

            var id_num = new SqlParameter("@ID_NUM", studentID);
            var eml = new SqlParameter("@eml", jobID);
            var shiftStartDateTime = new SqlParameter("@shift_start_datetime", shiftStart);
            var shiftEndDateTime = new SqlParameter("@shift_end_datetime", shiftEnd);
            var hours_worked = new SqlParameter("@hours_worked", hoursWorked);
            var notes = new SqlParameter("@shift_notes", shiftNotes);
            var changedBy = new SqlParameter("@last_changed_by", lastChangedBy);

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
    }
}