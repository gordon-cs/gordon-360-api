﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gordon360.Models.CCT
{
    public partial class FINALIZATION_GETHOLDSBYIDResult
    {
        public bool? FinancialHold { get; set; }
        public bool? HighSchoolHold { get; set; }
        public bool? MedicalHold { get; set; }
        public bool? MajorHold { get; set; }
        public bool? RegistrarHold { get; set; }
        public bool? LaVidaHold { get; set; }
        public bool? MustRegisterForClasses { get; set; }
        public int NewStudent { get; set; }
        public string FinancialHoldText { get; set; }
        public string MeetingDate { get; set; }
        public string MeetingLocations { get; set; }
    }
}
