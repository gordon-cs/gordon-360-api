﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.webSQL.Models
{
    public partial class GlobalSetting
    {
        [Key]
        [StringLength(20)]
        [Unicode(false)]
        public string Flag { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartDt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndDt { get; set; }
    }
}