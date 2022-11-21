﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT
{
    [Table("SeriesTeam", Schema = "RecIM")]
    public partial class SeriesTeam
    {
        [Key]
        public int ID { get; set; }
        public int TeamID { get; set; }
        public int SeriesID { get; set; }
        public int Win { get; set; }
        public int? Loss { get; set; }

        [ForeignKey("SeriesID")]
        [InverseProperty("SeriesTeam")]
        public virtual Series Series { get; set; }
        [ForeignKey("TeamID")]
        [InverseProperty("SeriesTeam")]
        public virtual Team Team { get; set; }
    }
}