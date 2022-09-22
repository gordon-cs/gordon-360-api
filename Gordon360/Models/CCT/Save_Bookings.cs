﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT
{
    [Table("Save_Bookings", Schema = "dbo")]
    public partial class Save_Bookings
    {
        [Key]
        [StringLength(25)]
        [Unicode(false)]
        public string ID { get; set; }
        [Key]
        [StringLength(10)]
        [Unicode(false)]
        public string rideID { get; set; }
        public byte isDriver { get; set; }

        [ForeignKey("rideID")]
        [InverseProperty("Save_Bookings")]
        public virtual Save_Rides ride { get; set; }
    }
}