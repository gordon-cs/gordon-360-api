﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[Keyless]
public partial class PART_DEF
{
    [Required]
    [StringLength(5)]
    [Unicode(false)]
    public string PART_CDE { get; set; }

    [Required]
    [StringLength(45)]
    [Unicode(false)]
    public string PART_DESC { get; set; }
}