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

        public IEnumerable<StudentTimesheetsViewModel> getSavedShiftsForUser(int ID_NUM)
        {
            IEnumerable<StudentTimesheetsViewModel> result = null;

            var id_num = new SqlParameter("@ID_NUM", ID_NUM);
            string query = "SELECT ID, ID_NUM, EML, EML_DESCRIPTION, SHIFT_START_DATETIME, SHIFT_END_DATETIME, HOURLY_RATE, HOURS_WORKED, SUPERVISOR, COMP_SUPERVISOR, STATUS, SUBMITTED_TO, SHIFT_NOTES, COMMENTS, PAY_WEEK_DATE, PAY_PERIOD_DATE, PAY_PERIOD_ID, LAST_CHANGED_BY, DATETIME_ENTERED from student_timesheets where ID_NUM = @ID_NUM AND STATUS != 'Paid' order by EML, SHIFT_START_DATETIME, STATUS";
            try
            {
                result = RawSqlQuery<StudentTimesheetsViewModel>.StudentTimesheetQuery(query, id_num);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return result;
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

        public IEnumerable<StudentTimesheetsViewModel> editShift(int rowID, DateTime shiftStart, DateTime shiftEnd, string hoursWorked, string username)
        {
            IEnumerable<StudentTimesheetsViewModel> result = null;
            var id = new SqlParameter("@ID", rowID);
            var newStart = new SqlParameter("@newStart", shiftStart);
            var newEnd = new SqlParameter("@newEnd", shiftEnd);
            var newHours = new SqlParameter("@newHours", hoursWorked);
            var lastChangedBy = new SqlParameter("@lastChangedBy", username);

            try
            {
                result = RawSqlQuery<StudentTimesheetsViewModel>.StudentTimesheetQuery("UPDATE student_timesheets SET STATUS = 'Saved', SHIFT_START_DATETIME = @newStart, SHIFT_END_DATETIME = @newEnd, HOURS_WORKED = @newHours, LAST_CHANGED_BY = @lastChangedBy, COMMENTS = null WHERE ID = @ID;", newStart, newEnd, newHours, id, lastChangedBy);
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
                result = RawSqlQuery<ActiveJobViewModel>.StudentTimesheetQuery("student_timesheets_select_emls_for_ajax_selectbox @start_datetime, @end_datetime, @ID_NUM", start_datetime, end_datetime, id_num);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return result;
        }

        public IEnumerable<OverlappingShiftIdViewModel> editShiftOverlapCheck(int studentID, DateTime shiftStart, DateTime shiftEnd, int rowID)
        {
            IEnumerable<OverlappingShiftIdViewModel> result = null;
            var id_num = new SqlParameter("@ID_NUM", studentID);
            var start_datetime = new SqlParameter("@start_datetime", shiftStart);
            var end_datetime = new SqlParameter("@end_datetime", shiftEnd);
            var shift_being_edited = new SqlParameter("@shift_being_edited", rowID);

            try
            {
                result = RawSqlQuery<OverlappingShiftIdViewModel>.StudentTimesheetQuery("student_timesheets_edit_shift_already_worked_these_hours @ID_NUM, @start_datetime, @end_datetime, @shift_being_edited", id_num, start_datetime, end_datetime, shift_being_edited);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return result;
        }

        public IEnumerable<OverlappingShiftIdViewModel> checkForOverlappingShift(int studentID, DateTime shiftStart, DateTime shiftEnd)
        {
            IEnumerable<OverlappingShiftIdViewModel> result = null;

            var id_num = new SqlParameter("@ID_NUM", studentID);
            var start_datetime = new SqlParameter("@start_datetime", shiftStart);
            var end_datetime = new SqlParameter("@end_datetime", shiftEnd);

            try
            {
                result = RawSqlQuery<OverlappingShiftIdViewModel>.StudentTimesheetQuery("student_timesheets_already_worked_these_hours @ID_NUM, @start_datetime, @end_datetime", id_num, start_datetime, end_datetime);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return result;
        }


        public ClockInViewModel ClockIn(bool state, string id)
        {

            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var idParam = new SqlParameter("@ID_Num", id);
            var stateParam = new SqlParameter("@CurrentState", state);

            var result = RawSqlQuery<ClockInViewModel>.query("INSERT_TIMESHEETS_CLOCK_IN_OUT @ID_Num, @CurrentState", idParam, stateParam); //run stored procedure
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

        public IEnumerable<ClockInViewModel> ClockOut(string id )
        {
            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var idParam = new SqlParameter("@ID_Num", id);

            var result = RawSqlQuery<ClockInViewModel>.query("GET_TIMESHEETS_CLOCK_IN_OUT @ID_NUM", idParam); //run stored procedure


            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }


            var clockOutModel = result.Select(x =>
            {
                ClockInViewModel y = new ClockInViewModel();

                y.currentState = x.currentState;

                y.timestamp = x.timestamp;


                return y;
            });



            return clockOutModel;

        }

        public ClockInViewModel DeleteClockIn(string id)
        {
            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }

            var idParam = new SqlParameter("@ID_Num", id);

            var result = RawSqlQuery<ClockInViewModel>.query("DELETE_CLOCK_IN @ID_NUM", idParam); //run stored procedure


            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }


            
            ClockInViewModel y = new ClockInViewModel();

            return y;

        }


        public IEnumerable<StaffCheckViewModel> CanUsePage(string id)
        {
            var _unitOfWork = new UnitOfWork();
            var query = _unitOfWork.AccountRepository.FirstOrDefault(x => x.gordon_id == id);
            if (query == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The account was not found." };
            }
            var idParam = new SqlParameter("@ID_Num", id);

            var result = RawSqlQuery<StaffCheckViewModel>.StaffTimesheetQuery("staff_timesheets_can_use_this_page @ID_NUM", idParam); // run stored procedure

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            var staffTimesheet = result.Select(x =>
            {
                StaffCheckViewModel y = new StaffCheckViewModel();
                y.EmIID = true;

                return y;
            });

            return staffTimesheet;

        }


        //staff functions

        public IEnumerable<StaffTimesheetsViewModel> saveShiftForStaff(int staffID, int jobID, DateTime shiftStart, DateTime shiftEnd, string hoursWorked, string shiftNotes, string lastChangedBy)
        {
            IEnumerable<StaffTimesheetsViewModel> result = null;

            var id_num = new SqlParameter("@ID_NUM", staffID);
            var eml = new SqlParameter("@eml", jobID);
            var shiftStartDateTime = new SqlParameter("@shift_start_datetime", shiftStart);
            var shiftEndDateTime = new SqlParameter("@shift_end_datetime", shiftEnd);
            var hours_worked = new SqlParameter("@hours_worked", hoursWorked);
            var notes = new SqlParameter("@shift_notes", shiftNotes);
            var changedBy = new SqlParameter("@last_changed_by", lastChangedBy);

            try
            {
                result = RawSqlQuery<StaffTimesheetsViewModel>.StaffTimesheetQuery("staff_timesheets_insert_shift @ID_NUM, @eml, @shift_start_datetime, @shift_end_datetime, @hours_worked, @shift_notes, @last_changed_by", id_num, eml, shiftStartDateTime, shiftEndDateTime, hours_worked, notes, changedBy);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return result;
        }


        public IEnumerable<StaffTimesheetsViewModel> getSavedShiftsForStaff(int ID_NUM)
        {
            IEnumerable<StaffTimesheetsViewModel> result = null;

            var id_num = new SqlParameter("@ID_NUM", ID_NUM);
            string query = "SELECT ID_NUM, EML, EML_DESCRIPTION, SHIFT_START_DATETIME, SHIFT_END_DATETIME, HOURLY_RATE, HOURS_WORKED, SUPERVISOR, COMP_SUPERVISOR, STATUS, SUBMITTED_TO, SHIFT_NOTES, COMMENTS, PAY_WEEK_DATE, PAY_PERIOD_DATE, PAY_PERIOD_ID, LAST_CHANGED_BY, DATETIME_ENTERED from staff_timesheets where ID_NUM = @ID_NUM AND STATUS != 'Paid' order by EML, SHIFT_START_DATETIME, STATUS";
            try
            {
                result = RawSqlQuery<StaffTimesheetsViewModel>.StaffTimesheetQuery(query, id_num);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return result;
        }

        public IEnumerable<StaffTimesheetsViewModel> editShiftStaff(int rowID, DateTime shiftStart, DateTime shiftEnd, string hoursWorked, string username)
        {
            IEnumerable<StaffTimesheetsViewModel> result = null;
            var id = new SqlParameter("@ID", rowID);
            var newStart = new SqlParameter("@newStart", shiftStart);
            var newEnd = new SqlParameter("@newEnd", shiftEnd);
            var newHours = new SqlParameter("@newHours", hoursWorked);
            var lastChangedBy = new SqlParameter("@lastChangedBy", username);

            try
            {
                result = RawSqlQuery<StaffTimesheetsViewModel>.StaffTimesheetQuery("UPDATE staff_timesheets SET STATUS = 'Saved', SHIFT_START_DATETIME = @newStart, SHIFT_END_DATETIME = @newEnd, HOURS_WORKED = @newHours, LAST_CHANGED_BY = @lastChangedBy, COMMENTS = null WHERE ID = @ID;", newStart, newEnd, newHours, id, lastChangedBy);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return result;
        }

        public IEnumerable<StaffTimesheetsViewModel> deleteShiftForStaff(int rowID, int staffID)
        {
            IEnumerable<StaffTimesheetsViewModel> result = null;

            var row_id = new SqlParameter("@row_num", rowID);
            var ID_NUM = new SqlParameter("@ID_NUM", staffID);

            try
            {
                result = RawSqlQuery<StaffTimesheetsViewModel>.StaffTimesheetQuery("staff_timesheets_delete_shift @row_num, @ID_NUM", row_id, ID_NUM);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return result;
        }

        public IEnumerable<StaffTimesheetsViewModel> submitShiftForStaff(int staffID, int jobID, DateTime shiftEnd, int submittedTo, string lastChangedBy)
        {
            IEnumerable<StaffTimesheetsViewModel> result = null;

            var ID_NUM = new SqlParameter("@ID_NUM", staffID);
            var eml = new SqlParameter("@eml", jobID);
            var shiftEndDateTime = new SqlParameter("@shift_end_datetime", shiftEnd);
            var submitted_to = new SqlParameter("@submitted_to", submittedTo);
            var changedBy = new SqlParameter("@last_changed_by", lastChangedBy);

            try
            {
                result = RawSqlQuery<StaffTimesheetsViewModel>.StaffTimesheetQuery("staff_timesheets_submit_job_shift @ID_NUM, @eml, @shift_end_datetime, @submitted_to, @last_changed_by", ID_NUM, eml, shiftEndDateTime, submitted_to, changedBy);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return result;
        }

        public IEnumerable<ActiveJobViewModel> getActiveJobsStaff(DateTime shiftStart, DateTime shiftEnd, int staffID)
        {
            IEnumerable<ActiveJobViewModel> result = null;

            var start_datetime = new SqlParameter("@start_datetime", shiftStart);
            var end_datetime = new SqlParameter("@end_datetime", shiftEnd);
            var id_num = new SqlParameter("@ID_NUM", staffID);

            try
            {
                result = RawSqlQuery<ActiveJobViewModel>.StaffTimesheetQuery("staff_timesheets_select_emls_for_ajax_selectbox @start_datetime, @end_datetime, @ID_NUM", start_datetime, end_datetime, id_num);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return result;
        }

        public IEnumerable<SupervisorViewModel> getStaffSupervisorNameForJob(int supervisorID)
        {
            IEnumerable<SupervisorViewModel> result = null;

            var supervisor = new SqlParameter("@supervisor", supervisorID);

            try
            {
                result = RawSqlQuery<SupervisorViewModel>.StaffTimesheetQuery("staff_timesheets_select_supervisor_name @supervisor", supervisor);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return result;
        }

        public IEnumerable<OverlappingShiftIdViewModel> editShiftOverlapCheckStaff(int staffID, DateTime shiftStart, DateTime shiftEnd, int rowID)
        {
            IEnumerable<OverlappingShiftIdViewModel> result = null;
            var id_num = new SqlParameter("@ID_NUM", staffID);
            var start_datetime = new SqlParameter("@start_datetime", shiftStart);
            var end_datetime = new SqlParameter("@end_datetime", shiftEnd);
            var shift_being_edited = new SqlParameter("@shift_being_edited", rowID);

            try
            {
                result = RawSqlQuery<OverlappingShiftIdViewModel>.StaffTimesheetQuery("staff_timesheets_edit_shift_already_worked_these_hours @ID_NUM, @start_datetime, @end_datetime, @shift_being_edited", id_num, start_datetime, end_datetime, shift_being_edited);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return result;
        }

        public IEnumerable<OverlappingShiftIdViewModel> checkForOverlappingShiftStaff(int staffID, DateTime shiftStart, DateTime shiftEnd)
        {
            IEnumerable<OverlappingShiftIdViewModel> result = null;

            var id_num = new SqlParameter("@ID_NUM", staffID);
            var start_datetime = new SqlParameter("@start_datetime", shiftStart);
            var end_datetime = new SqlParameter("@end_datetime", shiftEnd);

            try
            {
                result = RawSqlQuery<OverlappingShiftIdViewModel>.StaffTimesheetQuery("staff_timesheets_already_worked_these_hours @ID_NUM, @start_datetime, @end_datetime", id_num, start_datetime, end_datetime);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return result;
        }

        public IEnumerable<HourTypesViewModel> GetHourTypes()
        {
            var result = RawSqlQuery<HourTypesViewModel>.StaffTimesheetQuery("staff_timesheets_select_hour_types"); // run stored procedure

            if (result == null)
            {
                throw new ResourceNotFoundException() { ExceptionMessage = "The data was not found." };
            }

            var staffTimesheet = result.Select(x =>
            {
                HourTypesViewModel y = new HourTypesViewModel();
                y.type_id = x.type_id;
                y.type_description = x.type_description;

                return y;
            });

            return staffTimesheet;
        }

    }
}