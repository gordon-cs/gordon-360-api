﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[Keyless]
public partial class RA_Assigned_Ranges_View
{
    [StringLength(9)]
    [Unicode(false)]
    public string RA_ID { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Fname { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Lname { get; set; }

    [StringLength(45)]
    [Unicode(false)]
    public string Hall_Name { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string Room_Start { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string Room_End { get; set; }
}