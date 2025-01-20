﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[Keyless]
public partial class Current_On_Call
{
    [Required]
    [StringLength(10)]
    [Unicode(false)]
    public string Hall_ID { get; set; }

    [StringLength(45)]
    [Unicode(false)]
    public string Hall_Name { get; set; }

    [StringLength(5)]
    [Unicode(false)]
    public string RoomNumber { get; set; }

    [Required]
    [StringLength(101)]
    [Unicode(false)]
    public string RA_Name { get; set; }

    [StringLength(95)]
    [Unicode(false)]
    public string PreferredContact { get; set; }

    public DateTime Check_in_time { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string RD_Email { get; set; }

    [StringLength(46)]
    [Unicode(false)]
    public string RD_Name { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string RA_UserName { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string RD_UserName { get; set; }

    [StringLength(86)]
    [Unicode(false)]
    public string RA_Photo { get; set; }
}