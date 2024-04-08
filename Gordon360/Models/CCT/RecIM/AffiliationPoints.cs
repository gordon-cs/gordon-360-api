﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gordon360.Models.CCT;

[PrimaryKey("AffiliationName", "TeamID", "SeriesID")]
[Table("AffiliationPoints", Schema = "RecIM")]
public partial class AffiliationPoints
{
    [Key]
    [StringLength(50)]
    [Unicode(false)]
    public string AffiliationName { get; set; }

    [Key]
    public int TeamID { get; set; }

    [Key]
    public int SeriesID { get; set; }

    public int Points { get; set; }

    [ForeignKey("AffiliationName")]
    [InverseProperty("AffiliationPoints")]
    public virtual Affiliation AffiliationNameNavigation { get; set; }

    [ForeignKey("SeriesID")]
    [InverseProperty("AffiliationPoints")]
    public virtual Series Series { get; set; }

    [ForeignKey("TeamID")]
    [InverseProperty("AffiliationPoints")]
    public virtual Team Team { get; set; }
}