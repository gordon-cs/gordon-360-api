﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gordon360.Models.CCT
{
    public partial class UPCOMING_RIDESResult
    {
        public string rideID { get; set; }
        public int capacity { get; set; }
        public string destination { get; set; }
        public string meetingPoint { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public byte isCanceled { get; set; }
        public string notes { get; set; }
        public byte isDriver { get; set; }
        public int? seatsTaken { get; set; }
    }
}
