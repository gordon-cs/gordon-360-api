//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gordon360.Models
{
    using System;
    
    public partial class staff_timesheets_select_fixed_status_shifts_Result
    {
        public string eml_description { get; set; }
        public System.DateTime shift_start_datetime { get; set; }
        public System.DateTime shift_end_datetime { get; set; }
        public decimal hours_worked { get; set; }
        public string shift_notes { get; set; }
        public decimal hourly_rate { get; set; }
        public Nullable<int> submitted_to { get; set; }
        public string status { get; set; }
        public Nullable<System.DateTime> pay_week_date { get; set; }
    }
}
