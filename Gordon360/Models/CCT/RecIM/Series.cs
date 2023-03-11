﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT
{
    [Table("Series", Schema = "RecIM")]
    public partial class Series
    {
        public Series()
        {
            Match = new HashSet<Match>();
            SeriesSurface = new HashSet<SeriesSurface>();
            SeriesTeam = new HashSet<SeriesTeam>();
        }

        [Key]
        public int ID { get; set; }
        [Required]
        [StringLength(128)]
        [Unicode(false)]
        public string Name { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime EndDate { get; set; }
        public int ActivityID { get; set; }
        public int TypeID { get; set; }
        public int StatusID { get; set; }
        public int ScheduleID { get; set; }

        [ForeignKey("ActivityID")]
        [InverseProperty("Series")]
        public virtual Activity Activity { get; set; }
        [ForeignKey("ScheduleID")]
        [InverseProperty("Series")]
        public virtual SeriesSchedule Schedule { get; set; }
        [ForeignKey("StatusID")]
        [InverseProperty("Series")]
        public virtual SeriesStatus Status { get; set; }
        [ForeignKey("TypeID")]
        [InverseProperty("Series")]
        public virtual SeriesType Type { get; set; }
        [InverseProperty("Series")]
        public virtual ICollection<Match> Match { get; set; }
        [InverseProperty("Series")]
        public virtual ICollection<SeriesSurface> SeriesSurface { get; set; }
        [InverseProperty("Series")]
        public virtual ICollection<SeriesTeam> SeriesTeam { get; set; }
    }
}