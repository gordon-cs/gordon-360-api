﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gordon360.Models.CCT
{
    public partial class COURSES_FOR_PROFESSORResult
    {
        public string YR_CDE { get; set; }
        public string TRM_CDE { get; set; }
        public string CRS_CDE { get; set; }
        public string SUBTERM_CDE { get; set; }
        public string CRS_TITLE { get; set; }
        public string MONDAY_CDE { get; set; }
        public string TUESDAY_CDE { get; set; }
        public string WEDNESDAY_CDE { get; set; }
        public string THURSDAY_CDE { get; set; }
        public string FRIDAY_CDE { get; set; }
        public string SATURDAY_CDE { get; set; }
        public string SUNDAY_CDE { get; set; }
        public DateTime? BEGIN_TIM { get; set; }
        public DateTime? END_TIM { get; set; }
        public string LOC_CDE { get; set; }
        public string BLDG_CDE { get; set; }
        public string ROOM_CDE { get; set; }
        public int? PROFESSOR_ID_NUM { get; set; }
    }
}
