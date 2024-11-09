﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[Table("Categories", Schema = "LostAndFound")]
public partial class Categories
{
    [Key]
    [StringLength(255)]
    [Unicode(false)]
    public string CategoryName { get; set; }

    [InverseProperty("categoryNavigation")]
    public virtual ICollection<MissingReports> MissingReports { get; set; } = new List<MissingReports>();
}