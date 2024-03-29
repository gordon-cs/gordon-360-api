﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[Keyless]
public partial class UserCourses
{
    [StringLength(50)]
    [Unicode(false)]
    public string Username { get; set; }

    [Required]
    [StringLength(10)]
    [Unicode(false)]
    public string Role { get; set; }

    [Required]
    [StringLength(4)]
    [Unicode(false)]
    public string YR_CDE { get; set; }

    [Required]
    [StringLength(2)]
    [Unicode(false)]
    public string TRM_CDE { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string SUBTERM_DESC { get; set; }

    public int? SUBTERM_SORT_ORDER { get; set; }

    [Required]
    [StringLength(30)]
    [Unicode(false)]
    public string CRS_CDE { get; set; }

    [StringLength(35)]
    [Unicode(false)]
    public string CRS_TITLE { get; set; }

    public int? INSTRUCTOR_ID { get; set; }

    [StringLength(5)]
    [Unicode(false)]
    public string BLDG_CDE { get; set; }

    [StringLength(5)]
    [Unicode(false)]
    public string ROOM_CDE { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string MONDAY_CDE { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string TUESDAY_CDE { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string WEDNESDAY_CDE { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string THURSDAY_CDE { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string FRIDAY_CDE { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string SATURDAY_CDE { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string SUNDAY_CDE { get; set; }

    public TimeSpan? BEGIN_TIME { get; set; }

    public TimeSpan? END_TIME { get; set; }

    [Column(TypeName = "date")]
    public DateTime? BEGIN_DATE { get; set; }

    [Column(TypeName = "date")]
    public DateTime? END_DATE { get; set; }
}