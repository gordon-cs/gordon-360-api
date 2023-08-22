using Gordon360.Models.StudentTimesheets;
using System;

namespace Gordon360.Models.ViewModels;

public class StudentTimesheetsViewModel
{
    public int ID { get; set; }
    public int ID_NUM { get; set; }
    public int EML { get; set; }
    public string EML_DESCRIPTION { get; set; }
    public System.DateTime SHIFT_START_DATETIME { get; set; }
    public System.DateTime SHIFT_END_DATETIME { get; set; }
    public decimal HOURLY_RATE { get; set; }
    public decimal HOURS_WORKED { get; set; }
    public int SUPERVISOR { get; set; }
    public int COMP_SUPERVISOR { get; set; }
    public string STATUS { get; set; }
    public Nullable<int> SUBMITTED_TO { get; set; }
    public string SHIFT_NOTES { get; set; }
    public string COMMENTS { get; set; }
    public Nullable<System.DateTime> PAY_WEEK_DATE { get; set; }
    public Nullable<System.DateTime> PAY_PERIOD_DATE { get; set; }
    public Nullable<int> PAY_PERIOD_ID { get; set; }
    public string LAST_CHANGED_BY { get; set; }
    public System.DateTime DATETIME_ENTERED { get; set; }


    public static implicit operator StudentTimesheetsViewModel(student_timesheets t)
    {
        return new StudentTimesheetsViewModel
        {
            ID = t.ID,
            ID_NUM = t.ID_NUM,
            EML = t.eml,
            EML_DESCRIPTION = t.eml_description,
            SHIFT_START_DATETIME = t.shift_start_datetime,
            SHIFT_END_DATETIME = t.shift_end_datetime,
            HOURLY_RATE = t.hourly_rate,
            HOURS_WORKED = t.hours_worked,
            SUPERVISOR = t.supervisor,
            COMP_SUPERVISOR = t.comp_supervisor,
            STATUS = t.status,
            SUBMITTED_TO = t.submitted_to,
            SHIFT_NOTES = t.shift_notes,
            COMMENTS = t.comments,
            PAY_WEEK_DATE = t.pay_week_date,
            PAY_PERIOD_DATE = t.pay_period_date,
            PAY_PERIOD_ID = t.pay_period_id,
            LAST_CHANGED_BY = t.last_changed_by,
            DATETIME_ENTERED = t.datetime_entered,
        };
    }
}