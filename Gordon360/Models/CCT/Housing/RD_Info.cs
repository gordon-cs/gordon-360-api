﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[Keyless]
public partial class RD_Info
{
    [StringLength(45)]
    [Unicode(false)]
    public string HallName { get; set; }

    [Required]
    [StringLength(5)]
    [Unicode(false)]
    public string BuildingCode { get; set; }

    [Column("RD Email")]
    [StringLength(50)]
    [Unicode(false)]
    public string RD_Email { get; set; }

    [StringLength(10)]
    public string RDId { get; set; }

    [Required]
    [StringLength(46)]
    [Unicode(false)]
    public string RDName { get; set; }
}